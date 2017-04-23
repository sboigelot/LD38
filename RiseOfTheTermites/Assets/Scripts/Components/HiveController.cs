using Assets.Scripts.Controllers;
using Assets.Scripts.Managers.DialogBoxes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class HiveController : MonoBehaviour
    {
        public int HitPoints;
        public int MaximumHitPoints;
        
        // Update is called once per frame
        public void Update()
        {
            ComputeMaximumHitPointsMaximum();
        }

        /// <summary>
        /// Re Calculates all hit points
        /// </summary>
        void ComputeMaximumHitPointsMaximum()
        {
            MaximumHitPoints = 1000;
        }

        /// <summary>
        /// Removes hit points from damage and notifies if structure is destroyed
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage( int damage )
        {
            HitPoints -= damage;

            if (HitPoints <= 0)
            {
                //Game is lost
                HitPoints = 0;
                GameController.Instance.GameOver(false);
            }
        }
    }
}