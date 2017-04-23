using System;
using System.Xml.Serialization;

namespace Assets.Scripts.Models
{
    public class Wave : ICloneable
    {
        public Wave()
        {
            RatePerSecond = 0.05f;
            SoldierLevel = 1;
            Duration = int.MaxValue;
            AccumulatedSpawnTimer = 0.0f;
            AccumulatedDuration = 0.0f;
        }

        [XmlAttribute]
        public float RatePerSecond { get; set; }
        
        [XmlAttribute]
        public int SoldierLevel { get; set; }

        [XmlAttribute]
        public int Duration { get; set; }

        [XmlIgnore]
        public float AccumulatedSpawnTimer { get; set; }

        [XmlIgnore]
        public float AccumulatedDuration { get; set; }
        
        public object Clone()
        {
            return new Wave
            {
                RatePerSecond = RatePerSecond,
                SoldierLevel = SoldierLevel,
                Duration = Duration,
                AccumulatedSpawnTimer = AccumulatedSpawnTimer,
                AccumulatedDuration = AccumulatedDuration
            };
        }
    }
}