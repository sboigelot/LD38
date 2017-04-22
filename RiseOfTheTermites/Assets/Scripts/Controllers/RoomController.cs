using System.Linq;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using UnityEngine;

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

        private void Initialize()
        {
            if (Room == null)
            {
                if (!string.IsNullOrEmpty(InitializeAsRoom))
                {
                    var room = PrototypeManager.Instance.Rooms.SingleOrDefault(r => r.Name == InitializeAsRoom);
                    Room = room;
                }
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
            Debug.Log("OnMouseUpAsButton(" + GridLocation + ") in '" + (Room != null ? Room.Name : "No room room") +"'");
        }
    }
}