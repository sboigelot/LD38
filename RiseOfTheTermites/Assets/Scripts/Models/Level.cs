using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Assets.Scripts.Managers;
using Assets.Scripts.Components;
using Assets.Scripts.Controllers;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Level : ICloneable
    {
        const float ENEMY_DEFENSE_COMBAT_DISTANCE_THRESHOLD = 20.0f;
        const float ROUGHT_ENEMY_DEFENSE_COMBAT_DISTANCE_THRESHOLD = ENEMY_DEFENSE_COMBAT_DISTANCE_THRESHOLD * 2.0f;

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("ColonyStat")]
        public List<ColonyStat> ColonyStats { get; set; }

        [XmlElement("Room")]
        public List<Room> Rooms { get; set; }

        [XmlElement("Termite")]
        public List<Termite> Termites { get; set; }

        [XmlElement("Enemy")]
        public List<Enemy> Enemies { get; set; }

        [XmlAttribute]
        public float QueenEatAmount { get; set; }

        [XmlAttribute]
        public float SoldierEatAmount { get; set; }

        [XmlAttribute]
        public float WorkerEatAmount { get; set; }

        public bool IsDigging { get; set; }

        public Room DiggingRoom { get; set; }

        public float DiggingTimeLeft { get; set; }

        public object Clone()
        {
            return new Level
            {
                IsDigging = IsDigging,
                Name = Name,
                ColonyStats = ColonyStats.Select(r => (ColonyStat) r.Clone()).ToList(),
                Rooms = Rooms.Select(r =>
                {
                    var r2 = (Room) PrototypeManager.FindRoomPrototype(r.Name).Clone();
                    r2.GridLocationX = r.GridLocationX;
                    r2.GridLocationY = r.GridLocationY;
                    return r2;
                }).ToList(),
                Termites = Termites.Select(t=>(Termite)t.Clone()).ToList(),
                Enemies = Enemies
            };
        }

        public Level()
        {

        }

        public bool SwapRoom(Room oldRoom, Room newRoom)
        {
            if (CanAfford(newRoom))
            {
                newRoom.GridLocationX = oldRoom.GridLocationX;
                newRoom.GridLocationY = oldRoom.GridLocationY;

                Rooms[Rooms.IndexOf(oldRoom)] = newRoom;

                if (oldRoom.ResourceImpactsOnDestroy != null)
                {
                    foreach (var resourceImpact in oldRoom.ResourceImpactsOnDestroy)
                    {
                        ApplyImpact(resourceImpact, 1);
                    }
                }

                if (newRoom.ResourceImpactsOnBuilt != null)
                {
                    foreach (var resourceImpact in newRoom.ResourceImpactsOnBuilt)
                    {
                        ApplyImpact(resourceImpact, 1);
                    }
                }

                if (newRoom.ResourceImpactPrices != null)
                {
                    foreach (var resourceImpact in newRoom.ResourceImpactPrices)
                    {
                        ApplyImpact(resourceImpact, -1);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private readonly Dictionary<string, float> lastTickChanges = new Dictionary<string, float>();

        private void AddLastTickChanges(string resourceName, float change)
        {
            if (!lastTickChanges.ContainsKey(resourceName))
                lastTickChanges.Add(resourceName, change);
            else
            {
                lastTickChanges[resourceName] += change;
            }
        }

        public float GetLastTickChange(string resourceName)
        {
            if (!lastTickChanges.ContainsKey(resourceName))
                return 0f;
            return lastTickChanges[resourceName];
        }

        public void Tick()
        {
            lastTickChanges.Clear();
            
            foreach (var room in Rooms)
            {
                if(room.ResourceImpactsOnTick == null)
                    continue;
                
                foreach (var resourceImpact in room.ResourceImpactsOnTick)
                {
                    ApplyImpact(resourceImpact, room.GetWorkforce());
                }
            }

            var queenCount = Termites.Count(t => t.Job == TermiteType.Queen);
            ApplyImpact(new ResourceImpact
            {
                ImpactType = ResourceImpactType.Value,
                ResourceName = "Food",
                ImpactValuePerWorker = QueenEatAmount
            }, queenCount);

            var soldierCount = Termites.Count(t => t.Job == TermiteType.Soldier);
            ApplyImpact(new ResourceImpact
            {
                ImpactType = ResourceImpactType.Value,
                ResourceName = "Food",
                ImpactValuePerWorker = SoldierEatAmount
            }, soldierCount);

            var workerCount = Termites.Count(t => t.Job == TermiteType.Worker);
            ApplyImpact(new ResourceImpact
            {
                ImpactType = ResourceImpactType.Value,
                ResourceName = "Food",
                ImpactValuePerWorker = WorkerEatAmount
            }, workerCount);

            CheckEnemyProximity();
        }

        /// <summary>
        /// Verifies if there are enemies nearing the 
        /// </summary>
        /// <returns></returns>
        void CheckEnemyProximity()
        {
            var classedRooms = Rooms.FindAll(r => r.Name.Contains("BedRoom")
                    || r.Name.Contains("Throne")
                    || r.Name.Contains("GatheringRoom")
                    || r.Name.Contains("FarmRoom")
                    || r.Name.Contains("Barracks")
                    || r.Name.Contains("StorageRoom")
                    || r.Name.Contains("Soil generator"));

            var orderedRooms = classedRooms.OrderBy(o => o.GridLocationX).ToList();

            var referenceRoom = orderedRooms.Find(r => r.GridLocationY == 0);

            var roomAbsolutePosition = new Vector2(
                LevelController.Instance.RoomSpacing.x * referenceRoom.GridLocationX,
                LevelController.Instance.RoomSpacing.y * referenceRoom.GridLocationY
            );

            var lst = new List<FighterComponent>();

            var fightComp = LevelController.Instance.EnemyLayer.GetComponentsInChildren<FighterComponent>();

            lst.AddRange(fightComp);
            //get only next termites (those who have already passed through are ignored to simplify handling of directions
            lst = lst.FindAll(it => it.HitPoints > 0
                && !it.PlayerFighter
                && it.transform.position.y - roomAbsolutePosition.x < ROUGHT_ENEMY_DEFENSE_COMBAT_DISTANCE_THRESHOLD
                && it.transform.position.x - roomAbsolutePosition.x > 0.0f);

            foreach( var enemy in lst)
            {
                float squareEnemyDistance = (roomAbsolutePosition.x - enemy.transform.position.x) * (roomAbsolutePosition.x - enemy.transform.position.x) + (roomAbsolutePosition.y - enemy.transform.position.y) * (roomAbsolutePosition.y - enemy.transform.position.y);
                if (squareEnemyDistance < ENEMY_DEFENSE_COMBAT_DISTANCE_THRESHOLD * ENEMY_DEFENSE_COMBAT_DISTANCE_THRESHOLD)
                {
                    MoveTermitesForBattle(roomAbsolutePosition );
                    break;
                }
            }
        }

        void MoveTermitesForBattle(Vector2 combatDestinationTarget )
        {
            var lst = new List<FighterComponent>();

            lst.AddRange(LevelController.Instance.TermitesPanel.GetComponentsInChildren<FighterComponent>());
            //get only next termites (those who have already passed through are ignored to simplify handling of directions
            lst = lst.FindAll(it => it.HitPoints > 0 && it.PlayerFighter);

            foreach(var soldier in lst)
            {
                var termiteController = soldier.GetComponentInParent<TermiteController>();

                if ( !termiteController.ItIsInCombat)
                {
                    termiteController.ItIsInCombat = true;
                    termiteController.TargetLocation = new Vector3(combatDestinationTarget.x, combatDestinationTarget.y, 0.0f );
                    termiteController.StartLocation = termiteController.transform.position;
                }
            }
        }

        public void ApplyImpact(ResourceImpact Impact, int multipliyer)
        {
            if (multipliyer == 0)
                return;

            var resource = FindLevelResourceByName(Impact.ResourceName);
            if (resource == null)
                return;

            switch (Impact.ImpactType)
            {
                case ResourceImpactType.Value:
                    var change = Impact.ImpactValuePerWorker * multipliyer;
                    var desiredValue = resource.Value + change;
                    resource.Value = Math.Min(resource.MaxValue, Math.Max(resource.MinValue, desiredValue));
                    AddLastTickChanges(resource.Name, change);
                    break;
                case ResourceImpactType.MaxValue:
                    resource.MaxValue += Impact.ImpactValuePerWorker * multipliyer;
                    break;
            }
        }

        public ColonyStat FindLevelResourceByName(String name)
        {
            return ColonyStats.FirstOrDefault(r => r.Name == name);            
        }

        public bool CanAfford(string roomName)
        {
            var room = PrototypeManager.FindRoomPrototype(roomName);
            return CanAfford(room);
        }

        public bool CanAfford(Room room)
        {
            bool canAfford = true;
            if(room.ResourceImpactPrices != null)
            {
                foreach(var price in room.ResourceImpactPrices)
                {
                    var levelResource = FindLevelResourceByName(price.ResourceName);
                    if (levelResource != null)
                    {
                        if(levelResource.Value < price.ImpactValuePerWorker)
                        {
                            canAfford = false;
                        }                        
                    }                        
                }
            }
            return canAfford;
        }
    }
}