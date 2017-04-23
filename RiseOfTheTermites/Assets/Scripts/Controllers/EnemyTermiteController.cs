using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using Assets.Scripts.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class EnemyTermiteController : MonoBehaviour
    {
        const float ENEMY_COMBAT_DISTANCE = 0.2f;
        public float Velocity = 0.2f;

        public Vector3 StartLocation { get; set; }
        public Vector3 TargetLocation { get; set; }

        private GameObject _enemyTermiteTarget;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (_enemyTermiteTarget && _enemyTermiteTarget.GetComponent<FighterComponent>().HitPoints > 0 )
            {
                //Do the fighting
                //enemyTermiteTarget.

                GetComponent<FighterComponent>().PerformCombatWith(_enemyTermiteTarget.GetComponent<FighterComponent>(), Time.deltaTime );

                if (_enemyTermiteTarget.GetComponent<FighterComponent>().HitPoints <= 0)
                    _enemyTermiteTarget = null;

                return;
            }
            else
            {
                var lst = new List<FighterComponent>();

                lst.AddRange(LevelController.Instance.TermitesPanel.GetComponentsInChildren<FighterComponent>());
                //get only next termites (those who have already passed through are ignored to simplify handling of directions
                lst = lst.FindAll(it => it.HitPoints > 0 && it.PlayerFighter && ( Mathf.Abs(it.transform.position.x -transform.position.x) < ENEMY_COMBAT_DISTANCE * 2.0f));

                foreach ( var fighter in lst)
                {
                    float squareEnemyDistance = (transform.position.x - fighter.transform.position.x) * (transform.position.x - fighter.transform.position.x) + (transform.position.y - fighter.transform.position.y) * (transform.position.y - fighter.transform.position.y);

                    if (squareEnemyDistance < ENEMY_COMBAT_DISTANCE * ENEMY_COMBAT_DISTANCE)
                    {
                        //Now they are fighting
                        _enemyTermiteTarget = fighter.gameObject;

                        return;
                    }
                }
            }

            //Advance normally
            float distance = (transform.position.x - TargetLocation.x) * (transform.position.x - TargetLocation.x) + (transform.position.y - TargetLocation.y) * (transform.position.y - TargetLocation.y);
            distance = Mathf.Sqrt(distance);

            if (distance > Velocity)
            {
                transform.position = new Vector3(transform.position.x - Velocity * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
    }
}
