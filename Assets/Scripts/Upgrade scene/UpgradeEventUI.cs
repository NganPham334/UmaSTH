using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class UpgradeEventUI : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_4 = new(0.4f);
    [SerializeField] private StudyButton SpeedButton, WitButton, MemoryButton, LuckButton;
    [SerializeField] private UpgradedText SpeedUpgradedText, WitUpgradedText, MemoryUpgradedText, LuckUpgradedText;
    readonly Dictionary<StatType, int> count = new();
    [SerializeField]private CurrentRunData currentRunData;
    public static UpgradeEventUI Instance{get; private set;}
    private void Awake()
    {
        Instance = this;
        Debug.Log("UpgradeEventUI Instance assigned.");
    }

    void Start()
    {
        SpeedButton.Shrink();
        WitButton.Shrink();
        MemoryButton.Shrink();
        LuckButton.Shrink();
        List<StatType> results = StatsManager.Instance.progressionHandler.TriggerUpgradeEvent(currentRunData.baseUpgradePoints);
        StudyButton.UpdateAllButtons();
        Debug.Log($"Upgrade list: {results.ToString()}");
        StartCoroutine(DisplayUpgradeEvent(results));
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
                        SpeedButton.PopUp();
                        break;
                    case StatType.wit:
                        WitButton.PopUp();
                        break;
                    case StatType.mem:
                        MemoryButton.PopUp();
                        break;
                    case StatType.luk:
                        LuckButton.PopUp();
                        break;
                }
            } 
            else
            {
                switch (stat)
                {
                    case StatType.spd:
                        SpeedUpgradedText.UpdateUpgradedText(count[stat]);
                        break;
                    case StatType.wit:
                        WitUpgradedText.UpdateUpgradedText(count[stat]);
                        break;
                    case StatType.mem:
                        MemoryUpgradedText.UpdateUpgradedText(count[stat]);
                        break;
                    case StatType.luk:
                        LuckUpgradedText.UpdateUpgradedText(count[stat]);
                        break;
                }
            }
        }
    }

    public void Next()
    {
        count.Clear();
        GameStateMan.Instance.ReportActionComplete("from_upgrade_event");
    }
}
