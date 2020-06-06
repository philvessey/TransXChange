using CsvHelper.Configuration.Attributes;

namespace TransXChange.Common.Models
{
    public class GTFSRoute
    {
        [Name("route_id")]
        public string RouteId { get; set; }

        [Name("agency_id")]
        public string AgencyId { get; set; }

        [Name("route_short_name")]
        public string RouteShortName { get; set; }

        [Name("route_long_name")]
        public string RouteLongName { get; set; }

        [Name("route_desc")]
        public string RouteDesc { get; set; }

        [Name("route_type")]
        public string RouteType { get; set; }

        [Name("route_url")]
        public string RouteUrl { get; set; }

        [Name("route_color")]
        public string RouteColor { get; set; }

        [Name("route_text_color")]
        public string RouteTextColor { get; set; }

        [Name("route_sort_order")]
        public string RouteSortOrder { get; set; }
    }
}