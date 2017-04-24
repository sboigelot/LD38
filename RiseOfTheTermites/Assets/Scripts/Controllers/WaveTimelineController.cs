using Assets.Scripts.Managers;
using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Components;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class WaveTimelineController : MonoBehaviour
    {
        public GameObject EnemyTemplate;

        public WaveTimeline WaveTimeline;
        
        public void FixedUpdate()
        {
            if (WaveTimeline == null || !WaveTimeline.IsEnabled)
            {
                return;
            }

            if (WaveTimeline.WaveIndex >= WaveTimeline.Waves.Count)
            {
                return;
            }

            var wave = WaveTimeline.Waves[WaveTimeline.WaveIndex];

            wave.AccumulatedDuration += Time.fixedDeltaTime;
            wave.AccumulatedSpawnTimer += Time.fixedDeltaTime;

            //Check if wave is over
            if (wave.AccumulatedDuration >= wave.Duration)
            {
                WaveTimeline.WaveIndex++;
            }
            else if (wave.RatePerSecond * wave.AccumulatedSpawnTimer > 1)
            {
                //spawn
                wave.AccumulatedSpawnTimer = 0.0f;
                SpawnEnemy(wave);
            }
        }

        private void SpawnEnemy(Wave wave)
        {
            var newEnemy = Instantiate(EnemyTemplate);
            newEnemy.transform.parent = this.transform;

            
            newEnemy.transform.position = wave.StartLocation == WaveStartLocation.Left ? 
                GameController.Instance.EnemySpawnLocationLeft.position : 
                GameController.Instance.EnemySpawnLocationRight.position;
            newEnemy.SetActive(true);

            newEnemy.GetComponent<FighterComponent>().HitPoints = wave.HitPoint;

            var tooltip = newEnemy.GetComponent<LevelTooltipProvider>() ?? 
                newEnemy.AddComponent<LevelTooltipProvider>();
            tooltip.content =
                "A <b>Enemy</b> termite. It will attack your soldier and then your Queen. <b><color=red>Kill it before it kills you!</color></b>";

            var spriteRenderer = newEnemy.GetComponentInChildren<SpriteRenderer>();
            StartCoroutine(SpriteManager.Set(spriteRenderer, SpriteManager.TermitesFolder, "Enemy"));
        }
    }
}