using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Controllers;
using Assets.Scripts.Managers;
using Assets.Scripts.Managers.DialogBoxes;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UpgradeRoomItem : MonoBehaviour
    {
        public Text NameText;
        public Image Image;
        public Text CostText;
        
        public void Setup(RoomController roomController, bool noChange, string roomName)
        {
            var room = PrototypeManager.FindRoomPrototype(roomName);

            NameText.text = room.Name;
            bool enable = noChange || LevelController.Instance.Level.CanAfford(room);

            var button = GetComponent<Button>();
            button.interactable = enable;
            if (enable)
            {
                button.onClick.AddListener(() =>
                {
                    if (!noChange)
                    {
                        roomController.ChangeRoomType(roomName);
                    }
                    DialogBoxManager.Instance.Close(typeof(UpgradeRoomPanel));
                });
            }

            Image.sprite = SpriteManager.Get(room.SpritePath);

            string costs = "Costs:" + Environment.NewLine;
            if (noChange || room.ResourceImpactPrices == null || !room.ResourceImpactPrices.Any())
            {
                costs += "Free" + Environment.NewLine;
            }
            else
            {
                foreach (var price in room.ResourceImpactPrices)
                {
                    costs += string.Format("{0} {1}", price.ImpactValuePerWorker, price.ResourceName) + Environment.NewLine;
                }
            }

            var resourceOnDestroy = roomController.Room.ResourceImpactsOnDestroy;
            if (!noChange && resourceOnDestroy != null && resourceOnDestroy.Any())
            {
                costs += Environment.NewLine + "Collect:" + Environment.NewLine;
                foreach (var price in resourceOnDestroy)
                {
                    costs += string.Format("{0} {1}", price.ImpactValuePerWorker, price.ResourceName) + Environment.NewLine;
                }
            }

            CostText.text = costs;
        }
    }
}
