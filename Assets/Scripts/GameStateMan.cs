using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameStateMan : MonoBehaviour
{
    public static GameStateMan Instance { get; private set; }

    [Header("Run Data")]
    public CurrentRunData CurrentRun;

    [Header("Run Logic SOs")]
    public ExamSchedule ExamSchedule;

    public enum GameState
    {
        MainMenu,
        Launcher,
        Training,
        Resting,
        PreTest,
        Exam,
        StoryEvent,
        RunEnd
    }

    private GameState _currentState;
    public GameState CurrentStateType => _currentState; // Getter thing, pretty sure this shows up in the editor as well
    private Dictionary<string, object> _stateParameters;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _currentState = GameState.Launcher;
            _stateParameters = new Dictionary<string, object>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RequestState(GameState newState, Dictionary<string, object> parameters)
    {
        _stateParameters.Clear();
        if (parameters != null)
        {
            _stateParameters = new Dictionary<string, object>(parameters);
        }
        Debug.Log($"State requested: {newState} with {_stateParameters.Count} parameters.");
        ChangeState(newState);
    }

    public void RequestState(GameState newState)
    {
        RequestState(newState, null);
    }

    // Game scenes can call this upon creation to get params
    // e.g. idek what to use this for but i think itll come in handy
    // kinda like bundles when creating a fragment on android
    public bool TryGetStateParameter<T>(string key, out T result)
    {
        if (_stateParameters.TryGetValue(key, out object value))
        {
            if (value is T)
            {
                result = (T)value;
                return true;
            }
        }
        result = default(T);
        return false;
    }


    public void ReportActionComplete()
    {
        Debug.Log("Action complete. Advancing turn.");
        
        CurrentRun.AdvanceTurn();
        _stateParameters.Clear();
        
        if (ExamSchedule.IsExamScheduledForTurn(CurrentRun.CurrentTurn))
        {
            var exam = ExamSchedule.GetExamForTurn(CurrentRun.CurrentTurn);
            var parameters = new Dictionary<string, object>
            {
                { "ExamData", exam }
            };
            RequestState(GameState.Exam, parameters);
        }
        else if (ShouldRandomEventTrigger())
        {
            RequestState(GameState.StoryEvent);
        }
        else if (CurrentRun.CurrentTurn >= CurrentRun.TotalTurns) 
        {
            RequestState(GameState.RunEnd);
        }
        else
        {
            RequestState(GameState.Training);
        }
    }
    
    private void ChangeState(GameState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        Debug.Log($"--- Entering State: {_currentState} ---");
        
        SceneManager.LoadScene(_currentState.GetSceneName());
    }

    private bool ShouldRandomEventTrigger()
    {
        return Random.Range(0, 100) < 25; // 25% chance
    }
}