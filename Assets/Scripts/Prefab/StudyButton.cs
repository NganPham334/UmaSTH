using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using DG.Tweening;
public class StudyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public static List<StudyButton> allStudyButtons = new();
    [SerializeField] private TextMeshProUGUI buttonText, levelText;
    public enum ButtonType {Speed, Wit, Memory, Luck};
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private CurrentRunData currentRunData;
    [SerializeField] private GameObject mainStatGainPopup, secondaryStatGainPopup;
    [SerializeField] private Image lvImage;
    [SerializeField] private Color color1, color2, color3, color4, color5;
    [SerializeField] private StudyLvBar studyLvBar;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject UpgradeEventUI;
    private float originalX;

    void Start()
    {
        UpdateText();
        originalX = rectTransform.anchoredPosition.x;
    }

    void OnEnable()
    {
        allStudyButtons.Add(this);
    }
    void OnDisable()
    {
        allStudyButtons.Remove(this);
    }
    public void SetUp(ButtonType type)
    {
        buttonType = type;
    }
    public void UpdateText()
    {
        int level = currentRunData.GetStatLevel((buttonType) switch
        {
            ButtonType.Speed => StatType.spd,
            ButtonType.Wit => StatType.wit,
            ButtonType.Memory => StatType.mem,
            ButtonType.Luck => StatType.luk,
            _ => StatType.spd
        });
        levelText.SetText($"Lv. {level}");
        lvImage.color = level switch
        {
            1 => color1,
            2 => color2,
            3 => color3,
            4 => color4,
            5 => color5,
            _ => Color.white,
        };
        buttonText.SetText($"{buttonType}");
    }

    public static void UpdateAllButtons()
    {
        foreach (StudyButton button in allStudyButtons)
        {
            button.UpdateText();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse is hovering over: " + buttonType);
        rectTransform.DOAnchorPosX(originalX-30, 0.1f).SetLink(gameObject);
        rectTransform.DOScale(1.05f, 0.1f).SetLink(gameObject);
        // Show main and secondary stat gain popup
        if (mainStatGainPopup != null && secondaryStatGainPopup != null)
        {
            mainStatGainPopup.SetActive(true);  
            secondaryStatGainPopup.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Stat gain popups are not assigned.");
        }
        
        StatType primaryType = (buttonType) switch
        {
            ButtonType.Speed => StatType.spd,
            ButtonType.Wit => StatType.wit,
            ButtonType.Memory => StatType.mem,
            ButtonType.Luck => StatType.luk,
            _ => StatType.spd
        };
        // Get number from calculator
        var gains = StatsManager.Instance.GetExpectedGains(primaryType);
        // update text
        if (mainStatGainPopup != null && secondaryStatGainPopup != null)
        {
            mainStatGainPopup.GetComponentInChildren<TextMeshProUGUI>().SetText($"+{gains.pGain}");
            secondaryStatGainPopup.GetComponentInChildren<TextMeshProUGUI>().SetText($"+{gains.sGain}");
            
            mainStatGainPopup.SetActive(true);  
            secondaryStatGainPopup.SetActive(true);
        }

        if (studyLvBar != null)
        {
            studyLvBar.UpdateLevelText(currentRunData.GetStatLevel((buttonType) switch
            {
                ButtonType.Speed => StatType.spd,
                ButtonType.Wit => StatType.wit,
                ButtonType.Memory => StatType.mem,
                ButtonType.Luck => StatType.luk,
                _ => StatType.spd
            }), buttonType.ToString());
            studyLvBar.Activate();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse left: " + buttonType);
        rectTransform.DOAnchorPosX(originalX, 0.1f).SetLink(gameObject);
        rectTransform.DOScale(1f, 0.1f).SetLink(gameObject);
        // Hide main and secondary stat gain popups
        if (mainStatGainPopup != null && secondaryStatGainPopup != null)
        { 
            mainStatGainPopup.SetActive(false);
            secondaryStatGainPopup.SetActive(false);
        }
        if (studyLvBar != null)
        {
            studyLvBar.Deactivate();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Mouse is clicking on: " + buttonType);
        StatType primaryStat = (buttonType) switch
        {
            ButtonType.Speed => StatType.spd,
            ButtonType.Wit => StatType.wit,
            ButtonType.Memory => StatType.mem,
            ButtonType.Luck => StatType.luk,
            _ => StatType.spd
        };
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ExecuteStudyAction(primaryStat);

            int turn = currentRunData.CurrentTurn;
            if (turn > 1 && turn % 4 == 0)
            {
                List<StatType> results = StatsManager.Instance.progressionHandler.TriggerUpgradeEvent(currentRunData.baseUpgradePoints);
                currentRunData.baseUpgradePoints += 1;
                
                UpgradeEventUI.SetActive(true);
                StartCoroutine(UpgradeEventUI.GetComponent<UpgradeEventUI>().DisplayUpgradeStudy(results));
                // TODO: UpgradeUIManager.Instance.ShowSummary(results);
                return;
            }

            // 3. Report completion to move the game forward
            if (GameStateMan.Instance != null)
            {
                GameStateMan.Instance.ReportActionComplete();
            }
            Debug.Log($"Study Action executed for: {primaryStat}");
        }
        else
        {
            Debug.LogError("StatsManager Instance not found!");
        }
    }
}
