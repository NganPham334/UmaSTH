using UnityEngine;
using UnityEngine.UI;

public class LoadGameVariant : MonoBehaviour
{
    [Header("Button References")]
    [Tooltip("If assigned, button will be disabled when no save exists")]
    public Button loadButton;
    
    [Tooltip("If assigned, button will be disabled when no save exists")]
    public Button deleteButton;

    void Start()
    {
        // Update button state on start
        UpdateButtonState();
    }

    public void OnLoadGamePressed()
    {
        if (!HasSavedGame())
        {
            Debug.LogWarning("No saved game found!");
            return;
        }

        // Load the saved game data
        LoadGame();

        // Start the game from the saved state
        // Go directly to GameScene to continue playing
        GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
    }

    private void LoadGame()
    {
        if (!PlayerPrefs.HasKey("SavedGameData"))
        {
            Debug.LogWarning("No saved game data found");
            return;
        }

        string json = PlayerPrefs.GetString("SavedGameData");
        
        if (GameStateMan.Instance != null && GameStateMan.Instance.CurrentRun != null)
        {
            // Overwrite current run data with saved data
            JsonUtility.FromJsonOverwrite(json, GameStateMan.Instance.CurrentRun);
            Debug.Log("Game loaded successfully!");
        }
        else
        {
            Debug.LogError("Cannot load: GameStateMan or CurrentRun is null");
        }
    }

    private bool HasSavedGame()
    {
        return PlayerPrefs.HasKey("SavedGameData");
    }

    private void UpdateButtonState()
    {
        bool hasSave = HasSavedGame();
        
        // Disable/enable load button based on save existence
        if (loadButton != null)
        {
            loadButton.interactable = hasSave;
        }
        
        // Disable/enable delete button based on save existence
        if (deleteButton != null)
        {
            deleteButton.interactable = hasSave;
        }
    }

    // Optional: Call this to refresh button state (e.g., after deleting save)
    public void RefreshButtonState()
    {
        UpdateButtonState();
    }

    // Optional: Delete saved game
    public void DeleteSavedGame()
    {
        if (PlayerPrefs.HasKey("SavedGameData"))
        {
            PlayerPrefs.DeleteKey("SavedGameData");
            PlayerPrefs.Save();
            Debug.Log("Saved game deleted");
            UpdateButtonState();
        }
    }
}