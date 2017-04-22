using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Assets.Scripts.Models
{
    public class PlayerProfile : ICloneable
    {
        [XmlAttribute]
        public string Name { get; set; }
        
        public object Clone()
        {
            return new PlayerProfile
            {
                Name = Name
            };
        }
    }
}