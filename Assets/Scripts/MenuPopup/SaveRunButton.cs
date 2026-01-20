using UnityEngine;

public class SaveRunButton : MonoBehaviour
{
    private const string SAVE_KEY = "SavedGameData";

    // --- Emergency Auto-Save ---
    private void OnApplicationQuit()
    {
        // On Desktop, this triggers on Alt+F4 or clicking the 'X'
        Debug.Log("Application quitting: Emergency Save triggered.");
        SaveGame();
    }

    // --- UI Button Handlers ---

    public void OnSaveAndReturnHomePressed()
    {
        SaveGame();
        
        if (GameStateMan.Instance != null)
        {
            GameStateMan.Instance.RequestState(GameStateMan.GameState.Launcher);
        }
    }

    public void OnLoadingSavePressed()
    {
        if (HasSavedGame())
        {
            LoadGame();
            // After loading data, tell the manager to enter the game state
            GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
        }
        else
        {
            Debug.LogWarning("No save file found to continue from.");
        }
    }

    // --- Core Functionality ---

    public void SaveGame()
    {
        if (GameStateMan.Instance == null || GameStateMan.Instance.CurrentRun == null)
        {
            Debug.LogWarning("Save failed: GameStateMan or CurrentRun is null.");
            return;
        }

        string json = JsonUtility.ToJson(GameStateMan.Instance.CurrentRun);
        PlayerPrefs.SetString(SAVE_KEY, json);
        //PlayerPrefs.Save(); // Forces the data to disk immediately
        
        Debug.Log("Game saved successfully to PlayerPrefs!");
    }

    public static void LoadGame()
    {
        if (!HasSavedGame()) return;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        
        if (GameStateMan.Instance != null && GameStateMan.Instance.CurrentRun != null)
        {
            // This overwrites the values in the existing CurrentRun object
            JsonUtility.FromJsonOverwrite(json, GameStateMan.Instance.CurrentRun);
            Debug.Log("Game data restored successfully.");
        }
    }

    public static bool HasSavedGame()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }
}