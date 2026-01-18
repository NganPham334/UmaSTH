using UnityEngine;

namespace DefaultNamespace
{
    public class LauncherScreen : MonoBehaviour
    {
        public void Launch()
        {
            GameStateMan.Instance.RequestState(GameStateMan.GameState.MainMenu);
        }
    }
}