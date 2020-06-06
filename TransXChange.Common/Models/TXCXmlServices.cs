using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "Services", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlServices
	{
		[XmlElement(ElementName = "Service", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlService Service { get; set; }
	}
}