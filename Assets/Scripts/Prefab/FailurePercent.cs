using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FailurePercent : MonoBehaviour
{
    [SerializeField] private Image failureImage;
    [SerializeField] private TextMeshProUGUI failurePercent;
    [SerializeField] private Color LowFailureColor;
    [SerializeField] private Color MidFailureColor;
    [SerializeField] private Color HighFailureColor;
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void DisplayFailurePercent(float percent)
    {
        if (percent < 0.15)
        {
            failureImage.color = LowFailureColor;
        }
        else if (percent < 0.4)
        {
            failureImage.color = MidFailureColor;
        }
        else
        {
            failureImage.color = HighFailureColor;
        }
        failurePercent.SetText($"{percent*100}%");
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
