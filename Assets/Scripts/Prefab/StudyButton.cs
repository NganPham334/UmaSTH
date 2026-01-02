using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class StudyButton : MonoBehaviour
{
    public static List<StudyButton> allStudyButtons = new();
    [SerializeField] private TextMeshProUGUI buttonText, levelText;
    public enum ButtonType {Speed, Wit, Memory, Luck};
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private CurrentRunData currentRunData;

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
}
