using UnityEngine;
using UnityEngine.UIElements; 
using UnityButton = UnityEngine.UI.Button;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UIDocument))]
public class MenuPopupLoader : MonoBehaviour
{
    private VisualElement _root;
    private VisualElement _popup;
    private Label _ingame_time;
    [SerializeField] ControllerOfTime _formatted_time;

    public UnityButton sceneMenuButton; 
    public SaveAndReturnHome saveScript;
    public QuitToHomeButton giveUpScript;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _popup = _root.Q<VisualElement>("BlurBG");
        _ingame_time = _root.Q<Label>("time-label");

        if (_popup != null) _popup.style.display = DisplayStyle.None;

        if (sceneMenuButton != null)
            sceneMenuButton.onClick.AddListener(TogglePopup);

        SetupInternalButtons();
        _popup?.RegisterCallback<PointerDownEvent>(OnBackdropClick);
    }

    private void SetupInternalButtons()
    {
        Button closeBtn = _root.Q<Button>("close");
        if (closeBtn != null) closeBtn.clicked += TogglePopup;

        Button saveBtn = _root.Q<Button>("savegame");
        if (saveBtn != null && saveScript != null)
            saveBtn.clicked += saveScript.OnSaveAndReturnHomePressed; 

        Button giveUpBtn = _root.Q<Button>("giveup");
        if (giveUpBtn != null && giveUpScript != null)
            giveUpBtn.clicked += giveUpScript.OnQuitToHomePressed;
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePopup();
    }

    public void TogglePopup()
    {
        if (_popup == null) return;

        bool isOpening = (_popup.style.display == DisplayStyle.None);
        _popup.style.display = isOpening ? DisplayStyle.Flex : DisplayStyle.None;

        if (isOpening)
        {
            // This triggers every StatboxVariant in the scene to re-link to its label
            // and pull the latest data from the ScriptableObject
            StatboxVariant.UpdateAllStats();
            _ingame_time.text = _formatted_time.FormatTime();
        }
    }

    private void OnBackdropClick(PointerDownEvent evt)
    {
        if (evt.target == _popup) _popup.style.display = DisplayStyle.None;
    }

    private void OnDisable()
    {
        if (sceneMenuButton != null)
            sceneMenuButton.onClick.RemoveListener(TogglePopup);
    }
}