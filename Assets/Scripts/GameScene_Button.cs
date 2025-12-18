using System.Collections.Generic;
using UnityEngine;

public class GameScene_Button : MonoBehaviour
{
    public void OnStudyButtonPressed()
    {
        GameStateMan.Instance.RequestState(GameStateMan.GameState.Training);
    }

    public void OnTestButtonPressed()
    {
        GameStateMan.Instance.RequestState(GameStateMan.GameState.PreTest);
    }

    public void OnPastTimePressed()
    {
         GameStateMan.Instance.RequestState(GameStateMan.GameState.VisualNovel, new() {{"vn_type", "pastime"}});
    }

    public void OnRestButtonPressed()
    {
        GameStateMan.Instance.RequestState(GameStateMan.GameState.VisualNovel, new() {{"vn_type", "rest"}});
        
    }
}
