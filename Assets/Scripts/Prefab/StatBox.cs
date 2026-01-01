using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBox : MonoBehaviour
{
    [SerializeField] private CurrentRunData currentRunData;
    [SerializeField] private ExamSchedule examSchedule;
    private string statName;
    private int statValue;
    private static int speed, wit, memory, luck;
    private static int testSpeed, testWit, testMemory, testLuck;
    private enum StatBoxType {Speed, Wit, Memory, Luck};
    private enum StatBoxSide {Player, Test};
    [SerializeField] private StatBoxType statBoxType;
    [SerializeField] private StatBoxSide statBoxSide;
    [SerializeField] private TextMeshProUGUI statNameText, statValueText;
    private static StatBox instance;
    
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetStatName();
        UpdatePlayerStat();
        UpdateTestStat(examSchedule.GetExamForTurn(currentRunData.CurrentTurn));
    }

    private void SetStatName()
    {
        switch (statBoxType)
        {
            case StatBoxType.Speed:
                instance.statName = "Speed";
                break;
            case StatBoxType.Wit:
                instance.statName = "Wit";
                break;
            case StatBoxType.Memory:
                instance.statName = "Memory";
                break;
            case StatBoxType.Luck:
                instance.statName = "Luck";
                break;
        }
        statNameText.SetText(statName);

    }

    private void SetStatValue()
    {
        if (statBoxSide == StatBoxSide.Player)
        {
            switch (statBoxType)
            {
                case StatBoxType.Speed:
                    instance.statValue = speed;
                    break;
                case StatBoxType.Wit:
                    instance.statValue = wit;
                    break;
                case StatBoxType.Memory:
                    instance.statValue = memory;
                    break;
                case StatBoxType.Luck:
                    instance.statValue = luck;
                    break;
            }
        }
        if (statBoxSide == StatBoxSide.Test)
        {
            switch (statBoxType)
            {
                case StatBoxType.Speed:
                    instance.statValue = testSpeed;
                    break;
                case StatBoxType.Wit:
                    instance.statValue = testWit;
                    break;
                case StatBoxType.Memory:
                    instance.statValue = testMemory;
                    break;
                case StatBoxType.Luck:
                    instance.statValue = testLuck;
                    break;
            }
        }
        statValueText.SetText(statValue.ToString());
    }

    public static void UpdatePlayerStat()
    {
        speed = instance.currentRunData.GetStatValue(StatType.spd);
        wit = instance.currentRunData.GetStatValue(StatType.wit);
        memory = instance.currentRunData.GetStatValue(StatType.mem);
        luck = instance.currentRunData.GetStatValue(StatType.luk);

        instance.SetStatValue();
    }

    public void UpdateTestStat(ScheduledExam exam)
    {
        testSpeed = exam.GetStatValue(StatType.spd);
        testWit = exam.GetStatValue(StatType.wit);
        testMemory = exam.GetStatValue(StatType.mem);
        testLuck = exam.GetStatValue(StatType.luk);

        instance.SetStatValue();
    }
}
