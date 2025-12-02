using UnityEngine;

public class StudyButton : MonoBehaviour
{
    public void OnStudyButtonPressed()
    {
        // Go to Study Scene using the Study GameState
        GameStateMan.Instance.RequestState(GameStateMan.GameState.Training);
    }
}
