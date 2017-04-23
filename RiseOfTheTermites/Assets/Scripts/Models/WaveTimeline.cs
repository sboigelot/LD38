using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Scripts.Models
{
    public class WaveTimeline : ICloneable
    {
        public WaveTimeline()
        {
            WaveIndex = 0;
            IsEnabled = true;
        }

        [XmlElement("Wave")]
        public List<Wave> Waves { get; set; }

        [XmlAttribute]
        public float StartTime { get; set; }

        [XmlAttribute]
        public int StartPosition { get; set; }

        [XmlAttribute]
        public int HitPoint { get; set; }

        [XmlIgnore]
        public int WaveIndex { get; set; }

        [XmlIgnore]
        public bool IsEnabled { get; set; }

        public object Clone()
        {
            return new WaveTimeline
            {
                Waves = Waves,
                StartTime = StartTime,
                StartPosition = StartPosition,
                HitPoint = HitPoint,
                WaveIndex = WaveIndex,
                IsEnabled  = IsEnabled
            };
        }
    }
}