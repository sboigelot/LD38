using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Models;
using System.Collections.Generic;
using Assets.Scripts.Managers;

namespace Assets.Scripts.UI
{
    public class SelectLevelScreen : MonoBehaviourSingleton<SelectLevelScreen>, IBuildUi
    {
        public Button BackButton;
        public GameObject ItemTemplate;
        public Transform ItemPanel;

        // Use this for initialization
        void Start()
        {
            BackButton.onClick.AddListener(TaskOnClick);
        }

        void TaskOnClick()
        {
            MonoBehaviourSingleton<MainMenuController>.Instance.gameObject.SetActive(true);
            MonoBehaviourSingleton<SelectLevelScreen>.Instance.gameObject.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void Open()
        {
            BuildUi();
        }

        public void BuildUi()
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

                    MonoBehaviourSingleton<SelectLevelScreen>.Instance.gameObject.SetActive(false);
                    MonoBehaviourSingleton<GameHud>.Instance.gameObject.SetActive(true);
                });
            }
        }
    }
}
