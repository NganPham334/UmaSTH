using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Tooltip("Tên của scene như xuất hiện trong Build Settings")]
    public string sceneName;

    // Gọi từ Button OnClick trong Inspector
    public void LoadSceneByName()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("SceneLoader: sceneName chưa được set!");
            return;
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    // Nếu muốn load theo build index (cất giữ trong Build Settings)
    public void LoadSceneByIndex(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex, LoadSceneMode.Single);
    }
}
