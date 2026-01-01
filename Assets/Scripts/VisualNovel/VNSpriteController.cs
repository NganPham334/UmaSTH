using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using System;
using System.Collections;

namespace VisualNovel
{
    public class VNSpriteController : DialoguePresenterBase
    {
        [Header("Scene References")]
        public Image mainCharImage; 
        public Image sideCharImage; 

        [Header("Configuration")]
        public string mainCharName = "Blay";
        public Color activeColor = Color.white;
        public Color dimmedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        [Range(0.1f, 2.0f)]
        public float fadeDuration = 0.16f; 

        [Header("Positions")]
        public float centerX = 0f;  
        public float leftX = -400f; 

        private bool _isTwoSpriteMode = false;
        private string _currentSideCharBaseName = ""; 
        private string _currentMainCharBaseName = ""; 
        private string _currentSpeaker = "";
        
        private Coroutine _mainFadeRoutine;
        private Coroutine _sideFadeRoutine;
        private Coroutine _modeSwitchRoutine; 

        private void Awake()
        {
            if (TryGetComponent<DialogueRunner>(out var runner))
            {
                runner.AddCommandHandler<string>("main", SetMainSprite);
                runner.AddCommandHandler<string>("side", SetSideSprite);
                runner.AddCommandHandler<string>("mode", SetMode);
            }
            else
            {
                var foundRunner = FindObjectOfType<DialogueRunner>();
                if (foundRunner)
                {
                    foundRunner.AddCommandHandler<string>("main", SetMainSprite);
                    foundRunner.AddCommandHandler<string>("side", SetSideSprite);
                    foundRunner.AddCommandHandler<string>("mode", SetMode);
                }
            }
        }

        public override YarnTask RunLineAsync(LocalizedLine dialogueLine, LineCancellationToken token)
        {
            string speaker = dialogueLine.CharacterName;
            _currentSpeaker = speaker;

            if (!string.IsNullOrEmpty(speaker))
            {
                UpdateFocus(speaker);
                HandleAutoSpriteSwap(speaker);
            }

            return YarnTask.CompletedTask;
        }

        // Required Boilerplate
        public override YarnTask OnDialogueStartedAsync() => YarnTask.CompletedTask;
        public override YarnTask OnDialogueCompleteAsync() => YarnTask.CompletedTask;
        public override YarnTask<DialogueOption> RunOptionsAsync(DialogueOption[] o, LineCancellationToken t) => YarnTask<DialogueOption>.FromResult(null);

        private void SetMode(string mode)
        {
            if (mode == "1" || mode.ToLower() == "single") SetModeSingle();
            else if (mode == "2" || mode.ToLower() == "two" || mode.ToLower() == "side") SetModeTwo();
        }

        public void SetModeSingle()
        {
            if (!_isTwoSpriteMode) return; 
            if (IsInvisible(mainCharImage))
            {
                _isTwoSpriteMode = false;
                _currentSideCharBaseName = "";
                SetX(mainCharImage, centerX);
                sideCharImage.gameObject.SetActive(false);
                return;
            }
            
            if (_modeSwitchRoutine != null) StopCoroutine(_modeSwitchRoutine);
            _modeSwitchRoutine = StartCoroutine(PerformModeSwitch(toTwoMode: false));
        }

        public void SetModeTwo()
        {
            if (_isTwoSpriteMode) return; 
            if (IsInvisible(mainCharImage))
            {
                _isTwoSpriteMode = true;
                SetX(mainCharImage, leftX);
                return;
            }
            
            if (_modeSwitchRoutine != null) StopCoroutine(_modeSwitchRoutine);
            _modeSwitchRoutine = StartCoroutine(PerformModeSwitch(toTwoMode: true));
        }

        // Helper to check if we should skip animation
        private bool IsInvisible(Image target)
        {
            return !target.gameObject.activeInHierarchy || target.color.a <= 0.01f;
        }

