using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class StudyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static List<StudyButton> allStudyButtons = new();
    [SerializeField] private TextMeshProUGUI buttonText, levelText;
    public enum ButtonType {Speed, Wit, Memory, Luck};
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private CurrentRunData currentRunData;
    [SerializeField] private GameObject mainStatGainPopup, secondaryStatGainPopup;
    [SerializeField] private StatsManager StatsManager;


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
        transform.localScale = Vector3.one * 1.1f;
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
        // Hide main and secondary stat gain popups
        if (mainStatGainPopup != null && secondaryStatGainPopup != null)
        {
        mainStatGainPopup.SetActive(false);
        secondaryStatGainPopup.SetActive(false);
        }
    }
}
