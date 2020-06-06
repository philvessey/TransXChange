using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "Lines", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlLines
	{
		[XmlElement(ElementName = "Line", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlLine Line { get; set; }
	}
}