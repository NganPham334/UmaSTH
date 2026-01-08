using UnityEngine;
using TMPro;
using DG.Tweening;
public class StudyLvBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText, studyNameText;
    [SerializeField] private RectTransform rectTransform;
    
    public void UpdateLevelText(int level, string statName)
    {
        levelText.SetText($"{statName} Lv. {level}");
        studyNameText.SetText(statName switch
        {
            "Speed" => "Doing exercises",
            "Wit" => "Quizzing",
            "Memory" => "Doing flashcards",
            "Luck" => "Praying",
            _ => statName
        });
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        rectTransform.DOAnchorPosX(200, 0.5f).SetEase(Ease.OutQuad).SetLink(gameObject);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        rectTransform.DOAnchorPosX(-200, 0.5f).SetEase(Ease.OutQuad).SetLink(gameObject);
    }
}