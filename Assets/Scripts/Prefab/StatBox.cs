using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StatBox : MonoBehaviour
{
    public static List<StatBox> allBoxes = new();
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

    private void OnEnable()
    {
        allBoxes.Add(this);
    }

    private void OnDisable()
    {
        allBoxes.Remove(this);
    }

    void Start()
    {
        SetStatName();
        UpdateLocalPlayerStat();
        UpdateTestStat(examSchedule.GetExamForTurn(currentRunData.CurrentTurn));
    }

    private void SetStatName()
    {
        switch (statBoxType)
        {
            case StatBoxType.Speed:
                statName = "Speed";
                break;
            case StatBoxType.Wit:
                statName = "Wit";
                break;
            case StatBoxType.Memory:
                statName = "Memory";
                break;
            case StatBoxType.Luck:
                statName = "Luck";
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
                    statValue = speed;
                    break;
                case StatBoxType.Wit:
                    statValue = wit;
                    break;
                case StatBoxType.Memory:
                    statValue = memory;
                    break;
                case StatBoxType.Luck:
                    statValue = luck;
                    break;
            }
        }
        if (statBoxSide == StatBoxSide.Test)
        {
            switch (statBoxType)
            {
                case StatBoxType.Speed:
                    statValue = testSpeed;
                    break;
                case StatBoxType.Wit:
                    statValue = testWit;
                    break;
                case StatBoxType.Memory:
                    statValue = testMemory;
                    break;
                case StatBoxType.Luck:
                    statValue = testLuck;
                    break;
            }
        }
        statValueText.SetText(statValue.ToString());
    }

    public void UpdateLocalPlayerStat()
    {
        speed = currentRunData.GetStatValue(StatType.spd);
        wit = currentRunData.GetStatValue(StatType.wit);
        memory = currentRunData.GetStatValue(StatType.mem);
        luck = currentRunData.GetStatValue(StatType.luk);

        SetStatValue();
    }

    public static void UpdateAllStats()
    {
        foreach (StatBox box in allBoxes)
        {
            box.UpdateLocalPlayerStat();
        }
    }

    public void UpdateTestStat(ScheduledExam exam)
    {
        testSpeed = exam.GetStatValue(StatType.spd);
        testWit = exam.GetStatValue(StatType.wit);
        testMemory = exam.GetStatValue(StatType.mem);
        testLuck = exam.GetStatValue(StatType.luk);

       SetStatValue();
    }
}
