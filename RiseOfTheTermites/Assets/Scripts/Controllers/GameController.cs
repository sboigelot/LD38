using System.Collections;
using System.Linq;
using Assets.Scripts.Components;
using Assets.Scripts.Managers;
using Assets.Scripts.Managers.DialogBoxes;
using Assets.Scripts.Models;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviourSingleton<GameController>
    {
        private bool IsGameOver;

        public bool IsGamePaused;

        public void Awake()
        {
            StartCoroutine(PrototypeManager.Instance.LoadPrototypes());

            //SaveManager.Instance.LoadProfiles();
            DialogBoxManager.Instance.Show(typeof(MainMenuController));
        }

        public void NewGame(int level_index)
        {
            IsGameOver = false;
            //Hack: Don't want to store it for everyone
            PrototypeManager.Instance.Levels[level_index].Index = level_index;

            GameManager.Instance.NewGame((Level) PrototypeManager.Instance.Levels[level_index].Clone());
            StartCoroutine(GameTick());
        }

        public IEnumerator GameTick()
        {
            while (!IsGameOver)
            {
                if (GameManager.Instance.CurrentLevel != null)
                {
                    GameManager.Instance.CurrentLevel.Tick();
                    RemoveDeadFighters();
                    RebuildUi();
                }

                yield return new WaitForSeconds(1);
            }
        }

        public void RebuildUi()
        {
            GameHud.Instance.OnGameTick();

            var resources = "";

            /*foreach (var currentLevelResource in GameManager.Instance.CurrentLevel.ColonyStats)
            {
                resources += string.Format("{0}: {1} / {2}",
                                 currentLevelResource.Name,
                                 currentLevelResource.Value,
                                 currentLevelResource.MaxValue) + Environment.NewLine;
            }*/

            GameObject.Find("DebugText").GetComponent<Text>().text = resources;
        }

        /// <summary>
        ///     After all updates are done we can safely removed dead bodies
        /// </summary>
        void RemoveDeadFighters()
        {
            var deadEnemies = LevelController.
                Instance.
                EnemyLayer.
                GetComponentsInChildren<FighterComponent>().
                ToList().
                FindAll(o => o.HitPoints <= 0);

            foreach (var f in deadEnemies)
            {
                f.gameObject.SetActive(false);
                Destroy(f.gameObject);
            }

            var deadFighters =
                LevelController.Instance.TermitesPanel.GetComponentsInChildren<FighterComponent>()
                    .ToList()
                    .FindAll(o => o.HitPoints <= 0);

            foreach (var f in deadFighters)
            {
                var termiteController = f.GetComponentInParent<TermiteController>();

                GameManager.Instance.CurrentLevel.Termites.Remove(termiteController.Termite);

                var soldierLimit = GameManager.Instance.CurrentLevel.ColonyStats.FirstOrDefault(r => r.Name == "Soldier");
                if (soldierLimit != null && soldierLimit.Value > 0)
                {
                    soldierLimit.Value--;
                }

                f.gameObject.SetActive(false);

                Destroy(f.gameObject);
            }
        }

        public void GameOver(bool victory)
        {
            IsGameOver = true;
            LevelController.Instance.StopLevel();

            var screen = DialogBoxManager.Instance.Show(typeof(EndGameController)) as EndGameController;

            if (screen)
            {
                screen.GameIsSuccessful = false;
            }
        }
    }
}