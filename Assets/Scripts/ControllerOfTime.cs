using UnityEngine;
using UnityEngine.UIElements;
using System.Globalization;

public class ControllerOfTime : MonoBehaviour
{
    private Label _turnsLabel;
    private Label _dateLabel;

    public ExamSchedule schedule;
    public CurrentRunData runData;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        _turnsLabel = root.Q<Label>("turn-counter");
        _dateLabel = root.Q<Label>("time-label");

        if (schedule != null && runData != null)
        {
            int curTurn = runData.CurrentTurn;
            int nextTurn = 0;
            for (int i = curTurn; i <= runData.TotalTurns; i++)
            {
                if (schedule.IsExamScheduledForTurn(i))
                {
                    nextTurn = i;
                    break;
                }
            }

            _turnsLabel.text = $"{nextTurn - curTurn}";
            _dateLabel.text = FormatTime();
        }
    }

    // coding this much math without using gpt should entitle me to some kind of awards
    private string FormatTime()
    {
        string prod = "";
        int curTurn = runData.CurrentTurn;
        double year = (double) curTurn / 24;
        
        switch (System.Math.Ceiling(year))
        {
            case 1: prod += "1st Year "; break;
            case 2: prod += "2nd Year "; break;
            case 3: prod += "3rd Year "; break;
        }

        double month = (year - System.Math.Floor(year)) * 12;
        double halfMonth = month - System.Math.Floor(month);
        
        if (halfMonth > 0) prod += "Early ";
        else prod += "Late ";
        // Debug.Log($"{curTurn} {month} {(int) System.Math.Ceiling(month - 0.1)}");, not deleting this, slightly foreshadowing
        prod += CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName((int) System.Math.Ceiling(month - 0.1));

        return prod;
    }
}