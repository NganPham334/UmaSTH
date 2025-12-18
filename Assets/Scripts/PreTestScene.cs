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
    
    //Start
    void Start()
    {
        var gsm = GameStateMan.Instance;
        bool hasRealExam = false;

        //Try get exam data from GameStateMan
        if (gsm != null)
        {
            runData = gsm.CurrentRunData;

            if (gsm.TryGetStateParameter("ExamData", out examData))
                hasRealExam = true;

            gsm.TryGetStateParameter("OptionalTest", out isOptionalTest);
        }

        //Test mode
        if (!hasRealExam)
        {
            Debug.LogWarning("PreTestScene: Running TEST MODE.");

            //mock run data
            runData = ScriptableObject.CreateInstance<CurrentRunData>();
            runData.Speed = 120;
            runData.Wit = 80;
            runData.Memory = 55;
            runData.Luck = 35;

            //mock exam
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

    //Setup UI
    private void SetupUI()
    {
        //fill player stats
        PlayerSpeed.text = runData.Speed.ToString();
        PlayerMemory.text = runData.Memory.ToString();
        PlayerWit.text = runData.Wit.ToString();
        PlayerLuck.text = runData.Luck.ToString();

        //exam name
        ExamName.text = examData.ExamName;

        //requirements
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
    }

    //Setup buttons
    private void SetupButtons()
    {
        //Test button
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
                Debug.Log("TEST MODE: Test button clicked.");
            }
        });

        //Return button
        ReturnButton.onClick.RemoveAllListeners();

        if (isOptionalTest)
        {
            ReturnButton.onClick.AddListener(() =>
            {
                Debug.Log("Optional Test: Return → Cancel");
                if (GameStateMan.Instance != null)
                {
                    GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
                }
            });
        }
        else
        {
            ReturnButton.onClick.AddListener(() =>
            {
                Debug.Log("Real Exam: Return → Go back to GameScene");
                if (GameStateMan.Instance != null)
                {
                    GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
                }
            });
        }
    }
}
