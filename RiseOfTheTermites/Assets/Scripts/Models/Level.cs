using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Assets.Scripts.Managers;

namespace Assets.Scripts.Models
{
    public class Level : ICloneable
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Resource")]
        public List<Resource> Resources { get; set; }

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

        public object Clone()
        {
            return new Level
            {
                Name = Name,
                Resources = Resources.Select(r => (Resource) r.Clone()).ToList(),
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

        public bool SwapRoom(Room oldRoom, Room newRoom)
        {
            if(canAfford(newRoom))
            {
                Rooms[Rooms.IndexOf(oldRoom)] = newRoom;
                if (newRoom.ResourceImpactsOnBuilt != null)
                {
                    foreach (var resourceImpact in newRoom.ResourceImpactsOnBuilt)
                    {
                        ApplyImpact(resourceImpact, 1);
                    }
                }

                if (oldRoom.ResourceImpactsOnDestroy != null)
                {
                    foreach (var resourceImpact in oldRoom.ResourceImpactsOnDestroy)
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
                    ApplyImpact(resourceImpact, room.GetWorkerCount());
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

            var resource = findLevelResourceByName(Impact.ResourceName);
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

        private Resource findLevelResourceByName(String name)
        {
            return Resources.FirstOrDefault(r => r.Name == name);            
        }

        public bool canAfford(Room room)
        {
            bool canAfford = true;
            if(room.ResourceImpactPrices != null)
            {
                foreach(var price in room.ResourceImpactPrices)
                {
                    var levelResource = findLevelResourceByName(price.ResourceName);
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