using System.Collections.Generic;
using System.Xml.Serialization;

namespace MarketAudit.Entities.Models.Response
{
    [XmlRoot("resources")]
    public class ResourcesXml
    {
        [XmlElement("string")]
        public List<ResourceXml> Resources { get; set; }
    }

    public class ResourceXml
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
