using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PreTestScene : MonoBehaviour
{
    [Header("=== Player Stats (Left Panel) ===")]
    public TMP_Text PlayerSpeed;
    public TMP_Text PlayerMemory;
    public TMP_Text PlayerWit;
    public TMP_Text PlayerLuck;

    [Header("=== Exam Stats (Right Panel) ===")]
    public TMP_Text TestSpeed;
    public TMP_Text TestMemory;
    public TMP_Text TestWit;
    public TMP_Text TestLuck;

    [Header("=== Center UI ===")]
    public TMP_Text ExamName;
    public Button TestButton;
    public Button ReturnButton;

    [Header("Optional Test Controls")]
    public Button CancelButton;

    private CurrentRunData runData;
    private ScheduledExam examData;
    private bool isOptionalTest;



    // ================================================================
    // START
    // ================================================================
    void Start()
    {
        var gsm = GameStateMan.Instance;
        bool hasRealExam = false;

        // ---------------------------------------------------------------
        // TRY GET DATA FROM GAMESTATEMAN (REAL GAME MODE)
        // ---------------------------------------------------------------
        if (gsm != null)
        {
            runData = gsm.CurrentRunData;

            if (gsm.TryGetStateParameter("ExamData", out examData))
                hasRealExam = true;

            gsm.TryGetStateParameter("OptionalTest", out isOptionalTest);
        }

        // ---------------------------------------------------------------
        // TEST MODE: IF NO REAL EXAM DATA FOUND
        // ---------------------------------------------------------------
        if (!hasRealExam)
        {
            Debug.LogWarning("PreTestScene: Running TEST MODE (scene launched directly).");

            // mock run data
            runData = ScriptableObject.CreateInstance<CurrentRunData>();
            runData.Speed = 120;
            runData.Wit = 80;
            runData.Memory = 55;
            runData.Luck = 35;

            // mock exam
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

            isOptionalTest = true; // luôn cho hiện Cancel cho dễ test
        }


        // ---------------------------------------------------------------
        SetupUI();
        SetupButtons();
    }



    // ================================================================
    // SETUP UI
    // ================================================================
    private void SetupUI()
    {
        // Player Stats
        PlayerSpeed.text = runData.Speed.ToString();
        PlayerMemory.text = runData.Memory.ToString();
        PlayerWit.text = runData.Wit.ToString();
        PlayerLuck.text = runData.Luck.ToString();

        // Exam Name
        ExamName.text = examData.ExamName;

        // Exam Requirements
        int spd = 0, mem = 0, wit = 0, luk = 0;

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

        TestSpeed.text = spd.ToString();
        TestMemory.text = mem.ToString();
        TestWit.text = wit.ToString();
        TestLuck.text = luk.ToString();

        // Optional test → show cancel
        CancelButton.gameObject.SetActive(isOptionalTest);
    }



    // ================================================================
    // BUTTON SETUP
    // ================================================================
    private void SetupButtons()
    {
        // ---------------------------------------------------------------
        // TEST BUTTON
        // ---------------------------------------------------------------
        TestButton.onClick.AddListener(() =>
        {
            if (GameStateMan.Instance != null)
            {
                var parameter = new Dictionary<string, object>()
                {
                    { "ExamData", examData }
                };

                GameStateMan.Instance.RequestState(GameStateMan.GameState.Exam, parameter);
            }
            else
            {
                Debug.Log("TEST MODE: Test button clicked (no GameStateMan present).");
            }
        });



        // ---------------------------------------------------------------
        // RETURN BUTTON
        // ---------------------------------------------------------------
        ReturnButton.onClick.AddListener(() =>
        {
            if (GameStateMan.Instance != null)
            {
                GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
            }
            else
            {
                Debug.Log("TEST MODE: Return button clicked.");
            }
        });



        // ---------------------------------------------------------------
        // CANCEL (optional test only)
        // ---------------------------------------------------------------
        CancelButton.onClick.AddListener(() =>
        {
            if (GameStateMan.Instance != null)
            {
                GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
            }
            else
            {
                Debug.Log("TEST MODE: Cancel button clicked.");
            }
        });
    }
}
