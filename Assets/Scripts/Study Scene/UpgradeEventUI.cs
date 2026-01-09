using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class UpgradeEventUI : MonoBehaviour
{
    [SerializeField] private RectTransform SpeedButton, WitButton, MemoryButton, LuckButton;
    [SerializeField] private RectTransform SpeedTextRect, WitTextRect, MemoryTextRect, LuckTextRect;
    [SerializeField] private TextMeshProUGUI SpeedText, WitText, MemoryText, LuckText;
    readonly Dictionary<StatType, int> count = new();

    void Start()
    {
        SpeedButton.localScale = Vector3.zero;
        WitButton.localScale = Vector3.zero;
        MemoryButton.localScale = Vector3.zero;
        LuckButton.localScale = Vector3.zero;
    }
    public void DisplayUpgradeStudy(List<StatType> UpgradedStudies)
    {
        foreach (StatType stat in UpgradedStudies)
        {
            if (count.ContainsKey(stat))
            {
                count[stat]++;
            }
            else
            {
                count[stat] = 1;
            }
            if (count[stat] == 1)
            {
                switch (stat)
                {
                    case StatType.spd:
                        SpeedButton.DOScale(1f, 0.5f).SetDelay(0.3f).SetLink(gameObject);
                        break;
                    case StatType.wit:
                        WitButton.DOScale(1f, 0.5f).SetDelay(0.3f).SetLink(gameObject);
                        break;
                    case StatType.mem:
                        MemoryButton.DOScale(1f, 0.5f).SetDelay(0.3f).SetLink(gameObject);
                        break;
                    case StatType.luk:
                        LuckButton.DOScale(1f, 0.5f).SetDelay(0.3f).SetLink(gameObject);
                        break;
                }
            } 
            else
            {
                switch (stat)
                {
                    case StatType.spd:
                        SpeedText.SetText($"Upgraded! x{count[stat]}");
                        SpeedTextRect.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1).SetLink(gameObject);
                        break;
                    case StatType.wit:
                        WitText.SetText($"Upgraded! x{count[stat]}");
                        WitTextRect.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1).SetLink(gameObject);
                        break;
                    case StatType.mem:
                        MemoryText.SetText($"Upgraded! x{count[stat]}");
                        MemoryTextRect.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1).SetLink(gameObject);
                        break;
                    case StatType.luk:
                        LuckText.SetText($"Upgraded! x{count[stat]}");
                        LuckTextRect.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 1).SetLink(gameObject);
                        break;
                }
            }
        }
    }

    public void Next()
    {
        gameObject.SetActive(false);
        count.Clear();
        GameStateMan.Instance.ReportActionComplete();
    }
}
