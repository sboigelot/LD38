using Assets.Scripts.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class GameHud : MonoBehaviourSingleton<GameHud>
    {
        public Button PauseButton;
        public Text WorkerCount;
        public Text SoldierCount;
        public Text SoilAmount;
        public Text FoodAmount;
        public Text FoodRate;
        public Text DiggingCooldown;

        public Color OkColor;
        public Color KOColor;

        private void Start()
        {
            PauseButton.onClick.AddListener(() => {
                
            });
        }

        public void OnGameTick()
        {
            var level = LevelController.Instance.Level;

            //Population
            var population = level.Resources.Find(res => res.Name == "Population" );

            if (population == null)
                return;

            WorkerCount.text = string.Format("{0} / {1}", population.Value, population.MaxValue );
            float percentage = Mathf.Min(1.0f, ((population.MaxValue - population.Value) / population.MaxValue) * 10.0f);
            WorkerCount.color = (OkColor * percentage) + KOColor * (1.0f - percentage);

            //Soldiers
            var soldiers = level.Resources.Find(res => res.Name == "Soldier");

            if (soldiers == null)
                return;

            SoldierCount.text = string.Format("{0} / {1}", soldiers.Value, soldiers.MaxValue);
            percentage = Mathf.Min(1.0f, ((soldiers.MaxValue - soldiers.Value) / soldiers.MaxValue) * 10.0f);
            SoldierCount.color = (OkColor * percentage) + KOColor * (1.0f - percentage);

            //Soil
            var soil = level.Resources.Find(res => res.Name == "Soil");

            if (soil == null)
                return;

            SoilAmount.text = string.Format("{0} / {1}", soil.Value, soil.MaxValue);
            percentage = Mathf.Min(1.0f, ((soil.MaxValue - soil.Value) / soil.MaxValue) * 10.0f);
            SoilAmount.color = (OkColor * percentage) + KOColor * (1.0f - percentage);

            //Food
            var food = level.Resources.Find(res => res.Name == "Food");

            if (food == null)
                return;

            FoodAmount.text = string.Format("{0} / {1}", food.Value, food.MaxValue);

            percentage = Mathf.Min(1.0f, ((food.MaxValue - food.Value) / food.MaxValue) * 10.0f);
            FoodAmount.color = (OkColor * percentage) + KOColor * (1.0f - percentage);

            FoodRate.text = string.Format("{0:0.00}", level.GetLastTickChange( food.Name ));

            DiggingCooldown.text = string.Format("{0} sec left", (int)level.DiggingTimeLeft);
        }
    }
}