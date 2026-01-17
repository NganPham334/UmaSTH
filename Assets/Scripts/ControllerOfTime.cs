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

            _turnsLabel.text = $"{nextTurn - curTurn} turns";
            _dateLabel.text = FormatTime();
        }
    }
    
    private string FormatTime()
    {
        int t = runData.CurrentTurn - 1;
        int yearNum = (t / 24) + 1;
    
        string prod = "";
        switch (yearNum)
        {
            case 1: prod += "1st Year "; break;
            case 2: prod += "2nd Year "; break;
            case 3: prod += "3rd Year "; break;
        }

        bool isEarly = t % 2 == 0;
        prod += isEarly ? "Early " : "Late ";
        
        int monthIndex = ((t % 24) / 2) + 1;

        prod += System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthIndex);
        return prod;
    }
}