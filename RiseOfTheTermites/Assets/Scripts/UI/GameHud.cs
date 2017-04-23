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
        public Text SoilRate;
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
            var population = level.ColonyStats.Find(res => res.Name == "Population" );

            if (population == null)
                return;

            WorkerCount.text = string.Format("{0} / {1}", population.Value, population.MaxValue );
            float percentage = Mathf.Min(1.0f, ((population.MaxValue - population.Value) / population.MaxValue) * 10.0f);

            //Soldiers
            var soldiers = level.ColonyStats.Find(res => res.Name == "Soldier");

            if (soldiers == null)
                return;

            SoldierCount.text = string.Format("{0} / {1}", soldiers.Value, soldiers.MaxValue);
            percentage = Mathf.Min(1.0f, ((soldiers.MaxValue - soldiers.Value) / soldiers.MaxValue) * 10.0f);

            //Soil
            var soil = level.ColonyStats.Find(res => res.Name == "Soil");

            if (soil == null)
                return;

            SoilAmount.text = string.Format("{0} / {1}", soil.Value, soil.MaxValue);
            percentage = Mathf.Min(1.0f, ((soil.MaxValue - soil.Value) / soil.MaxValue) * 10.0f);

            //Food
            var food = level.ColonyStats.Find(res => res.Name == "Food");

            if (food == null)
                return;

            FoodAmount.text = string.Format("{0} / {1}", food.Value, food.MaxValue);

            percentage = Mathf.Min(1.0f, ((food.MaxValue - food.Value) / food.MaxValue) * 10.0f);

            FoodRate.text = string.Format("+ {0:0.00}", level.GetLastTickChange( food.Name ));
            SoilRate.text = string.Format("+ {0:0.00}", level.GetLastTickChange(soil.Name));
            
            DiggingCooldown.text = string.Format("{0} sec left", (int)level.DiggingTimeLeft);
        }
    }
}