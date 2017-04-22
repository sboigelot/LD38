using Assets.Scripts.Controllers;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class GameHud : MonoBehaviourSingleton<GameHud>
    {
        public Button PauseButton;
        
        public void OnGameTick()
        {
            var level = LevelController.Instance.Level;
        }
    }
}