using UnityEngine;
using UnityEngine.UIElements;
using UnityButton = UnityEngine.UI.Button;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class MenuPopupLoader : MonoBehaviour
{
    public static MenuPopupLoader Instance { get; private set; }

    [Header("Optional Scene Triggers")]
    [SerializeField] private UnityButton canvasTriggerButton; 
    [SerializeField] private UIDocument externalTriggerDocument;
    [SerializeField] private string uiToolkitTriggerName = "loadgame"; 

    [Header("Logic Scripts")]
    public SaveRunButton blueButtonScript; 
    public AbandonRunButton redButtonScript;   
    [SerializeField] private ControllerOfTime _timeController;

    private VisualElement _menuRoot;
    private VisualElement _popup;
    private Label _headerLabel, _ingameTime;
    private Button _blueBtn, _redBtn, _closeBtn;
    
    // Reference to the trigger button to disable it after abandonment
    private VisualElement _cachedExternalTrigger;
    private EventCallback<ClickEvent> _externalTriggerCallback;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void OnEnable()
    {
        _menuRoot = GetComponent<UIDocument>().rootVisualElement;
        
        _popup = _menuRoot.Q<VisualElement>("BlurBG");
        _headerLabel = _menuRoot.Q<Label>("header-label");
        _ingameTime = _menuRoot.Q<Label>("time-label");
        _blueBtn = _menuRoot.Q<Button>("savegame"); 
        _redBtn = _menuRoot.Q<Button>("giveup");    
        _closeBtn = _menuRoot.Q<Button>("close");

        if (_popup != null) _popup.style.display = DisplayStyle.None;

        _externalTriggerCallback = _ => TogglePopup();
        _closeBtn.clicked += TogglePopup;
        _popup?.RegisterCallback<PointerDownEvent>(OnBackdropClick);

        SetupTriggers();
    }

    private void SetupTriggers()
    {
        if (canvasTriggerButton != null)
        {
            canvasTriggerButton.onClick.RemoveListener(TogglePopup);
            canvasTriggerButton.onClick.AddListener(TogglePopup);
        }

        var searchRoot = externalTriggerDocument != null ? externalTriggerDocument.rootVisualElement : _menuRoot;
        _cachedExternalTrigger = searchRoot.Q<VisualElement>(uiToolkitTriggerName);

        if (_cachedExternalTrigger != null)
        {
            _cachedExternalTrigger.pickingMode = PickingMode.Position;
            _cachedExternalTrigger.RegisterCallback(_externalTriggerCallback);
            
            // Initial state check: if no save exists, disable the trigger immediately
            RefreshTriggerInteractivity();
        }
    }

    // Helper to disable/enable the "Continue" button on the Launcher
    private void RefreshTriggerInteractivity()
    {
        bool hasSave = SaveRunButton.HasSavedGame();
        
        // Handle UI Toolkit Trigger
        if (_cachedExternalTrigger != null)
        {
            _cachedExternalTrigger.SetEnabled(hasSave);
            // Optional: Add a class to dim it visually if USS doesn't handle :disabled
            if (!hasSave) _cachedExternalTrigger.AddToClassList("button--disabled");
            else _cachedExternalTrigger.RemoveFromClassList("button--disabled");
        }

        // Handle Canvas Trigger
        if (canvasTriggerButton != null)
        {
            canvasTriggerButton.interactable = hasSave;
        }
    }

    public void TogglePopup()
    {
        if (_popup == null) return;

        bool isOpening = _popup.style.display == DisplayStyle.None;
        _popup.style.display = isOpening ? DisplayStyle.Flex : DisplayStyle.None;

        if (isOpening) RefreshPopupContent();
    }

    private void RefreshPopupContent()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        _blueBtn.clicked -= OnContinuePressed;
        if (blueButtonScript != null) _blueBtn.clicked -= blueButtonScript.OnSaveAndReturnHomePressed;
        _redBtn.clicked -= OnAbandonLogic; 
        if (redButtonScript != null) _redBtn.clicked -= redButtonScript.OnQuitToHomePressed;

        if (currentScene == "LauncherScreen") SetupLauncherMenu();
        else SetupInGameMenu();

        if (_timeController != null) _ingameTime.text = _timeController.FormatTime();
        StatboxVariant.UpdateAllStats(); 
    }

    private void SetupLauncherMenu()
    {
        _headerLabel.text = "Continue";
        _blueBtn.text = "Load Save";
        _redBtn.text = "Abandon Save";
        _blueBtn.SetEnabled(SaveRunButton.HasSavedGame());
        _blueBtn.clicked += OnContinuePressed;
        _redBtn.clicked += OnAbandonLogic;
    }

    private void SetupInGameMenu()
    {
        _headerLabel.text = "Menu";
        _blueBtn.text = "Save & Exit";
        _redBtn.text = "Give Up";
        if (blueButtonScript != null) _blueBtn.clicked -= blueButtonScript.OnSaveAndReturnHomePressed;
        if (blueButtonScript != null) _blueBtn.clicked += blueButtonScript.OnSaveAndReturnHomePressed;
        
        if (redButtonScript != null) _redBtn.clicked -= redButtonScript.OnQuitToHomePressed;
        if (redButtonScript != null) _redBtn.clicked += redButtonScript.OnQuitToHomePressed;
    }

    private void OnContinuePressed()
    {
        blueButtonScript?.OnLoadingSavePressed();
        TogglePopup();
    }

    private void OnAbandonLogic()
    {
        // 1. Execute deletion logic
        redButtonScript?.OnAbandonSavePressed();
        
        // 2. Disable the buttons inside the popup immediately
        _blueBtn.SetEnabled(false); 
        
        // 3. Update the External Trigger (Launcher Button) so it becomes unclickable
        RefreshTriggerInteractivity();
        
        if (LauncherThing.Instance != null)
        {
            LauncherThing.Instance.RefreshUIState();
        }

        // 4. Close the popup
        TogglePopup();
        
        Debug.Log("Save abandoned. Popup closed and Launcher trigger disabled.");
    }

    private void Update()
    {
        if (Keyboard.current?.escapeKey.wasPressedThisFrame == true)
            TogglePopup();
    }

    private void OnBackdropClick(PointerDownEvent evt)
    {
        if (evt.target == _popup) TogglePopup();
    }

    private void OnDisable()
    {
        if (canvasTriggerButton != null) canvasTriggerButton.onClick.RemoveListener(TogglePopup);
        _closeBtn.clicked -= TogglePopup;

        if (_cachedExternalTrigger != null)
            _cachedExternalTrigger.UnregisterCallback(_externalTriggerCallback);
    }
}