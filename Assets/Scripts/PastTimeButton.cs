using UnityEngine;

public class PastTimeButton : MonoBehaviour
{
    public void OnPastTimePressed()
    {
        GameStateMan.Instance.RequestState(GameStateMan.GameState.VisualNovel, new() {{"vn_type", "pastime"}});
    }
}
