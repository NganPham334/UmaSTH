using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PreTestScene : MonoBehaviour
{
    [Header("Player Stats")]
    public TMP_Text PlayerSpeed;
    public TMP_Text PlayerMemory;
    public TMP_Text PlayerWit;
    public TMP_Text PlayerLuck;

    [Header("Exam Stats")]
    public TMP_Text TestSpeed;
    public TMP_Text TestMemory;
    public TMP_Text TestWit;
    public TMP_Text TestLuck;

    [Header("Exam Info")]
    public TMP_Text ExamName;

    [Header("Buttons")]
    public Button TestButton;
    public Button ReturnButton;

    private CurrentRunData runData;
    private ScheduledExam examData;
    private bool isOptionalTest;

    void Start()
    {
        var gsm = GameStateMan.Instance;
        bool hasRealExam = false;

        if (gsm != null)
        {
            runData = gsm.CurrentRun;

            if (gsm.TryGetStateParameter("ExamData", out examData))
                hasRealExam = true;

            gsm.TryGetStateParameter("OptionalTest", out isOptionalTest);
        }

        // ================= TEST MODE =================
        // Nếu không có dữ liệu thật, tự tạo dữ liệu giả để test
        if (!hasRealExam)
        {
            Debug.LogWarning("PreTestScene: TEST MODE ENABLED (Running with fake data)");

            // Tạo instance mới nếu runData chưa có
            if (runData == null)
            {
                runData = ScriptableObject.CreateInstance<CurrentRunData>();
                runData.Speed = 120;
                runData.Wit = 80;
                runData.Memory = 55;
                runData.Luck = 35;
            }

            examData = new ScheduledExam
            {
                ExamName = "TEST MODE EXAM",
                Speed = 100,
                Wit = 60,
                Memory = 50,
                Luck = 40
            };

            isOptionalTest = true;
        }

        SetupUI();
        SetupButtons();
    }

    private void SetupUI()
    {
        // Kiểm tra an toàn: Nếu runData bị null thì dừng ngay, tránh crash
        if (runData == null)
        {
            Debug.LogError("PreTestScene: RunData is NULL! Cannot setup UI.");
            return;
        }

        // -------- PLAYER STATS (Sử dụng dấu ? để tránh lỗi NullReference) --------
        if (PlayerSpeed != null)  PlayerSpeed.text  = runData.Speed.ToString();
        if (PlayerMemory != null) PlayerMemory.text = runData.Memory.ToString();
        if (PlayerWit != null)    PlayerWit.text    = runData.Wit.ToString();
        if (PlayerLuck != null)   PlayerLuck.text   = runData.Luck.ToString();

        // -------- EXAM INFO --------
        if (ExamName != null)
        {
            ExamName.text = examData != null ? examData.ExamName : "UNKNOWN EXAM";
        }

        // -------- EXAM STATS --------
        if (examData != null)
        {
            if (TestSpeed != null)  TestSpeed.text  = examData.GetStatValue(StatType.spd).ToString();
            if (TestWit != null)    TestWit.text    = examData.GetStatValue(StatType.wit).ToString();
            if (TestMemory != null) TestMemory.text = examData.GetStatValue(StatType.mem).ToString();
            if (TestLuck != null)   TestLuck.text   = examData.GetStatValue(StatType.luk).ToString();
        }
        else
        {
            if (TestSpeed != null)  TestSpeed.text  = "-";
            if (TestWit != null)    TestWit.text    = "-";
            if (TestMemory != null) TestMemory.text = "-";
            if (TestLuck != null)   TestLuck.text   = "-";
        }
    }

    private void SetupButtons()
    {
        // Kiểm tra TestButton trước khi gán sự kiện
        if (TestButton != null)
        {
            TestButton.onClick.RemoveAllListeners();
            TestButton.onClick.AddListener(() =>
            {
                if (GameStateMan.Instance != null)
                {
                    GameStateMan.Instance.RequestState(
                        GameStateMan.GameState.Exam,
                        new Dictionary<string, object>
                        {
                            { "ExamData", examData }
                        }
                    );
                }
                else
                {
                    Debug.Log("Test Button Clicked (No GameStateMan found)");
                }
            });
        }
        else
        {
            Debug.LogWarning("PreTestScene: 'TestButton' chưa được gán trong Inspector!");
        }

        // Kiểm tra ReturnButton trước khi gán sự kiện
        if (ReturnButton != null)
        {
            ReturnButton.onClick.RemoveAllListeners();
            ReturnButton.onClick.AddListener(() =>
            {
                if (GameStateMan.Instance != null)
                {
                    GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
                }
                else
                {
                    Debug.Log("Return Button Clicked (No GameStateMan found)");
                }
            });
        }
        else
        {
            Debug.LogWarning("PreTestScene: 'ReturnButton' chưa được gán trong Inspector!");
        }
    }
}