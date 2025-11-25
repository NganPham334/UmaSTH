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
		StatType selectedStat = (StatType)statIndex;
		Debug.Log("Study Button Pressed for: " + selectedStat.ToString());
		
		if (StatsManager.Instance != null)
		{
			StatsManager.Instance.IncrementStat(selectedStat);
			UpdateStatDisplay(selectedStat); // Update text on screen
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
