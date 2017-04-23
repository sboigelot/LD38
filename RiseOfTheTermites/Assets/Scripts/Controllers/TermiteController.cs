using System.Collections;
using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Components;
using Assets.Scripts.Models;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Controllers
{
    public class TermiteController : MonoBehaviour
    {
        public int LayerIndexNonSelected = 1;
        public const int LayerIndexSelected = 1000;

        public Termite Termite { get; set; }

        public Vector2 PositionInRoom;

        public Vector2 DestinationInRoom;

        public float MovementSpeedInRoom = 0.2f;

        public GameObject Selector;

        #region COMBAT

        const float ENEMY_COMBAT_DISTANCE = 0.2f;
        float FORCED_VELOCITY = -0.2f; // Dirty : Should calculate according to startLocation.x - targetLocation.x
        public Vector3 StartLocation { get; set; }
        public Vector3 TargetLocation { get; set; } // CombatLocation
        public bool ItIsInCombat = false;

        #endregion

        private GameObject _enemyTermiteTarget;
        
        public void FixedUpdate()
        {
            if (Termite == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (Termite.Job == TermiteType.Soldier && ItIsInCombat)
            {
                UpdateCombat();

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

        #region COMBAT
        private void UpdateCombat()
        {
            if (_enemyTermiteTarget && _enemyTermiteTarget.GetComponent<FighterComponent>().HitPoints > 0)
            {
                //Do the fighting
                //enemyTermiteTarget.

                GetComponent<FighterComponent>().PerformCombatWith(_enemyTermiteTarget.GetComponent<FighterComponent>(), Time.deltaTime);

                if (_enemyTermiteTarget.GetComponent<FighterComponent>().HitPoints <= 0)
                    _enemyTermiteTarget = null;

                return;
            }
            else
            {
                var lst = new List<FighterComponent>();

                lst.AddRange(LevelController.Instance.EnemyLayer.GetComponentsInChildren<FighterComponent>());
                //get only next termites (those who have already passed through are ignored to simplify handling of directions
                lst = lst.FindAll(it => it.HitPoints > 0 && !it.PlayerFighter && (Mathf.Abs(it.transform.position.x - transform.position.x) < ENEMY_COMBAT_DISTANCE * 2.0f) );

                foreach (var fighter in lst)
                {
                    float squareEnemyDistance = (transform.position.x - fighter.transform.position.x) * (transform.position.x - fighter.transform.position.x) + (transform.position.y - fighter.transform.position.y) * (transform.position.y - fighter.transform.position.y);

                    if (squareEnemyDistance < ENEMY_COMBAT_DISTANCE * ENEMY_COMBAT_DISTANCE)
                    {
                        //Now they are fighting
                        _enemyTermiteTarget = fighter.gameObject;

                        return;
                    }
                }
            }

            //Advance normally
            float distance = (transform.position.x - TargetLocation.x) * (transform.position.x - TargetLocation.x) + (transform.position.y - TargetLocation.y) * (transform.position.y - TargetLocation.y);
            distance = Mathf.Sqrt(distance);

            if (distance > FORCED_VELOCITY)
            {
                transform.rotation = transform.position.x > TargetLocation.x ? new Quaternion(0f, 0f, 0f, 0f) : new Quaternion(0f, 180f, 0f, 0f);
                transform.position = new Vector3(transform.position.x - FORCED_VELOCITY * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
        #endregion

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

            transform.rotation = PositionInRoom.x > DestinationInRoom.x ? new Quaternion(0f, 0f, 0f, 0f) : new Quaternion(0f, 180f, 0f, 0f);

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
            gameObject.SetActive(true);

            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            switch (termite.Job)
            {
                case TermiteType.Queen:
                    StartCoroutine(SpriteManager.Set(spriteRenderer, SpriteManager.TermitesFolder, "Queen"));
                    LayerIndexNonSelected = 1;
                    Destroy(GetComponentInChildren<FighterComponent>());
                    break;
                case TermiteType.Worker:
                    StartCoroutine(SpriteManager.Set(spriteRenderer, SpriteManager.TermitesFolder, "Worker"));
                    LayerIndexNonSelected = 3;
                    Destroy(GetComponentInChildren<FighterComponent>());
                    break;
                case TermiteType.Soldier:
                    StartCoroutine(SpriteManager.Set(spriteRenderer, SpriteManager.TermitesFolder, "Soldier"));
                    LayerIndexNonSelected = 2;
                    break;
            }
            spriteRenderer.sortingOrder = LayerIndexNonSelected;

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
            if (Termite.CanBeMoved)
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
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sortingOrder = LayerIndexSelected;
        }
        
        public void OnMouseUp()
        {
            Termite.IsDragging = false;
            Termite.HasMouseOver = false;

            if (!Termite.CanBeMoved)
            {
                BlinkTermite(Color.red);
                return;
            }
            
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            spriteRenderer.sortingOrder = LayerIndexNonSelected;

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
