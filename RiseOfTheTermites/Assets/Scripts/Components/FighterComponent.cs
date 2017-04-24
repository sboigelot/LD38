﻿using Assets.Scripts.Controllers;
using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Components
{
    public class FighterComponent : MonoBehaviour
    {
        private float combatTimer;

        public float AttackSpeed;
        public int Damage;
        private int initialHitpoint;
        public int HitPoints;
        public bool PlayerFighter;

        public SpriteRenderer HitpointSpriteRenderer;

        public GameObject SpitTemplate;

        public FighterComponent()
        {
            initialHitpoint = 0;
            HitPoints = 0;
            Damage = 0;
            AttackSpeed = 1.0f;
            combatTimer = 0.0f;
        }

        /// <summary>
        ///     This method is responsible to update the combat loop for this component and perform damange if needed
        ///     This method should not remove enemy Termite because it might also have yet to attack during this Gametick
        /// </summary>
        /// <param name="enemyComponent"></param>
        /// <param name="time"></param>
        public void PerformCombatWith(FighterComponent enemyComponent, float time)
        {
            combatTimer += time;

            if (combatTimer >= AttackSpeed)
            {
                combatTimer = 0.0f;
                enemyComponent.DealDamage(Damage);
                SpitAt(enemyComponent.transform);
            }
        }

        private void SpitAt(Transform target)
        {
            var spit = Instantiate(SpitTemplate);
            spit.transform.position = transform.position;
            var spitController = spit.GetComponent<SpitController>();
            spitController.Target = target;
            spit.SetActive(true);
        }

        /// <summary>
        ///     Sets the amount of damage taken by this unit
        /// </summary>
        /// <param name="amount"></param>
        /// <returns>true if unit is dead false otherwise</returns>
        public bool DealDamage(int amount)
        {
            Debug.Assert(amount > 0);

            if (PlayerFighter)
            {
                var level = LevelController.Instance.Level;
                var venom = level.FindLevelResourceByName("Venom");
                if (venom != null && venom.Value >= 1)
                {
                    level.ApplyImpact(new ResourceImpact
                    {
                        ResourceName = "Venom",
                        ImpactValuePerWorker = -1,
                        ImpactType = ResourceImpactType.Value,
                    }, 1);
                    amount *= 2;
                }
            }

            if (initialHitpoint == 0)
            {
                initialHitpoint = HitPoints;
            }

            HitPoints -= amount;
            HitPoints = Mathf.Max(HitPoints, 0);
            
            if (HitpointSpriteRenderer != null)
            {
                var hitpointPercent = (float)HitPoints / initialHitpoint;
                HitpointSpriteRenderer.color = Color.Lerp(Color.black, Color.white, hitpointPercent);
            }

            return HitPoints == 0;
        }

        /// <summary>
        ///     This hits enemy structure if there is no more enemies and that we are at target location (Throne room)
        /// </summary>
        public void HitColonyLife(Transform queen, float time)
        {
            combatTimer += time;

            if (combatTimer >= AttackSpeed)
            {
                combatTimer = 0.0f;
                LevelController.Instance.Level.ColonyTakeDamage(Damage);

                SpitAt(queen);
            }
        }
    }
}