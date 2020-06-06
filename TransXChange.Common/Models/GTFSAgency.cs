using CsvHelper.Configuration.Attributes;

namespace TransXChange.Common.Models
{
    public class GTFSAgency
    {
        [Name("agency_id")]
        public string AgencyId { get; set; }

        [Name("agency_name")]
        public string AgencyName { get; set; }

        [Name("agency_url")]
        public string AgencyUrl { get; set; }

        [Name("agency_timezone")]
        public string AgencyTimezone { get; set; }

        [Name("agency_lang")]
        public string AgencyLang { get; set; }

        [Name("agency_phone")]
        public string AgencyPhone { get; set; }

        [Name("agency_fare_url")]
        public string AgencyFareUrl { get; set; }

        [Name("agency_email")]
        public string AgencyEmail { get; set; }
    }
}