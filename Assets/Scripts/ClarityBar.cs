using TMPro;
using UnityEngine;
using UnityEngine.UI;   

public class ClarityBar : MonoBehaviour
{
    [SerializeField] private Slider clarityBarSlider;
    private readonly float maxClarity = 100f;
    private float currentClarity;
    private float targetClarity;
    [SerializeField] private Gradient clarityGradient;
    [SerializeField] private Image fill, moodBox;
    [SerializeField] private TextMeshProUGUI moodText;

    void Start()
    {
        currentClarity = maxClarity;
        targetClarity = currentClarity;
        fill.color = clarityGradient.Evaluate(clarityBarSlider.normalizedValue);
        SetMoodText("Depressed");
        UpdateClarity(-90f);
    }

    public void UpdateClarity(float change)
    {
        targetClarity = Mathf.Clamp(currentClarity + change, 0, maxClarity);
    }
    

    void Update()
    {
        if (currentClarity != targetClarity)
        {
            currentClarity = Mathf.Lerp(currentClarity, targetClarity, 5 * Time.deltaTime);
        }
        clarityBarSlider.value = currentClarity / maxClarity;
        fill.color = clarityGradient.Evaluate(clarityBarSlider.normalizedValue);
    }

    public void SetMoodText(string mood)
    {
        moodText.SetText(mood);
        switch (mood)
        {
            case "Depressed":
                moodBox.color = Color.purple;
                moodText.fontSize = 20;
                break;
            case "Bad":
                moodBox.color = Color.blue;
                moodText.fontSize = 30;
                break;
            case "Normal":
                moodBox.color = Color.yellow;
                moodText.fontSize = 30;
                break;
            case "Good":
                moodBox.color = Color.orange;
                moodText.fontSize = 30;
                break;
            case "Umazing":
                moodBox.color = Color.hotPink;
                moodText.fontSize = 30;
                break;
        }
    }
}
