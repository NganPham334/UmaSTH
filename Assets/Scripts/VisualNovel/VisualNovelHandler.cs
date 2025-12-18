using System;
using UnityEngine;
using VisualNovel;

public class VisualNovelHandler : MonoBehaviour
{
    /*
     * vn_type:
     * studying; add another param named "study_type" with value spd / mem / wit / luk
     * pastime
     * rest
     * post_test; add another parameter "post_test_dialogue" with the TextAsset inkJsonPass or inkJsonFail as the value
     * determined
     * random
     */

    // inspector configurations
    public DeterminedEventsTemplate timelineData;
    public DialogueManager dialogueManager; 
    public CurrentRunData currentRunData;
    
    private bool isEventPlaying = false;
    private String _vnType;
    
    void Awake()
    {
        if (!GameStateMan.Instance.TryGetStateParameter<String>("vn_type", out var type))
        {
            Debug.LogError("VisualNovelHandler: Novel Type not found");
            return;
        }
        _vnType = type;
        Debug.Log(type);
    }

    private void FinishScene()
    {
        if (_vnType == "determined")
        {
            GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
        }
        
        GameStateMan.Instance.ReportActionComplete();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch (_vnType)
        {
            case "determined":
                TextAsset storyEvent = timelineData.GetEventForTurn(currentRunData.CurrentTurn);

                if (storyEvent != null)
                {
                    PlayEvent(storyEvent);
                }
                else
                {
                    Debug.LogError("VisualNovelHandler: Determined event not found despite instruction from GameStateMan!");
                }
                break;
            case "post_test":
                GameStateMan.Instance.TryGetStateParameter<TextAsset>("post_test_dialogue", out var json);

                if (json != null)
                {
                    PlayEvent(json);
                }
                else
                {
                    Debug.LogError("VisualNovelHandler: Post-test dialogue not found!");
                }
                break;
        }
    }
    
    private void PlayEvent(TextAsset json)
    {
        isEventPlaying = true;
        
        dialogueManager.StartDialogue(json, () => {
            isEventPlaying = false;
            FinishScene();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
