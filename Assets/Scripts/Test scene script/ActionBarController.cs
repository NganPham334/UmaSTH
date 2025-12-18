using UnityEngine;
using UnityEngine.UI;
//[ExecuteInEditMode]

public class ActionBarController : MonoBehaviour


{
    public Slider actionBarSlider;
    public float max= 100;
    public float current= 0;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        current = Mathf.MoveTowards(current, max, speed/40 * Time.deltaTime);
        UpdateActionValue();
        if (current >= max)
        {
            current = 0;
        }
    }
    public void UpdateActionValue()
    {
        actionBarSlider.value = current/max;
    }
}
