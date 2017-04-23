using System.Linq;
using Assets.Scripts.Components;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    [RequireComponent(typeof(FighterComponent))]
    public class EnemyTermiteController : MonoBehaviour
    {
        const float ENEMY_COMBAT_DISTANCE = 0.2f;

        private FighterComponent targetEnemy;

        private FighterComponent fighterComponent;

        public float Velocity = 0.5f;

        public Vector3 TargetLocation { get; set; }

        public void Start()
        {
            fighterComponent = GetComponent<FighterComponent>();
        }

        public void Update()
        {
            if (targetEnemy == null)
            {
                targetEnemy = SearchNextValidEnemy();
            }

            if (targetEnemy != null && targetEnemy.HitPoints > 0)
            {
                //Do the fighting
                fighterComponent.PerformCombatWith(targetEnemy, Time.deltaTime);

                if (targetEnemy.HitPoints <= 0)
                {
                    targetEnemy = null;
                }

                return;
            }

            AdvanceNormaly();
        }

        private void AdvanceNormaly()
        {
            //Advance normally
            var distance =
                (transform.position.x - TargetLocation.x) *
                (transform.position.x - TargetLocation.x) +
                (transform.position.y - TargetLocation.y) *
                (transform.position.y - TargetLocation.y);

            distance = Mathf.Sqrt(distance);

            if (distance > Velocity)
            {
                transform.position =
                    new Vector3(
                        transform.position.x - Velocity * Time.deltaTime,
                        transform.position.y,
                        transform.position.z);
            }
            else
            {
                fighterComponent.HitEnemyStructure(Time.deltaTime);
            }
        }

        private FighterComponent SearchNextValidEnemy()
        {
            var figthers = LevelController.
                Instance.
                TermitesPanel.
                GetComponentsInChildren<FighterComponent>().
                ToList();

            //get only next termites (those who have already passed through are ignored to simplify handling of directions
            figthers = figthers.
                FindAll(it => it.HitPoints > 0 &&
                              it.PlayerFighter &&
                              Mathf.Abs(it.transform.position.x - transform.position.x) < ENEMY_COMBAT_DISTANCE * 2.0f);

            foreach (var fighter in figthers)
            {
                var squareEnemyDistance =
                    (transform.position.x - fighter.transform.position.x) *
                    (transform.position.x - fighter.transform.position.x) +
                    (transform.position.y - fighter.transform.position.y) *
                    (transform.position.y - fighter.transform.position.y);

                if (squareEnemyDistance < ENEMY_COMBAT_DISTANCE * ENEMY_COMBAT_DISTANCE)
                {
                    //Now they are fighting
                    return fighter.gameObject.GetComponent<FighterComponent>();
                }
            }

            return null;
        }
    }
}