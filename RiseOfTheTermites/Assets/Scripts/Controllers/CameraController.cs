﻿using System.Linq;
using Assets.Scripts.Managers.DialogBoxes;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class CameraController : MonoBehaviour
    {
        public Rect Bounds;
        private Vector3 mouseOrigin;

        void Update()
        {
            if (!Input.GetMouseButtonDown(0) &&
                !Input.GetMouseButton(0) ||
                DialogBoxManager.Instance.AnyActiveModal)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                mouseOrigin = Input.mousePosition;
            }

            if (LevelController.Instance.Level != null &&
                LevelController.Instance.Level.Termites != null &&
                LevelController.Instance.Level.Termites.Any(t => t.HasMouseOver || t.IsDragging))
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                var deltaPixel = Input.mousePosition - mouseOrigin;

                var move = deltaPixel / 100;
                transform.Translate(-move, Space.Self);
                transform.position = BoundCameraPostion();

                mouseOrigin = Input.mousePosition;
            }
        }

        private Vector3 BoundCameraPostion()
        {
            var pos = transform.position;
            var min = Bounds.min;
            var max = Bounds.max;

            pos = new Vector3(
                Mathf.Min(max.x, Mathf.Max(min.x, pos.x)),
                Mathf.Min(max.y, Mathf.Max(min.y, pos.y)),
                pos.z);

            return pos;
        }
    }
}