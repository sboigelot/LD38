using System;
using System.Xml.Serialization;

namespace Assets.Scripts.Models
{
    public class ResourceImpact
    {
        [XmlAttribute]
        public string ResourceName { get; set; }

        [XmlAttribute]
        public int ImpactValuePerWorker { get; set; }

        [XmlIgnore]
        public ResourceImpactType ImpactType { get; set; }

        [XmlAttribute("ImpactType")]
        public string XmlValidLocation
        {
            get { return Enum.GetName(typeof(ResourceImpactType), ImpactType); }
            set { ImpactType = (ResourceImpactType)Enum.Parse(typeof(ResourceImpactType), value); }
        }
    }
}
