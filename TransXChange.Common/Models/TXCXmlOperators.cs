using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "Operators", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlOperators
	{
		[XmlElement(ElementName = "Operator", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlOperator Operator { get; set; }
	}
}