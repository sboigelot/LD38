using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Assets.Scripts.Controllers;

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

        [XmlAttribute]
        public bool CanAttack { get; set; }

        [XmlAttribute]
        public bool IsDiggingAction { get; set; }

        [XmlAttribute("ValidLocation")]
        public string XmlValidLocation
        {
            get { return Enum.GetName(typeof(RoomValidLocation), ValidLocation); }
            set { ValidLocation = (RoomValidLocation)Enum.Parse(typeof(RoomValidLocation), value); }
        }

        [XmlElement("PossibleUpgrade")]
        public List<string> PossibleUpgrades { get; set; }

        [XmlElement("ResourceImpactOnTick")]
        public List<ResourceImpact> ResourceImpactsOnTick { get; set; }

        [XmlElement("ResourceImpactOnDestroy")]
        public List<ResourceImpact> ResourceImpactsOnDestroy { get; set; }

        [XmlElement("ResourceImpactOnBuilt")]
        public List<ResourceImpact> ResourceImpactsOnBuilt { get; set; }

        [XmlElement("ResourceImpactPrice")]
        public List<ResourceImpact> ResourceImpactPrices { get; set; }

        [XmlAttribute]
        public int BuildingTime { get; set; }

        [XmlAttribute]
        public int MaxWorker { get; set; }

        [XmlAttribute("X")]
        public int GridLocationX { get; set; }

        [XmlAttribute("Y")]
        public int GridLocationY { get; set; }

        [XmlAttribute("ConstructionTime")]
        public int ConstructionTime { get; set; }

        [XmlAttribute("DestructionTime")]
        public int DestructionTime { get; set; }

        [XmlAttribute]
        public bool HideIfNoNeighboard { get; set; }

        public Room()
        {
            
        }

        public object Clone()
        {
            return new Room
            {
                Name = Name,
                SpritePath = SpritePath,
                ValidLocation = ValidLocation,
                PossibleUpgrades = PossibleUpgrades.ToList(),
                ResourceImpactsOnTick = ResourceImpactsOnTick.ToList(),
                ResourceImpactsOnBuilt = ResourceImpactsOnBuilt.ToList(),
                ResourceImpactsOnDestroy = ResourceImpactsOnDestroy.ToList(),
                ResourceImpactPrices = ResourceImpactPrices.ToList(),
                BuildingTime = BuildingTime,
                MaxWorker = MaxWorker,
                CanAttack = CanAttack,
                ConstructionTime = ConstructionTime,
                DestructionTime = DestructionTime,
                IsDiggingAction = IsDiggingAction,
                HideIfNoNeighboard = HideIfNoNeighboard
            };
        }

        [XmlIgnore]
        private int lastComputedWorkforce;

        public int GetWorkforce()
        {
            lastComputedWorkforce = LevelController.Instance.Level.Termites.Count(
                t => t.RoomX == GridLocationX &&
                     t.RoomY == GridLocationY &&
                     t.Job == TermiteType.Worker);

            return 1 + Math.Min(lastComputedWorkforce, MaxWorker);
        }
    }
}