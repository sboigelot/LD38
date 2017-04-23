using System.Collections;
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

            startSwapTime = Time.time;
            SwapTarget = roomName;
            var otherRoom = PrototypeManager.FindRoomPrototype(roomName);
            completedSwapTime = startSwapTime + Room.DestructionTime + otherRoom.ConstructionTime;
        }

        public void Update()
        {
            bool isSwapping = !string.IsNullOrEmpty(SwapTarget);

            if(TimerBackground!= null)
                TimerBackground.gameObject.SetActive(isSwapping);

            if (Timer != null)
                Timer.gameObject.SetActive(isSwapping);

            if (!isSwapping)
            {
                return;
            }

            if (Time.time < completedSwapTime)
            {
                var elapsed = Time.time - startSwapTime;
                var duration = completedSwapTime - startSwapTime;
                float percentProgress = Mathf.Clamp(elapsed / duration, 0f, 1f);
                Debug.Log(percentProgress);
                Timer.size = new Vector2(TimerBackground.size.x * percentProgress, TimerBackground.size.y);
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

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = SpriteManager.Get(Room.SpritePath);
            }
        }

        public void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Initialize();
        }

        public void OnMouseEnter()
        {
            Selector.SetActive(true);
        }

        public void OnMouseExit()
        {
            Selector.SetActive(false);
        }

        public void OnMouseUpAsButton()
        {
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