using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Controllers
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class RoomController : MonoBehaviour
    {
        public Vector2 GridLocation;

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
                var prototype = PrototypeManager.Instance.Rooms.SingleOrDefault(r => r.Name == roomName);
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

            Debug.Log("OnMouseUpAsButton(" + GridLocation + ") in '" + (Room != null ? Room.Name : "No room room") +"'");
            UpgradeRoomPanel.Instance.Open(this);
        }
    }
}