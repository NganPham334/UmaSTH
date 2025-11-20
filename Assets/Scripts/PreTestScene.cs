using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button testButton;
    public Button returnButton;

    [Header("Date / Time")]
    public TMP_Text dateTimeText;

    private void Start()
    {
        testButton.onClick.AddListener(OnClickTest);
        returnButton.onClick.AddListener(OnClickReturn);
    }

    private void Update()
    {
        dateTimeText.text = System.DateTime.Now.ToString("dd/MM/yyyy\nHH:mm:ss");
    }

    void OnClickTest()
    {
        // Load scene test
        SceneManager.LoadScene("Test(Combat)Screen");
    }

    void OnClickReturn()
    {
        // Load scene quay về
        SceneManager.LoadScene("Study Screen");
    }
}
