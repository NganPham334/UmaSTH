using UnityEngine;
using TMPro;
using UnityEngine.Diagnostics;

public class DamagePopUp : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private const float DISAPPEAR_TIMER_MAX = 2f;

    public void Awake()
    {
        textMesh = transform.GetComponent<TextMeshProUGUI>();
        disappearTimer = DISAPPEAR_TIMER_MAX;
        moveVector = new Vector3(0, 30f, 0);
    }

    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= 1f * Time.deltaTime * moveVector;

        // --- 3. FADE & DESPAWN ---
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Start fading out
            float fadeSpeed = 3f;
            textColor.a -= fadeSpeed * Time.deltaTime;
            textMesh.color = textColor;

            // Destroy when completely invisible
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }

    }

    public void Setup(int damageAmount, bool isCrit)
    {
        if (isCrit)
        {
            textColor = Color.orangeRed;
            textMesh.fontSize = 50;
            textMesh.SetText('-'+damageAmount.ToString()+'!');

        }
        else 
        {
            textColor = Color.yellow;
            textMesh.fontSize = 40;
            textMesh.SetText('-'+damageAmount.ToString());
        }
        textMesh.faceColor = textColor;
    }
}
