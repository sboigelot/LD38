using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class LevelController : MonoBehaviourSingleton<LevelController>
    {
        public Transform EnemyLayer;
        public GameObject EnemyTemplate;
        public Vector2 RoomSpacing = new Vector2(1.6f, 0.9f);

        public Transform RoomsPanel;
        public GameObject RoomTemplate;

        public Transform TermitesPanel;
        public GameObject TermitesTemplate;
        public Level Level { get; private set; }
        
        public void StartLevel(Level level)
        {
            Level = level;
            RebuildChildren();
        }

        public void StopLevel()
        {
            EnemyLayer.ClearChildren();
            TermitesPanel.ClearChildren();
            RoomsPanel.ClearChildren();
            Level = null;
        }

        public void RebuildChildren()
        {
            RebuildRooms();
            RebuildEnemies();
            DisplayTermites();
        }

        public void FixedUpdate()
        {
            if (GameController.Instance.IsGamePaused)
                return;

            if (Level != null && Level.IsDigging)
            {
                var workforce = Level.DiggingRoom.GetWorkforce();
                Level.DiggingTimeLeft -= (Time.fixedDeltaTime* workforce);
                if (Level.DiggingTimeLeft <= 0)
                {
                    Level.IsDigging = false;
                    Level.DiggingRoom = null;
                    Level.DiggingTimeLeft = 0f;
                }
            }
        }

        private void RebuildRooms()
        {
            RoomsPanel.ClearChildren();

            if (Level == null || !Level.Rooms.Any())
                return;

            var roomControllers = new List<RoomController>();
            foreach (var room in Level.Rooms)
            {
                var newRoom = Instantiate(RoomTemplate);
                newRoom.name = string.Format("Room {0}, {1}", room.GridLocationX, room.GridLocationY);
                var roomController = newRoom.GetComponent<RoomController>();
                roomController.Room = room;
                roomControllers.Add(roomController);
                newRoom.transform.position = new Vector3(
                    RoomSpacing.x * room.GridLocationX,
                    RoomSpacing.y * room.GridLocationY,
                    0);
                //newRoom.transform.localScale = new Vector3(RoomSpacing.y, RoomSpacing.x, 1);
                newRoom.transform.SetParent(RoomsPanel, false);
                newRoom.SetActive(true);
            }

            foreach (var roomController in roomControllers)
            {
                roomController.Room.ShowHideRoom();
            }
        }

        private void RebuildEnemies()
        {
            EnemyLayer.ClearChildren();

            if (Level == null || !Level.WaveTimelines.Any())
                return;

            var enemy_index = 0;
            foreach (var enemy in Level.WaveTimelines)
            {
                var newEnemy = new GameObject();
                newEnemy.transform.SetParent(EnemyLayer, false);
                newEnemy.name = string.Format("WaveTimelines {0}", enemy_index++);
                newEnemy.SetActive(true);
                newEnemy.transform.position = new Vector3(
                    0.0f,
                    0.0f,
                    0);
                var controller = newEnemy.AddComponent<WaveTimelineController>();
                controller.WaveTimeline = enemy;
                controller.EnemyTemplate = EnemyTemplate;
            }
        }

        private void DisplayTermites()
        {
            TermitesPanel.ClearChildren();

            if (Level == null || !Level.Termites.Any())
                return;

            foreach (var termite in Level.Termites)
            {
                InstanciateTermite(termite);
            }
        }

        public void InstanciateTermite(Termite termite)
        {
            var newTermite = Instantiate(TermitesTemplate);
            newTermite.name = string.Format("Termite {0}, {1}", termite.RoomX, termite.RoomY);
            newTermite.GetComponent<TermiteController>().SetTermiteAndRoom(termite, termite.RoomX, termite.RoomY);
            //newTermite.transform.localScale = new Vector3(RoomSpacing.y, RoomSpacing.x, 1);
            newTermite.transform.SetParent(TermitesPanel, false);
            newTermite.SetActive(true);
        }
    }
}