using UnityEngine;
using TMPro;
using UnityEngine.Diagnostics;

public class DamagePopUp : MonoBehaviour
{
    private TextMeshPro textMesh;

    public void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    public void Setup(int damageAmount, bool isCrit)
    {
        if (isCrit)
        {
            textMesh.color = Color.red;
            textMesh.fontSize = 45;
        }
        else 
        {
            textMesh.color = Color.orange;
            textMesh.fontSize = 36;
        }
        textMesh.SetText(damageAmount.ToString());
    }
}
