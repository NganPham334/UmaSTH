using UnityEngine;
using UnityEngine.UI;
//[ExecuteInEditMode]

public class ActionBarController : MonoBehaviour
{
    [SerializeField] private Slider actionBarSlider;
    [SerializeField] private float max= 100;
    [SerializeField] private float current= 0;
    [SerializeField] private float speed;
    private bool isRecharging = false;
    [SerializeField] private GameObject CombatLogicObject;
    private enum ActionBarType{Player,Test};
    [SerializeField] private ActionBarType actionBarType;


    // Update is called once per frame
    void Update()
    {
        if (!isRecharging)
        {
            current = Mathf.MoveTowards(current, max, speed/2 * Time.deltaTime);
        }
        if (isRecharging)
        {
            current = Mathf.MoveTowards(current, 0, 1000 * Time.deltaTime);
        }
        if (current == max)
        {
            isRecharging = true;
            if (actionBarType == ActionBarType.Player)
                CombatLogicObject.GetComponent<CombatLogic>().TestTakeDamage();
            if (actionBarType == ActionBarType.Test)
                CombatLogicObject.GetComponent<CombatLogic>().PlayerTakeDamage();
        }
        if (current <= 0)
        {
            isRecharging = false;
        }
        actionBarSlider.value = current/max;

    }
    
    public void setSpeed(float value)
    {
        speed = value;
    }
}
