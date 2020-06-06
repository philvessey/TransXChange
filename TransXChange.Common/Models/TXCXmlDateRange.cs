using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "DateRange", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlDateRange
	{
		[XmlElement(ElementName = "StartDate", Namespace = "http://www.transxchange.org.uk/")]
		public string StartDate { get; set; }
        
		[XmlElement(ElementName = "EndDate", Namespace = "http://www.transxchange.org.uk/")]
		public string EndDate { get; set; }
	}
}