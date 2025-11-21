using UnityEngine;

public static class GameStateExtensions
{
    // Specify scene names here so the manager can work correctly
    public static string GetSceneName(this GameStateMan.GameState state)
    {
        switch (state)
        {
            case GameStateMan.GameState.MainMenu:
                return "HomeScreen";
            case GameStateMan.GameState.Launcher:
                return "LauncherScreen";
            case GameStateMan.GameState.Resting:
                return "Resting Scene";
            case GameStateMan.GameState.RunEnd:
                return "Run End Scene";
            case GameStateMan.GameState.Training:
                return "Study Screen";
            case GameStateMan.GameState.PreTest:
                return "PreTestUI";  
            case GameStateMan.GameState.Exam:
                return "Test scene";
            case GameStateMan.GameState.StoryEvent:
                return "Visual Novel Screen";
            default:
                Debug.LogError($"No scene name defined for state: {state}");
                return "HomeScreen";
        }
    }
}