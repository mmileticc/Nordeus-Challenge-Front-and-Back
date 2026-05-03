using UnityEngine;
using TMPro;
using GauntletOfTheHero.Gameplay.Managers;
using GauntletOfTheHero.Backend.Models;
using System.Collections.Generic;
using System.Linq;

public class MapUI : MonoBehaviour
{
    [Header("Hero Stats")]
    public TextMeshProUGUI heroNameText;
    public TextMeshProUGUI heroStatsText;
    public TextMeshProUGUI equippedMovesText;

    [Header("Encounters")]
    public Transform encounterContainer;
    public GameObject encounterPrefab;

    [Header("Buttons")]
    public UnityEngine.UI.Button moveManagementButton;
    public UnityEngine.UI.Button refreshRunButton;

    [Header("Error/Status")]
    public TextMeshProUGUI statusText;

    private RunManager runManager;
    private bool isBusy;

   
    private void Awake()
    {
        runManager = RunManager.Instance;
        if (runManager == null)
        {
            Debug.LogError("RunManager not found in scene!");
            enabled = false;
            return;
        }
    }
    private async void Start()
    {
        // Ako nema podataka (startovali smo direktno iz Map scene za test)
        if (runManager.CurrentRun == null)
        {
            statusText.text = "No run detected. Attempting to refresh...";
            try {
                await runManager.RefreshRunAsync();
            } catch {
                statusText.text = "Failed to load run. Please start from Main Menu.";
            }
        }
    }

    private void OnEnable()
    {
        if (runManager == null) return;
        
        runManager.RunUpdated += RefreshUI;
        if (runManager.CurrentRun != null)
        {
            RefreshUI(runManager.CurrentRun);
        }
        
        if (moveManagementButton != null)
            moveManagementButton.onClick.AddListener(OnMoveManagementClicked);
        if (refreshRunButton != null)
            refreshRunButton.onClick.AddListener(OnRefreshRunClicked);
    }

    private void OnDisable()
    {
        if (runManager == null) return;
        
        runManager.RunUpdated -= RefreshUI;
        
        if (moveManagementButton != null)
            moveManagementButton.onClick.RemoveListener(OnMoveManagementClicked);
        if (refreshRunButton != null)
            refreshRunButton.onClick.RemoveListener(OnRefreshRunClicked);
    }

    private void RefreshUI(RunConfigDto run)
    {
        if (run == null) return;

        // 1. Update Hero Stats
        if (heroNameText != null)
            heroNameText.text = $"{run.hero.heroName} LVL {run.hero.level}";
        
        if (heroStatsText != null)
            heroStatsText.text = $" XP: {run.hero.xp}/{run.hero.xpToNextLevel} \n HP: {run.hero.currentHealth}/{run.hero.maxHealth} \n ATK: {run.hero.attack} \n DEF: {run.hero.defense} \n MAG: {run.hero.magic}";

        // 2. Show Equipped Moves
        if (equippedMovesText != null)
        {
            if (run.equippedMoveIds != null && run.equippedMoveIds.Count > 0)
            {
                var equippedMoveNames = run.learnedMoves
                    .Where(m => run.equippedMoveIds.Contains(m.id))
                    .Select(m => m.name)
                    .ToList();
                equippedMovesText.text = "Equipped: \n " + (equippedMoveNames.Count > 0 ? string.Join(",\n ", equippedMoveNames) : "None");
            }
            else
            {
                equippedMovesText.text = "Equipped: None";
            }
        }

        // 3. Update Encounters
        if (encounterContainer != null && encounterPrefab != null)
        {
            foreach (Transform child in encounterContainer)
                Destroy(child.gameObject);
            // Učitava sve sprajtove iz tog jednog sheet-a
            Sprite[] allSprites = Resources.LoadAll<Sprite>("Monsters/AllMonstersSheet");


            for (int i = 0; i < run.encounters.Count; i++)
            {
                
                var encounter = run.encounters[i];
                var itemGo = Instantiate(encounterPrefab, encounterContainer); // Pravi kopiju prefaba

                // Uzimamo našu novu skriptu sa prefaba
                var ui = itemGo.GetComponent<EncounterUIElement>();

                if (ui != null)
                {
                    // Postavljamo tekst
                    ui.infoText.text = $"{encounter.monsterName}\nRank: {encounter.difficultyRank}";

                    // Učitavamo sliku iz Resources/Monsters/ ImeMonstruma
                    // Traži onaj koji se zove kao monstrum
                    Sprite s = System.Array.Find(allSprites, s => s.name == encounter.monsterName);
                    Sprite ms = Resources.Load<Sprite>("Monsters/" + encounter.monsterName );
                    if (s != null) ui.monsterIcon.sprite = s;

                    // Povezujemo klik
                    int index = i;
                    ui.actionButton.onClick.AddListener(() => OnEncounterClicked(index));
                        ui.actionButton.interactable = encounter.unlocked;
                        // Allow replaying cleared encounters for XP/grind per design.
                        ui.infoText.text = $"{encounter.monsterName}\nRank: {encounter.difficultyRank}" + (encounter.defeated ? " (Cleared)" : "");
                }
            }
        }
    }

    private async void OnEncounterClicked(int index)
    {
        if (isBusy) return;
        
        isBusy = true;
        try
        {
            await runManager.StartEncounterAsync(index);
            UnityEngine.SceneManagement.SceneManager.LoadScene("04_Battle");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to start encounter: {ex.Message}");
            if (statusText != null)
                statusText.text = "Error: " + ex.Message;
        }
        finally
        {
            isBusy = false;
        }
    }

    private async void OnMoveManagementClicked()
    {
        if (isBusy) return;
        
        try
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("03_MoveManagement");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load Move Management scene: {ex.Message}");
        }
    }

    private async void OnRefreshRunClicked()
    {
        if (isBusy) return;
        
        isBusy = true;
        try
        {
            await runManager.RefreshRunAsync();
            if (statusText != null)
                statusText.text = "Run refreshed!";
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to refresh run: {ex.Message}");
            if (statusText != null)
                statusText.text = "Error: " + ex.Message;
        }
        finally
        {
            isBusy = false;
        }
    }
}