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
    [SerializeField] private Image fill;
    [SerializeField] private TextMeshProUGUI moodText;

    void Start()
    {
        currentClarity = maxClarity;
        targetClarity = currentClarity;
        fill.color = clarityGradient.Evaluate(clarityBarSlider.normalizedValue);
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
    }
}
