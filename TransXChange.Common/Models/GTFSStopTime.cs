using CsvHelper.Configuration.Attributes;

namespace TransXChange.Common.Models
{
    public class GTFSStopTime
    {
        [Name("trip_id")]
        public string TripId { get; set; }

        [Name("arrival_time")]
        public string ArrivalTime { get; set; }

        [Name("departure_time")]
        public string DepartureTime { get; set; }

        [Name("stop_id")]
        public string StopId { get; set; }

        [Name("stop_sequence")]
        public string StopSequence { get; set; }

        [Name("stop_headsign")]
        public string StopHeadsign { get; set; }

        [Name("pickup_type")]
        public string PickupType { get; set; }

        [Name("drop_off_type")]
        public string DropOffType { get; set; }

        [Name("shape_dist_traveled")]
        public string ShapeDistTraveled { get; set; }

        [Name("timepoint")]
        public string Timepoint { get; set; }
    }
}