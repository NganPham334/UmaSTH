using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
public class StudyLvBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText, studyNameText;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image Icon;
    [SerializeField] private Sprite SpeedIcon, WitIcon, MemoryIcon, LuckIcon;
    
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
        Icon.sprite = statName switch
        {
            "Speed" => SpeedIcon,
            "Wit" => WitIcon,
            "Memory" => MemoryIcon,
            "Luck" => LuckIcon,
            _ => SpeedIcon
        };
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