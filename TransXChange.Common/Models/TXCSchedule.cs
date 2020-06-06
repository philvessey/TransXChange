using System.Collections.Generic;

namespace TransXChange.Common.Models
{
    public class TXCSchedule
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Direction { get; set; }
        public string Line { get; set; }
        public string Mode { get; set; }
        public string OperatorCode { get; set; }
        public string OperatorName { get; set; }
        public string ServiceCode { get; set; }
        public TXCCalendar Calendar { get; set; }
        public List<TXCStop> Stops { get; set; }
    }
}