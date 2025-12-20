using System;
using System.Text.RegularExpressions;
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
        
        [YarnCommand("stat")]
        public void ModifyStat(string[] inputs)
        {
            foreach (string input in inputs)
            {
                // Regex matches: Name (Group 1), Operator (Group 2), Number (Group 3)
                var match = Regex.Match(input, @"^(\w+)([-+])(\d+)$");

                if (!match.Success)
                {
                    Debug.LogWarning($"[StatSystem] Invalid syntax: '{input}'. Usage: <<stat luk+5 mem-2>>");
                    continue;
                }

                string statName = match.Groups[1].Value.ToLower();
                int mod = match.Groups[2].Value == "+" ? 1 : -1;
                int amount = int.Parse(match.Groups[3].Value);

                switch (statName)
                {
                    case "spd":
                        currentRunData.Speed = Math.Min(0, currentRunData.Speed + mod * amount);
                        break;
                    case "mem":
                        currentRunData.Memory = Math.Min(0, currentRunData.Memory + mod * amount);
                        break;
                    case "wit":
                        currentRunData.Wit = Math.Min(0, currentRunData.Wit + mod * amount);
                        break;
                    case "clr":
                        currentRunData.Clarity = Math.Min(0, currentRunData.Clarity + mod * amount);
                        break;
                    case "luk":
                        currentRunData.Luck = Math.Min(0, currentRunData.Luck + mod * amount);
                        break;
                    default:
                        Debug.LogWarning($"[StatSystem] Unknown stat: '{statName}'");
                        break;
                }
            }
        }
    }
}