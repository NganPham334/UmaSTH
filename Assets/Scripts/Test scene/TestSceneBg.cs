using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSceneBg : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private Sprite BackgroundSprite1;
    [SerializeField] private Sprite BackgroundSprite2;
    [SerializeField] private Sprite BackgroundSprite3;

    public void SetBackground(int year)
    {
        switch (year)
        {
            case 1:background.sprite = BackgroundSprite1; break;
            case 2:background.sprite = BackgroundSprite2; break;
            case 3:background.sprite = BackgroundSprite3; break;
            case 0: Debug.Log("Test year empty");break;
        }
    }
}
