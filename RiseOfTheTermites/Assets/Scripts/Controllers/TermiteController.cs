using System.Collections;
using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class TermiteController : MonoBehaviour
    {
        public Termite Termite { get; set; }

        public Vector2 PositionInRoom;

        public Vector2 DestinationInRoom;

        public float MovementSpeedInRoom = 0.2f;

        public GameObject Selector;

        public void FixedUpdate()
        {
            if (Termite == null)
            {
                gameObject.SetActive(false);
                return;
            }

            Termite.Update(Time.fixedDeltaTime);

            if (Termite.IsDragging && Termite.CanBeMoved)
            {
                DragMoveTermite();
                return;
            }

            if (!Termite.HasMouseOver)
            {
                RandomlyMoveTermite();
            }
        }

        private void DragMoveTermite()
        {
            Termite.HasMouseOver = true;
            var worldPosition = Input.mousePosition;
            worldPosition.z = 10.0f;
            worldPosition = Camera.main.ScreenToWorldPoint(worldPosition);
            transform.position = worldPosition;
        }

        private void RandomlyMoveTermite()
        {
            var direction = DestinationInRoom - PositionInRoom;

            if (direction.magnitude >= MovementSpeedInRoom * 2)
            {
                var move = direction * MovementSpeedInRoom * Time.fixedDeltaTime;
                PositionInRoom += move;
            }
            else
            {
                var halfx = LevelController.Instance.RoomSpacing.x / 2.5f;
                var halfy = LevelController.Instance.RoomSpacing.x / 2.5f;
                DestinationInRoom = new Vector2(
                    Random.Range(-halfx, halfx),
                    Random.Range(-halfy, halfy)
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

            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            switch (termite.Job)
            {
                case TermiteType.Queen:
                    spriteRenderer.sprite = SpriteManager.Get("Queen");
                    break;
                case TermiteType.Worker:
                    spriteRenderer.sprite = SpriteManager.Get("Worker");
                    break;
                case TermiteType.Soldier:
                    spriteRenderer.sprite = SpriteManager.Get("Soldier");
                    break;
            }

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
            Selector.SetActive(true);
        }

        public void OnMouseExit()
        {
            Termite.HasMouseOver = false;
            Selector.SetActive(false);
        }

        public void OnMouseDown()
        {
            Termite.IsDragging = true;
        }
        
        public void OnMouseUp()
        {
            if (!Termite.CanBeMoved)
            {
                BlinkTermite(Color.red);
                return;
            }

            var worldPosition = Input.mousePosition;
            worldPosition.z = 10.0f;
            worldPosition = Camera.main.ScreenToWorldPoint(worldPosition);

            var rs = LevelController.Instance.RoomSpacing;

            var gridPositionX = worldPosition.x / rs.x;
           if (gridPositionX < 0)
                gridPositionX -= rs.x - 1;
            else
                gridPositionX += rs.x - 1;

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

        private void BlinkTermite(Color color)
        {
            GetComponentInChildren<SpriteRenderer>().color = color;
            StartCoroutine(RevertTermiteToWhite());
        }

        private IEnumerator RevertTermiteToWhite()
        {
            yield return new WaitForSeconds(0.5f);
            GetComponentInChildren<SpriteRenderer>().color = Color.white;
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
