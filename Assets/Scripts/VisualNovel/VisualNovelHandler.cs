using System;
using UnityEngine;
using VisualNovel;

public class VisualNovelHandler : MonoBehaviour
{
    /*
     * vn_type:
     * studying: add another param named "study_type" with value spd / mem / wit / luk
     * pastime
     * rest
     * pre_test
     * post_test
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
                TurnEvent storyEvent = timelineData.GetEventForTurn(currentRunData.CurrentTurn);

                if (storyEvent != null)
                {
                    PlayEvent(storyEvent);
                }
                else
                {
                    Debug.LogError("VisualNovelHandler: Determined event not found despite instruction from GameStateMan!");
                }
                break;
        }
    }
    
    private void PlayEvent(TurnEvent evt)
    {
        isEventPlaying = true;
        
        // Removed 'evt.speakerName'
        dialogueManager.StartDialogue(evt.inkJSON, () => {
            isEventPlaying = false;
            FinishScene();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
