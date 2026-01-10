using UnityEngine;

public class EndTestScreen : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
    public void ShowEndTestScreen()
    {
        gameObject.SetActive(true);
        Invoke(nameof(PauseGame), 0.5f);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
}
