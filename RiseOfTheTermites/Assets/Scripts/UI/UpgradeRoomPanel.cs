using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Managers.DialogBoxes;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UpgradeRoomPanel : ContextualDialogBoxBase<UpgradeRoomPanel, RoomController>
    {
        public Transform ItemPanel;

        public GameObject ItemTemplate;
        private RoomController roomController;
        
        protected override void OnScreenOpen(RoomController context)
        {
            if (context.Room == null)
                return;

            roomController = context;
            gameObject.SetActive(true);
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
                var index1 = index;
                newItem.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    if (index1 != 0)
                        roomController.ChangeRoomType(upgrade);
                    CloseDialog();
                });
            }
        }
    }
}