using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "DaysOfNonOperation", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlDaysOfNonOperation
	{
		[XmlElement(ElementName = "AllBankHolidays", Namespace = "http://www.transxchange.org.uk/")]
		public string AllBankHolidays { get; set; }

		[XmlElement(ElementName = "AllHolidaysExceptChristmas", Namespace = "http://www.transxchange.org.uk/")]
		public string AllHolidaysExceptChristmas { get; set; }

		[XmlElement(ElementName = "Christmas", Namespace = "http://www.transxchange.org.uk/")]
		public string Christmas { get; set; }

		[XmlElement(ElementName = "DisplacementHolidays", Namespace = "http://www.transxchange.org.uk/")]
		public string DisplacementHolidays { get; set; }

		[XmlElement(ElementName = "EarlyRunOff", Namespace = "http://www.transxchange.org.uk/")]
		public string EarlyRunOff { get; set; }

		[XmlElement(ElementName = "HolidayMondays", Namespace = "http://www.transxchange.org.uk/")]
		public string HolidayMondays { get; set; }

		[XmlElement(ElementName = "Holidays", Namespace = "http://www.transxchange.org.uk/")]
		public string Holidays { get; set; }

		[XmlElement(ElementName = "NewYearsDay", Namespace = "http://www.transxchange.org.uk/")]
		public string NewYearsDay { get; set; }

		[XmlElement(ElementName = "NewYearsDayHoliday", Namespace = "http://www.transxchange.org.uk/")]
		public string NewYearsDayHoliday { get; set; }

		[XmlElement(ElementName = "Jan2ndScotland", Namespace = "http://www.transxchange.org.uk/")]
		public string Jan2ndScotland { get; set; }

		[XmlElement(ElementName = "Jan2ndScotlandHoliday", Namespace = "http://www.transxchange.org.uk/")]
		public string Jan2ndScotlandHoliday { get; set; }

		[XmlElement(ElementName = "GoodFriday", Namespace = "http://www.transxchange.org.uk/")]
		public string GoodFriday { get; set; }

		[XmlElement(ElementName = "EasterMonday", Namespace = "http://www.transxchange.org.uk/")]
		public string EasterMonday { get; set; }

		[XmlElement(ElementName = "MayDay", Namespace = "http://www.transxchange.org.uk/")]
		public string MayDay { get; set; }

		[XmlElement(ElementName = "SpringBank", Namespace = "http://www.transxchange.org.uk/")]
		public string SpringBank { get; set; }

		[XmlElement(ElementName = "AugustBankHolidayScotland", Namespace = "http://www.transxchange.org.uk/")]
		public string AugustBankHolidayScotland { get; set; }

		[XmlElement(ElementName = "LateSummerBankHolidayNotScotland", Namespace = "http://www.transxchange.org.uk/")]
		public string LateSummerBankHolidayNotScotland { get; set; }

		[XmlElement(ElementName = "StAndrewsDay", Namespace = "http://www.transxchange.org.uk/")]
		public string StAndrewsDay { get; set; }

		[XmlElement(ElementName = "StAndrewsDayHoliday", Namespace = "http://www.transxchange.org.uk/")]
		public string StAndrewsDayHoliday { get; set; }

		[XmlElement(ElementName = "ChristmasEve", Namespace = "http://www.transxchange.org.uk/")]
		public string ChristmasEve { get; set; }

		[XmlElement(ElementName = "ChristmasDay", Namespace = "http://www.transxchange.org.uk/")]
		public string ChristmasDay { get; set; }

		[XmlElement(ElementName = "ChristmasDayHoliday", Namespace = "http://www.transxchange.org.uk/")]
		public string ChristmasDayHoliday { get; set; }

		[XmlElement(ElementName = "BoxingDay", Namespace = "http://www.transxchange.org.uk/")]
		public string BoxingDay { get; set; }

		[XmlElement(ElementName = "BoxingDayHoliday", Namespace = "http://www.transxchange.org.uk/")]
		public string BoxingDayHoliday { get; set; }

		[XmlElement(ElementName = "NewYearsEve", Namespace = "http://www.transxchange.org.uk/")]
		public string NewYearsEve { get; set; }

		[XmlElement(ElementName = "DateRange", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlDateRange DateRange { get; set; }
	}
}