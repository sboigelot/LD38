using System;
using System.Collections;
using Assets.Scripts.Managers;
using Assets.Scripts.Managers.DialogBoxes;
using Assets.Scripts.Models;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    class GameController : MonoBehaviourSingleton<GameController>
    {
        public void Awake()
        {
            PrototypeManager.Instance.LoadPrototypes();
            SaveManager.Instance.LoadProfiles();
            DialogBoxManager.Instance.Show(typeof(MainMenuController));
        }

        public void NewGame(int level_index)
        {
            GameManager.Instance.NewGame((Level) PrototypeManager.Instance.Levels[level_index].Clone());
            StartCoroutine(GameTick());
        }

        public IEnumerator GameTick()
        {
            while (true)
            {
                if (GameManager.Instance.CurrentLevel != null)
                {
                    GameManager.Instance.CurrentLevel.Tick();
                    RebuildUi();
                }

                yield return new WaitForSeconds(1);
            }
        }

        public void RebuildUi()
        {
            GameHud.Instance.OnGameTick();

            var resources = "";

            /*foreach (var currentLevelResource in GameManager.Instance.CurrentLevel.Resources)
            {
                resources += string.Format("{0}: {1} / {2}",
                                 currentLevelResource.Name,
                                 currentLevelResource.Value,
                                 currentLevelResource.MaxValue) + Environment.NewLine;
            }*/

            GameObject.Find("DebugText").GetComponent<Text>().text = resources;
        }
    }
}