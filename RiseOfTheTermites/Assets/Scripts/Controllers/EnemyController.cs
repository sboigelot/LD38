using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class EnemyController : MonoBehaviour
    {
        public GameObject EnemyTemplate;

        public Enemy Enemy;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Enemy != null && Enemy.IsEnabled)
                Spawn();
        }

        private void Spawn()
        {
            if (Enemy.WaveIndex >= Enemy.Waves.Count)
                return;

            var wave = Enemy.Waves[Enemy.WaveIndex];

            wave.AccumulatedDuration += Time.deltaTime;
            wave.AccumulatedSpawnTimer += Time.deltaTime;

            //Check if wave is over
            if (wave.AccumulatedDuration >= wave.Duration)
            {
                Enemy.WaveIndex++;
            }
            else if (wave.RatePerSecond * wave.AccumulatedSpawnTimer > 1)
            {
                //spawn
                wave.AccumulatedSpawnTimer = 0.0f;

                var enemy = Instantiate(EnemyTemplate);

                enemy.transform.parent = this.transform;

                var spriteRenderer = enemy.GetComponentInChildren<SpriteRenderer>();
                StartCoroutine(SpriteManager.Set(spriteRenderer, SpriteManager.TermitesFolder, "Soldier"));

                enemy.SetActive(true);

                var enemyTermiteController = enemy.GetComponentInChildren<EnemyTermiteController>();

                var throneRoom = GameManager.Instance.CurrentLevel.Rooms.Find(room => room.Name == "Throne Room" );
                enemyTermiteController.StartLocation = new Vector3(6.0f * LevelController.Instance.RoomSpacing.x, 0.0f, 0.0f);

                var transform = enemy.GetComponentInChildren<Transform>();

                transform.position = enemyTermiteController.StartLocation;
            }
        }
    }
}