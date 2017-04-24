using System;
using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.Managers.DialogBoxes;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ObjectiveMenuController : DialogBoxBase<ObjectiveMenuController>
    {
        public Button CloseButton;

        public Text ObjectivesText;

        public Text DescriptionText;

        public void Start()
        {
            CloseButton.onClick.AddListener(() =>
            {
                CloseDialog();
                GameController.Instance.IsGamePaused = false;
            });
        }

        protected override void OnDialogOpen()
        {
            StartCoroutine(PauseNextFrame());
            RebuildUi();
        }

        private IEnumerator PauseNextFrame()
        {
            yield return new WaitForSeconds(.1f);
            GameController.Instance.IsGamePaused = true;
            yield break;
        }

        private void RebuildUi()
        {
            var objectives = 
                CreateObjectiveText("Do not die", true);

            var level = LevelController.Instance.Level;
            if (level != null)
            {
                DescriptionText.text = level.Description;

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