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
            case GameStateMan.GameState.VisualNovel:
                return "Visual Novel Screen";
            case GameStateMan.GameState.UpgradeEvent:
                return "Upgrade Scene";
            case GameStateMan.GameState.Training:
                return "Study Screen";
            case GameStateMan.GameState.GameScene:
                return "Game Scene";
            case GameStateMan.GameState.PreTest:
                return "PreTestUI";  
            case GameStateMan.GameState.Exam:
                return "Test scene";
            default:
                Debug.LogError($"No scene name defined for state: {state}");
                return "HomeScreen";
        }
    }
}