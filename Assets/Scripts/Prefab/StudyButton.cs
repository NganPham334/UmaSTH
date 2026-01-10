using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
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
    private RectTransform rectTransform;
    private float originalX;

    // Helper method for switch case
    private StatType MyStatType => buttonType switch
    {
        ButtonType.Speed => StatType.spd,
        ButtonType.Wit => StatType.wit,
        ButtonType.Memory => StatType.mem,
        ButtonType.Luck => StatType.luk,
        _ => StatType.spd
    };

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

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
        rectTransform.DOAnchorPosX(originalX-30, 0.1f).SetLink(gameObject);
        rectTransform.DOScale(1.05f, 0.1f).SetLink(gameObject);

        // NOTE: Possibly redundant
        // // Show main and secondary stat gain popup
        // if (mainStatGainPopup != null && secondaryStatGainPopup != null)
        // {
        //     mainStatGainPopup.SetActive(true);  
        //     secondaryStatGainPopup.SetActive(true);
        // }
        // else
        // {
        //     Debug.LogWarning("Stat gain popups are not assigned.");
        // }
        
        // Get number from calculator
        var gains = StatsManager.Instance.GetExpectedGains(MyStatType);
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
            studyLvBar.UpdateLevelText(currentRunData.GetStatLevel(MyStatType), buttonType.ToString());
            studyLvBar.Activate();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
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
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ExecuteStudyAction(MyStatType);

            int turn = currentRunData.CurrentTurn;
            if (turn > 1 && turn % 4 == 0)
            {
                StatsManager.Instance.progressionHandler.TriggerUpgradeEvent(currentRunData.baseUpgradePoints);
                currentRunData.baseUpgradePoints += 1;
                return;
            }

            // 3. Report completion to move the game forward
            if (GameStateMan.Instance != null)
            {
                GameStateMan.Instance.ReportActionComplete();
            }
            Debug.Log($"Study Action executed for: {MyStatType}");
        }
        else
        {
            Debug.LogError("StatsManager Instance not found!");
        }
    }

    public void Shrink()
    {
        rectTransform.localScale = Vector3.zero;
    }

    public void PopUp()
    {
        rectTransform.DOScale(1f, 0.2f).SetLink(gameObject);
    }
}
