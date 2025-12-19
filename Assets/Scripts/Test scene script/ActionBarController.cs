using UnityEngine;
using UnityEngine.UI;
//[ExecuteInEditMode]

public class ActionBarController : MonoBehaviour


{
    public Slider actionBarSlider;
    public float max= 100;
    public float current= 0;
    public float speed;
    public bool isRecharging = false;
    public GameObject CombatLogicObject;
    public enum ActionBarType{Player,Test};
    public ActionBarType actionBarType;


    // Update is called once per frame
    void Update()
    {
        if (!isRecharging)
        {
            current = Mathf.MoveTowards(current, max, speed/20 * Time.deltaTime);
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
}
