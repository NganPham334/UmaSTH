using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PreTestScene : MonoBehaviour
{
    [Header("Data References")]
    [SerializeField] private CurrentRunData currentRunData;
    [SerializeField] private ExamSchedule examSchedule;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI dateText;

    [Header("Scene Configuration")]
    // The names of the scenes to load
    [SerializeField] private string testSceneName = "Test scene";
    [SerializeField] private string gameSceneName = "Game Scene";

    private void Start()
    {
        if (currentRunData != null)
        {
            Debug.Log($"PreTest Loaded. Current Turn: {currentRunData.CurrentTurn}");
        }

        if (dateText != null)
        {
            dateText.text = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }
    }

    // Attach this function to the OnClick() event of the "Start Test" button
    public void OnTestButtonPress()
    {
        // Check if the test scene name is set
        if (!string.IsNullOrEmpty(testSceneName))
        {
            SceneManager.LoadScene(testSceneName);
        }
        else
        {
            Debug.LogError("Test Scene name missing in Inspector!");
        }
    }

    // Attach this function to the OnClick() event of the "Return" button
    public void OnReturnButtonPress()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("Game Scene name missing in Inspector!");
        }
    }
}