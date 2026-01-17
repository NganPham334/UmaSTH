using UnityEngine;

public class SaveAndReturnHome : MonoBehaviour
{
    // Just in case we unexpectedly exited the game
    private void OnApplicationQuit()
    {
        Debug.Log("Application quitting: Emergency Save triggered.");
        SaveGame();
    }
    public void OnSaveAndReturnHomePressed()
    {
        // Save the current game state
        SaveGame();
        
        // Return to main menu
        GameStateMan.Instance.RequestState(GameStateMan.GameState.MainMenu);
    }

    private void SaveGame()
    {
        if (GameStateMan.Instance == null || GameStateMan.Instance.CurrentRun == null)
        {
            Debug.LogWarning("Cannot save: GameStateMan or CurrentRun is null");
            return;
        }

        // Serialize CurrentRunData to JSON
        string json = JsonUtility.ToJson(GameStateMan.Instance.CurrentRun);
        
        // Save to PlayerPrefs (or you can use file system)
        PlayerPrefs.SetString("SavedGameData", json);
        PlayerPrefs.Save();
        
        Debug.Log("Game saved successfully!");
    }

    // Optional: Method to load the saved game
    public static void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SavedGameData"))
        {
            Debug.LogWarning("No saved game found");
            return;
        }

        string json = PlayerPrefs.GetString("SavedGameData");
        
        if (GameStateMan.Instance != null && GameStateMan.Instance.CurrentRun != null)
        {
            JsonUtility.FromJsonOverwrite(json, GameStateMan.Instance.CurrentRun);
            Debug.Log("Game loaded successfully!");
        }
    }

    // Optional: Check if a save file exists
    public static bool HasSavedGame()
    {
        return PlayerPrefs.HasKey("SavedGameData");
    }
}