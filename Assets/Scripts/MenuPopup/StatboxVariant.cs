using UnityEngine;
using UnityEngine.UIElements;

public class StatboxVariant : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument; 
    [SerializeField] private CurrentRunData currentRunData;
    [SerializeField] private ExamSchedule examSchedule;

    // We no longer need a static list if one script handles everything
    private Label _speedLabel, _memoryLabel, _witLabel, _luckLabel;

    public void SetupAndRefreshAll()
    {
        if (uiDocument == null) return;
        VisualElement root = uiDocument.rootVisualElement;

        // Find each "Value" label by using its unique sibling as an anchor
        _speedLabel = FindValueLabel(root, "Speed");
        _memoryLabel = FindValueLabel(root, "Memory");
        _witLabel = FindValueLabel(root, "Wit");
        _luckLabel = FindValueLabel(root, "Luck");

        UpdateAllValues();
    }

    private Label FindValueLabel(VisualElement root, string statName)
    {
        // Find the label that literally says "Speed", "Memory", etc.
        Label anchor = root.Q<Label>(statName);
        if (anchor != null)
        {
            // Move up to the Card and find the "Value" label inside it
            // Structure: Card -> Header -> StatName
            // We go: StatName -> Parent (Header) -> Parent (Card) -> Q("Value")
            return anchor.parent?.parent?.Q<Label>("Value");
        }
        return null;
    }

    public void UpdateAllValues()
    {
        // Update each label with the specific data from the ScriptableObject
        if (_speedLabel != null) _speedLabel.text = currentRunData.GetStatValue(StatType.spd).ToString();
        if (_memoryLabel != null) _memoryLabel.text = currentRunData.GetStatValue(StatType.mem).ToString();
        if (_witLabel != null) _witLabel.text = currentRunData.GetStatValue(StatType.wit).ToString();
        if (_luckLabel != null) _luckLabel.text = currentRunData.GetStatValue(StatType.luk).ToString();
    }

    // This is the static method your MenuPopupLoader calls
    public static void UpdateAllStats()
    {
        // Find the single instance in the scene and tell it to refresh
        var instance = FindFirstObjectByType<StatboxVariant>();
        if (instance != null) instance.SetupAndRefreshAll();
    }
}