using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayFromFirstScene
{
    static PlayFromFirstScene()
    {
        // Putanja do tvoje početne scene (proveri da li se zove MainMenu)
        string scenePath = "Assets/Scenes/01_Main_Menu.unity"; 
        
        SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

        if (sceneAsset != null)
        {
            // Postavlja scenu koja će se pokrenuti na Play
            EditorSceneManager.playModeStartScene = sceneAsset;
            Debug.Log("Unity će uvek pokrenuti: " + scenePath);
        }
        else
        {
            Debug.LogError("PlayFromFirstScene: Scena nije pronađena na putanji " + scenePath);
        }
    }
}