using System;
using System.Linq;
using System.Xml.Serialization;
using Assets.Scripts.Controllers;

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

        [XmlAttribute]
        public float QueenBirthTimer { get; set; }

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
                Hp = Hp,
                QueenBirthTimer = QueenBirthTimer
            };
        }

        public void Update(float deltaTime)
        {
            switch (Job)
            {
                case TermiteType.Queen:
                    UpdateQueenBehaviour(deltaTime);
                    break;
            }
        }

        private float lastBirthTime;

        private void UpdateQueenBehaviour(float deltaTime)
        {
            lastBirthTime -= deltaTime;

            if (lastBirthTime <= -QueenBirthTimer)
            {
                lastBirthTime = 0;
                QueenGiveBirthIfPossible();
            }
        }

        private void QueenGiveBirthIfPossible()
        {
            var level = LevelController.Instance.Level;
            if (level != null)
            {
                var soldierLimit = level.Resources.FirstOrDefault(r => r.Name == "Soldier");
                if (soldierLimit != null && soldierLimit.Value < soldierLimit.MaxValue)
                {
                    soldierLimit.Value++;

                    var newborn = new Termite
                    {
                        RoomX = RoomX,
                        RoomY = RoomY,
                        Job = TermiteType.Soldier,
                        Hp = 50,
                    };
                    level.Termites.Add(newborn);
                    LevelController.Instance.InstanciateTermite(newborn);
                    return;
                }

                var population = level.Resources.FirstOrDefault(r => r.Name == "Population");
                if (population != null && population.Value < population.MaxValue)
                {
                    population.Value++;

                    var newborn = new Termite
                    {
                        RoomX = RoomX,
                        RoomY = RoomY,
                        Job = TermiteType.Worker,
                        Hp = 50,
                    };
                    level.Termites.Add(newborn);
                    LevelController.Instance.InstanciateTermite(newborn);
                }
            }
        }
    }
}