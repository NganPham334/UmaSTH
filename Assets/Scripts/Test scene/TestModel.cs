using UnityEngine;
using UnityEngine.UI;
public class TestModel : MonoBehaviour
{
    [SerializeField] private Image testModel;
    [SerializeField] private Sprite TestSprite1;
    [SerializeField] private Sprite TestSprite2;
    [SerializeField] private Sprite TestSprite3;

    public void SetTestSprite(int year)
    {
        switch (year)
        {
            case 1:testModel.sprite = TestSprite1; break;
            case 2:testModel.sprite = TestSprite2; break;
            case 3:testModel.sprite = TestSprite3; break;
            case 0: Debug.Log("Test year empty");break;
        }
    }
}
