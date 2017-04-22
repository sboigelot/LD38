using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class TermiteController : MonoBehaviour
    {
        public Termite Termite { get; set; }

        public Vector2 PositionInRoom = new Vector2();

        public Vector2 DestinationInRoom = new Vector2();

        public float movementSpeedInRoom = 0.5f;

        //public Vector2 RoomDestination = new Vector2();

        public void FixedUpdate()
        {
            if (Termite == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (Termite.IsDragging)
            {
                Termite.HasMouseOver = true;
                var worldPosition = Input.mousePosition;
                worldPosition.z = 10.0f;
                worldPosition = Camera.main.ScreenToWorldPoint(worldPosition);
                transform.position = worldPosition;
                return;
            }

            if (Termite.HasMouseOver)
            {
                return;
            }

            var direction = DestinationInRoom - PositionInRoom;

            if (direction.magnitude >= movementSpeedInRoom * 2)
            {
                var move = direction * movementSpeedInRoom * Time.fixedDeltaTime;
                PositionInRoom += move;
            }
            else
            {
                var halfx = LevelController.Instance.RoomSpacing.x / 2.5f;
                var halfy = LevelController.Instance.RoomSpacing.x / 2.5f;
                DestinationInRoom = new Vector2(
                    UnityEngine.Random.Range(-halfx, halfx),
                    UnityEngine.Random.Range(-halfy, halfy)
                    );
            }

            gameObject.SetActive(true);
            transform.position = new Vector3(
                Termite.RoomX * LevelController.Instance.RoomSpacing.x + PositionInRoom.x,
                Termite.RoomY * LevelController.Instance.RoomSpacing.y + PositionInRoom.y,
                transform.position.z
            );
        }

        public void SetTermiteAndRoom(Termite termite, int termiteRoomX, int termiteRoomY)
        {
            Termite = termite;
            termite.RoomX = termiteRoomX;
            termite.RoomY = termiteRoomY;
            transform.position = new Vector3(
                LevelController.Instance.RoomSpacing.x * termite.RoomX,
                LevelController.Instance.RoomSpacing.y * termite.RoomY,
                0);
        }

        public void OnMouseEnter()
        {
            Termite.HasMouseOver = true;
        }

        public void OnMouseExit()
        {
            Termite.HasMouseOver = false;
        }

        public void OnMouseDown()
        {
            Termite.IsDragging = true;
        }
        
        public void OnMouseUp()
        {
            var worldPosition = Input.mousePosition;
            worldPosition.z = 10.0f;
            worldPosition = Camera.main.ScreenToWorldPoint(worldPosition);

            var rs = LevelController.Instance.RoomSpacing;

            var gridPositionX = worldPosition.x / rs.x;
            if (gridPositionX < 0)
                gridPositionX -= rs.x / 2;
            else
                gridPositionX += rs.x / 2;

            var gridPositionY = worldPosition.y / rs.y;
            if (gridPositionY < 0)
                gridPositionY -= rs.y / 2;
            else
                gridPositionY += rs.y / 2;
            
            Termite.RoomX = (int)gridPositionX;
            Termite.RoomY = (int)gridPositionY;

            BlinkRoom();

            PositionInRoom = new Vector3(
                    worldPosition.x - rs.x * Termite.RoomX,
                    worldPosition.y - rs.y * Termite.RoomY,
                0);
            DestinationInRoom = 
                new Vector2(0,0);
            gameObject.name = string.Format("Termite {0}, {1}", gridPositionX, gridPositionY);

            transform.position = worldPosition;
            Termite.IsDragging = false;
        }

        private void BlinkRoom()
        {
            var room =
                LevelController.Instance.Level.Rooms.FirstOrDefault(
                    r => r.GridLocationX == Termite.RoomX && r.GridLocationY == Termite.RoomY);

            if (room != null)
            {
                var roomControllers = FindObjectsOfType<RoomController>().FirstOrDefault(r => r.Room == room);
                if (roomControllers != null)
                {
                    roomControllers.Blink();
                }
            }
        }
    }
}
