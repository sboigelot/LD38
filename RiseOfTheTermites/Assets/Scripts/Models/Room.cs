using System;
using System.Xml.Serialization;

namespace Assets.Scripts.Models
{
    public class Room : ICloneable
    {
        [XmlAttribute]
        public string Name { get; set; }

        public object Clone()
        {
            return new Room
            {
                Name = Name
            };
        }
    }
}