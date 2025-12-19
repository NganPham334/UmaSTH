using UnityEngine;
using Yarn.Unity;

namespace VisualNovel
{

    public class VisualNovelHandler : MonoBehaviour
    {
        [Header("Yarn References")]
        public DialogueRunner dialogueRunner;

        [Header("Game Data")] 
        public CurrentRunData currentRunData;
        
        /*
         * vn_type:
         * studying; add another param named "study_type" with value spd / mem / wit / luk
         * pastime
         * rest
         * post_test; add another parameter "post_test_name" with the dialogue node's name as the value
         * determined
         * random
         */
        
        private string _vnType;

        void Awake()
        {
            // Get the current Visual Novel Mode (Determined, Post-Test, etc.)
            if (!GameStateMan.Instance.TryGetStateParameter<string>("vn_type", out var type))
            {
                Debug.LogError("VisualNovelHandler: Novel Type not found");
                return;
            }

            _vnType = type;

            if (dialogueRunner != null)
            {
                dialogueRunner.onDialogueComplete.AddListener(FinishScene);
            }
        }

        void OnDestroy()
        {
            // Clean up event listener to prevent memory leaks
            if (dialogueRunner != null)
            {
                dialogueRunner.onDialogueComplete.RemoveListener(FinishScene);
            }
        }

        void Start()
        {
            string nodeName = "";
            switch (_vnType)
            {
                case "determined":
                    nodeName = $"Turn_{currentRunData.CurrentTurn}";
                    break;

                case "post_test":
                    GameStateMan.Instance.TryGetStateParameter("post_test_node", out nodeName);
                    break;
                
                case "studying":
                    GameStateMan.Instance.TryGetStateParameter("study_type", out nodeName);
                    nodeName = "Study_" + nodeName;
                    break;
            }
            
            if (!string.IsNullOrEmpty(nodeName) && dialogueRunner.Dialogue.NodeExists(nodeName))
            {
                dialogueRunner.StartDialogue(nodeName);
            }
            else
            {
                Debug.LogError($"Yarn Node '{nodeName}' not found! Closing scene.");
                FinishScene();
            }
        }
        
        private void FinishScene()
        {
            if (_vnType == "determined")
            {
                GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
                return; // god knows why this is needed
            }
            
            GameStateMan.Instance.ReportActionComplete();
        }
    }
}