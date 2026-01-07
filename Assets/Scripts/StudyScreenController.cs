using UnityEngine;

public class StudyScreenController : MonoBehaviour
{	
	// REMOVED: All TextMeshProUGUI reference
	
	public void OnStudyButtonPressed(int statIndex)
	{
		StatType primaryStat = (StatType)statIndex;
		
		if (StatsManager.Instance != null)
		{
			StatsManager.Instance.ExecuteStudyAction(primaryStat);
			// REMOVED: UpdateStatDisplay method call
			// ExecuteStudyAction will call ApplyStatGain, so I don't need to call the method here again
			// ApplyStatGain in StatsManager calls SetStatValue -> trigger StatBox.UpdateAllStats()
			
			if (GameStateMan.Instance != null)
			{
				GameStateMan.Instance.ReportActionComplete();
			}
			
			Debug.Log($"Value Changed for {primaryStat}");
		}
		else
		{
			Debug.LogError("StatsManager.Instance is null. Check StatsManager in Script Execution Order");
		}
	}
	
	// REMOVED: UpdateStatDisplay(StatType statType)
	// REMOVED: Start() (StatBox.cs handle its own initialization)
	
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