using UnityEngine;

public class PastTimeButton : MonoBehaviour
{
    public void OnPastTimePressed()
    {
        // TODO: set params to define visualnovel type
        GameStateMan.Instance.RequestState(GameStateMan.GameState.VisualNovel);
    }
}
