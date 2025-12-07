using UnityEngine;

public class RestButton : MonoBehaviour
{
    public void OnRestButtonPressed()
    {
        // Go to Resting Scene using the Resting GameState
        // TODO: set params to define visualnovel type
        GameStateMan.Instance.RequestState(GameStateMan.GameState.VisualNovel);
    }
}
