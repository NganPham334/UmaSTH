using UnityEngine;
using UnityEngine.UI;

public class LoadGameButton : MonoBehaviour
{
    [Header("Button References")]
    [Tooltip("Load/Continue button - enabled when save exists")]
    public Button loadButton;
    
    [Tooltip("Delete button - enabled when save exists")]
    public Button deleteButton;
    
    [Tooltip("New Game button - enabled when no save exists OR always enabled")]
    public Button newGameButton;
    
    [Header("New Game Behavior")]
    [Tooltip("If true, New Game is only available when no save exists. If false, always available.")]
    public bool exclusiveNewGame = true;

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
        if (GameStateMan.Instance != null)
        {
            GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
        }
        else
        {
            Debug.LogError("GameStateMan instance not found!");
        }
    }

    public void OnNewGamePressed()
    {
        // Clear any existing save data (optional - remove if you want to keep old save)
        if (exclusiveNewGame && HasSavedGame())
        {
            Debug.Log("Starting new game will overwrite existing save");
        }
        
        // Start a fresh new game
        if (GameStateMan.Instance != null)
        {
            GameStateMan.Instance.StartGame();
        }
        else
        {
            Debug.LogError("GameStateMan instance not found!");
        }
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
        
        if (exclusiveNewGame)
        {
            // Exclusive mode: Only show either "Continue/Delete" OR "New Game"
            // When save exists: show Continue/Delete, hide New Game
            // When no save: hide Continue/Delete, show New Game
            
            if (loadButton != null)
                loadButton.interactable = hasSave;
            
            if (deleteButton != null)
                deleteButton.interactable = hasSave;
            
            if (newGameButton != null)
                newGameButton.interactable = !hasSave;
        }
        else
        {
            // Non-exclusive mode: New Game always available
            // Continue/Delete only available when save exists
            
            if (loadButton != null)
                loadButton.interactable = hasSave;
            
            if (deleteButton != null)
                deleteButton.interactable = hasSave;
            
            if (newGameButton != null)
                newGameButton.interactable = true;
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