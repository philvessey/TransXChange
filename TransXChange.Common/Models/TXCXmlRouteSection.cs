using System.Collections.Generic;
using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "RouteSection", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlRouteSection
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "RouteLink", Namespace = "http://www.transxchange.org.uk/")]
		public List<TXCXmlRouteLink> RouteLink { get; set; }
	}
}