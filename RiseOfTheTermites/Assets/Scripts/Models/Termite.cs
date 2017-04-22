using System;
using System.Xml.Serialization;

namespace Assets.Scripts.Models
{
    public class Termite : ICloneable
    {
        public Termite()
        {
            Hp = 100;
        }

        [XmlAttribute]
        public int RoomX { get; set; }

        [XmlAttribute]
        public int RoomY { get; set; }

        [XmlIgnore]
        public TermiteType Job { get; set; }

        [XmlAttribute("Job")]
        public string XmlJob
        {
            get { return Enum.GetName(typeof(TermiteType), Job); }
            set { Job = (TermiteType) Enum.Parse(typeof(TermiteType), value); }
        }

        [XmlIgnore]
        public int Hp { get; set; }

        public bool CanBeMoved
        {
            get { return Job == TermiteType.Worker; }
        }

        public bool HasMouseOver;

        public bool IsDragging;

        public object Clone()
        {
            return new Termite
            {
                RoomX = RoomX,
                RoomY = RoomY,
                Job = Job,
                Hp = Hp
            };
        }
    }
}