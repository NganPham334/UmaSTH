using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MMenuButtons : MonoBehaviour
{
    public void LoadScene(String scene)
    {
        SceneManager.LoadScene(scene);
    }
}
