using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "OperatingProfile", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlOperatingProfile
	{
		[XmlElement(ElementName = "RegularDayType", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlRegularDayType RegularDayType { get; set; }

		[XmlElement(ElementName = "BankHolidayOperation", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlBankHolidayOperation BankHolidayOperation { get; set; }

		[XmlElement(ElementName = "SpecialDaysOperation", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlSpecialDaysOperation SpecialDaysOperation { get; set; }
	}
}