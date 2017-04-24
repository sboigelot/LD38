using System.Collections;
using System.Linq;
using Assets.Scripts.Components;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class TermiteController : MonoBehaviour
    {
        public const int LayerIndexSelected = 1000;

        public Vector2 DestinationInRoom;
        public int LayerIndexNonSelected = 1;

        public float MovementSpeedInRoom = 0.1f;

        public Vector2 PositionInRoom;

        public GameObject Selector;

        public Termite Termite { get; set; }

        public void Start()
        {
            fighterComponent = GetComponent<FighterComponent>();
        }

        public void FixedUpdate()
        {
            if (Termite == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (Termite.Job == TermiteType.Soldier)
            {
                if (UpdateCombat())
                    return;

                if (MoveToBarrack())
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

        private bool MoveToBarrack()
        {
            var barrack =
                LevelController
                    .Instance
                    .Level
                    .Rooms
                    .Where(r => r.Name.Contains("Barracks")) //This is bad but not time to do better
                    .OrderBy(
                        r =>
                            Vector2.Distance(new Vector2(Termite.RoomX, Termite.RoomY),
                                new Vector2(r.GridLocationX, r.GridLocationY)))
                    .FirstOrDefault();

            if (barrack == null)
            {
                PositionInRoom = new Vector2(
                    transform.position.x - (Termite.RoomX * LevelController.Instance.RoomSpacing.x),
                    transform.position.y - (Termite.RoomY * LevelController.Instance.RoomSpacing.y)
                    );

                return false;
            }

            var soldierRoom = new Vector2(Termite.RoomX, Termite.RoomY);
            var barrackRoom = new Vector2(barrack.GridLocationX, barrack.GridLocationY);

            if (soldierRoom == barrackRoom)
            {
                return false;
            }

            if (!MoveToTarget(barrackRoom))
            {
                return true;
            }

            Termite.RoomX = barrack.GridLocationX;
            Termite.RoomY = barrack.GridLocationY;

            PositionInRoom = new Vector2(
                transform.position.x - (Termite.RoomX * LevelController.Instance.RoomSpacing.x),
                transform.position.y - (Termite.RoomY * LevelController.Instance.RoomSpacing.y)
                );

            return false;
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

            transform.rotation = PositionInRoom.x > DestinationInRoom.x
                ? new Quaternion(0f, 0f, 0f, 0f)
                : new Quaternion(0f, 180f, 0f, 0f);

            if (direction.magnitude >= MovementSpeedInRoom * 2)
            {
                var move = direction.normalized * MovementSpeedInRoom * Time.fixedDeltaTime;
                PositionInRoom += move;
            }
            else
            {
                var halfx = LevelController.Instance.RoomSpacing.x / 3f;
                var halfy = LevelController.Instance.RoomSpacing.x / 3f;
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

            var tooltip = gameObject.GetComponent<LevelTooltipProvider>() ??
                          gameObject.AddComponent<LevelTooltipProvider>();
            switch (termite.Job)
            {
                case TermiteType.Queen:
                    StartCoroutine(SpriteManager.Set(spriteRenderer, SpriteManager.TermitesFolder, "Queen"));
                    LayerIndexNonSelected = 1;
                    tooltip.content =
                        "Your <b>Queen</b> will produce new soldiers and workers if you have enougth space in your colony. Protect it!";
                    break;
                case TermiteType.Worker:
                    StartCoroutine(SpriteManager.Set(spriteRenderer, SpriteManager.TermitesFolder, "Worker"));
                    LayerIndexNonSelected = 3;
                    GetComponentInChildren<FighterComponent>().Damage = 0; // Allow the worker to be target for attack ?
                    tooltip.content =
                        "A <b>Worker</b> termite. Try to drag it around in different rooms. It may work and help you produce resources faster.";
                    break;
                case TermiteType.Soldier:
                    StartCoroutine(SpriteManager.Set(spriteRenderer, SpriteManager.TermitesFolder, "Soldier"));
                    LayerIndexNonSelected = 2;
                    tooltip.content =
                        "A <b>Soldier</b> termite. It will attack any enemy in sigth and become stronger if you produce <b>Venom</b>";
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

            Termite.RoomX = (int) gridPositionX;
            Termite.RoomY = (int) gridPositionY;

            BlinkRoom();

            PositionInRoom = new Vector3(
                worldPosition.x - rs.x * Termite.RoomX,
                worldPosition.y - rs.y * Termite.RoomY,
                0);
            DestinationInRoom =
                new Vector2(0, 0);
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

        #region COMBAT

        private FighterComponent targetEnemy;

        private FighterComponent fighterComponent;

        private const float ENEMY_COMBAT_DISTANCE = 0.6f;

        public float CombatSpeed = 0.5f;

        private bool UpdateCombat()
        {
            if (targetEnemy == null || targetEnemy.HitPoints <= 0)
            {
                targetEnemy = SearchNextValidEnemy();
            }

            if (targetEnemy != null)
            {
                var distanceToTarget = Vector3.Distance(transform.position, targetEnemy.transform.position);
                if (distanceToTarget <= ENEMY_COMBAT_DISTANCE)
                {
                    AttackTaget();
                }
                else
                {
                    MoveToTarget(targetEnemy.transform.position);
                }
            }

            return targetEnemy != null;
        }

        private FighterComponent SearchNextValidEnemy()
        {
            return LevelController.
                Instance.
                EnemyLayer.
                GetComponentsInChildren<FighterComponent>().
                ToList()
                .FindAll(it => it.HitPoints > 0 &&
                               !it.PlayerFighter)
                .OrderBy(f => Random.Range(0f, 100f)).
                FirstOrDefault();
        }

        private bool MoveToTarget(Vector3 target)
        {
            var direction = target - transform.position;

            transform.rotation = transform.position.x > target.x
                ? new Quaternion(0f, 0f, 0f, 0f)
                : new Quaternion(0f, 180f, 0f, 0f);

            if (direction.magnitude >= ENEMY_COMBAT_DISTANCE)
            {
                var move = direction.normalized * CombatSpeed * Time.deltaTime;
                transform.position = new Vector3(
                    transform.position.x + move.x,
                    transform.position.y + move.y,
                    transform.position.z
                );
                return false;
            }

            return true;
        }

        private void AttackTaget()
        {
            fighterComponent.PerformCombatWith(targetEnemy, Time.deltaTime);

            if (targetEnemy.HitPoints <= 0)
            {
                targetEnemy = null;
            }
        }

        #endregion
    }
}