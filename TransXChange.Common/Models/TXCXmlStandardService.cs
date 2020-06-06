using System.Collections.Generic;
using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "StandardService", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlStandardService
	{
		[XmlElement(ElementName = "Origin", Namespace = "http://www.transxchange.org.uk/")]
		public string Origin { get; set; }

		[XmlElement(ElementName = "Destination", Namespace = "http://www.transxchange.org.uk/")]
		public string Destination { get; set; }

		[XmlElement(ElementName = "JourneyPattern", Namespace = "http://www.transxchange.org.uk/")]
		public List<TXCXmlJourneyPattern> JourneyPattern { get; set; }
	}
}