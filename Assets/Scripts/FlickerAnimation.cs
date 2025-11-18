using UnityEngine;
using UnityEngine.UI;

public class FlickerAnimation : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float flickerSpeed = 4f;  // Adjust for flicker pace
    public float minAlpha = 0.3f;    // Lower bound of alpha
    public float maxAlpha = 1f;      // Upper bound of alpha

    void Start()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            Debug.LogError("Attach a CanvasGroup to the button or assign it manually.");
    }

    void Update()
    {
        float ping = Mathf.PingPong(Time.time * flickerSpeed, 1f);
        canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, ping);
    }
}
