using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Managers.DialogBoxes;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class GameHud : MonoBehaviourSingleton<GameHud>
    {
        public Text DiggingCooldown;
        public Text FoodAmount;
        public Text FoodRate;
        public Text LifeAmount;
        public Text LifeRate;

        public Button PauseButton;
        public Button MenuButton;

        public Text SoilAmount;
        public Text SoilRate;
        public Text SoldierCount;
        public Text VenomAmount;
        public Text WorkerCount;
        
        public Button ShowObjectiveButton;

        public void Start()
        {
            PauseButton.onClick.AddListener(() =>
            {
                GameController.Instance.IsGamePaused = !GameController.Instance.IsGamePaused;
                PauseButton.GetComponentInChildren<Text>().text = GameController.Instance.IsGamePaused
                    ? "Unpause"
                    : "Pause";
            });

            MenuButton.onClick.AddListener(() =>
            {
                GameController.Instance.GameOver(false);
            });

            ShowObjectiveButton.onClick.AddListener(() =>
            {
                DialogBoxManager.Instance.Show(typeof(ObjectiveMenuController));
            });
        }

        public void OnGameTick()
        {
            BindResource("Population", WorkerCount, null);
            BindResource("Soldier", SoldierCount, null);
            BindResource("Soil", SoilAmount, SoilRate);
            BindResource("Food", FoodAmount, FoodRate);
            BindResource("Venom", VenomAmount, null);
            BindResource("ColonyLife", LifeAmount, LifeRate);

            var level = LevelController.Instance.Level;
            DiggingCooldown.text = string.Format("{0} sec left", (int) level.DiggingTimeLeft);
        }

        private void BindResource(string statName, Text count, Text rate)
        {
            var level = LevelController.Instance.Level;

            //Population
            var stat = level.ColonyStats.FirstOrDefault(res => res.Name == statName);
            if (stat == null)
                return;

            count.text = string.Format("{0} / {1}", (int)Mathf.FloorToInt(stat.Value), Mathf.FloorToInt(stat.MaxValue));

            if (rate != null)
            {
                var change = level.GetLastTickChange(statName);
                rate.text = string.Format("{0} {1:0.00}", change > 0 ? "+" : "", change);
            }
        }
    }
}