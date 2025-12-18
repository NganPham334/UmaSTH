using UnityEngine;

public class NextScene : MonoBehaviour
{
    public void OnNextSceneButtonPressed()
    {
        GameStateMan.Instance.RequestState(GameStateMan.GameState.VisualNovel);
    }
}
