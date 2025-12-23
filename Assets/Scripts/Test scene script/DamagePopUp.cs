using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    public TextMeshPro damageText;

    public void Setup(int damageAmount)
    {
        damageText.text = damageAmount.ToString();
    }
}
