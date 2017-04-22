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
                var roomName = possibleUpgrades[index];
                var newItem = Instantiate(ItemTemplate);
                newItem.name = "Item " + roomName;
                newItem.transform.SetParent(ItemPanel, false);
                newItem.SetActive(true);
                newItem.GetComponent<UpgradeRoomItem>().Setup(roomController, index == 0, roomName);
            }
        }
    }
}