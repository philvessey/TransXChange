using System.Collections.Generic;
using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "JourneyPatternSections", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlJourneyPatternSections
	{
		[XmlElement(ElementName = "JourneyPatternSection", Namespace = "http://www.transxchange.org.uk/")]
		public List<TXCXmlJourneyPatternSection> JourneyPatternSection { get; set; }
	}
}