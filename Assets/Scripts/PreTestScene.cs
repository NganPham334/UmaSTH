using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PreTestScene : MonoBehaviour
{
    [Header("Bindings")]
    public TMP_Text SpeedText;
    public TMP_Text MemoryText;
    public TMP_Text WitText;
    public TMP_Text LuckText;
    public TMP_Text TurnText;
    public TMP_Text ExamNameText;

    public Button TestButton;
    public Button ReturnButton;
    public Button CancelButton;

    private CurrentRunData runData;
    private ExamData examData;
    private bool isOptionalTest = false;

    void Start()
    {
        // Lấy SO hiện tại
        runData = GameStateMan.Instance.CurrentRunData;

        // Lấy dữ liệu Exam từ GameStateMan
        if (!GameStateMan.Instance.TryGetStateParameter("ExamData", out examData))
        {
            Debug.LogError("PreTestScene: ExamData was not provided!");
            return;
        }

        // Lấy flag Optional test
        GameStateMan.Instance.TryGetStateParameter("OptionalTest", out isOptionalTest);

        SetupUI();
        SetupButtonLogic();
    }

    void SetupUI()
    {
        // Stats
        SpeedText.text = runData.Speed.ToString();
        MemoryText.text = runData.Memory.ToString();
        WitText.text = runData.Wit.ToString();
        LuckText.text = runData.Luck.ToString();

        // Turn
        TurnText.text = "Turn " + runData.CurrentTurn;

        // Exam name
        if (examData != null)
            ExamNameText.text = examData.ExamName;
        else
            ExamNameText.text = "Unknown Test";

        // Optional test → show cancel
        CancelButton.gameObject.SetActive(isOptionalTest);
    }

    void SetupButtonLogic()
    {
        TestButton.onClick.AddListener(() =>
        {
            // Chuyển sang state Exam (Test Scene)
            var param = new Dictionary<string, object>
            {
                { "ExamData", examData }
            };

            GameStateMan.Instance.RequestState(GameStateMan.GameState.Exam, param);
        });

        ReturnButton.onClick.AddListener(() =>
        {
            if (isOptionalTest == true)
            {
                // Quay lại GameScene
                GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
            }
            else
            {
                // Forced test logic
                if (runData.CurrentTurn > 1)
                {
                    GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
                }
                else
                {
                    Debug.Log("PreTestScene: Return blocked because Turn == 1 and this is not optional.");
                }
            }
        });

        CancelButton.onClick.AddListener(() =>
        {
            // Optional test only → quay về GameScene
            GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
        });
    }
}

internal class ExamData
{
    public string ExamName { get; internal set; }
}