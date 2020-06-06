using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "AnnotatedStopPointRef", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlAnnotatedStopPointRef
	{
		[XmlElement(ElementName = "StopPointRef", Namespace = "http://www.transxchange.org.uk/")]
		public string StopPointRef { get; set; }

		[XmlElement(ElementName = "CommonName", Namespace = "http://www.transxchange.org.uk/")]
		public string CommonName { get; set; }
	}
}