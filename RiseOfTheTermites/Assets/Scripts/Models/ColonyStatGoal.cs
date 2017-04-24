using System;
using System.Xml.Serialization;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.Models
{
    public class ColonyStatGoal : ICloneable
    {
        [XmlAttribute]
        public string ResourceName { get; set; }
        
        [XmlAttribute]
        public float TargetValue { get; set; }

        public bool IsAchieved()
        {
            var resource = LevelController.Instance.Level.FindLevelResourceByName(ResourceName);
            return resource.Value >= TargetValue;
        }

        public ColonyStatGoal()
        {
            
        }

        public object Clone()
        {
            return new ColonyStatGoal
            {
                ResourceName = ResourceName,
                TargetValue = TargetValue
            };
        }
    }
}