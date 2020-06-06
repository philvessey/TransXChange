using System.Collections.Generic;
using System.Xml.Serialization;

namespace TransXChange.Common.Models
{
	[XmlRoot(ElementName = "StopPoints", Namespace = "http://www.transxchange.org.uk/")]
	public class TXCXmlStopPoints
	{
		[XmlElement(ElementName = "AnnotatedStopPointRef", Namespace = "http://www.transxchange.org.uk/")]
		public List<TXCXmlAnnotatedStopPointRef> AnnotatedStopPointRef { get; set; }
	}
}