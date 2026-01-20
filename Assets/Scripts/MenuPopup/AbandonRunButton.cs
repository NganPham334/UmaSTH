using UnityEngine;

public class AbandonRunButton : MonoBehaviour
{
    private const string SAVE_KEY = "SavedGameData";

    /// <summary>
    /// Used when "Give Up" is pressed while actively playing.
    /// Clears memory, deletes the file, and sends user home.
    /// </summary>
    public void OnQuitToHomePressed()
    {
        Debug.Log("Abandoning active run and returning to launcher...");
        WipeAllProgress();
        ReturnToLauncher();
    }

    /// <summary>
    /// Used in the Launcher menu to delete an existing save file.
    /// </summary>
    public void OnAbandonSavePressed()
    {
        Debug.Log("Deleting saved game data from disk...");
        WipeAllProgress();
        // No need to return to launcher as we are already there
    }

    private void WipeAllProgress()
    {
        // 1. Delete the physical save file from Desktop (PlayerPrefs)
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            PlayerPrefs.DeleteKey(SAVE_KEY);
            //PlayerPrefs.Save(); // Ensure it's deleted immediately
            Debug.Log("Save deleted .");
        }

        // 2. Clear the data in memory
        if (GameStateMan.Instance != null && GameStateMan.Instance.CurrentRun != null)
        {
            // Reset the object values to defaults
            GameStateMan.Instance.CurrentRun.InitializeRun();
            Debug.Log("CurrentRun object reset to defaults.");
        }
    }
    
    private void ReturnToLauncher()
    {
        if (GameStateMan.Instance != null)
        {
            GameStateMan.Instance.RequestState(GameStateMan.GameState.Launcher);
        }
    }
}