using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class EnemyTermiteController : MonoBehaviour
    {
        public float Velocity = 0.2f;

        public Vector3 StartLocation { get; set; }
        public Vector3 TargetLocation { get; set; }

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            float distance = (transform.position.x - TargetLocation.x) * (transform.position.x - TargetLocation.x) + (transform.position.y - TargetLocation.y) * (transform.position.y - TargetLocation.y);
            distance = Mathf.Sqrt(distance);

            if (distance > Velocity)
            {
                transform.position = new Vector3(transform.position.x - Velocity * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
    }
}
