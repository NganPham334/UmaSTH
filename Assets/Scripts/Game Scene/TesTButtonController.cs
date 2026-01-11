
using UnityEngine;
using UnityEngine.UI;

public class TestButtonController : MonoBehaviour
{
    [Header("Button References")]
    public Button testButton;      
    public Button restButton;      
    public Button pastTimeButton;  
    public Button studyButton;     
    public Button detailsButton;   

    private void Start()
    {
        UpdateButtonStates();
    }

    private void OnEnable()
    {
        
        UpdateButtonStates();
    }

    public void UpdateButtonStates()
    {
        if (GameStateMan.Instance == null || GameStateMan.Instance.CurrentRun == null)
        {
            SetDefaultButtonStates();
            return;
        }

        int currentTurn = GameStateMan.Instance.CurrentRun.CurrentTurn;
        bool isTestTurn = IsTestTurn(currentTurn);

        if (isTestTurn)
        {
           
            SetButtonInteractable(testButton, true);
            SetButtonInteractable(restButton, false);
            SetButtonInteractable(pastTimeButton, false);
            SetButtonInteractable(studyButton, false);
            SetButtonInteractable(detailsButton, false);
        }
        else
        {
            
            SetButtonInteractable(testButton, false);
            SetButtonInteractable(restButton, true);
            SetButtonInteractable(pastTimeButton, true);
            SetButtonInteractable(studyButton, true);
            SetButtonInteractable(detailsButton, true);
        }
    }

    private bool IsTestTurn(int turn)
    {
        
        if (GameStateMan.Instance.ExamSchedule != null)
        {
            return GameStateMan.Instance.ExamSchedule.IsExamScheduledForTurn(turn);
        }

        return false;
    }

    private void SetButtonInteractable(Button button, bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }

    private void SetDefaultButtonStates()
    {
       
        SetButtonInteractable(testButton, false);
        SetButtonInteractable(restButton, true);
        SetButtonInteractable(pastTimeButton, true);
        SetButtonInteractable(studyButton, true);
        SetButtonInteractable(detailsButton, true);
    }

    
    public void OnTestButtonClicked()
    {
        if (GameStateMan.Instance == null) return;

        int currentTurn = GameStateMan.Instance.CurrentRun.CurrentTurn;
        
        if (GameStateMan.Instance.ExamSchedule == null) return;
        
        var exam = GameStateMan.Instance.ExamSchedule.GetExamForTurn(currentTurn);

        if (exam != null)
        {
            var param = new System.Collections.Generic.Dictionary<string, object>
            {
                { "ExamData", exam }
            };

            GameStateMan.Instance.RequestState(GameStateMan.GameState.PreTest, param);
        }
    }
}