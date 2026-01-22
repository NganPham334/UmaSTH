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

    // This is NOT thread safe as the name might suggest
    private bool _transitionLock = false;
    private bool _endRun = false;

    public enum GameState
    {
        MainMenu,
        Launcher,
        Training,
        GameScene,
        VisualNovel,
        PreTest,
        Exam,
        UpgradeEvent
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
        if (_transitionLock)
        {
            return;
        }

        _transitionLock = true;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        _stateParameters.Clear();

        if (parameters != null)
            _stateParameters = new Dictionary<string, object>(parameters);

        ChangeState(newState);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        _transitionLock = false;
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

    public void EndRun()
    {
        _endRun = true;
    }
    
    public void ReportActionComplete(string flag = null)
    {
        if (_transitionLock)
        {
            return;
        }
        
        int turn = CurrentRun.CurrentTurn;
        if (turn >= 72)
        {
            RequestState(GameState.VisualNovel, new() {{"vn_type", "run_end"}});
            return;
        }
        
        if (turn > 1 && turn % 6 == 0 && flag != "from_random" && flag != "from_upgrade_event" && !CurrentRun.isFullyUpgraded)
        {
            RequestState(GameState.UpgradeEvent);
            return;
        }
        
        if (Random.Range(0.0F, 1.0F) > 0.8 && flag != "from_random")
        {
            RequestState(GameState.VisualNovel, new() {{ "vn_type", "random" }});
            return;
        }
        
        CurrentRun.AdvanceTurn();
        turn = CurrentRun.CurrentTurn;
        _stateParameters.Clear();
        
        if (HasEventForTurn(turn))
        {
            RequestState(GameState.VisualNovel, new() {{"vn_type", "determined"}});
            return;
        }

        RequestState(GameState.GameScene);
    }

    public bool HasEventForTurn(int turn)
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
    public void LoadSavedGame()
    {
        if (PlayerPrefs.HasKey("SavedGameData"))
        {
            string jsonData = PlayerPrefs.GetString("SavedGameData");
            JsonUtility.FromJsonOverwrite(jsonData, CurrentRun);
            Debug.Log("Game loaded successfully!");
        }
        else
        {
            Debug.LogWarning("No saved game found!");
        }
    }
    private void OnApplicationQuit()
    {
        // Check the internal _currentState variable, not the Instance itself
        if (_currentState == GameState.Launcher || _currentState == GameState.MainMenu)
        {
            return; // Don't auto-save in menus
        }

        // Check if the run has actually moved past turn 0/1
        if (CurrentRun != null && CurrentRun.CurrentTurn > 1 && !_endRun)
        {
            SaveRunButton.SaveGame(); 
            Debug.Log("Emergency Auto-Save complete.");
        }
    }
}

