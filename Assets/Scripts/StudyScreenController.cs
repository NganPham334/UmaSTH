using UnityEngine;
using UnityEngine.SceneManagement;

public class StudyScreenController : MonoBehaviour
{
	public void OnRestButtonPressed()
	{
		Debug.Log("Rest button pressed");
	}
	
	public void OnReturnButtonPressed()
	{
		Debug.Log("Return button pressed");
		SceneManager.LoadScene("Game Scene");
	}
    
}
