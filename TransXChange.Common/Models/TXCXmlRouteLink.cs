using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "RouteLink", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlRouteLink
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "From", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlFrom From { get; set; }

		[XmlElement(ElementName = "To", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlTo To { get; set; }

		[XmlElement(ElementName = "Direction", Namespace = "http://www.transxchange.org.uk/")]
		public string Direction { get; set; }
	}
}