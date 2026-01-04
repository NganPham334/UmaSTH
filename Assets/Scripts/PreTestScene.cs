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
    // Tên Scene bạn muốn chuyển đến (nhập chính xác tên file .unity)
    [SerializeField] private string testSceneName = "Test scene";
    [SerializeField] private string gameSceneName = "Game Scene";

    private void Start()
    {
        // Cập nhật chỉ số khi vào màn hình
        StatBox.UpdateAllStats();

        if (currentRunData != null)
        {
            Debug.Log($"Scene PreTest Loaded. Current Turn: {currentRunData.CurrentTurn}");
        }
    }

    // Gắn hàm này vào sự kiện OnClick() của nút "Test"
    public void OnTestButtonPress()
    {
        // Kiểm tra xem tên Scene có trống không trước khi chuyển
        if (!string.IsNullOrEmpty(testSceneName))
        {
            SceneManager.LoadScene(testSceneName);
        }
        else
        {
            Debug.LogError("Chưa nhập tên Test Scene trong Inspector!");
        }
    }

    // Gắn hàm này vào sự kiện OnClick() của nút "Return"
    public void OnReturnButtonPress()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("Chưa nhập tên Game Scene trong Inspector!");
        }
    }
}