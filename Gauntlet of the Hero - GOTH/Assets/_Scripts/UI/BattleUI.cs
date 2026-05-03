using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using GauntletOfTheHero.Gameplay.Managers;
using GauntletOfTheHero.Gameplay.Models;
using GauntletOfTheHero.Backend.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [Header("Visuals")]
    public BattleCharacterVisuals heroVis;
    public BattleCharacterVisuals monsterVis;

    [Header("UI Bars & Text")]
    public Image heroHealthBar;
    public Image monsterHealthBar;
    public TextMeshProUGUI heroNameText;
    public TextMeshProUGUI monsterNameText;
    public TextMeshProUGUI heroStatsText;
    public TextMeshProUGUI monsterStatsText;
    public TextMeshProUGUI logText;

    [Header("Moves")]
    public Transform movesGrid;
    public GameObject moveButtonPrefab;
    public GameObject loadingOverlay;
    public GameObject moveTooltip; // container object for tooltip (assign in Inspector)
    public TextMeshProUGUI moveTooltipText;

    [Header("Result")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public Button continueButton;

    private RunManager runManager;
    private BattleManager battleManager;
    private bool isBusy;
    private bool sessionAdopted;
    private BattleSnapshotModel lastSnapshot;

    private void Awake()
    {
        runManager = RunManager.Instance != null ? RunManager.Instance : FindAnyObjectByType<RunManager>();
        battleManager = FindAnyObjectByType<BattleManager>();

        if (loadingOverlay != null)
            loadingOverlay.SetActive(false);

        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (battleManager != null)
        {
            battleManager.SessionStarted += OnSessionStarted;
            battleManager.TurnCompleted += OnTurnCompleted;
            battleManager.SessionEnded += OnSessionEnded;
        }

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        EnsureBattleSession();
        RefreshDisplay();
    }

    private void OnDisable()
    {
        if (battleManager != null)
        {
            battleManager.SessionStarted -= OnSessionStarted;
            battleManager.TurnCompleted -= OnTurnCompleted;
            battleManager.SessionEnded -= OnSessionEnded;
        }

        if (continueButton != null)
            continueButton.onClick.RemoveListener(OnContinueClicked);
    }

    private void EnsureBattleSession()
    {
        if (battleManager == null || runManager == null)
            return;

        if (!sessionAdopted && battleManager.CurrentSnapshot == null && runManager.CurrentBattleSession != null)
        {
            battleManager.AdoptSession(runManager.CurrentBattleSession);
            sessionAdopted = true;
        }
    }

    private void OnSessionStarted(BattleSnapshotModel snapshot)
    {
        RefreshFromSnapshot(snapshot);
    }

    private void OnTurnCompleted(BattleSnapshotModel snapshot)
    {
        // Fire-and-forget the feedback sequencing so UI still updates immediately
        _ = PlayTurnFeedbackAsync(lastSnapshot, snapshot);
        RefreshFromSnapshot(snapshot);
    }

    private void OnSessionEnded()
    {
        isBusy = false;
        UpdateLoading(false);
    }

    private void RefreshDisplay()
    {
        EnsureBattleSession();
        RefreshFromSnapshot(battleManager != null ? battleManager.CurrentSnapshot : null);
    }

    private void RefreshFromSnapshot(BattleSnapshotModel snapshot)
    {
        if (snapshot == null)
            return;

        lastSnapshot = snapshot;

        ApplyHeroUI(snapshot.hero);
        ApplyMonsterUI(snapshot.monster);
        ApplyHealthBars(snapshot);
        ApplyLogs(snapshot.logs);
        GenerateMoveButtons(snapshot.hero != null ? snapshot.hero.moves : null, snapshot.battleFinished);
        ApplyResultUI(snapshot);
        UpdateLoading(false);
    }

    private void ApplyHeroUI(HeroModel hero)
    {
        if (hero == null)
            return;

        if (heroNameText != null)
            heroNameText.text = hero.name;

        if (heroStatsText != null && hero.stats != null)
            heroStatsText.text = $"HP {hero.stats.currentHealth}/{hero.stats.maxHealth}\nATK {hero.stats.attack} | DEF {hero.stats.defense} | MAG {hero.stats.magic}";

        if (heroVis != null)
            StartCoroutine(EnsurePortraitSprite(heroVis, LoadHeroSprite(hero.name)));
    }

    private void ApplyMonsterUI(MonsterModel monster)
    {
        if (monster == null)
            return;

        if (monsterNameText != null)
            monsterNameText.text = monster.name;

        if (monsterStatsText != null && monster.stats != null)
            monsterStatsText.text = $"HP {monster.stats.currentHealth}/{monster.stats.maxHealth}\nATK {monster.stats.attack} | DEF {monster.stats.defense} | MAG {monster.stats.magic}";

        if (monsterVis != null)
            StartCoroutine(EnsurePortraitSprite(monsterVis, LoadMonsterSprite(monster.name)));
    }

    private void ApplyHealthBars(BattleSnapshotModel snapshot)
    {
        if (snapshot.hero != null && snapshot.hero.stats != null && heroHealthBar != null)
        {
            heroHealthBar.fillAmount = snapshot.hero.stats.maxHealth > 0
                ? (float)snapshot.hero.stats.currentHealth / snapshot.hero.stats.maxHealth
                : 0f;
        }

        if (snapshot.monster != null && snapshot.monster.stats != null && monsterHealthBar != null)
        {
            monsterHealthBar.fillAmount = snapshot.monster.stats.maxHealth > 0
                ? (float)snapshot.monster.stats.currentHealth / snapshot.monster.stats.maxHealth
                : 0f;
        }
    }

    private void ApplyLogs(IReadOnlyList<string> logs)
    {
        if (logText == null)
            return;

        if (logs != null && logs.Count > 0)
        {
            logText.text = string.Join("\n", logs);
        }
        else
        {
            logText.text = "Battle started.";
        }
    }

    private void GenerateMoveButtons(IReadOnlyList<MoveModel> moves, bool battleFinished)
    {
        if (movesGrid == null || moveButtonPrefab == null)
            return;

        foreach (Transform child in movesGrid)
            Destroy(child.gameObject);

        if (moves == null)
            return;

        foreach (MoveModel move in moves)
        {
            GameObject buttonGo = Instantiate(moveButtonPrefab, movesGrid);
            Button button = buttonGo.GetComponent<Button>();
            TextMeshProUGUI label = buttonGo.GetComponentInChildren<TextMeshProUGUI>();

            if (label != null)
                label.text = $"{move.name}\nPWR {move.power}";

            if (button != null)
            {
                long moveId = move.id;
                button.interactable = !battleFinished && !isBusy;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnMoveSelected(moveId));
                // Add hover tooltip (requires moveTooltip & moveTooltipText assigned)
                if (moveTooltip != null && moveTooltipText != null)
                {
                    var evt = buttonGo.GetComponent<UnityEngine.EventSystems.EventTrigger>();
                    if (evt == null) evt = buttonGo.AddComponent<UnityEngine.EventSystems.EventTrigger>();

                    // Pointer Enter
                    var entryEnter = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter };
                    entryEnter.callback.AddListener((data) => ShowMoveTooltip(move));
                    evt.triggers.Add(entryEnter);

                    // Pointer Exit
                    var entryExit = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit };
                    entryExit.callback.AddListener((data) => HideMoveTooltip());
                    evt.triggers.Add(entryExit);
                }
            }
        }
    }

    private void ShowMoveTooltip(MoveModel move)
    {
        if (moveTooltip == null || moveTooltipText == null) return;
        moveTooltip.SetActive(true);
        moveTooltipText.text = BuildMoveDescription(move);
    }

    private void HideMoveTooltip()
    {
        if (moveTooltip == null) return;
        moveTooltip.SetActive(false);
    }

    private string BuildMoveDescription(MoveModel move)
    {
        if (move == null) return string.Empty;
        // Short human-friendly description based on effect
        switch (move.effectType)
        {
            case EffectType.Damage:
                return $"{move.name}: Deals {move.power} base {(move.moveType == MoveType.Physical ? "physical" : "magic")} damage (scales with {(move.moveType == MoveType.Physical ? "Attack" : "Magic")}).";
            case EffectType.Heal:
                return $"{move.name}: Heals the user for {move.power} base (scales with Magic).";
            case EffectType.Buff:
                string attr = move.buffAttribute.HasValue ? move.buffAttribute.Value.ToString() : "stat";
                return $"{move.name}: Buffs {attr} by {move.buffAmount} for {move.buffDuration} turns.";
            default:
                return move.name;
        }
    }

    private async void OnMoveSelected(long moveId)
    {
        if (isBusy || battleManager == null)
            return;

        isBusy = true;
        UpdateLoading(true);

        try
        {
            BattleSnapshotModel before = battleManager.CurrentSnapshot;
            int heroHpBefore = before?.hero?.stats?.currentHealth ?? 0;
            int monsterHpBefore = before?.monster?.stats?.currentHealth ?? 0;

            if (heroVis != null)
                await heroVis.PlayAttackAnimation();

            BattleSnapshotModel after = await battleManager.PlayTurnAsync(moveId);

            await PlayTurnFeedbackFromValuesAsync(heroHpBefore, monsterHpBefore, after);
            RefreshFromSnapshot(after);
        }
        catch (Exception ex)
        {
            Debug.LogError("Battle Error: " + ex.Message);
        }
        finally
        {
            isBusy = false;
            UpdateLoading(false);

            // After the turn completes and we cleared busy state, refresh UI
            // so move buttons reflect the updated `isBusy` and snapshot state.
            try
            {
                RefreshFromSnapshot(battleManager != null ? battleManager.CurrentSnapshot : null);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Failed to refresh UI after turn: " + ex.Message);
            }
        }
    }

    private async System.Threading.Tasks.Task PlayTurnFeedbackAsync(BattleSnapshotModel before, BattleSnapshotModel after)
    {
        if (before == null || after == null)
            return;

        await PlayTurnFeedbackFromValuesAsync(
            before.hero?.stats?.currentHealth ?? 0,
            before.monster?.stats?.currentHealth ?? 0,
            after);
    }

    private async System.Threading.Tasks.Task PlayTurnFeedbackFromValuesAsync(int heroHpBefore, int monsterHpBefore, BattleSnapshotModel after)
    {
        int heroHpAfter = after?.hero?.stats?.currentHealth ?? heroHpBefore;
        int monsterHpAfter = after?.monster?.stats?.currentHealth ?? monsterHpBefore;

        // If monster lost HP, show monster hurt
        if (monsterVis != null && monsterHpAfter < monsterHpBefore)
            monsterVis.PlayHurt();

        // If hero lost HP, first play monster attack animation, then hero hurt
        if (heroVis != null && heroHpAfter < heroHpBefore)
        {
            if (monsterVis != null)
            {
                try { await monsterVis.PlayAttackAnimation(); } catch { }
            }
            heroVis.PlayHurt();
        }

        // Pulses for heals/buffs
        if (monsterVis != null && monsterHpAfter > monsterHpBefore)
            StartCoroutine(PulseTransform(monsterVis.transform, 0.18f, 1.08f));

        if (heroVis != null && heroHpAfter > heroHpBefore)
            StartCoroutine(PulseTransform(heroVis.transform, 0.18f, 1.08f));
    }

    private void ApplyResultUI(BattleSnapshotModel snapshot)
    {
        bool finished = snapshot != null && snapshot.battleFinished;

        if (resultPanel != null)
            resultPanel.SetActive(finished);

        if (resultText != null && finished)
            resultText.text = snapshot.winner == "HERO" ? "Hero wins" : "Monster wins";

        if (continueButton != null)
            continueButton.interactable = finished && !isBusy;
    }

    private void UpdateLoading(bool busy)
    {
        if (loadingOverlay != null)
            loadingOverlay.SetActive(busy);

        if (continueButton != null)
            continueButton.interactable = !busy && battleManager != null && battleManager.CurrentSnapshot != null && battleManager.CurrentSnapshot.battleFinished;
    }

    private IEnumerator EnsurePortraitSprite(BattleCharacterVisuals visuals, Sprite sprite)
    {
        if (visuals == null || sprite == null)
            yield break;

        SpriteRenderer renderer = visuals.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.sprite = sprite;

        Image image = visuals.GetComponent<Image>();
        if (image != null)
            image.sprite = sprite;

        yield break;
    }

    private IEnumerator PulseTransform(Transform target, float duration, float scaleFactor)
    {
        if (target == null)
            yield break;

        Vector3 original = target.localScale;
        Vector3 pulsed = original * scaleFactor;

        float half = duration * 0.5f;
        float time = 0f;
        while (time < half)
        {
            time += Time.deltaTime;
            target.localScale = Vector3.Lerp(original, pulsed, Mathf.Clamp01(time / half));
            yield return null;
        }

        time = 0f;
        while (time < half)
        {
            time += Time.deltaTime;
            target.localScale = Vector3.Lerp(pulsed, original, Mathf.Clamp01(time / half));
            yield return null;
        }

        target.localScale = original;
    }

    private Sprite LoadHeroSprite(string heroName)
    {
        if (string.IsNullOrWhiteSpace(heroName))
            heroName = "Knight of Dawn";

        Sprite direct = Resources.Load<Sprite>($"Heroes/{heroName}");
        if (direct != null)
            return direct;

        Sprite[] heroSheet = Resources.LoadAll<Sprite>("Heroes/rogues");
        if (heroSheet != null && heroSheet.Length > 0)
            return heroSheet[0];

        return null;
    }

    private Sprite LoadMonsterSprite(string monsterName)
    {
        if (string.IsNullOrWhiteSpace(monsterName))
            return null;

        Sprite direct = Resources.Load<Sprite>($"Monsters/{monsterName}");
        if (direct != null)
            return direct;

        Sprite[] monsterSheet = Resources.LoadAll<Sprite>("Monsters/AllMonstersSheet");
        if (monsterSheet != null)
        {
            Sprite match = monsterSheet.FirstOrDefault(s => string.Equals(s.name, monsterName, StringComparison.OrdinalIgnoreCase));
            if (match != null)
                return match;
        }

        return null;
    }

    private async void OnContinueClicked()
    {
        if (battleManager == null || runManager == null)
            return;

        BattleSnapshotModel snapshot = battleManager.CurrentSnapshot;
        if (snapshot == null || !snapshot.battleFinished)
            return;

        isBusy = true;
        UpdateLoading(true);

        try
        {
            bool heroWon = string.Equals(snapshot.winner, "HERO", StringComparison.OrdinalIgnoreCase);
            int encounterIndex = runManager.CurrentEncounterIndex;

            if (encounterIndex >= 0)
                await runManager.ResolveEncounterAsync(encounterIndex, heroWon);

            await battleManager.EndSessionAsync();
            SceneManager.LoadScene("02_Map");
        }
        catch (Exception ex)
        {
            Debug.LogError("Battle Error: " + ex.Message);
        }
        finally
        {
            isBusy = false;
            UpdateLoading(false);
        }
    }
}