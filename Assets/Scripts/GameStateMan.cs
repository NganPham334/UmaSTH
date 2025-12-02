using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameStateMan : MonoBehaviour
{
    public static GameStateMan Instance { get; private set; }

    [Header("Run Data (leave empty during prototype)")]
    public CurrentRunData CurrentRun;

    [Header("Exam Schedule (leave empty during prototype)")]
    public ExamSchedule ExamSchedule;

    public enum GameState
    {
        MainMenu,
        Launcher,
        Training,
        PastTime,
        GameScene,
        Resting,
        PreTest,
        Exam,
        StoryEvent,
        RunEnd
    }

    private GameState _currentState;
    public GameState CurrentStateType => _currentState;
    public CurrentRunData CurrentRunData { get; internal set; }

    private Dictionary<string, object> _stateParameters;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _currentState = GameState.Launcher;
            _stateParameters = new Dictionary<string, object>();


            // -----------------------------------------------------
            // TEST MODE: if no RunData assigned in Inspector
            // -----------------------------------------------------
            if (CurrentRun == null)
            {
                Debug.LogWarning("GameStateMan: CurrentRunData NOT assigned → Using MOCK data.");

                CurrentRunData = ScriptableObject.CreateInstance<CurrentRunData>();
                CurrentRunData.Speed = 120;
                CurrentRunData.Wit = 80;
                CurrentRunData.Memory = 50;
                CurrentRunData.Luck = 40;
                CurrentRunData.CurrentTurn = 1;
            }
            else
            {
                CurrentRunData = CurrentRun;
                CurrentRunData.InitializeRun();
            }


            // -----------------------------------------------------
            // TEST MODE: if no ExamSchedule assigned in Inspector
            // -----------------------------------------------------
            if (ExamSchedule == null)
            {
                Debug.LogWarning("GameStateMan: ExamSchedule NOT assigned → Using MOCK exam schedule.");

                ExamSchedule = ScriptableObject.CreateInstance<ExamSchedule>();
                ExamSchedule.exams = new List<ScheduledExam>()
                {
                    new ScheduledExam {
                        ExamName = "Mock Exam",
                        Turn = 1,
                        Requirements = new List<StatRequirement>() {
                            new StatRequirement { Stat = StatRequirement.StatType.SPD, MinValue = 50 },
                            new StatRequirement { Stat = StatRequirement.StatType.WIT, MinValue = 40 },
                            new StatRequirement { Stat = StatRequirement.StatType.MEM, MinValue = 30 },
                            new StatRequirement { Stat = StatRequirement.StatType.LUK, MinValue = 20 }
                        }
                    }
                };
            }
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
            _stateParameters = new Dictionary<string, object>(parameters);

        ChangeState(newState);
    }

    public void RequestState(GameState newState)
    {
        RequestState(newState, null);
    }



    public bool TryGetStateParameter<T>(string key, out T result)
    {
        if (_stateParameters.TryGetValue(key, out object value))
        {
            if (value is T cast)
            {
                result = cast;
                return true;
            }
        }

        result = default;
        return false;
    }



    public void ReportActionComplete()
    {
        CurrentRunData.AdvanceTurn();
        int turn = CurrentRunData.CurrentTurn;

        _stateParameters.Clear();

        // If exam exists → go to PreTest
        if (ExamSchedule.IsExamScheduledForTurn(turn))
        {
            var exam = ExamSchedule.GetExamForTurn(turn);

            var param = new Dictionary<string, object>
            {
                { "ExamData", exam },
                { "OptionalTest", false }
            };

            RequestState(GameState.PreTest, param);
            return;
        }

        // Default loop
        RequestState(GameState.GameScene);
    }



    private void ChangeState(GameState newState)
    {
        _currentState = newState;

        string sceneName = _currentState.GetSceneName();
        SceneManager.LoadScene(sceneName);
    }
}
