using System;
using System.Xml.Serialization;

namespace Assets.Scripts.Models
{
    public class Resource : ICloneable
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string SpritePath { get; set; }

        [XmlAttribute]
        public int Value { get; set; }

        [XmlAttribute]
        public int MinValue { get; set; }

        [XmlAttribute]
        public int MaxValue { get; set; }

        [XmlAttribute]
        public bool IsVisible { get; set; }

        public object Clone()
        {
            return new Resource
            {
                Name = Name,
                SpritePath = SpritePath,
                Value = Value,
                MinValue = MinValue,
                MaxValue = MaxValue
            };
        }
    }
}