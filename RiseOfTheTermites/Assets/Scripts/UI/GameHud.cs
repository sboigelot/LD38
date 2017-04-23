using Assets.Scripts.Controllers;
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
        public Text SoilAmount;
        public Text SoilRate;
        public Text SoldierCount;
        public Text VenomAmount;
        public Text WorkerCount;

        private void Start()
        {
            PauseButton.onClick.AddListener(() => { });
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
            var stat = level.ColonyStats.Find(res => res.Name == statName);
            if (stat == null)
                return;

            count.text = string.Format("{0} / {1}", stat.Value, stat.MaxValue);

            if (rate != null)
            {
                var change = level.GetLastTickChange(statName);
                rate.text = string.Format("{0} {1:0.00}", change > 0 ? "+" : "", change);
            }
        }
    }
}