using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "Operator", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlOperator
	{
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }

		[XmlElement(ElementName = "NationalOperatorCode", Namespace = "http://www.transxchange.org.uk/")]
		public string NationalOperatorCode { get; set; }

		[XmlElement(ElementName = "OperatorCode", Namespace = "http://www.transxchange.org.uk/")]
		public string OperatorCode { get; set; }

		[XmlElement(ElementName = "OperatorShortName", Namespace = "http://www.transxchange.org.uk/")]
		public string OperatorShortName { get; set; }
	}
}