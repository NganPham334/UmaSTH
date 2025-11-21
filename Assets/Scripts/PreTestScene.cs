using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreTestScene : MonoBehaviour
{
    [Header("UI References")]
    public Button btnReturn;
    public Button btnTest;
    public TMP_Text timeText;

    void Awake()
    {
        // Tự động tìm nếu quên gán trong Inspector
        if (btnReturn == null)
            btnReturn = GameObject.Find("ReturnButton")?.GetComponent<Button>();

        if (btnTest == null)
            btnTest = GameObject.Find("TestButton")?.GetComponent<Button>();

        if (timeText == null)
            timeText = GameObject.Find("DateTimeText")?.GetComponent<TMP_Text>();
        if (GameStateMan.Instance == null)
    {
        Debug.LogWarning("GameStateMan missing – auto creating.");
        var gsm = new GameObject("GameStateMan_Auto");
        gsm.AddComponent<GameStateMan>();
    }
    }

    void Start()
    {
        // Kiểm tra null để tránh crash
        if (btnReturn != null)
            btnReturn.onClick.AddListener(OnClickReturn);
        else
            Debug.LogWarning("[PreTestScene] btnReturn chưa được gán!");

        if (btnTest != null)
            btnTest.onClick.AddListener(OnClickTest);
        else
            Debug.LogWarning("[PreTestScene] btnTest chưa được gán!");

        if (timeText == null)
            Debug.LogWarning("[PreTestScene] timeText chưa được gán!");
    }

    void Update()
    {
        if (timeText != null)
            timeText.text = System.DateTime.Now.ToString("dd/MM/yyyy\nHH:mm:ss");
    }

    void OnClickReturn()
    {
        if (GameStateMan.Instance == null)
        {
            Debug.LogError("[PreTestScene] GameStateMan.Instance == null");
            return;
        }

        GameStateMan.Instance.RequestState(GameStateMan.GameState.Training);
    }

    void OnClickTest()
    {
        if (GameStateMan.Instance == null)
        {
            Debug.LogError("[PreTestScene] GameStateMan.Instance == null");
            return;
        }

        GameStateMan.Instance.RequestState(GameStateMan.GameState.Exam);
    }
}
