using System.Linq;
using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SelectLevelItem : MonoBehaviour
    {
        public Level Level;

        public Transform RoomPanel;
        public GameObject RoomTemplate;
        public Vector2 RoomSpacing = new Vector2(.16f, .09f);

        public void Setup(Level level)
        {
            Level = level;
            RebuildChildren();
        }

        public void RebuildChildren()
        {
            RebuildRooms();
        }

        private void RebuildRooms()
        {
            RoomPanel.ClearChildren();

            if (Level == null || !Level.Rooms.Any())
                return;
            
            foreach (var room in Level.Rooms)
            {
                var newRoom = Instantiate(RoomTemplate);
                newRoom.name = string.Format("Room {0}, {1}", room.GridLocationX, room.GridLocationY);
                
                newRoom.transform.position = new Vector3(
                    RoomSpacing.x * room.GridLocationX,
                    RoomSpacing.y * room.GridLocationY,
                    0);
                
                newRoom.transform.SetParent(RoomPanel, false);
                newRoom.SetActive(true);
            }
        }
    }
}