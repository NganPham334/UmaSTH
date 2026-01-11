using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using Yarn.Unity;

public class GameStateMan : MonoBehaviour
{
    public static GameStateMan Instance { get; private set; }
    
    [Header("Run Data")]
    public CurrentRunData CurrentRun;

    [Header("Exam Schedule")]
    public ExamSchedule ExamSchedule;
    public YarnProject mainScript;

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

    public void StartGame()
    {
        CurrentRun.InitializeRun();
        RequestState(GameState.VisualNovel, new() {{"vn_type", "intro"}});
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
        if (Random.Range(0.0F, 1.0F) > 0.8 && !CurrentRun.doneREvent)
        {
            RequestState(GameState.VisualNovel, new() { { "vn_type", "random" } });
            CurrentRun.doneREvent = true;
            return;
        }

        CurrentRun.doneREvent = false;
        
        CurrentRun.AdvanceTurn();
        int turn = CurrentRun.CurrentTurn;

        _stateParameters.Clear();
        if (turn > 1 && turn % 4 == 1)
        {
            // Go to Upgrade Event
            return;
        }

        if (HasEventForCurrentTurn(turn))
        {
            RequestState(GameState.VisualNovel, new() {{"vn_type", "determined"}});
            return;
        }

        RequestState(GameState.GameScene);
    }

    public bool HasEventForCurrentTurn(int turn)
    {
        if (mainScript == null) return false;
        string expectedNode = $"Turn_{turn}";
        return mainScript.NodeNames.Contains(expectedNode);
    }

    private void ChangeState(GameState newState)
    {
        _currentState = newState;

        string sceneName = _currentState.GetSceneName();
        SceneManager.LoadScene(sceneName);
    }
}
