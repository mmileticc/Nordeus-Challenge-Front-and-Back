using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GauntletOfTheHero.Backend.Models;
using GauntletOfTheHero.Backend.Networking;
using UnityEngine;

namespace GauntletOfTheHero.Gameplay.Managers
{
    public class RunManager : MonoBehaviour
    {
        [Header("Backend")]
        [SerializeField] private string backendBaseUrl = "http://localhost:8080";

        public event Action<RunConfigDto> RunUpdated;
        public event Action<BattleSessionDto> EncounterBattleStarted;
        public event Action<RunResolveEncounterResponseDto> EncounterResolved;
        public event Action<Exception> RequestFailed;

        public RunConfigDto CurrentRun => currentRun;
        public BattleSessionDto CurrentBattleSession => currentBattleSession;
        public int CurrentEncounterIndex => currentEncounterIndex;

        private BackendApiClient apiClient;
        private CancellationTokenSource lifetimeTokenSource;
        private RunConfigDto currentRun;
        private BattleSessionDto currentBattleSession;
        private int currentEncounterIndex = -1;

        public static RunManager Instance { get; private set; }
        private void Awake()
        {
            // 1. Singleton provera
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            // 2. Osiguraj da menadžer preživi promenu scene
            DontDestroyOnLoad(gameObject);

            // 3. Inicijalizacija zavisnosti (Samo za glavnu instancu!)
            // Ovo radimo ovde jer apiClient i tokenSource trebaju RunManageru 
            // odmah čim se igra pokrene.
            apiClient = new BackendApiClient(backendBaseUrl);
            lifetimeTokenSource = new CancellationTokenSource();
            
            Debug.Log($"RunManager inicijalizovan na adresi: {backendBaseUrl}");
        }

        private void OnDestroy()
        {
            lifetimeTokenSource?.Cancel();
            lifetimeTokenSource?.Dispose();
            lifetimeTokenSource = null;
        }

        public async Task<RunConfigDto> StartRunAsync(long? heroId = null, CancellationToken cancellationToken = default)
        {
            CancellationToken token = LinkToken(cancellationToken);
            try
            {
                currentRun = await apiClient.StartRunAsync(new RunStartRequestDto { heroId = heroId }, token);
                RunUpdated?.Invoke(currentRun);
                return currentRun;
            }
            catch (Exception ex)
            {
                RequestFailed?.Invoke(ex);
                throw;
            }
        }

        public async Task<RunConfigDto> RefreshRunAsync(CancellationToken cancellationToken = default)
        {
            if (currentRun == null)
            {
                throw new InvalidOperationException("Run is not started yet.");
            }

            CancellationToken token = LinkToken(cancellationToken);
            try
            {
                currentRun = await apiClient.GetRunConfigAsync(currentRun.runId, token);
                RunUpdated?.Invoke(currentRun);
                return currentRun;
            }
            catch (Exception ex)
            {
                RequestFailed?.Invoke(ex);
                throw;
            }
        }

        public async Task<RunConfigDto> EquipMovesAsync(IReadOnlyList<long> moveIds, CancellationToken cancellationToken = default)
        {
            if (currentRun == null)
            {
                throw new InvalidOperationException("Run is not started yet.");
            }

            CancellationToken token = LinkToken(cancellationToken);
            try
            {
                currentRun = await apiClient.EquipMovesAsync(
                    currentRun.runId,
                    new RunEquipMovesRequestDto
                    {
                        moveIds = moveIds == null ? new List<long>() : new List<long>(moveIds)
                    },
                    token
                );
                RunUpdated?.Invoke(currentRun);
                return currentRun;
            }
            catch (Exception ex)
            {
                RequestFailed?.Invoke(ex);
                throw;
            }
        }

        public async Task<BattleSessionDto> StartEncounterAsync(int encounterIndex, CancellationToken cancellationToken = default)
        {
            if (currentRun == null)
            {
                throw new InvalidOperationException("Run is not started yet.");
            }

            CancellationToken token = LinkToken(cancellationToken);
            try
            {
                RunStartEncounterResponseDto response = await apiClient.StartEncounterAsync(currentRun.runId, encounterIndex, token);
                currentRun = response.run;
                currentEncounterIndex = encounterIndex;
                currentBattleSession = response.battleSession;
                RunUpdated?.Invoke(currentRun);
                EncounterBattleStarted?.Invoke(response.battleSession);
                return response.battleSession;
            }
            catch (Exception ex)
            {
                RequestFailed?.Invoke(ex);
                throw;
            }
        }

        public async Task<RunResolveEncounterResponseDto> ResolveEncounterAsync(int encounterIndex, bool heroWon, CancellationToken cancellationToken = default)
        {
            if (currentRun == null)
            {
                throw new InvalidOperationException("Run is not started yet.");
            }

            CancellationToken token = LinkToken(cancellationToken);
            try
            {
                RunResolveEncounterResponseDto response = await apiClient.ResolveEncounterAsync(
                    currentRun.runId,
                    encounterIndex,
                    new RunResolveEncounterRequestDto { heroWon = heroWon },
                    token
                );
                currentRun = response.run;
                RunUpdated?.Invoke(currentRun);
                EncounterResolved?.Invoke(response);
                return response;
            }
            catch (Exception ex)
            {
                RequestFailed?.Invoke(ex);
                throw;
            }
        }

        private CancellationToken LinkToken(CancellationToken externalToken)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(lifetimeTokenSource.Token, externalToken).Token;
        }
    }
}