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
                }).ToList()
            };
        }

        public void Tick()
        {
            
        }
    }
}