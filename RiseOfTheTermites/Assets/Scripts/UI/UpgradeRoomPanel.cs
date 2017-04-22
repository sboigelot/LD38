using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UpgradeRoomPanel : MonoBehaviourSingleton<UpgradeRoomPanel>, IBuildUi
    {
        private RoomController roomController;

        public GameObject ItemTemplate;
        public Transform ItemPanel;

        public void Open(RoomController controller)
        {
            if(controller.Room == null)
                return;

            roomController = controller;
            gameObject.SetActive(true);
            BuildUi();
        }

        public void BuildUi()
        {
            RebuildChildren();
        }

        private void RebuildChildren()
        {
            ItemPanel.ClearChildren();

            if (roomController == null)
            {
                return;
            }

            var possibleUpgrades = roomController.Room.PossibleUpgrades.ToList();
            possibleUpgrades.Insert(0, roomController.Room.Name);

            for (var index = 0; index < possibleUpgrades.Count; index++)
            {
                var upgrade = possibleUpgrades[index];
                var newItem = Instantiate(ItemTemplate);
                newItem.name = "Item " + upgrade;
                newItem.transform.SetParent(ItemPanel, false);
                newItem.SetActive(true);

                newItem.GetComponentInChildren<Text>().text = (index == 0 ? "Stay a " : "Upgrade to ") + upgrade;
                newItem.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    roomController.ChangeRoomType(upgrade);
                    this.gameObject.SetActive(false);
                });
            }
        }
    }
}
