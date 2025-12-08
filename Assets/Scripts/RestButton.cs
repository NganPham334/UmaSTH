using System.Collections.Generic;
using UnityEngine;

public class RestButton : MonoBehaviour
{
    public void OnRestButtonPressed()
    {
        // Go to Resting Scene using the Resting GameState
        GameStateMan.Instance.RequestState(GameStateMan.GameState.VisualNovel, new() {{"vn_type", "rest"}});
    }
}
