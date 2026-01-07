using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class StudyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static List<StudyButton> allStudyButtons = new();
    [SerializeField] private TextMeshProUGUI buttonText, levelText;
    public enum ButtonType {Speed, Wit, Memory, Luck};
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private CurrentRunData currentRunData;
    [SerializeField] private GameObject mainStatGainPopup, secondaryStatGainPopup;
    [SerializeField] private StatsManager StatsManager;
    [SerializeField] private Image lvImage;
    [SerializeField] private Color color1, color2, color3, color4, color5;

    void Start()
    {
        UpdateText();
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
        transform.localScale = Vector3.one * 1.03f;
        transform.localPosition += new Vector3(-30f, 0f, 0f);
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
        // Update the popup texts
        // mainStatGainPopup.GetComponentInChildren<TextMeshProUGUI>().SetText($"+{}");
        // secondaryStatGainPopup.GetComponentInChildren<TextMeshProUGUI>().SetText($"+{}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse left: " + buttonType);
        transform.localScale = Vector3.one;
        transform.localPosition += new Vector3(30f, 0f, 0f);
        // Hide main and secondary stat gain popups
        if (mainStatGainPopup != null && secondaryStatGainPopup != null)
        {
        mainStatGainPopup.SetActive(false);
        secondaryStatGainPopup.SetActive(false);
        }
    }
}
