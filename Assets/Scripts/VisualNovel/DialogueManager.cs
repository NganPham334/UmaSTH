namespace VisualNovel
{
    using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bodyText;
    public Transform choiceRoot;
    public GameObject buttonPrefab;

    private Story story;
    private Action onDialogueComplete;

    // REMOVED 'string npcName' from arguments
    public void StartDialogue(TextAsset inkJSON, Action onComplete)
    {
        story = new Story(inkJSON.text);
        onDialogueComplete = onComplete;
        
        panel.SetActive(true);
        RefreshView();
    }

    private void RefreshView()
    {
        foreach (Transform child in choiceRoot) Destroy(child.gameObject);

        if (story.canContinue)
        {
            string rawText = story.Continue();
            ParseNameAndBody(rawText);
            
            if (story.currentChoices.Count > 0)
            {
                foreach (Choice choice in story.currentChoices)
                {
                    GameObject btn = Instantiate(buttonPrefab, choiceRoot);
                    btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
                    int i = choice.index;
                    btn.GetComponent<Button>().onClick.AddListener(() => OnChoose(i));
                }
            }
        }
        else
        {
            EndDialogue();
        }
    }

    private void ParseNameAndBody(string rawText)
    {
        if (rawText.Contains(":"))
        {
            // Split into 2 parts, allows the dialogue to contain ":" without issues
            string[] split = rawText.Split(new char[] { ':' }, 2);
            
            nameText.text = split[0].Trim();
            bodyText.text = split[1].Trim();
        }
        else
        {
            // Fallback 
            nameText.text = "???"; 
            bodyText.text = rawText.Trim();
        }
    }

    void OnChoose(int choiceIndex)
    {
        story.ChooseChoiceIndex(choiceIndex);
        RefreshView();
    }

    void EndDialogue()
    {
        panel.SetActive(false);
        onDialogueComplete?.Invoke();
        
    }

    void Update()
    {
        if (panel.activeSelf && story != null && story.currentChoices.Count == 0)
        {
            // 1. Check Keyboard (Spacebar)
            bool spacePressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;

            // 2. Check "Pointer" (Covers Mouse, Touch, and Pen all at once)
            // This is better for touchscreen laptops than checking Mouse/Touch separately
            bool pointerPressed = UnityEngine.InputSystem.Pointer.current != null && UnityEngine.InputSystem.Pointer.current.press.wasPressedThisFrame;

            if (spacePressed || pointerPressed)
            {
                RefreshView();
            }
        }
    }
}
}