using UnityEngine;

public class TestButton : MonoBehaviour
{
    public void OnTestButtonPressed()
    {
        // Go to Study Scene using the Petest GameState
        GameStateMan.Instance.RequestState(GameStateMan.GameState.PreTest);
    }
}
