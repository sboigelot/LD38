﻿using System;
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
        public int Index { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("ColonyStat")]
        public List<ColonyStat> ColonyStats { get; set; }

        [XmlElement("Room")]
        public List<Room> Rooms { get; set; }

        [XmlElement("Termite")]
        public List<Termite> Termites { get; set; }

        [XmlElement("WaveTimeline")]
        public List<WaveTimeline> WaveTimelines { get; set; }

        [XmlAttribute]
        public float QueenEatAmount { get; set; }

        [XmlAttribute]
        public float SoldierEatAmount { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public float WorkerEatAmount { get; set; }

        [XmlAttribute("WaveIndexGoal")]
        public int WaveIndexGoal { get; set; }

        [XmlElement("ColonyStatGoal")]
        public List<ColonyStatGoal> ColonyStatGoals { get; set; }

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
                WaveTimelines = WaveTimelines.Select(w=>(WaveTimeline)w.Clone()).ToList(),
                Index = Index,
                WaveIndexGoal = WaveIndexGoal,
                ColonyStatGoals = ColonyStatGoals.ToList(),
                Description = Description
            };
        }

        public Level()
        {

        }

        public bool SwapRoom(Room oldRoom, Room newRoom)
        {
            if (!CanAfford(newRoom))
            {
                return false;
            }

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
            if (GameController.Instance.IsGamePaused)
                return;

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
        
        public void ColonyTakeDamage(int damage)
        {
            ApplyImpact(new ResourceImpact
            {
                ImpactValuePerWorker = -damage,
                ResourceName = "ColonyLife",
                ImpactType = ResourceImpactType.Value
                
            }, 1);

            var colonyLife = ColonyStats.FirstOrDefault(s => s.Name == "ColonyLife");
            if(colonyLife == null)
                return;
            
            if (colonyLife.Value <= 0)
            {
                //Game is lost
                GameController.Instance.GameOver(false);
            }
        }
    }
}