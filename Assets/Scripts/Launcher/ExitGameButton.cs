using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ExitGameButton : MonoBehaviour
{
    [SerializeField] private string exitButtonName = "exit";

    private VisualElement _root;
    private VisualElement _exitBtn;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _exitBtn = _root.Q<VisualElement>(exitButtonName);

        if (_exitBtn != null)
        {
            // Ensure the custom box is clickable
            _exitBtn.pickingMode = PickingMode.Position;
            
            // Register the callback for custom VisualElements
            _exitBtn.RegisterCallback<ClickEvent>(OnExitClicked);
            Debug.Log($"<color=cyan>ExitGameButton:</color> Successfully bound {exitButtonName}.");
        }
    }

    private void OnExitClicked(ClickEvent evt)
    {
        Debug.Log("Exit Button Clicked. Closing Application...");

        // 1. If running in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 2. If running in a standalone build (.exe, .app)
        Application.Quit();
#endif
    }

    private void OnDisable()
    {
        if (_exitBtn != null)
        {
            _exitBtn.UnregisterCallback<ClickEvent>(OnExitClicked);
        }
    }
}