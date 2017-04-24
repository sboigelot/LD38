using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Components;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    [RequireComponent(typeof(FighterComponent))]
    public class EnemyTermiteController : MonoBehaviour
    {
        const float ENEMY_COMBAT_DISTANCE = 0.6f;

        private FighterComponent targetEnemy;

        private FighterComponent fighterComponent;

        public float Speed = 0.5f;
        
        public void Start()
        {
            fighterComponent = GetComponent<FighterComponent>();
            fighterComponent.OnTakeDamage = Retaliate;
        }

        private void Retaliate(FighterComponent attaker)
        {
            if (attaker == null || attaker.HitPoints <= 0)
            {
                return;
            }

            targetEnemy = attaker;
        }

        public void Update()
        {
            if (targetEnemy == null || targetEnemy.HitPoints <= 0)
            {
                targetEnemy = SearchNextValidEnemy();
            }

            if (targetEnemy != null)
            {
                var distanceToTarget = Vector3.Distance(transform.position, targetEnemy.transform.position);
                if (distanceToTarget <= ENEMY_COMBAT_DISTANCE)
                {
                    AttackTaget();
                }
                else
                {
                    MoveToTarget();
                }
                return;
            }

            //we should never reach here
            targetEnemy = null;
        }

        private void MoveToTarget()
        {
            var direction = targetEnemy.transform.position - transform.position;

            transform.rotation = transform.position.x > targetEnemy.transform.position.x ?
                new Quaternion(0f, 0f, 0f, 0f) :
                new Quaternion(0f, 180f, 0f, 0f);
            
            if (direction.magnitude >= ENEMY_COMBAT_DISTANCE)
            {
                var move = direction.normalized * Speed * Time.deltaTime;
                transform.position = new Vector3(
                    transform.position.x + move.x,
                    transform.position.y + move.y,
                    transform.position.z
                );
            }
        }

        private void AttackTaget()
        {
            var termite = targetEnemy.gameObject.GetComponent<TermiteController>();

            var job = TermiteType.Worker;
            if (termite != null && termite.Termite != null)
            {
                job = termite.Termite.Job;
            }

            if (job == TermiteType.Queen)
            {
                fighterComponent.HitColonyLife(targetEnemy.transform, Time.deltaTime);
            }
            else
            {
                fighterComponent.PerformCombatWith(targetEnemy, Time.deltaTime);
            }

            if (targetEnemy.HitPoints <= 0)
            {
                if (job != TermiteType.Queen)
                {
                    //TODO Destroy()   
                }
                targetEnemy = null;
            }
        }
        

        private FighterComponent SearchNextValidEnemy()
        {
            var figthers = LevelController.
                Instance.
                TermitesPanel.
                GetComponentsInChildren<FighterComponent>().
                ToList()
                .FindAll(it => it.HitPoints > 0 &&
                               it.PlayerFighter);

            var queens = figthers.Where(f => f.GetComponent<TermiteController>().Termite.Job == TermiteType.Queen).ToList();
            var others = figthers.Where(f => !queens.Contains(f)).ToList();
            if (others.Any())
            {
                return others.OrderBy(f => Random.Range(0f, 100f)).First();
            }

            //var soldier = figthers.Where(f => f.GetComponent<TermiteController>().Termite.Job == TermiteType.Soldier).ToList();
            //if (soldier.Any())
            //{
            //    return soldier.OrderBy(f => Random.Range(0f, 100f)).First();
            //}

            //var workers = figthers.Where(f => f.GetComponent<TermiteController>().Termite.Job == TermiteType.Worker).ToList();
            //if (workers.Any())
            //{
            //    return workers.OrderBy(f => Random.Range(0f, 100f)).First();
            //}

            return queens.OrderBy(f => Random.Range(0f, 100f)).FirstOrDefault();
        }
    }
}