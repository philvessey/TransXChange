using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "StopRequirements", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlStopRequirements
	{
		[XmlElement(ElementName = "NoNewStopsRequired", Namespace = "http://www.transxchange.org.uk/")]
		public string NoNewStopsRequired { get; set; }
	}
}