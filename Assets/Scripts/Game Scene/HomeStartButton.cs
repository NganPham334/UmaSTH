using UnityEngine;

public class HomeStudyButton : MonoBehaviour
{
    public void OnStudyButtonPressed()
    {
        // Go to Game Scene using the Training GameState
        GameStateMan.Instance.StartGame();
    }
}
