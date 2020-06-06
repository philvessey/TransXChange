using CsvHelper.Configuration.Attributes;

namespace TransXChange.Common.Models
{
    public class GTFSStop
    {
        [Name("stop_id")]
        public string StopId { get; set; }

        [Name("stop_code")]
        public string StopCode { get; set; }

        [Name("stop_name")]
        public string StopName { get; set; }

        [Name("stop_desc")]
        public string StopDesc { get; set; }

        [Name("stop_lat")]
        public string StopLat { get; set; }

        [Name("stop_lon")]
        public string StopLon { get; set; }

        [Name("zone_id")]
        public string ZoneId { get; set; }

        [Name("stop_url")]
        public string StopUrl { get; set; }

        [Name("location_type")]
        public string LocationType { get; set; }

        [Name("parent_station")]
        public string ParentStation { get; set; }

        [Name("stop_timezone")]
        public string StopTimezone { get; set; }

        [Name("wheelchair_boarding")]
        public string WheelchairBoarding { get; set; }

        [Name("level_id")]
        public string LevelId { get; set; }

        [Name("platform_code")]
        public string PlatformCode { get; set; }
    }
}