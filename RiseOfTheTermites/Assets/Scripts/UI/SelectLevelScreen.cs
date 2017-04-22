using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Models;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using Assets.Scripts.Managers.DialogBoxes;

namespace Assets.Scripts.UI
{
    public class SelectLevelScreen : DialogBoxBase<SelectLevelScreen>
    {
        public Button BackButton;
        public GameObject ItemTemplate;
        public Transform ItemPanel;
        
        public void Start()
        {
            BackButton.onClick.AddListener(OnBackButtonClick);
        }

        public void OnBackButtonClick()
        {
            DialogBoxManager.Instance.Show(typeof(SelectLevelScreen));
        }
        
        protected override void OnScreenOpen()
        {
            RebuildChildren();
        }

        private void RebuildChildren()
        {
            List<Level> lvls = PrototypeManager.Instance.Levels;

            ItemPanel.ClearChildren();

            for (var index = 0; index < lvls.Count; index++)
            {
                var level = lvls[index];

                var newItem = Instantiate(ItemTemplate);
                
                //newItem.name = "Item " + upgrade;
                newItem.transform.SetParent(ItemPanel, false);
                newItem.SetActive(true);

                //newItem.GetComponentInChildren<Text>().text = (index == 0 ? "Stay a " : "Upgrade to ") + upgrade;
                var index1 = index;
                var bt = newItem.GetComponentInChildren<Button>();

                var btn_label = bt.GetComponentInChildren<Text>();
                btn_label.text = string.Format("Level {0}", index);
                bt.onClick.AddListener(() =>
                {
                    Debug.Log("On level selected");
                    GameController.Instance.NewGame(index1);
                    DialogBoxManager.Instance.Show(typeof(GameHud));
                });
            }
        }
    }
}
