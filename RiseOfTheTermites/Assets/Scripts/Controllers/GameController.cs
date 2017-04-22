using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Controllers
{
    class GameController : MonoBehaviourSingleton<GameController>, IBuildUi
    {
        public void Awake()
        {
            PrototypeManager.Instance.LoadPrototypes();
            SaveManager.Instance.LoadProfiles();
            NewGame();
        }

        public void NewGame()
        {
            GameManager.Instance.NewGame((Level)PrototypeManager.Instance.Levels[0].Clone());
            BuildUi();
        }

        public void BuildUi()
        {
        }
    }
}
