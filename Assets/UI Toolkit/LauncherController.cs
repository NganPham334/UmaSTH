using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class LauncherController : MonoBehaviour
{
    private Label _versionLabel;
    private Label _userCodeLabel;
    private Label _startPrompt;
    private VisualElement _screenRoot;

    // Use Application.version for the real game version
    private string _gameVersion => Application.version; 
    private string _fakeUserCode = "ID-6000-X2";

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // 1. Query Elements
        _screenRoot = root.Q<VisualElement>("ScreenRoot");
        _versionLabel = root.Q<Label>("VersionLabel");
        _userCodeLabel = root.Q<Label>("UserCodeLabel");
        _startPrompt = root.Q<Label>("StartPrompt");

        // 2. Set Initial Data
        _versionLabel.text = $"v{_gameVersion}";
        _userCodeLabel.text = $"User: {_fakeUserCode}";

        // 3. Setup Click Event (The whole screen)
        _screenRoot.RegisterCallback<PointerDownEvent>(OnLauncherClicked);

        // 4. Start Flicker
        StartCoroutine(FlickerRoutine());
    }

    private void OnLauncherClicked(PointerDownEvent evt)
    {
        Debug.Log("Launcher Clicked! Loading Game...");
        // Add SceneManager.LoadScene here
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Toggle the CSS class to trigger the transition
            _startPrompt.ToggleInClassList("prompt-faded");
            yield return new WaitForSeconds(0.8f);
        }
    }
}