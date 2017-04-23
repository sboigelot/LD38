using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class WaveTimelineController : MonoBehaviour
    {
        public GameObject EnemyTemplate;

        public WaveTimeline WaveTimeline;
        
        public void Update()
        {
            if (WaveTimeline != null && WaveTimeline.IsEnabled)
                Spawn();
        }

        private void Spawn()
        {
            if (WaveTimeline.WaveIndex >= WaveTimeline.Waves.Count)
                return;

            var wave = WaveTimeline.Waves[WaveTimeline.WaveIndex];

            wave.AccumulatedDuration += Time.deltaTime;
            wave.AccumulatedSpawnTimer += Time.deltaTime;

            //Check if wave is over
            if (wave.AccumulatedDuration >= wave.Duration)
            {
                WaveTimeline.WaveIndex++;
            }
            else if (wave.RatePerSecond * wave.AccumulatedSpawnTimer > 1)
            {
                //spawn
                wave.AccumulatedSpawnTimer = 0.0f;
                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            var newEnemy = Instantiate(EnemyTemplate);
            newEnemy.transform.parent = this.transform;
            var startLocation = new Vector3(6.0f * LevelController.Instance.RoomSpacing.x, 0.0f, 0.0f); //TODO this is a static location, not related to the position of the enemy room
            newEnemy.transform.position = startLocation;
            newEnemy.SetActive(true);

            var spriteRenderer = newEnemy.GetComponentInChildren<SpriteRenderer>();
            StartCoroutine(SpriteManager.Set(spriteRenderer, SpriteManager.TermitesFolder, "Enemy"));
        }
    }
}