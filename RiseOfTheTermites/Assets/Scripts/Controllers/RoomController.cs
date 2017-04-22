using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class RoomController : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        private Room room;

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

        public void ChangeRoomType(string roomName)
        {
            if (!string.IsNullOrEmpty(roomName))
            {
                var prototype = PrototypeManager.FindRoomPrototype(roomName);
                if (prototype != null)
                {
                    Room = (Room) prototype.Clone();
                    Initialize();
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

        public void OnMouseUpAsButton()
        {
            if(UpgradeRoomPanel.Instance.gameObject.activeSelf)
                return;

            Debug.Log(Room == null
                ? "OnMouseUpAsButton(?) in 'No room'"
                : string.Format("OnMouseUpAsButton({0}, {1}) in '{2}'", room.GridLocationX, room.GridLocationY,
                    room.Name));
            UpgradeRoomPanel.Instance.Open(this);
        }
    }
}