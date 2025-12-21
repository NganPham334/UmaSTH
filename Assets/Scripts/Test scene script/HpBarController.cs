using UnityEngine;
using UnityEngine.UI;
//[ExecuteInEditMode]
public class HpBarController : MonoBehaviour
{
    public Slider hpBarSlider;
    public float maxHp;
    public float currentHp;
    public float targetHp;
    public Gradient hpGradient;
    public Image fill;
    public enum HpBarType{Player,Test};
    void Start()
    {
        currentHp = maxHp;
        targetHp = currentHp;
    }

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
}
