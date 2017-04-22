using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    class GameController : MonoBehaviourSingleton<GameController>, IBuildUi
    {
        public void Awake()
        {
            PrototypeManager.Instance.LoadPrototypes();
            SaveManager.Instance.LoadProfiles();
            //NewGame();
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


        public void NewGame( int level_index )
        {
            GameManager.Instance.NewGame((Level)PrototypeManager.Instance.Levels[ level_index ].Clone());
            StartCoroutine(GameTick());
        }

        public void BuildUi()
        {
            string resources = "";

            foreach (var currentLevelResource in GameManager.Instance.CurrentLevel.Resources)
            {
                resources += string.Format("{0}: {1} / {2}", 
                    currentLevelResource.Name,
                    currentLevelResource.Value,
                    currentLevelResource.MaxValue) + Environment.NewLine;
            }

            GameObject.Find("DebugText").GetComponent<Text>().text = resources;
        }
    }
}
