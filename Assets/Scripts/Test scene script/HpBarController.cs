using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
//[ExecuteInEditMode]
public class HpBarController : MonoBehaviour
{
    [SerializeField] private Slider hpBarSlider;
    private float maxHp;
    private float currentHp;
    private float targetHp;
    [SerializeField] private Gradient hpGradient;
    [SerializeField] private Image fill;
    void Update()
    {
        if (currentHp != targetHp)
        {
            currentHp = Mathf.Lerp(currentHp, targetHp, 5 * Time.deltaTime);
        }
        hpBarSlider.value = currentHp / maxHp;
        fill.color = hpGradient.Evaluate(hpBarSlider.normalizedValue);
    }

    public void TakeDamage(float damage)
    {
        targetHp = currentHp - damage;
        Debug.Log($"HP reduced by {damage}. Current HP: {targetHp}.");
    }

    public void SetMaxHp(float hp)
    {
        maxHp = hp;
        currentHp = maxHp;
        targetHp = currentHp;
    }

    public float GetValue()
    {
        return hpBarSlider.value;
    }
}
