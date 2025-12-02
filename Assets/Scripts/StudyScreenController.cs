using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StudyScreenController : MonoBehaviour
{	
	public TextMeshProUGUI spdText;
	public TextMeshProUGUI witText;
	public TextMeshProUGUI memText;
	public TextMeshProUGUI lukText;
	
	public void OnStudyButtonPressed(int statIndex)
	{
		StatType primaryStat = (StatType)statIndex;
		Debug.Log("Study Button Pressed for: " + primaryStat.ToString());
		
		if (StatsManager.Instance != null)
		{
			// Now take the StatType of Secondary Stat
			StatType secondaryStat = StatsManager.Instance.IncrementStat(primaryStat);
			UpdateStatDisplay(primaryStat); // Update text on screen for Primary Stat
			UpdateStatDisplay(secondaryStat); // Update text on screen for Secondary Stat
		}
		else
		{
			Debug.LogError("StatsManager.Instance is null. Is the StatsManager GameObject active in the scene?");
		}
	}
	
	public void UpdateStatDisplay(StatType statType)
	{
		if (StatsManager.Instance == null) return;
		
		int currentValue = StatsManager.Instance.GetStatValue(statType);
		
		switch (statType)
		{
			case StatType.SPD:
				if (spdText != null)
					spdText.text = currentValue.ToString();
				break;
			case StatType.WIT:
				if (witText != null)
					witText.text = currentValue.ToString();
				break;
			case StatType.MEM:
				if (memText != null)
					memText.text = currentValue.ToString();
				break;
			case StatType.LUK:
				if (lukText != null)
					lukText.text = currentValue.ToString();
				break;
		}
	}
		
	// Optional: Call UpdateStatDisplay when scene load to set initial value
	private void Start()
	{
		foreach (StatType type in System.Enum.GetValues(typeof(StatType)))
		{
			UpdateStatDisplay(type);
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
