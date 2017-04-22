using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Assets.Scripts.Models
{
    public class Room : ICloneable
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string SpritePath { get; set; }

        [XmlIgnore]
        public RoomValidLocation ValidLocation { get; set; }

        [XmlAttribute("ValidLocation")]
        public string XmlValidLocation
        {
            get { return Enum.GetName(typeof(RoomValidLocation), ValidLocation); }
            set { ValidLocation = (RoomValidLocation)Enum.Parse(typeof(RoomValidLocation), value); }
        }
        [XmlElement("PossibleUpgrade")]
        public List<string> PossibleUpgrades { get; set; }

        //[XmlElement("RoomBehaviour")]
        //public List<string> RoomBehaviours { get; set; }

        public object Clone()
        {
            return new Room
            {
                Name = Name,
                SpritePath = SpritePath,
                ValidLocation = ValidLocation,
                PossibleUpgrades = PossibleUpgrades.ToList()
            };
        }
    }
}