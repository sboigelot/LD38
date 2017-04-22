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
        public Level Level { get; private set; }

        public void StartLevel(Level level)
        {
            Level = level;
            RebuildChildren();
        }

        public Transform RoomsPanel;
        public GameObject RoomTemplate;
        public Vector2 RoomSpacing = new Vector2(1.6f, 0.9f);

        public Transform TermitesPanel;
        public GameObject TermitesTemplate;

        public void RebuildChildren()
        {
            RebuildRooms();
            DisplayTermites();
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


        private void DisplayTermites()
        {
            TermitesPanel.ClearChildren();

            if (Level == null || !Level.Termites.Any())
                return;

            foreach (var termite in Level.Termites)
            {
                var newTermite = Instantiate(TermitesTemplate);
                newTermite.name = string.Format("Termite {0}, {1}", termite.RoomX, termite.RoomY);
                newTermite.GetComponent<TermiteController>().SetTermiteAndRoom(termite, termite.RoomX, termite.RoomY);
                //newTermite.transform.localScale = new Vector3(RoomSpacing.y, RoomSpacing.x, 1);
                newTermite.transform.SetParent(RoomsPanel, false);
                newTermite.SetActive(true);
            }
        }
    }
}
