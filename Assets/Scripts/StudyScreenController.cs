using UnityEngine;
using UnityEngine.SceneManagement;

public class StudyScreenController : MonoBehaviour
{
	public void OnStudyButtonPressed(int statIndex)
	{
		StatType selectedStat = (StatType)statIndex;
		Debug.Log("Study Button Pressed for: " + selectedStat.ToString());
		
		if (StatsManager.Instance != null)
		{
			StatsManager.Instance.IncrementStat(selectedStat);
		}
		else
		{
			Debug.LogError("StatsManager.Instance is null. Is the StatsManager GameObject active in the scene?");
		}
	}
	
	public void OnRestButtonPressed()
	{
		Debug.Log("Rest button pressed");
		GameStateMan.Instance.RequestState(GameStateMan.GameState.Resting);
	}
	
	public void OnReturnButtonPressed()
	{
		Debug.Log("Return button pressed");
		SceneManager.LoadScene("Game Scene");
	}
    
}
