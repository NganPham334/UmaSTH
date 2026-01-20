using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class LoadGameButton : MonoBehaviour
{
    private const string SAVE_KEY = "SavedGameData";

    [Header("UI Configuration")]
    [SerializeField] private string newGameButtonName = "newgame";

    private VisualElement _root;
    private VisualElement _newGameBtn;

    private void OnEnable()
    {
        // Get the root from the UIDocument component on this GameObject
        _root = GetComponent<UIDocument>().rootVisualElement;
        
        // Find the SkewedRoundedBox or VisualElement by name
        _newGameBtn = _root.Q<VisualElement>(newGameButtonName);

        if (_newGameBtn != null)
        {
            // Ensure the element can receive pointer events
            _newGameBtn.pickingMode = PickingMode.Position;
            
            // Register the ClickEvent explicitly
            _newGameBtn.RegisterCallback<ClickEvent>(OnNewGameClicked);
            Debug.Log($"<color=cyan>LoadGameButton:</color> Successfully bound {newGameButtonName}.");
        }
        else
        {
            Debug.LogWarning($"<color=red>LoadGameButton:</color> Could not find {newGameButtonName} in UXML.");
        }
    }

    private void OnNewGameClicked(ClickEvent evt)
    {
        if (HasSavedGame())
        {
            Debug.LogWarning("Existing save detected. Overwriting with a New Game.");
            DeleteSavedGameData();
            
            // If the MenuPopupLoader is active in the scene, tell it to refresh its buttons
            // because a save no longer exists.
            if (MenuPopupLoader.Instance != null)
            {
                // We use a public refresh method if available, or TogglePopup to trigger RefreshPopupContent
                MenuPopupLoader.Instance.TogglePopup(); // Close/Open to refresh
                MenuPopupLoader.Instance.TogglePopup(); 
            }
        }

        StartNewRun();
    }

    private void StartNewRun()
    {
        if (GameStateMan.Instance != null)
        {
            if (GameStateMan.Instance.CurrentRun != null)
            {
                GameStateMan.Instance.CurrentRun.InitializeRun();
            }

            GameStateMan.Instance.StartGame();
            Debug.Log("New Game Started.");
        }
        else
        {
            Debug.LogError("GameStateMan instance missing!");
        }
    }

    private bool HasSavedGame()
    {
        return PlayerPrefs.HasKey(SAVE_KEY);
    }

    private void DeleteSavedGameData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        Debug.Log("Old save data purged.");
    }

    private void OnDisable()
    {
        // Clean up callbacks to avoid memory leaks or double-firing
        if (_newGameBtn != null)
        {
            _newGameBtn.UnregisterCallback<ClickEvent>(OnNewGameClicked);
        }
    }
}