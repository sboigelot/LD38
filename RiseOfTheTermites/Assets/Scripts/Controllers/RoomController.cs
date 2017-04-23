using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Managers.DialogBoxes;
using Assets.Scripts.Models;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class RoomController : MonoBehaviour
    {
        public Color BlinkColor = Color.magenta;

        private SpriteRenderer spriteRenderer;

        private Room room;

        public SpriteRenderer TimerBackground;

        public SpriteRenderer Timer;

        public GameObject Selector;

        public Room Room
        {
            get { return room; }
            set
            {
                if (room == value)
                    return;

                room = value;
                Initialize();
            }
        }
        
        public string InitializeAsRoom;

        public float startSwapTime;

        public float completedSwapTime;

        public string SwapTarget;

        public void StartChangeRoomType(string roomName)
        {
            if (string.IsNullOrEmpty(roomName))
                return;

            if (Room.IsDiggingAction)
            {
                var level = LevelController.Instance.Level;
                if (level.IsDigging)
                    return;
                
                // start digging
                level.IsDigging = true;
                level.DiggingRoom = Room;
                level.DiggingTimeLeft = (float)Room.DestructionTime;
            }

            startSwapTime = Time.time;
            SwapTarget = roomName;
            var otherRoom = PrototypeManager.FindRoomPrototype(roomName);
            completedSwapTime = startSwapTime + Room.DestructionTime + otherRoom.ConstructionTime;
        }

        public void Update()
        {
            bool isSwapping = !string.IsNullOrEmpty(SwapTarget);
            
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = Room != null && Room.IsVisible;
            }

            if (TimerBackground!= null)
                TimerBackground.gameObject.SetActive(isSwapping);

            if (Timer != null)
                Timer.gameObject.SetActive(isSwapping);

            if (!isSwapping)
            {
                return;
            }

            if (Time.time < completedSwapTime)
            {
                var workerCount = Room.GetWorkforce() - 1;
                startSwapTime -= workerCount * Time.deltaTime;

                var elapsed = Time.time - startSwapTime;
                var duration = completedSwapTime - startSwapTime;
                float percentProgress = Mathf.Clamp(elapsed / duration, 0f, 1f);
                var sizeX = TimerBackground.size.x * percentProgress;
                Timer.size = new Vector2(sizeX, TimerBackground.size.y);
            }
            else
            {
                ChangeRoomType(SwapTarget);
                SwapTarget = null;
            }
        }

        private void ChangeRoomType(string roomName)
        {
            if (!string.IsNullOrEmpty(roomName) && LevelController.Instance != null && LevelController.Instance.Level != null)
            {
                var prototype = PrototypeManager.FindRoomPrototype(roomName);
                if (prototype != null)
                {
                    var room = (Room) prototype.Clone();
                    bool swapped = LevelController.Instance.Level.SwapRoom(Room, room);
                    if(swapped)
                    {
                        Room = room;
                        Room.ShowHideRoomNeighboard();
                        Initialize();
                    }
                }
            }
        }

        private void Initialize()
        {
            if (Room == null)
            {
                ChangeRoomType(InitializeAsRoom);
                return;
            }

            if (Room == null)
                return;

            var tooltip = gameObject.GetComponent<LevelTooltipProvider>() ?? gameObject.AddComponent<LevelTooltipProvider>();
            tooltip.content = Room.Description;
            tooltip.CheckIfTooltipShouldBeDisplayed = () => Room.IsVisible;

            SpawnTermitesOnBuilt();

            if (spriteRenderer != null)
            {
                StartCoroutine(SpriteManager.Set(spriteRenderer, SpriteManager.RoomFolder, Room.SpritePath));
            }
            
            Room.ShowHideRoom();
        }

        private void SpawnTermitesOnBuilt()
        {
            if (Room.SpawnTermitesOnBuild != null)
            {
                foreach (var termiteTemplate in Room.SpawnTermitesOnBuild.ToList())
                {
                    Room.SpawnTermitesOnBuild.Remove(termiteTemplate);
                    var population = LevelController.Instance.Level.ColonyStats.FirstOrDefault(r => r.Name == "Population");
                    if (population != null)
                    {
                        population.Value++;

                        var newborn = new Termite
                        {
                            RoomX =Room.GridLocationX,
                            RoomY = Room.GridLocationY,
                            Job = termiteTemplate.Job,
                            Hp = termiteTemplate.Hp,
                            QueenBirthTimer = termiteTemplate.QueenBirthTimer
                        };
                        LevelController.Instance.Level.Termites.Add(newborn);
                        LevelController.Instance.InstanciateTermite(newborn);
                    }
                }
            }
        }

        public void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Initialize();
        }

        public void OnMouseEnter()
        {
            if(Room == null || !Room.IsVisible)
                return;

            if(Selector != null)
                Selector.SetActive(true);
        }

        public void OnMouseExit()
        {
            if (Selector != null)
                Selector.SetActive(false);
        }

        public void OnMouseUpAsButton()
        {
            if (Room == null || !Room.IsVisible)
                return;

            if (DialogBoxManager.Instance.AnyActiveModal)
                return;

            Debug.Log(Room == null
                ? "OnMouseUpAsButton(?) in 'No room'"
                : string.Format("OnMouseUpAsButton({0}, {1}) in '{2}'",
                    room.GridLocationX,
                    room.GridLocationY,
                    room.Name));

            DialogBoxManager.Instance.Show(typeof(UpgradeRoomPanel), this);
        }

        public void Blink()
        {
            spriteRenderer.color = BlinkColor;
            StartCoroutine(RevertToWhite());
        }

        private IEnumerator RevertToWhite()
        {
            yield return new WaitForSeconds(0.5f);
            spriteRenderer.color = Color.white;
        }
    }
}