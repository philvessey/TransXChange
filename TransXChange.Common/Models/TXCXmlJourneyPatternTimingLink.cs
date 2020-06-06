using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "JourneyPatternTimingLink", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlJourneyPatternTimingLink
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "From", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlFrom From { get; set; }

		[XmlElement(ElementName = "To", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlTo To { get; set; }

		[XmlElement(ElementName = "RouteLinkRef", Namespace = "http://www.transxchange.org.uk/")]
		public string RouteLinkRef { get; set; }

		[XmlElement(ElementName = "Direction", Namespace = "http://www.transxchange.org.uk/")]
		public string Direction { get; set; }

		[XmlElement(ElementName = "RunTime", Namespace = "http://www.transxchange.org.uk/")]
		public string RunTime { get; set; }
	}
}