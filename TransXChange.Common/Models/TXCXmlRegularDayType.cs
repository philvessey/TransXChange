using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "RegularDayType", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlRegularDayType
	{
		[XmlElement(ElementName = "DaysOfWeek", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlDaysOfWeek DaysOfWeek { get; set; }
	}
}