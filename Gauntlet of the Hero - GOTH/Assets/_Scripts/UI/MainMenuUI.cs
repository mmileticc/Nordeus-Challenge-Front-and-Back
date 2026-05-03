using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using GauntletOfTheHero.Gameplay.Managers; 
using System;

public class MainMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RunManager runManager;
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private void Start()
    {
        // Povezujemo klik dugmeta sa funkcijom
        startButton.onClick.AddListener(HandleStartRun);
        statusText.text = "Ready to start?";
    }

    private async void HandleStartRun()
    {
        // 1. UI Feedback
        startButton.interactable = false;
        statusText.text = "Contacting backend...";
        statusText.color = Color.white;

        try
        {
            // 2. Pozivamo tvoj RunManager
            // heroId ostavljamo null za sad (backend će dodeliti default)
            var config = await runManager.StartRunAsync();

            statusText.text = "Success! Loading map...";
            statusText.color = Color.green;

            // 3. Prelazak na mapu (Index 1 u Build Settings)
            SceneManager.LoadScene(1);
        }
        catch (Exception ex)
        {
            // 4. Ako backend nije upaljen ili pukne
            startButton.interactable = true;
            statusText.text = "Backend Error: Check if server is running.";
            statusText.color = Color.red;
            Debug.LogError($"StartRun failed: {ex.Message}");
        }
    }
    // Dodaj ovo u MainMenuUI klasu
    public void HandleExitGame()
    {
        Debug.Log("Exit button clicked!"); // Da vidiš u konzoli da radi dok si u editoru

        // Ugasi aplikaciju
        Application.Quit();

        // Napomena: Application.Quit ne radi unutar samog Unity Editora (dok klikćeš Play),
        // zato dodajemo ovo samo za testiranje u editoru:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}