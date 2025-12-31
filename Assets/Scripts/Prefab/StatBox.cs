using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBox : MonoBehaviour
{
    [SerializeField] private CurrentRunData currentRunData;
    private string statName;
    private int statValue;
    private static int speed, wit, memory, luck;
    private static int testSpeed, testWit, testMemory, testLuck;
    private enum StatBoxType {Speed, Wit, Memory, Luck, TestSpeed, TestWit, TestMemory, TestLuck};
    [SerializeField] private StatBoxType statBoxType;
    [SerializeField] private TextMeshProUGUI statNameText, statValueText;
    private static StatBox instance;
    
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdatePlayerStat();
        SetStatType();
        SetNameText();
        SetValueText();
    }

    private void SetStatType()
    {
        switch (statBoxType)
        {
            case StatBoxType.Speed:
                instance.statValue = speed;
                instance.statName = "Speed";
                break;
            case StatBoxType.Wit:
                instance.statValue = wit;
                instance.statName = "Wit";
                break;
            case StatBoxType.Memory:
                instance.statValue = memory;
                instance.statName = "Memory";
                break;
            case StatBoxType.Luck:
                instance.statValue = luck;
                instance.statName = "Luck";
                break;
            case StatBoxType.TestSpeed:
                instance.statValue = testSpeed;
                instance.statName = "Speed";
                break;
            case StatBoxType.TestWit:
                instance.statValue = testWit;
                instance.statName = "Wit";
                break;
            case StatBoxType.TestMemory:
                instance.statValue = testMemory;
                instance.statName = "Memory";
                break;
            case StatBoxType.TestLuck:
                instance.statValue = testLuck;
                instance.statName = "Luck";
                break;
        }
    }

    public static void UpdatePlayerStat()
    {
        speed = instance.currentRunData.GetStatValue(StatType.spd);
        wit = instance.currentRunData.GetStatValue(StatType.wit);
        memory = instance.currentRunData.GetStatValue(StatType.mem);
        luck = instance.currentRunData.GetStatValue(StatType.luk);
    }

    public static void UpdateTestStat(ScheduledExam exam)
    {
        testSpeed = exam.GetStatValue(StatType.spd);
        testWit = exam.GetStatValue(StatType.wit);
        testMemory = exam.GetStatValue(StatType.mem);
        testLuck = exam.GetStatValue(StatType.luk);
    }

    private void SetNameText()
    {
        statNameText.SetText(statName);
    }

    private void SetValueText()
    {
        statValueText.SetText(statValue.ToString());
    }
}
