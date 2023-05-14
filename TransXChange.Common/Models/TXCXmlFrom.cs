using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "From", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlFrom
	{
		[XmlElement(ElementName = "WaitTime", Namespace = "http://www.transxchange.org.uk/")]
		public string WaitTime { get; set; }

		[XmlElement(ElementName = "StopPointRef", Namespace = "http://www.transxchange.org.uk/")]
		public string StopPointRef { get; set; }

		[XmlElement(ElementName = "TimingStatus", Namespace = "http://www.transxchange.org.uk/")]
		public string TimingStatus { get; set; }
	}
}