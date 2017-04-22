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
                Termites = Termites.Select(t=>(Termite)t.Clone()).ToList()
            };
        }

        public void SwapRoom(Room oldRoom, Room newRoom)
        {
            Rooms[Rooms.IndexOf(oldRoom)] = newRoom;

            if (newRoom.ResourceImpactsOnBuilt != null)
            {
                foreach (var resourceImpact in newRoom.ResourceImpactsOnBuilt)
                {
                    ApplyImpact(resourceImpact, 1);
                }
            }
        }

        public void Tick()
        {
            foreach (var room in Rooms)
            {
                if(room.ResourceImpactsOnTick == null)
                    continue;
                
                foreach (var resourceImpact in room.ResourceImpactsOnTick)
                {
                    ApplyImpact(resourceImpact, room.GetWorkerCount());
                }
            }
        }

        public void ApplyImpact(ResourceImpact Impact, int multipliyer)
        {
            if (multipliyer == 0)
                return;

            var resource = Resources.FirstOrDefault(r => r.Name == Impact.ResourceName);
            if (resource == null)
                return;

            switch (Impact.ImpactType)
            {
                case ResourceImpactType.Value:
                    var desiredValue = resource.Value + Impact.ImpactValuePerWorker * multipliyer;
                    resource.Value = Math.Min(resource.MaxValue, Math.Max(resource.MinValue, desiredValue));
                    break;
                case ResourceImpactType.MaxValue:
                    resource.MaxValue += Impact.ImpactValuePerWorker * multipliyer;
                    break;
            }
        }
    }
}