using CsvHelper.Configuration.Attributes;

namespace TransXChange.Common.Models
{
    public class GTFSCalendarDate
    {
        [Name("service_id")]
        public string ServiceId { get; set; }

        [Name("date")]
        public string Date { get; set; }

        [Name("exception_type")]
        public string ExceptionType { get; set; }
    }
}