        private IEnumerator PerformModeSwitch(bool toTwoMode)
        {
            _isTwoSpriteMode = toTwoMode;
            if (!toTwoMode) _currentSideCharBaseName = "";

            float halfDuration = fadeDuration / 2f;
            
            StartFade(mainCharImage, mainCharImage.color, 0f, halfDuration, ref _mainFadeRoutine);
            if (!toTwoMode && sideCharImage.gameObject.activeSelf)
            {
                StartFade(sideCharImage, sideCharImage.color, 0f, halfDuration, ref _sideFadeRoutine);
            }

            yield return new WaitForSeconds(halfDuration);

            // --- MOVE ---
            if (toTwoMode)
            {
                SetX(mainCharImage, leftX);
            }
            else
            {
                SetX(mainCharImage, centerX);
                sideCharImage.gameObject.SetActive(false);
            }
            
            Color finalColorForBlay = activeColor;
            
            if (!string.IsNullOrEmpty(_currentSpeaker) && _currentSpeaker != mainCharName)
            {
                finalColorForBlay = dimmedColor;
            }

            StartFade(mainCharImage, finalColorForBlay, 1f, halfDuration, ref _mainFadeRoutine);
            
            _modeSwitchRoutine = null;
        }
        
        private void SetMainSprite(string spriteName)
        {
            LoadSpriteIntoImage(mainCharImage, spriteName, ref _mainFadeRoutine);
            _currentMainCharBaseName = spriteName.Split('_')[0].ToLower(); 
        }

        private void SetSideSprite(string spriteName)
        {
            if (spriteName.ToLower() == "none" || spriteName == "1")
            {
                SetModeSingle();
            }
            else
            {
                SetModeTwo();
                LoadSpriteIntoImage(sideCharImage, spriteName, ref _sideFadeRoutine);
                _currentSideCharBaseName = spriteName.Split('_')[0].ToLower();
            }
        }

        private void HandleAutoSpriteSwap(string speaker)
        {
            if (speaker == mainCharName)
            {
                if (string.Equals(speaker, _currentMainCharBaseName, StringComparison.OrdinalIgnoreCase)) return;
                SetMainSprite(speaker);
                return;
            }

            if (_isTwoSpriteMode)
            {
                if (string.Equals(speaker, _currentSideCharBaseName, StringComparison.OrdinalIgnoreCase)) return; 
                SetSideSprite(speaker);
            }
        }

        private void UpdateFocus(string currentSpeaker)
        {
            if (_modeSwitchRoutine != null) return; // Wait for mode switch to finish

            if (!_isTwoSpriteMode)
            {
                StartFade(mainCharImage, activeColor, 1f, fadeDuration, ref _mainFadeRoutine);
                return;
            }

            if (currentSpeaker == mainCharName)
            {
                StartFade(mainCharImage, activeColor, 1f, fadeDuration, ref _mainFadeRoutine);
                StartFade(sideCharImage, dimmedColor, 1f, fadeDuration, ref _sideFadeRoutine);
            }
            else
            {
                StartFade(mainCharImage, dimmedColor, 1f, fadeDuration, ref _mainFadeRoutine);
                StartFade(sideCharImage, activeColor, 1f, fadeDuration, ref _sideFadeRoutine);
            }
        }

        private void LoadSpriteIntoImage(Image target, string spriteName, ref Coroutine fadeRoutine)
        {
            Sprite s = Resources.Load<Sprite>($"chars/{spriteName.ToLower()}");

            if (s != null)
            {
                target.sprite = s;
                target.preserveAspect = true;

                // APPEARANCE FADE: Only if it was previously OFF
                if (!target.gameObject.activeInHierarchy)
                {
                    Color c = target.color; 
                    c.a = 0f; 
                    target.color = c;
                    
                    target.gameObject.SetActive(true);
                    StartFade(target, activeColor, 1f, fadeDuration, ref fadeRoutine);
                }
            }
            else
            {
                Debug.LogWarning($"[VNSpriteController] Sprite not found: chars/{spriteName.ToLower()}");
            }
        }

        private void StartFade(Image target, Color targetColor, float targetAlpha, float duration, ref Coroutine currentRoutine)
        {
            if (target == null) return;
            if (currentRoutine != null) StopCoroutine(currentRoutine);
            currentRoutine = StartCoroutine(FadeRoutine(target, targetColor, targetAlpha, duration));
        }

        private IEnumerator FadeRoutine(Image target, Color endColor, float endAlpha, float duration)
        {
            Color startColor = target.color;
            float startAlpha = startColor.a;
            
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Lerp both Color and Alpha
                Color current = Color.Lerp(startColor, endColor, t);
                current.a = Mathf.Lerp(startAlpha, endAlpha, t);
                
                target.color = current;
                yield return null;
            }

            endColor.a = endAlpha;
            target.color = endColor;
        }

        private void SetX(Image img, float x)
        {
            RectTransform rt = img.GetComponent<RectTransform>();
            Vector2 pos = rt.anchoredPosition;
            pos.x = x;
            rt.anchoredPosition = pos;
        }
    }
}