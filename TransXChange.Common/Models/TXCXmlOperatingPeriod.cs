using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "OperatingPeriod", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlOperatingPeriod
	{
		[XmlElement(ElementName = "StartDate", Namespace = "http://www.transxchange.org.uk/")]
		public string StartDate { get; set; }

		[XmlElement(ElementName = "EndDate", Namespace = "http://www.transxchange.org.uk/")]
		public string EndDate { get; set; }
	}
}