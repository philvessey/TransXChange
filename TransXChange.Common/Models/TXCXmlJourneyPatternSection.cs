using System.Collections.Generic;
using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "JourneyPatternSection", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlJourneyPatternSection
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "JourneyPatternTimingLink", Namespace = "http://www.transxchange.org.uk/")]
		public List<TXCXmlJourneyPatternTimingLink> JourneyPatternTimingLink { get; set; }
	}
}