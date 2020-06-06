using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "DaysOfWeek", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlDaysOfWeek
	{
		[XmlElement(ElementName = "MondayToFriday", Namespace = "http://www.transxchange.org.uk/")]
		public string MondayToFriday { get; set; }

		[XmlElement(ElementName = "MondayToSaturday", Namespace = "http://www.transxchange.org.uk/")]
		public string MondayToSaturday { get; set; }

		[XmlElement(ElementName = "MondayToSunday", Namespace = "http://www.transxchange.org.uk/")]
		public string MondayToSunday { get; set; }

		[XmlElement(ElementName = "Weekend", Namespace = "http://www.transxchange.org.uk/")]
		public string Weekend { get; set; }

		[XmlElement(ElementName = "NotMonday", Namespace = "http://www.transxchange.org.uk/")]
		public string NotMonday { get; set; }

		[XmlElement(ElementName = "NotTuesday", Namespace = "http://www.transxchange.org.uk/")]
		public string NotTuesday { get; set; }

		[XmlElement(ElementName = "NotWednesday", Namespace = "http://www.transxchange.org.uk/")]
		public string NotWednesday { get; set; }

		[XmlElement(ElementName = "NotThursday", Namespace = "http://www.transxchange.org.uk/")]
		public string NotThursday { get; set; }

		[XmlElement(ElementName = "NotFriday", Namespace = "http://www.transxchange.org.uk/")]
		public string NotFriday { get; set; }

		[XmlElement(ElementName = "NotSaturday", Namespace = "http://www.transxchange.org.uk/")]
		public string NotSaturday { get; set; }

		[XmlElement(ElementName = "NotSunday", Namespace = "http://www.transxchange.org.uk/")]
		public string NotSunday { get; set; }

		[XmlElement(ElementName = "Monday", Namespace = "http://www.transxchange.org.uk/")]
		public string Monday { get; set; }

		[XmlElement(ElementName = "Tuesday", Namespace = "http://www.transxchange.org.uk/")]
		public string Tuesday { get; set; }

		[XmlElement(ElementName = "Wednesday", Namespace = "http://www.transxchange.org.uk/")]
		public string Wednesday { get; set; }

		[XmlElement(ElementName = "Thursday", Namespace = "http://www.transxchange.org.uk/")]
		public string Thursday { get; set; }

		[XmlElement(ElementName = "Friday", Namespace = "http://www.transxchange.org.uk/")]
		public string Friday { get; set; }

		[XmlElement(ElementName = "Saturday", Namespace = "http://www.transxchange.org.uk/")]
		public string Saturday { get; set; }

		[XmlElement(ElementName = "Sunday", Namespace = "http://www.transxchange.org.uk/")]
		public string Sunday { get; set; }
	}
}