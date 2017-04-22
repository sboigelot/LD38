using Assets.Scripts.Managers;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Controllers
{
    class GameController : MonoBehaviourSingleton<GameController>, IBuildUi
    {
        public void Awake()
        {
            PrototypeManager.Instance.LoadPrototypes();
            SaveManager.Instance.LoadProfiles();
        }

        public void NewGame()
        {
            GameManager.Instance.NewGame();
            BuildUi();
        }

        public void BuildUi()
        {
        }
    }
}
