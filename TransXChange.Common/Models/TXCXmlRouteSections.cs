using System.Collections.Generic;
using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "RouteSections", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlRouteSections
	{
		[XmlElement(ElementName = "RouteSection", Namespace = "http://www.transxchange.org.uk/")]
		public List<TXCXmlRouteSection> RouteSection { get; set; }
	}
}