using UnityEngine;
using UnityEngine.UIElements;
using System.Globalization;

public class ControllerOfTime : MonoBehaviour
{
    private Label _turnsLabel;
    private Label _dateLabel;
    private Label _displayLabel;
    
    private VisualElement _clickableArea;

    public ExamSchedule schedule;
    public CurrentRunData runData;
    
    private bool _showFinalsCountdown = true;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        _displayLabel = root.Q<Label>("display-header");
        _turnsLabel = root.Q<Label>("turn-counter");
        _dateLabel = root.Q<Label>("time-label");
        _clickableArea = root.Q<VisualElement>("ThingiesContainer");
        
        if (_clickableArea != null)
        {
            _clickableArea.RegisterCallback<ClickEvent>(_ =>
            {
                ToggleDisplay();
            });
        }

        UpdateUI();
    }

    private void ToggleDisplay()
    {
        _showFinalsCountdown = !_showFinalsCountdown;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (schedule == null && runData == null)
        {
            return;
        }
        if (_showFinalsCountdown)
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
            _displayLabel.text = "Until Finals";
            _turnsLabel.text = $"{nextTurn - curTurn} turns";
            
        }
        else
        {
            _displayLabel.text = "Current Turn";
            _turnsLabel.text = $"Turn {runData.CurrentTurn}";
        }
        
        _dateLabel.text = FormatTime();
    }

    public string FormatTime()
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