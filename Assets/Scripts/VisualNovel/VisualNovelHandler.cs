using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using Yarn.Unity;
using System.Linq;
using UnityEngine.Serialization;
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
        
        [FormerlySerializedAs("_backgroundImage")] 
        [Header("BG Image Panel object")]
        public Image backgroundImage;
        
        [Header("Transition Timing")]
        [Range(0.1f, 2.0f)]
        public float fadeSpeed = 0.5f;

        [Range(0.0f, 2.0f)] 
        public float blackHold = 0.5f;
        
        public VNSpriteController spriteController;
        
        [Tooltip("Fader panel")]
        public CanvasGroup blackScreenOverlay; 
        private Coroutine _activeTransition;
        
        /*
         * vn_type:
         * studying; add another param named "study_type" with value spd / mem / wit / luk
         * plus suffix pass / fail;  Exmaple: spd_fail, mem_pass
         * pastime
         * rest
         * post_test; add another parameter "post_test_node" with the dialogue node's name as the value
         * determined
         * random
         * intro
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
            if (dialogueRunner != null)
            {
                dialogueRunner.onDialogueComplete.RemoveListener(FinishScene);
            }
            Time.timeScale = 1.0f;
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
                
                case "rest" :
                    nodeName = "Rest";
                    break;
                
                case "pastime":
                    nodeName = "Pastime";
                    break;
                
                case "random":
                    var events = dialogueRunner.Dialogue.NodeNames
                        .Where(node => node.StartsWith("Random_")).ToList();
                    nodeName = events[Random.Range(0, events.Count())]; // for integers the max param is exclusive
                    break;
                
                case "intro":
                    nodeName = "Introduction";
                    break;
                
                default:
                    Debug.Log(_vnType);
                    Debug.LogError("VN Type not found /!\\ this is bad, closing scene...");
                    FinishScene();
                    break;
            }

            if (string.IsNullOrEmpty(nodeName))
            {
                Debug.LogWarning($"[VNHandler] nodeName not present!");
                FinishScene();
                return;
            }

            if (!dialogueRunner.Dialogue.NodeExists(nodeName))
            {
                Debug.LogWarning($"[VNHandler] Node not found: {nodeName}");
                FinishScene();
                return;
            }
            
            Debug.Log($"[VNHandler] Node: {nodeName}, vnType: {_vnType}");
            string tags = dialogueRunner.Dialogue.GetHeaderValue(nodeName, "tags");
            if (tags != null) ParseHeaderTags(tags);
            dialogueRunner.StartDialogue(nodeName);
        }

        private void ParseHeaderTags(string tags)
        {
            foreach (string tag in tags.Split(" "))
            {
                if (tag.ToLower().StartsWith("scene:"))
                {
                    SetSceneImmediate(tag.Substring("scene:".Length));
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
            if (_vnType == "determined" || _vnType == "intro")
            {
                GameStateMan.Instance.RequestState(GameStateMan.GameState.GameScene);
                return; // god knows why this is needed
            }
            
            GameStateMan.Instance.ReportActionComplete(_vnType == "random" ? "from_random" : null);
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
                
                Debug.Log($"{statName} change: {amount * mod}");
                var runData = Instance.currentRunData;
                switch (statName)
                {
                    case "spd":
                        runData.SetStatValue(StatType.spd, runData.Speed + mod * amount);
                        break;
                    case "mem":
                        runData.SetStatValue(StatType.mem, runData.Memory + mod * amount);
                        break;
                    case "wit":
                        runData.SetStatValue(StatType.wit, runData.Wit + mod * amount);
                        break;
                    case "clr":
                        runData.SetStatValue(StatType.clr, runData.Clarity + mod * amount);
                        break;
                    case "luk":
                        runData.SetStatValue(StatType.luk, runData.Luck + mod * amount);
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
        private void SetSceneImmediate(string spriteName)
        {
            if (backgroundImage == null) return;

            Sprite newBg = Resources.Load<Sprite>($"Backgrounds/{spriteName}");
            if (newBg == null) newBg = Resources.Load<Sprite>(spriteName);

            if (newBg != null)
            {
                backgroundImage.sprite = newBg;
                backgroundImage.preserveAspect = true;
                backgroundImage.color = new Color(255, 255, 255, 255);
            }
        }

        [YarnCommand("scene")]
        public static void SetScene(string spriteName)
        {
            if (Instance == null) return;
            
            Sprite newBg = Resources.Load<Sprite>($"Backgrounds/{spriteName}");
            if (newBg == null) newBg = Resources.Load<Sprite>(spriteName); 

            if (newBg != null)
            {
                if (Instance._activeTransition != null) Instance.StopCoroutine(Instance._activeTransition);
                Instance._activeTransition = Instance.StartCoroutine(Instance.PlaySceneTransition(newBg));
            }
            else
            {
                Debug.LogWarning($"[VNHandler] Could not find background '{spriteName}'");
            }
        }

        private IEnumerator PlaySceneTransition(Sprite newBg)
        {
            if (blackScreenOverlay == null)
            {
                if (backgroundImage != null)
                {
                    backgroundImage.color = new Color(1f, 1f, 1f, 1f);
                    backgroundImage.sprite = newBg;
                    backgroundImage.preserveAspect = true; 
                }
                yield break;
            }
            
            ForceStretchToScreen(blackScreenOverlay.GetComponent<RectTransform>());

            // so we can change the speeds relative to each other if needed
            float fadeOutTime = fadeSpeed;
            float fadeInTime = fadeSpeed; 
            
            blackScreenOverlay.blocksRaycasts = true; 
            yield return StartCoroutine(FadeRoutine(blackScreenOverlay, 0f, 1f, fadeOutTime, true));
            
            if (backgroundImage != null)
            {
                backgroundImage.sprite = newBg;
                backgroundImage.color = new Color(1f, 1f, 1f, 1f);
                backgroundImage.preserveAspect = true; 
            }
            yield return new WaitForSeconds(blackHold);
            
            yield return StartCoroutine(FadeRoutine(blackScreenOverlay, 1f, 0f, fadeInTime, false));

            _activeTransition = null;
        }
        
        private IEnumerator FadeRoutine(CanvasGroup cg, float startAlpha, float endAlpha, float duration, bool isEaseIn)
        {
            float elapsed = 0f;
            bool inputReleased = false;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                
                float easedT = 0f;
                if (isEaseIn)
                {
                    easedT = (t == 0f) ? 0f : Mathf.Pow(2f, 10f * t - 10f);
                }
                else
                {
                    easedT = (t == 1f) ? 1f : 1f - Mathf.Pow(2f, -10f * t);
                }

                cg.alpha = Mathf.Lerp(startAlpha, endAlpha, easedT);
                
                if (endAlpha == 0f && !inputReleased && t >= 0.7f)
                {
                    cg.blocksRaycasts = false;
                    inputReleased = true;
                }

                yield return null;
            }
            cg.alpha = endAlpha;
            if (endAlpha == 0f) cg.blocksRaycasts = false;
        }
        
        private void ForceStretchToScreen(RectTransform rt)
        {
            if (rt == null) return;
            rt.anchorMin = Vector2.zero; // Bottom Left
            rt.anchorMax = Vector2.one;  // Top Right
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero; // No margins
            rt.offsetMax = Vector2.zero;
            rt.localScale = Vector3.one;
        }
    }
}