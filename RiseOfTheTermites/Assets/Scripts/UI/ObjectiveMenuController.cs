using System;
using Assets.Scripts.Controllers;
using Assets.Scripts.Managers.DialogBoxes;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ObjectiveMenuController : DialogBoxBase<ObjectiveMenuController>
    {
        public Button CloseButton;

        public Text ObjectivesText;

        public void Start()
        {
            CloseButton.onClick.AddListener(CloseDialog);
        }

        protected override void OnDialogOpen()
        {
            RebuildUi();
        }

        private void RebuildUi()
        {
            var objectives = 
                CreateObjectiveText("Do not die", true);

            var level = LevelController.Instance.Level;
            if (level != null)
            {
                if (level.WaveIndexGoal != 0)
                {
                    objectives += CreateObjectiveText("Surive until enemy wave " + level.WaveIndexGoal,
                        GameController.Instance.IsWaveGoalAchieved());
                }

                if(level.ColonyStatGoals!=null)
                foreach (var colonyStatGoal in level.ColonyStatGoals)
                {
                    objectives +=
                        CreateObjectiveText("Collect " + colonyStatGoal.TargetValue + " " + colonyStatGoal.ResourceName,
                            colonyStatGoal.IsAchieved());
                }
            }

            ObjectivesText.text = objectives;
        }

        private string CreateObjectiveText(string newObjectif, bool isSuceeded)
        {
            return string.Format("<color={1}>{0}</color>", newObjectif, isSuceeded ? "green" : "white") + Environment.NewLine;
        }
    }
}