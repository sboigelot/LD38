using System.Collections;
using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.Utils;
using UnityEngine;

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

        public IEnumerator GameTick()
        {
            while (true)
            {
                if (GameManager.Instance.CurrentLevel != null)
                {
                    GameManager.Instance.CurrentLevel.Tick();
                    BuildUi();
                }

                yield return new WaitForSeconds(1);
            }
        }


        public void NewGame()
        {
            GameManager.Instance.NewGame((Level)PrototypeManager.Instance.Levels[0].Clone());
            StartCoroutine(GameTick());
        }

        public void BuildUi()
        {
        }
    }
}
