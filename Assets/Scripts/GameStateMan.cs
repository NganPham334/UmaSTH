using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using VisualNovel;

public class GameStateMan : MonoBehaviour
{
    public static GameStateMan Instance { get; private set; }

    // pretty sure if claude has a reason for why this should be empty during prototype
    // you should comment it here so other people can understand
    // maybe document it also
    [Header("Run Data (leave empty during prototype)")]
    public CurrentRunData CurrentRun;

    [Header("Exam Schedule (leave empty during prototype)")]
    public ExamSchedule ExamSchedule;

    public DeterminedEventsTemplate DeterminedEvents;

    public enum GameState
    {
        MainMenu,
        Launcher,
        Training,
        GameScene,
        VisualNovel,
        PreTest,
        Exam,
        
        RunEnd
        // TODO: determine if this should be a separate scene
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
    
    
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoLoadSingleton()
    {
        if (Instance != null) return;

        var managerPrefab = Resources.Load<GameStateMan>("GameManContainer");
        if (managerPrefab == null) return;

        var instanceObj = Instantiate(managerPrefab);
        
        // to differentiate from normally created gamestateman
        instanceObj.name = "GameStateManager (Auto-Loaded)"; 
        
        // testing
        Debug.Log($"<color=#FFFF00><b>[Development Mode]</b> Auto-Initialized GameStateManager from Resources.</color>");
        DontDestroyOnLoad(instanceObj);
    }
#endif


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

        if (DeterminedEvents.GetEventForTurn(turn) != null)
        {
            RequestState(GameState.VisualNovel, new() {{"vn_type", "determined"}});
        }

        RequestState(GameState.GameScene);
    }



    private void ChangeState(GameState newState)
    {
        _currentState = newState;

        string sceneName = _currentState.GetSceneName();
        SceneManager.LoadScene(sceneName);
    }
}
