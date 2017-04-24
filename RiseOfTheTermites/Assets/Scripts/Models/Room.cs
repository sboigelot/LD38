using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Assets.Scripts.Controllers;
using UnityEngine;

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

        [XmlElement("SpawnTermiteOnBuild")]
        public List<Termite> SpawnTermitesOnBuild { get; set; }

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

        [XmlAttribute]
        public bool IsPassable { get; set; }

        [XmlAttribute("Description")]
        public string Description { get; set; }

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
                HideIfNoNeighboard = HideIfNoNeighboard,
                IsPassable = IsPassable,
                SpawnTermitesOnBuild = SpawnTermitesOnBuild.ToList(),
                Description = Description
            };
        }

        public int LastComputedWorkforce;
        public int GetWorkforce()
        {
            var level = LevelController.Instance.Level;
            if (level == null || level.Termites == null)
            {
                return 0;
            }

            LastComputedWorkforce = LevelController.Instance.Level.Termites.Count(
                t => t.RoomX == GridLocationX &&
                     t.RoomY == GridLocationY &&
                     t.Job == TermiteType.Worker);

            return 1 + Math.Min(LastComputedWorkforce, MaxWorker);
        }


        [XmlIgnore]
        public bool IsVisible;

        public void ShowHideRoom()
        {
            IsVisible = !HideIfNoNeighboard || HasBuiltNeighboard();
        }

        public void ShowHideRoomNeighboard()
        {
            var searchLocation = GetNeighboardLocation();

            var neighboard = LevelController.Instance.Level.Rooms.FindAll(r => r.HideIfNoNeighboard &&
                                                                 searchLocation.Contains(new Vector2(r.GridLocationX,
                                                                     r.GridLocationY)));

            foreach (var n in neighboard)
            {
                n.ShowHideRoom();
            }
        }

        private bool HasBuiltNeighboard()
        {
            var searchLocation = GetNeighboardLocation();

            return LevelController.Instance.Level.Rooms.Any(r => r.IsPassable &&
                                                                 searchLocation.Contains(new Vector2(r.GridLocationX,
                                                                     r.GridLocationY)));
        }

        private List<Vector2> GetNeighboardLocation()
        {
            var gridLocation = new Vector2(GridLocationX, GridLocationY);

            var left = gridLocation - new Vector2(-1, 0);
            var right = gridLocation - new Vector2(1, 0);
            var down = gridLocation - new Vector2(0, 1);
            var up = gridLocation - new Vector2(0, -1);

            return new List<Vector2>
            {
                left,
                right,
                down,
                up
            };
        }

    }
}