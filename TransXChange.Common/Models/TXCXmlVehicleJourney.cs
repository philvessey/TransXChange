using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "VehicleJourney", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlVehicleJourney
	{
		[XmlElement(ElementName = "OperatorRef", Namespace = "http://www.transxchange.org.uk/")]
		public string OperatorRef { get; set; }

		[XmlElement(ElementName = "OperatingProfile", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlOperatingProfile OperatingProfile { get; set; }

		[XmlElement(ElementName = "VehicleJourneyCode", Namespace = "http://www.transxchange.org.uk/")]
		public string VehicleJourneyCode { get; set; }

		[XmlElement(ElementName = "ServiceRef", Namespace = "http://www.transxchange.org.uk/")]
		public string ServiceRef { get; set; }

		[XmlElement(ElementName = "LineRef", Namespace = "http://www.transxchange.org.uk/")]
		public string LineRef { get; set; }

		[XmlElement(ElementName = "JourneyPatternRef", Namespace = "http://www.transxchange.org.uk/")]
		public string JourneyPatternRef { get; set; }

		[XmlElement(ElementName = "DepartureTime", Namespace = "http://www.transxchange.org.uk/")]
		public string DepartureTime { get; set; }
	}
}