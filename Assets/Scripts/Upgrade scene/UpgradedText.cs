using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class UpgradedText : MonoBehaviour
{
    private RectTransform _rectTransform;
    private TextMeshProUGUI _textMeshProUGUI;
    void Awake()
    {
        // Fetch the RectTransform attached to this GameObject
        _rectTransform = GetComponent<RectTransform>();
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }
    public void UpdateUpgradedText(int count)
    {
        _textMeshProUGUI.SetText($"Upgraded! x{count}");
        _rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1).SetLink(gameObject);
    }
}
