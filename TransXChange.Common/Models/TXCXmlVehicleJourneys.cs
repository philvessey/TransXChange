using System.Collections.Generic;
using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "VehicleJourneys", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlVehicleJourneys
	{
		[XmlElement(ElementName = "VehicleJourney", Namespace = "http://www.transxchange.org.uk/")]
		public List<TXCXmlVehicleJourney> VehicleJourney { get; set; }
	}
}