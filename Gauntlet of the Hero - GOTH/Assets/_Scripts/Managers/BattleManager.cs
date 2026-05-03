using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GauntletOfTheHero.Backend.Models;
using GauntletOfTheHero.Backend.Networking;
using GauntletOfTheHero.Gameplay.Models;
using UnityEngine;

namespace GauntletOfTheHero.Gameplay.Managers
{
    public class BattleManager : MonoBehaviour
    {
        [Header("Backend")]
        [SerializeField] private string backendBaseUrl = "http://localhost:8080";

        public event Action<IReadOnlyList<HeroModel>, IReadOnlyList<MonsterModel>> CatalogLoaded;
        public event Action<BattleSnapshotModel> SessionStarted;
        public event Action<BattleSnapshotModel> TurnCompleted;
        public event Action SessionEnded;
        public event Action<Exception> BackendRequestFailed;

        public IReadOnlyList<HeroModel> Heroes => heroes;
        public IReadOnlyList<MonsterModel> Monsters => monsters;
        public BattleSnapshotModel CurrentSnapshot => currentSnapshot;
        public bool HasActiveSession => currentSessionId.HasValue;

        private readonly List<HeroModel> heroes = new List<HeroModel>();
        private readonly List<MonsterModel> monsters = new List<MonsterModel>();

        private BackendApiClient apiClient;
        private Guid? currentSessionId;
        private BattleSnapshotModel currentSnapshot;
        private CancellationTokenSource lifetimeTokenSource;

        private void Awake()
        {
            apiClient = new BackendApiClient(backendBaseUrl);
            lifetimeTokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            lifetimeTokenSource?.Cancel();
            lifetimeTokenSource?.Dispose();
            lifetimeTokenSource = null;
        }

        public async Task LoadCatalogAsync(CancellationToken cancellationToken = default)
        {
            CancellationToken token = CancellationTokenSource
                .CreateLinkedTokenSource(lifetimeTokenSource.Token, cancellationToken)
                .Token;

            try
            {
                List<HeroDto> heroDtos = await apiClient.GetHeroesAsync(token);
                List<MonsterDto> monsterDtos = await apiClient.GetMonstersAsync(token);

                heroes.Clear();
                heroes.AddRange(heroDtos?.Select(BattleModelMapper.ToHeroModel).Where(h => h != null) ?? Enumerable.Empty<HeroModel>());

                monsters.Clear();
                monsters.AddRange(monsterDtos?.Select(BattleModelMapper.ToMonsterModel).Where(m => m != null) ?? Enumerable.Empty<MonsterModel>());

                CatalogLoaded?.Invoke(heroes, monsters);
            }
            catch (Exception ex)
            {
                BackendRequestFailed?.Invoke(ex);
                throw;
            }
        }

        public async Task<BattleSnapshotModel> StartSessionAsync(long heroId, long monsterId, CancellationToken cancellationToken = default)
        {
            CancellationToken token = CancellationTokenSource
                .CreateLinkedTokenSource(lifetimeTokenSource.Token, cancellationToken)
                .Token;

            try
            {
                BattleSessionDto session = await apiClient.CreateBattleSessionAsync(
                    new BattleSessionCreateRequestDto
                    {
                        heroId = heroId,
                        monsterId = monsterId
                    },
                    token
                );

                currentSessionId = session.sessionId;
                currentSnapshot = BattleModelMapper.ToSnapshot(session);
                SessionStarted?.Invoke(currentSnapshot);
                return currentSnapshot;
            }
            catch (Exception ex)
            {
                BackendRequestFailed?.Invoke(ex);
                throw;
            }
        }

        public BattleSnapshotModel AdoptSession(BattleSessionDto session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            currentSessionId = session.sessionId;
            currentSnapshot = BattleModelMapper.ToSnapshot(session);
            SessionStarted?.Invoke(currentSnapshot);
            return currentSnapshot;
        }

        public async Task<BattleSnapshotModel> PlayTurnAsync(long selectedMoveId, CancellationToken cancellationToken = default)
        {
            if (!currentSessionId.HasValue)
            {
                throw new InvalidOperationException("No active battle session. StartSessionAsync must be called first.");
            }

            CancellationToken token = CancellationTokenSource
                .CreateLinkedTokenSource(lifetimeTokenSource.Token, cancellationToken)
                .Token;

            try
            {
                BattleTurnResultDto turnResult = await apiClient.PlaySessionTurnAsync(
                    currentSessionId.Value,
                    new BattleSessionTurnRequestDto
                    {
                        selectedMoveId = selectedMoveId
                    },
                    token
                );

                currentSnapshot = BattleModelMapper.ToSnapshot(currentSessionId.Value, currentSnapshot, turnResult);
                TurnCompleted?.Invoke(currentSnapshot);
                return currentSnapshot;
            }
            catch (Exception ex)
            {
                BackendRequestFailed?.Invoke(ex);
                throw;
            }
        }

        public async Task EndSessionAsync(CancellationToken cancellationToken = default)
        {
            if (!currentSessionId.HasValue)
            {
                currentSnapshot = null;
                SessionEnded?.Invoke();
                return;
            }

            CancellationToken token = CancellationTokenSource
                .CreateLinkedTokenSource(lifetimeTokenSource.Token, cancellationToken)
                .Token;

            try
            {
                await apiClient.DeleteBattleSessionAsync(currentSessionId.Value, token);
            }
            catch (BackendApiException ex) when (ex.BackendError?.code == "RESOURCE_NOT_FOUND")
            {
                // Session may already be cleaned up by TTL or closed elsewhere.
            }
            catch (Exception ex)
            {
                BackendRequestFailed?.Invoke(ex);
                throw;
            }
            finally
            {
                currentSessionId = null;
                currentSnapshot = null;
                SessionEnded?.Invoke();
            }
        }
    }
}