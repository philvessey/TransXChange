using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "Service", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlService
	{
		[XmlElement(ElementName = "ServiceCode", Namespace = "http://www.transxchange.org.uk/")]
		public string ServiceCode { get; set; }

		[XmlElement(ElementName = "Lines", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlLines Lines { get; set; }

		[XmlElement(ElementName = "OperatingPeriod", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlOperatingPeriod OperatingPeriod { get; set; }

		[XmlElement(ElementName = "OperatingProfile", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlOperatingProfile OperatingProfile { get; set; }

		[XmlElement(ElementName = "RegisteredOperatorRef", Namespace = "http://www.transxchange.org.uk/")]
		public string RegisteredOperatorRef { get; set; }

		[XmlElement(ElementName = "StopRequirements", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlStopRequirements StopRequirements { get; set; }

		[XmlElement(ElementName = "Mode", Namespace = "http://www.transxchange.org.uk/")]
		public string Mode { get; set; }

		[XmlElement(ElementName = "Description", Namespace = "http://www.transxchange.org.uk/")]
		public string Description { get; set; }

		[XmlElement(ElementName = "StandardService", Namespace = "http://www.transxchange.org.uk/")]
		public TXCXmlStandardService StandardService { get; set; }
	}
}