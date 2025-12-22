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
            runData = gsm.CurrentRunData;

            if (gsm.TryGetStateParameter("ExamData", out examData))
                hasRealExam = true;

            gsm.TryGetStateParameter("OptionalTest", out isOptionalTest);
        }

        // ================= TEST MODE =================
        if (!hasRealExam)
        {
            Debug.LogWarning("PreTestScene: TEST MODE");

            runData = ScriptableObject.CreateInstance<CurrentRunData>();
            runData.Speed = 120;
            runData.Wit = 80;
            runData.Memory = 55;
            runData.Luck = 35;

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
        // -------- PLAYER STATS --------
        PlayerSpeed.text  = runData.Speed.ToString();
        PlayerMemory.text = runData.Memory.ToString();
        PlayerWit.text    = runData.Wit.ToString();
        PlayerLuck.text   = runData.Luck.ToString();

        // -------- EXAM INFO --------
        ExamName.text = examData != null ? examData.ExamName : "UNKNOWN EXAM";

        // -------- EXAM STATS --------
        if (examData != null)
        {
            TestSpeed.text  = examData.GetStatValue(StatType.spd).ToString();
            TestWit.text    = examData.GetStatValue(StatType.wit).ToString();
            TestMemory.text = examData.GetStatValue(StatType.mem).ToString();
            TestLuck.text   = examData.GetStatValue(StatType.luk).ToString();
        }
        else
        {
            TestSpeed.text  = "-";
            TestWit.text    = "-";
            TestMemory.text = "-";
            TestLuck.text   = "-";
        }
    }

    private void SetupButtons()
    {
        TestButton.onClick.RemoveAllListeners();
        ReturnButton.onClick.RemoveAllListeners();

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
        });

        ReturnButton.onClick.AddListener(() =>
        {
            if (GameStateMan.Instance != null)
            {
                GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
            }
        });
    }
}
