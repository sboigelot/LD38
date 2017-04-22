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
    }
}
