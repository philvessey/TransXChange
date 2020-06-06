using CsvHelper.Configuration.Attributes;

namespace TransXChange.Common.Models
{
    public class GTFSCalendar
    {
        [Name("service_id")]
        public string ServiceId { get; set; }

        [Name("monday")]
        public string Monday { get; set; }

        [Name("tuesday")]
        public string Tuesday { get; set; }

        [Name("wednesday")]
        public string Wednesday { get; set; }

        [Name("thursday")]
        public string Thursday { get; set; }

        [Name("friday")]
        public string Friday { get; set; }

        [Name("saturday")]
        public string Saturday { get; set; }

        [Name("sunday")]
        public string Sunday { get; set; }

        [Name("start_date")]
        public string StartDate { get; set; }

        [Name("end_date")]
        public string EndDate { get; set; }
    }
}