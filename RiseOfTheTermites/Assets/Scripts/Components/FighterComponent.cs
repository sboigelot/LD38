using Assets.Scripts.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class FighterComponent : MonoBehaviour
    {
        public int HitPoints;
        public int Damage;
        public float AttackSpeed;
        public bool PlayerFighter;
        public GameObject EnemyHiveObject;

        private float _combatTimer;

        public FighterComponent()
        {
            HitPoints = 0;
            Damage = 0;
            AttackSpeed = 1.0f;
            _combatTimer = 0.0f;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// This method is responsible to update the combat loop for this component and perform damange if needed
        /// This method should not remove enemy Termite because it might also have yet to attack during this Gametick
        /// </summary>
        /// <param name="enemyComponent"></param>
        /// <param name="time"></param>
        public void PerformCombatWith(FighterComponent enemyComponent, float time )
        {
            _combatTimer += time;

            if ( _combatTimer >= AttackSpeed)
            {
                _combatTimer = 0.0f;

                enemyComponent.DealDamage(Damage);
            }
        }

        /// <summary>
        /// Sets the amount of damage taken by this unit
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>true if unit is dead false otherwise</returns>
        public bool DealDamage( int amount )
        {
            Debug.Assert(amount > 0);

            HitPoints -= amount;

            HitPoints = Mathf.Max(HitPoints, 0);

            return HitPoints == 0;
        }

        /// <summary>
        /// This hits enemy structure if there is no more enemies and that we are at target location (Throne room)
        /// </summary>
        public void HitEnemyStructure()
        {
            EnemyHiveObject.GetComponent<HiveController>().TakeDamage(Damage);
        }
    }
}