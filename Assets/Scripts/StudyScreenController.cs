using UnityEngine;

public class StudyScreenController : MonoBehaviour
{	
	// REMOVED: OnStudyButtonPressed(int statIndex)
	// in favour of OnPointerClick method in StudyButton.cs
	
	public void OnRestButtonPressed()
	{
		Debug.Log("Rest button pressed");
		if (GameStateMan.Instance != null)
		{
			GameStateMan.Instance.RequestState(GameStateMan.GameState.VisualNovel, new() {{"vn_type", "rest"}});
		}
	}
	
	public void OnReturnButtonPressed()
	{
		Debug.Log("Return button pressed");
		if (GameStateMan.Instance != null)
		{
			GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
		}
	}
}