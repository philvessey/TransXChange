using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "SpecialDaysOperation", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlSpecialDaysOperation
	{
		[XmlElement(ElementName = "DaysOfOperation", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlDaysOfOperation DaysOfOperation { get; set; }

		[XmlElement(ElementName = "DaysOfNonOperation", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlDaysOfNonOperation DaysOfNonOperation { get; set; }
	}
}