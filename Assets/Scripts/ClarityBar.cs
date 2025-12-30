using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class ClarityBar : MonoBehaviour
{
    [SerializeField] private Slider clarityBarSlider;
    private static readonly float maxClarity = 100f;
    private static float currentClarity = maxClarity;
    private static float targetClarity = maxClarity;
    private static string currentMood = "Normal", previousMood = "Normal";
    [SerializeField] private Gradient clarityGradient;
    [SerializeField] private Image fill, moodBox;
    [SerializeField] private TextMeshProUGUI moodText;
    void Start()
    {
        SetMoodText(currentMood);
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
    public void UpdateClarity(float change)
    {
        targetClarity = Mathf.Clamp(currentClarity + change, 0, maxClarity);
    }

    public void UpdateMood(string mood)
    {
        currentMood = mood;
        if (currentMood != previousMood)
        {
            SetMoodText(currentMood);
            previousMood = currentMood;
        }
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
                moodBox.color = Color.yellowGreen;
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
