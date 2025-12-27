using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Yarn.Unity;
using System.Linq;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace VisualNovel
{

    public class VisualNovelHandler : MonoBehaviour
    {
        private static VisualNovelHandler Instance { get; set; }
        
        [Header("Yarn References")]
        public DialogueRunner dialogueRunner;

        [Header("Game Data")] 
        public CurrentRunData currentRunData;
        
        public Image backgroundImage;
        
        public VNSpriteController spriteController;
        
        /*
         * vn_type:
         * studying; add another param named "study_type" with value spd / mem / wit / luk
         * plus suffix pass / fail;  Exmaple: spd_fail, mem_pass
         * pastime
         * rest
         * post_test; add another parameter "post_test_name" with the dialogue node's name as the value
         * determined
         * random
         */
        
        private string _vnType;

        void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(gameObject); 
                return;
            }
            Instance = this;
            
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
            if (Instance == this) Instance = null;
            
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
                
                case "random":
                    var events = dialogueRunner.Dialogue.NodeNames
                        .Where(node => node.StartsWith("Random_")).ToList();
                    nodeName = events[Random.Range(0, events.Count())]; // for integers the max param is exclusive
                    break;
            }
            
            if (!string.IsNullOrEmpty(nodeName) && dialogueRunner.Dialogue.NodeExists(nodeName))
            {
                string tags = dialogueRunner.Dialogue.GetHeaderValue(nodeName, "tags");
                if (tags != null) ParseHeaderTags(tags);
                dialogueRunner.StartDialogue(nodeName);

            }
            else
            {
                Debug.LogError($"Yarn Node '{nodeName}' of scene {_vnType} not found! Closing scene.");
                FinishScene();
            }
        }

        private void ParseHeaderTags(string tags)
        {
            foreach (string tag in tags.Split(" "))
            {
                if (tag.ToLower().StartsWith("scene:"))
                {
                    SetScene(tag.Substring("scene:".Length));
                }

                if (tag.ToLower() == "2sp")
                {
                    if (spriteController != null) 
                    {
                        spriteController.SetModeTwo();
                    }
                }
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
        public static void ModifyStat(string[] inputs)
        {
            foreach (string input in inputs)
            {
                // Regex matches: Name (Group 1), Operator (Group 2), Number (Group 3)
                var match = Regex.Match(input, @"^(\w+)([-+])(\d+)$");

                if (!match.Success)
                {
                    Debug.LogWarning($"[VNHandler] Invalid syntax: '{input}'. Usage: <<stat luk+5 mem-2>>");
                    continue;
                }

                string statName = match.Groups[1].Value.ToLower();
                int mod = match.Groups[2].Value == "+" ? 1 : -1;
                int amount = int.Parse(match.Groups[3].Value);
                
                var runData = Instance.currentRunData;
                
                Debug.Log(statName + " " + mod * amount);
                switch (statName)
                {
                    case "spd":
                        runData.Speed = Math.Max(0, Math.Min(1000, runData.Speed + mod * amount));
                        break;
                    case "mem":
                        runData.Memory = Math.Max(0, Math.Min(1000, runData.Memory + mod * amount));
                        break;
                    case "wit":
                        runData.Wit = Math.Max(0, Math.Min(1000, runData.Wit + mod * amount));
                        break;
                    case "clr":
                        runData.Clarity = Math.Max(0, Math.Min(1000, runData.Clarity + mod * amount));
                        break;
                    case "luk":
                        runData.Luck = Math.Max(0, Math.Min(1000, runData.Luck + mod * amount));
                        break;
                    case "mood":
                        runData.ChangeMood(mod * amount);
                        break;
                    default:
                        Debug.LogWarning($"[VNHandler] Unknown stat: '{statName}'");
                        break;
                }
            }
        }

        [YarnCommand("randMod")]
        public static void ModifyRandom(int change)
        {
            var runData = Instance.currentRunData;
            
            switch (Random.Range(0, 4))
            {
                case 0:
                    runData.Speed += change;
                    break;
                case 1:
                    runData.Memory += change;
                    break;
                case 2:
                    runData.Wit += change;
                    break;
                case 3:
                    runData.Luck += change;
                    break;
            }
        }
        
        // USAGE: <<scene "navia_test">> 
        // Loads from: Assets/Resources/Backgrounds/navia_test.png (jpg should work too i think)
        [YarnCommand("scene")]
        public static void SetScene(string spriteName)
        {
            if (Instance == null || Instance.backgroundImage == null) return;
            
            Sprite newBg = Resources.Load<Sprite>($"Backgrounds/{spriteName}");

            if (newBg != null)
            {
                Instance.backgroundImage.sprite = newBg;
                Instance.backgroundImage.preserveAspect = true; 
                Instance.backgroundImage.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                Debug.LogWarning($"[VNHandler] Could not find 'Resources/Backgrounds/{spriteName}'");
            }
        }
    }
}