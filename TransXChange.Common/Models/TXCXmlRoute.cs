using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "Route", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlRoute
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "Description", Namespace = "http://www.transxchange.org.uk/")]
		public string Description { get; set; }

		[XmlElement(ElementName = "RouteSectionRef", Namespace = "http://www.transxchange.org.uk/")]
		public string RouteSectionRef { get; set; }
	}
}