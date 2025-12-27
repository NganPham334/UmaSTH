using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using System;

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

        [Header("Positions")]
        public float centerX = 0f;  
        public float leftX = -400f; 

        private bool _isTwoSpriteMode = false;
        
        // Trackers to prevent "Neutral" face overwriting "Shocked" face
        private string _currentSideCharBaseName = ""; 
        private string _currentMainCharBaseName = ""; 
        private void Awake()
        {
            if (TryGetComponent<DialogueRunner>(out var runner))
            {
                runner.AddCommandHandler<string>("main", SetMainSprite);
                runner.AddCommandHandler<string>("side", SetSideSprite);
            }
            else
            {
                var foundRunner = FindObjectOfType<DialogueRunner>();
                if (foundRunner)
                {
                    foundRunner.AddCommandHandler<string>("main", SetMainSprite);
                    foundRunner.AddCommandHandler<string>("side", SetSideSprite);
                }
            }
        }

        public override YarnTask RunLineAsync(LocalizedLine dialogueLine, LineCancellationToken token)
        {
            string speaker = dialogueLine.CharacterName;

            if (!string.IsNullOrEmpty(speaker))
            {
                UpdateFocus(speaker);
                HandleAutoSpriteSwap(speaker);
            }

            return YarnTask.CompletedTask;
        }

        // boilerplate
        public override YarnTask OnDialogueStartedAsync() => YarnTask.CompletedTask;
        public override YarnTask OnDialogueCompleteAsync() => YarnTask.CompletedTask;
        public override YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] options, LineCancellationToken token) => YarnTask<DialogueOption?>.FromResult(null);

        private void SetMainSprite(string spriteName)
        {
            LoadSpriteIntoImage(mainCharImage, spriteName);
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
                LoadSpriteIntoImage(sideCharImage, spriteName);
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
            if (!_isTwoSpriteMode)
            {
                mainCharImage.color = activeColor;
                return;
            }

            if (currentSpeaker == mainCharName)
            {
                mainCharImage.color = activeColor;
                sideCharImage.color = dimmedColor;
            }
            else
            {
                mainCharImage.color = dimmedColor;
                sideCharImage.color = activeColor;
            }
        }

        public void SetModeSingle()
        {
            _isTwoSpriteMode = false;
            _currentSideCharBaseName = "";
            
            SetX(mainCharImage, centerX);
            mainCharImage.color = activeColor; 
            sideCharImage.gameObject.SetActive(false);
        }

        public void SetModeTwo()
        {
            if (_isTwoSpriteMode) return;
            _isTwoSpriteMode = true;

            SetX(mainCharImage, leftX);
        }

        private void LoadSpriteIntoImage(Image target, string spriteName)
        {
            Sprite s = Resources.Load<Sprite>($"chars/{spriteName.ToLower()}");

            if (s != null)
            {
                target.sprite = s;
                target.preserveAspect = true;
                
                target.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"[VNSpriteController] Sprite not found: chars/{spriteName.ToLower()}");
            }
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