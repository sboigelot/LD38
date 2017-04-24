using System;
using System.Xml.Serialization;

namespace Assets.Scripts.Models
{
    public class Wave : ICloneable
    {
        public Wave()
        {
            RatePerSecond = 0.05f;
            Damage = 1;
            Duration = int.MaxValue;
            AccumulatedSpawnTimer = 0.0f;
            AccumulatedDuration = 0.0f;
        }

        [XmlAttribute]
        public float RatePerSecond { get; set; }
        
        [XmlAttribute]
        public int Damage { get; set; }

        [XmlAttribute]
        public int Duration { get; set; }

        [XmlAttribute]
        public int HitPoint { get; set; }

        [XmlIgnore]
        public WaveStartLocation StartLocation { get; set; }

        [XmlAttribute("StartLocation")]
        public string XmlStartLocation
        {
            get { return Enum.GetName(typeof(WaveStartLocation), StartLocation); }
            set { StartLocation = (WaveStartLocation)Enum.Parse(typeof(WaveStartLocation), value); }
        }

        [XmlIgnore]
        public float AccumulatedSpawnTimer { get; set; }

        [XmlIgnore]
        public float AccumulatedDuration { get; set; }
        
        public object Clone()
        {
            return new Wave
            {
                RatePerSecond = RatePerSecond,
                Damage = Damage,
                Duration = Duration,
                AccumulatedSpawnTimer = AccumulatedSpawnTimer,
                AccumulatedDuration = AccumulatedDuration,
                HitPoint = HitPoint,
                StartLocation = StartLocation
            };
        }
    }
}