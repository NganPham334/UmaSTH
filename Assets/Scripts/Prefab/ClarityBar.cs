using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class ClarityBar : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_05 = new(0.05f);
    [SerializeField] private Slider clarityBarSlider;
    private static float maxClarity = 100f;
    private static float currentClarity = maxClarity;
    private static float targetClarity = maxClarity;
    private static string currentMood = "Normal", previousMood = "Normal";
    public static ClarityBar Instance{get; private set;}
    [SerializeField] private Gradient clarityGradient;
    [SerializeField] private Image fill, moodBox;
    [SerializeField] private TextMeshProUGUI moodText;
    [SerializeField] private RectTransform moodRect;
    [SerializeField] private CurrentRunData currentRunData;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        currentClarity = targetClarity = currentRunData.Clarity;
        clarityBarSlider.value = currentClarity / maxClarity;
        fill.color = clarityGradient.Evaluate(clarityBarSlider.normalizedValue);
        SetMoodText(currentRunData.GetMood());
    }
    void Update()
    {
        // Set the target to match actual data
        targetClarity = currentRunData.Clarity;
        if (currentClarity != targetClarity)
        {
            currentClarity = Mathf.Lerp(currentClarity, targetClarity, 5 * Time.deltaTime);
        }
        clarityBarSlider.value = currentClarity / maxClarity;
        fill.color = clarityGradient.Evaluate(clarityBarSlider.normalizedValue);
    }
    public static void UpdateClarity(float change)
    {
        targetClarity = Mathf.Clamp(currentClarity + change, 0, maxClarity);
    }

    public static void UpdateMood(string mood)
    {
        currentMood = mood;
        if (currentMood != previousMood)
        {
            Instance.SetMoodText(currentMood);
            Instance.Animate();
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

    public void Animate()
    {
        StartCoroutine(AnimateMood());
    }

    public IEnumerator AnimateMood()
    {
        yield return _waitForSeconds0_05;
        moodRect.DOPunchScale(Vector3.one * 0.4f, 0.3f, 1, 0.3f).SetLink(gameObject);
    }
}
