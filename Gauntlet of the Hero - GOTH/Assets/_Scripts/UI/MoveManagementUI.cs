using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using GauntletOfTheHero.Gameplay.Managers;
using GauntletOfTheHero.Backend.Models;
using System.Linq;
using UnityEngine.SceneManagement;

public class MoveManagementUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform contentContainer;
    public GameObject moveItemPrefab;
    public TextMeshProUGUI counterText;
    public Button applyButton;
    public Button backButton;

    private List<string> localSelectedIds = new List<string>();
    private const int MAX_MOVES = 4;
    private RunManager runManager;

    private void Awake()
    {
        runManager = FindAnyObjectByType<RunManager>();
    }

    private void Start()
    {
        if (runManager != null && runManager.CurrentRun != null)
        {
            Setup(runManager.CurrentRun);
        }
        else
        {
            Debug.LogError("RunManager ili CurrentRun nisu dostupni u ovoj sceni!");
            enabled = false;
            return;
        }

        if (applyButton != null) applyButton.onClick.AddListener(OnApplyClicked);
        if (backButton != null) backButton.onClick.AddListener(OnBackClicked);
    }

    public void Setup(RunConfigDto run)
    {
        if (contentContainer != null)
        {
            foreach (Transform child in contentContainer) Destroy(child.gameObject);
        }

        // Uzimamo trenutno opremljene poteze iz backenda i konvertujemo u stringove
        localSelectedIds = run?.equippedMoveIds?.Select(id => id.ToString()).ToList() ?? new List<string>();

        // Safety checks: ensure we have what we need to generate UI
        if (run?.learnedMoves == null || moveItemPrefab == null || contentContainer == null)
        {
            if (moveItemPrefab == null)
                Debug.LogWarning("MoveManagementUI: moveItemPrefab is not assigned in the Inspector. Skipping move list generation.");
            return;
        }

        foreach (var move in run.learnedMoves)
        {
            var itemGo = Instantiate(moveItemPrefab, contentContainer);
            var uiItem = itemGo.GetComponent<MoveItemUI>();

            if (uiItem != null)
            {
                // MoveItemUI stores id as string in this project
                uiItem.moveId = move.id.ToString();
                uiItem.moveInfoText.text = BuildMoveInfo(move);
                uiItem.moveToggle.SetIsOnWithoutNotify(localSelectedIds.Contains(move.id.ToString()));

                string moveIdStr = move.id.ToString();
                uiItem.moveToggle.onValueChanged.AddListener((isOn) =>
                {
                    HandleToggleChange(moveIdStr, isOn, uiItem.moveToggle);
                });
            }
        }
        UpdateCounterUI();
    }

    private string BuildMoveInfo(GauntletOfTheHero.Backend.Models.MoveDto move)
    {
        if (move == null) return string.Empty;

        // Effect summary (Damage / Heal / Buff)
        string effect = move.effectType.ToString();
        string type = move.moveType.ToString();
        string power = move.power > 0 ? $"PWR {move.power}" : "";

        string buff = "";
        if (move.effectType.ToString() == "Buff" && move.buffAttribute.HasValue)
        {
            buff = $"{move.buffAttribute.Value} +{move.buffAmount} ({move.buffDuration} turns)";
        }

        var parts = new System.Collections.Generic.List<string> { effect };
        if (!string.IsNullOrEmpty(type)) parts.Add(type);
        if (!string.IsNullOrEmpty(power)) parts.Add(power);
        if (!string.IsNullOrEmpty(buff)) parts.Add(buff);

        return string.Join(" | ", parts);
    }

    private void HandleToggleChange(string id, bool isOn, Toggle toggle)
    {
        if (isOn)
        {
            if (localSelectedIds.Count < MAX_MOVES)
            {
                if (!localSelectedIds.Contains(id)) localSelectedIds.Add(id);
            }
            else
            {
                toggle.SetIsOnWithoutNotify(false);
            }
        }
        else
        {
            localSelectedIds.Remove(id);
        }
        UpdateCounterUI();
    }

    private void UpdateCounterUI()
    {
        if (counterText != null)
            counterText.text = $"Selected: {localSelectedIds.Count}/{MAX_MOVES}";
        if (applyButton != null)
            applyButton.interactable = localSelectedIds.Count > 0;
    }

    public async void OnApplyClicked()
    {
        try 
        {
        
            if (runManager == null) return;
            var longIds = localSelectedIds.Select(s => long.Parse(s)).ToList();
            await runManager.EquipMovesAsync(longIds);
            SceneManager.LoadScene("02_Map");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Greška pri opremanju: {ex.Message}");
        }
    }

    public void OnBackClicked()
    {
        SceneManager.LoadScene("02_Map");
    }
}