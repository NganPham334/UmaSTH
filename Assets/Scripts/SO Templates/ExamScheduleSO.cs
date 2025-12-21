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
    
    // IDEK if these flags are needed, phuc u decide
    [Tooltip("Flag to set if the player *fails* the objective (e.g., 'failed_quang_ads')")]
    public string FailureFlagToSet;

    [Tooltip("Flag to set if the player *meets* the objective (e.g., 'passed_quang_ads')")]
    public string SuccessFlagToSet;

    [Header("Post-test dialogues")]
    [Tooltip("Dialogue when passed")]
    public string nodeNamePass;
    
    [Tooltip("Dialogue when failed")]
    public string nodeNameFail;

    [Header("Test's stats")]
//Exam stat for blay
    public int Speed;
    public int Wit, Memory, Luck;
    public int GetStatValue(StatType type)
	{
        return type switch
        {
            StatType.SPD => Speed,
            StatType.WIT => Wit,
            StatType.MEM => Memory,
            StatType.LUK => Luck,
            _ => 0,
        };
    }
}