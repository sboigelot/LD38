using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Assets.Scripts.Models;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class LevelController : MonoBehaviourSingleton<LevelController>
    {
        private Level Level;

        public void StartLevel(Level level)
        {
            Level = level;
            RebuildChildren();
        }

        public Transform RoomsPanel;
        public GameObject RoomTemplate;
        public Vector2 RoomSpacing = new Vector2(1.2f, 0.8f);

        public void RebuildChildren()
        {
            RebuildRooms();
            //DisplayResources();
        }

        private void RebuildRooms()
        {
            RoomsPanel.ClearChildren();

            if(Level == null || !Level.Rooms.Any())
                return;

            foreach (var room in Level.Rooms)
            {
                var newRoom = Instantiate(RoomTemplate);
                newRoom.name = string.Format("Room {0}, {1}", room.GridLocationX, room.GridLocationY);
                newRoom.GetComponent<RoomController>().Room = room;
                newRoom.transform.position = new Vector3(
                    RoomSpacing.x * room.GridLocationX,
                    RoomSpacing.y * room.GridLocationY,
                    0);
                //newRoom.transform.localScale = new Vector3(RoomSpacing.y, RoomSpacing.x, 1);
                newRoom.transform.SetParent(RoomsPanel, false);
                newRoom.SetActive(true);
            }
        }

        public void SwapRoom(Room oldRoom, Room newRoom)
        {
            Level.Rooms[Level.Rooms.IndexOf(oldRoom)] = newRoom;

            if(newRoom.ResourceImpactsOnBuilt !=null)
            {
                foreach (var resourceImpact in newRoom.ResourceImpactsOnBuilt)
                {
                    Level.ApplyImpact(resourceImpact, 1);
                }
            }
        }
    }
}
