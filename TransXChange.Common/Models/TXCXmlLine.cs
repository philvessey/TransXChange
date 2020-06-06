using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "Line", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlLine
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "LineName", Namespace = "http://www.transxchange.org.uk/")]
		public string LineName { get; set; }
	}
}