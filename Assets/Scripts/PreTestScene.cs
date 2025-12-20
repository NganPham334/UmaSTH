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

    [Header("Center UI")]
    public TMP_Text ExamName;
    public Button TestButton;
    public Button ReturnButton;

    private CurrentRunData runData;
    private ScheduledExam examData;
    private bool isOptionalTest;

    // =========================================================
    // START
    // =========================================================
    void Start()
    {
        var gsm = GameStateMan.Instance;
        bool hasRealExam = false;

        // -----------------------------------------------------
        // TRY GET REAL DATA FROM GAMESTATE MANAGER
        // -----------------------------------------------------
        if (gsm != null)
        {
            runData = gsm.CurrentRunData;

            if (gsm.TryGetStateParameter("ExamData", out examData))
                hasRealExam = true;

            gsm.TryGetStateParameter("OptionalTest", out isOptionalTest);
        }

        // -----------------------------------------------------
        // TEST MODE (SCENE RUN DIRECTLY)
        // -----------------------------------------------------
        if (!hasRealExam)
        {
            Debug.LogWarning("PreTestScene: Running TEST MODE.");

            // Mock run data
            runData = ScriptableObject.CreateInstance<CurrentRunData>();
            runData.Speed = 120;
            runData.Wit = 80;
            runData.Memory = 55;
            runData.Luck = 35;

            // Mock exam
            examData = new ScheduledExam()
            {
                ExamName = "TEST MODE EXAM",
                Requirements = new List<StatRequirement>()
                {
                    new StatRequirement { Stat = StatRequirement.StatType.SPD, MinValue = 60 },
                    new StatRequirement { Stat = StatRequirement.StatType.WIT, MinValue = 50 },
                    new StatRequirement { Stat = StatRequirement.StatType.MEM, MinValue = 30 },
                    new StatRequirement { Stat = StatRequirement.StatType.LUK, MinValue = 20 }
                }
            };

            isOptionalTest = true;
        }

        SetupUI();
        SetupButtons();
    }

    // =========================================================
    // SETUP UI
    // =========================================================
    private void SetupUI()
    {
        // Player stats
        PlayerSpeed.text = runData.Speed.ToString();
        PlayerMemory.text = runData.Memory.ToString();
        PlayerWit.text = runData.Wit.ToString();
        PlayerLuck.text = runData.Luck.ToString();

        // Exam name (safe)
        ExamName.text = examData?.ExamName ?? "UNKNOWN EXAM";

        // Exam requirements
        int spd = 0, mem = 0, wit = 0, luk = 0;

        if (examData != null && examData.Requirements != null)
        {
            foreach (var req in examData.Requirements)
            {
                switch (req.Stat)
                {
                    case StatRequirement.StatType.SPD: spd = req.MinValue; break;
                    case StatRequirement.StatType.MEM: mem = req.MinValue; break;
                    case StatRequirement.StatType.WIT: wit = req.MinValue; break;
                    case StatRequirement.StatType.LUK: luk = req.MinValue; break;
                }
            }
        }

        TestSpeed.text = spd.ToString();
        TestMemory.text = mem.ToString();
        TestWit.text = wit.ToString();
        TestLuck.text = luk.ToString();
    }

    // =========================================================
    // BUTTON SETUP
    // =========================================================
    private void SetupButtons()
    {
        TestButton.onClick.RemoveAllListeners();
        ReturnButton.onClick.RemoveAllListeners();

        // ------------------------------
        // TEST BUTTON
        // ------------------------------
        TestButton.onClick.AddListener(() =>
        {
            if (GameStateMan.Instance != null)
            {
                var param = new Dictionary<string, object>
                {
                    { "ExamData", examData }
                };

                GameStateMan.Instance.RequestState(
                    GameStateMan.GameState.Exam,
                    param
                );
            }
            else
            {
                Debug.Log("TEST MODE: Test button clicked.");
            }
        });

        // ------------------------------
        // RETURN BUTTON
        // (Cancel + Return merged)
        // ------------------------------
        ReturnButton.onClick.AddListener(() =>
        {
            Debug.Log(isOptionalTest
                ? "Optional Test: Return = Cancel"
                : "Forced Exam: Return");

            if (GameStateMan.Instance != null)
            {
                GameStateMan.Instance.RequestState(
                    GameStateMan.GameState.GameScene
                );
            }
        });
    }
}
