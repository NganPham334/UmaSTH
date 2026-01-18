using UnityEngine;
using UnityEngine.UIElements; 
using UnityButton = UnityEngine.UI.Button; // Standard Scene Button
using UnityEngine.InputSystem; 

[RequireComponent(typeof(UIDocument))]
public class MenuPopupLoader : MonoBehaviour
{
    private VisualElement _root;
    private VisualElement _popup;

    [Header("Scene Trigger")]
    [Tooltip("The GameObject button in your scene that opens the menu")]
    public UnityButton sceneMenuButton; 

    [Header("Script References")]
    [Tooltip("Drag the GameObject that has the SaveAndReturnHome_Button script")]
    public SaveAndReturnHome saveScript;
    
    [Tooltip("Drag the GameObject that has the GameScene_Button script")]
    public GameScene_Button giveUpScript;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _popup = _root.Q<VisualElement>("BlurBG");

        // Hide the popup by default
        if (_popup != null) _popup.style.display = DisplayStyle.None;

        // 1. Scene Button Toggle
        if (sceneMenuButton != null)
        {
            sceneMenuButton.onClick.AddListener(TogglePopup);
        }

        // 2. Bind Internal UI Toolkit Buttons
        SetupInternalButtons();

        // 3. Backdrop Click Close
        _popup?.RegisterCallback<PointerDownEvent>(OnBackdropClick);
    }

    private void SetupInternalButtons()
    {
        // Close Button (#close)
        Button closeBtn = _root.Q<Button>("close");
        if (closeBtn != null) closeBtn.clicked += TogglePopup;

        // Save and Return Button (#savegame)
        Button saveBtn = _root.Q<Button>("savegame");
        if (saveBtn != null && saveScript != null)
        {
            // This assumes your script has a public method to trigger the save
            saveBtn.clicked += saveScript.OnSaveAndReturnHomePressed; 
        }

        // Give Up Button (#giveup)
        Button giveUpBtn = _root.Q<Button>("giveup");
        if (giveUpBtn != null && giveUpScript != null)
        {
            // This assumes your script has a public method for the button logic
            giveUpBtn.clicked += giveUpScript.OnStudyButtonPressed;
        }
    }

    private void OnDisable()
    {
        if (sceneMenuButton != null)
            sceneMenuButton.onClick.RemoveListener(TogglePopup);
    }

    private void Update()
    {
        // Toggle with Escape key (New Input System)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePopup();
        }
    }

    public void TogglePopup()
    {
        if (_popup == null) return;
        _popup.style.display = (_popup.style.display == DisplayStyle.None) 
            ? DisplayStyle.Flex 
            : DisplayStyle.None;
    }

    private void OnBackdropClick(PointerDownEvent evt)
    {
        if (evt.target == _popup) _popup.style.display = DisplayStyle.None;
    }
}