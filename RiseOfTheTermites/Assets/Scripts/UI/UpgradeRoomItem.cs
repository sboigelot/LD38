﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Controllers;
using Assets.Scripts.Managers;
using Assets.Scripts.Managers.DialogBoxes;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UpgradeRoomItem : MonoBehaviour
    {
        public Text NameText;
        public Image Image;
        public Text CostText;
        public TooltipProvider TooltipProvider;
        private Room room;
        private bool noChange;
        private RoomController roomController;
        private string roomName;

        public void Setup(RoomController roomController, bool noChange, string roomName)
        {
            this.room = PrototypeManager.FindRoomPrototype(roomName);
            this.noChange = noChange;
            this.roomController = roomController;
            this.roomName = roomName;

            RefreshUi();
        }

        public void FixedUpdate()
        {
            RefreshUi();
        }

        public void RefreshUi()
        { 
            TooltipProvider.content = noChange ? "Don't change anything" : room.Description;
            NameText.text = room.Name;
            
            bool diggingConflict = !noChange && roomController.Room.IsDiggingAction && LevelController.Instance.Level.IsDigging;
            bool enable = !diggingConflict && (noChange || LevelController.Instance.Level.CanAfford(room));
        
            var button = GetComponent<Button>();
            button.interactable = enable;
            if (enable)
            {
                button.onClick.AddListener(() =>
                {
                    if (!noChange)
                    {
                        roomController.StartChangeRoomType(roomName);
                    }
                    DialogBoxManager.Instance.Close(typeof(UpgradeRoomPanel));
                });
            }

            StartCoroutine(SpriteManager.Set(Image, SpriteManager.RoomFolder, room.SpritePath));

            string costs = "Costs:" + Environment.NewLine;
            if (noChange || room.ResourceImpactPrices == null || !room.ResourceImpactPrices.Any())
            {
                costs += "Free" + Environment.NewLine;
            }
            else
            {
                foreach (var price in room.ResourceImpactPrices)
                {
                    var res = LevelController.Instance.Level.FindLevelResourceByName(price.ResourceName);
                    costs += string.Format("<color={2}>{0} {1}</color>", 
                        price.ImpactValuePerWorker, 
                        price.ResourceName,
                        res != null && res.Value >= price.ImpactValuePerWorker ? "green" : "red") + Environment.NewLine;
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

            if (diggingConflict)
            {
                costs = "You can only dig one room at a time!";
            }

            if (noChange)
            {
                costs = "Do not change anything!";
            }

            CostText.text = costs;
        }
    }
}
