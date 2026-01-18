using UnityEngine;

public class QuitToHomeButton : MonoBehaviour
{
    [Header("Options")]
    [Tooltip("If true, shows a confirmation dialog before quitting (requires confirmation UI)")]
    public bool requireConfirmation = false;

    public void OnQuitToHomePressed()
    {
        if (requireConfirmation)
        {
            // TODO: Show confirmation dialog
            // For now, just quit directly
            Debug.LogWarning("Confirmation dialog not implemented yet, quitting directly");
        }

        QuitCurrentGame();
    }

    private void QuitCurrentGame()
    {
        if (GameStateMan.Instance == null)
        {
            Debug.LogError("GameStateMan instance not found!");
            return;
        }

        // Clear the current run data
        if (GameStateMan.Instance.CurrentRun != null)
        {
            // Reset/clear the current run
            // This will clear all progress in the current playthrough
            GameStateMan.Instance.CurrentRun.InitializeRun();
            Debug.Log("Current run data cleared");
        }
        else
        {
            Debug.LogWarning("CurrentRun is null, nothing to clear");
        }

        // Return to main menu (HomeScreen)
        GameStateMan.Instance.RequestState(GameStateMan.GameState.MainMenu);
        
        Debug.Log("Quit to home - current game abandoned");
    }
}