using CsvHelper.Configuration.Attributes;

namespace TransXChange.Common.Models
{
    public class GTFSTrip
    {
        [Name("route_id")]
        public string RouteId { get; set; }

        [Name("service_id")]
        public string ServiceId { get; set; }

        [Name("trip_id")]
        public string TripId { get; set; }

        [Name("trip_headsign")]
        public string TripHeadsign { get; set; }

        [Name("trip_short_name")]
        public string TripShortName { get; set; }

        [Name("direction_id")]
        public string DirectionId { get; set; }

        [Name("block_id")]
        public string BlockId { get; set; }

        [Name("shape_id")]
        public string ShapeId { get; set; }

        [Name("wheelchair_accessible")]
        public string WheelchairAccessible { get; set; }

        [Name("bikes_allowed")]
        public string BikesAllowed { get; set; }
    }
}