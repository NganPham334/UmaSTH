using UnityEngine;

public class SaveRunButton : MonoBehaviour
{
    private const string SAVE_KEY = "SavedGameData";

    // --- UI Button Handlers (Bridges for MenuPopupLoader) ---

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
            // Transition is handled by GameStateMan to ensure parameters/logic are set
            GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
        }
        else
        {
            Debug.LogWarning("No save file found to continue from.");
        }
    }

    // --- Core Logic (Static so other scripts can call it) ---

    public static void SaveGame()
    {
        if (GameStateMan.Instance == null || GameStateMan.Instance.CurrentRun == null)
        {
            Debug.LogWarning("Save failed: GameStateMan or CurrentRun is null.");
            return;
        }

        string json = JsonUtility.ToJson(GameStateMan.Instance.CurrentRun);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save(); 
        
        Debug.Log("<color=green>Save System:</color> Game saved successfully.");
    }

    public static void LoadGame()
    {
        if (!HasSavedGame()) return;

        string json = PlayerPrefs.GetString(SAVE_KEY);
        
        if (GameStateMan.Instance != null && GameStateMan.Instance.CurrentRun != null)
        {
            JsonUtility.FromJsonOverwrite(json, GameStateMan.Instance.CurrentRun);
            Debug.Log("<color=cyan>Save System:</color> Data restored via Overwrite.");
        }
    }

    public static bool HasSavedGame()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }
}