using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "TransXChange", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlTransXChange
	{
		[XmlElement(ElementName = "StopPoints", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlStopPoints StopPoints { get; set; }

		[XmlElement(ElementName = "RouteSections", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlRouteSections RouteSections { get; set; }

		[XmlElement(ElementName = "Routes", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlRoutes Routes { get; set; }

		[XmlElement(ElementName = "JourneyPatternSections", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlJourneyPatternSections JourneyPatternSections { get; set; }

		[XmlElement(ElementName = "Operators", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlOperators Operators { get; set; }

		[XmlElement(ElementName = "Services", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlServices Services { get; set; }

		[XmlElement(ElementName = "VehicleJourneys", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlVehicleJourneys VehicleJourneys { get; set; }
	}
}