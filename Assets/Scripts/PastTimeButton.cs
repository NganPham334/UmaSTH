using UnityEngine;

public class PastTimeButton : MonoBehaviour
{
    public void OnPastTimePressed()
    {
        GameStateMan.Instance.RequestState(GameStateMan.GameState.PastTime);
    }
}
