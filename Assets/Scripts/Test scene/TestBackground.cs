using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class TestBackground : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private List<Sprite> backgroundSprites;

    public void ChangeTestBackground(int year)
    {
        if (backgroundSprites[year-1] != null)
        background.sprite = backgroundSprites[year-1];
        else 
        Debug.Log("Empty background sprite");
    }
}
