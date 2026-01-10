using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class UpgradeEventUI : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_4 = new(0.4f);
    [SerializeField] private RectTransform SpeedButton, WitButton, MemoryButton, LuckButton;
    [SerializeField] private RectTransform SpeedUpgradedTextRect, WitUpgradedTextRect, MemoryUpgradedTextRect, LuckUpgradedTextRect;
    [SerializeField] private TextMeshProUGUI SpeedUpgradedText, WitUpgradedText, MemoryUpgradedText, LuckUpgradedText;
    readonly Dictionary<StatType, int> count = new();
    public static UpgradeEventUI Instance{get; private set;}
    private void Awake()
    {
        Instance = this;
        Debug.Log("UpgradeEventUI Instance assigned.");
    }

    void Start()
    {
        gameObject.SetActive(false);
        SpeedButton.localScale = Vector3.zero;
        WitButton.localScale = Vector3.zero;
        MemoryButton.localScale = Vector3.zero;
        LuckButton.localScale = Vector3.zero;
    }
    public IEnumerator DisplayUpgradeEvent(List<StatType> UpgradedStudies)
    {
        foreach (StatType stat in UpgradedStudies)
        {
            yield return _waitForSeconds0_4;
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
                        SpeedButton.DOScale(1f, 0.2f).SetLink(gameObject);
                        break;
                    case StatType.wit:
                        WitButton.DOScale(1f, 0.2f).SetLink(gameObject);
                        break;
                    case StatType.mem:
                        MemoryButton.DOScale(1f, 0.2f).SetLink(gameObject);
                        break;
                    case StatType.luk:
                        LuckButton.DOScale(1f, 0.2f).SetLink(gameObject);
                        break;
                }
            } 
            else
            {
                switch (stat)
                {
                    case StatType.spd:
                        SpeedUpgradedText.SetText($"Upgraded! x{count[stat]}");
                        SpeedUpgradedTextRect.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1).SetLink(gameObject);
                        break;
                    case StatType.wit:
                        WitUpgradedText.SetText($"Upgraded! x{count[stat]}");
                        WitUpgradedTextRect.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1).SetLink(gameObject);
                        break;
                    case StatType.mem:
                        MemoryUpgradedText.SetText($"Upgraded! x{count[stat]}");
                        MemoryUpgradedTextRect.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1).SetLink(gameObject);
                        break;
                    case StatType.luk:
                        LuckUpgradedText.SetText($"Upgraded! x{count[stat]}");
                        LuckUpgradedTextRect.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1).SetLink(gameObject);
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
