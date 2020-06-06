using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "JourneyPattern", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlJourneyPattern
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "Direction", Namespace = "http://www.transxchange.org.uk/")]
		public string Direction { get; set; }

		[XmlElement(ElementName = "JourneyPatternSectionRefs", Namespace = "http://www.transxchange.org.uk/")]
		public string JourneyPatternSectionRefs { get; set; }
	}
}