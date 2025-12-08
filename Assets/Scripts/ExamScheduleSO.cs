using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "ExamSchedule", menuName = "Exam Schedule")]
public class ExamSchedule : ScriptableObject
{
    // add exams from editor
    public List<ScheduledExam> exams;
    
    public bool IsExamScheduledForTurn(int turn)
    {
        return exams.Any(exam => exam.Turn == turn);
    }
    
    public ScheduledExam GetExamForTurn(int turn)
    {
        return exams.FirstOrDefault(exam => exam.Turn == turn);
    }
}

[System.Serializable]
public class ScheduledExam
{
    [Header("Info")]
    public string ExamName;
    public int Turn;
    public string ExamSceneName;

    [Header("Entry Requirements")]
    [Tooltip("The player MUST meet these to enter. Failure means a 'game over' or bad event.")]
    public List<StatRequirement> Requirements;
    
    [Tooltip("Flag to set if the player *fails* the objective (e.g., 'failed_quang_ads')")]
    public string FailureFlagToSet;

    [Tooltip("Flag to set if the player *meets* the objective (e.g., 'passed_quang_ads')")]
    public string SuccessFlagToSet;
    
    public bool CheckEntryRequirements(CurrentRunData runData)
    {
        foreach (var req in Requirements)
        {
            if (!req.IsMet(runData))
            {
                return false;
            }
        }
        return true;
    }
}

[System.Serializable]
public class StatRequirement
{
    public enum StatType
    {
        SPD,
        WIT,
        MEM,
        LUK
    }

    public StatType Stat;
    public int MinValue;
    
    public bool IsMet(CurrentRunData runData)
    {
        switch (Stat)
        {
            case StatType.SPD:
                return runData.Speed >= MinValue;
            case StatType.WIT:
                return runData.Wit >= MinValue;
            case StatType.MEM:
                return runData.Memory >= MinValue;
            case StatType.LUK:
                return runData.Luck >= MinValue;
            default:
                return false;
        }
    }
}