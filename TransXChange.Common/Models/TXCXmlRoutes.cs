using System.Collections.Generic;
using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "Routes", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlRoutes
	{
		[XmlElement(ElementName = "Route", Namespace = "http://www.transxchange.org.uk/")]
		public List<TXCXmlRoute> Route { get; set; }
	}
}