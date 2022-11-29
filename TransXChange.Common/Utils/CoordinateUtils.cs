using CoordinateSharp;

namespace TransXChange.Common.Utils
{
    public static class CoordinateUtils
    {
        public static Coordinate GetFromEastingNorthing(int easting, int northing)
        {
            Coordinate coordinate = UniversalTransverseMercator.ConvertUTMtoLatLong(new UniversalTransverseMercator("30U", easting, northing));
            coordinate.FormatOptions.Format = CoordinateFormatType.Decimal;
            coordinate.FormatOptions.Round = 7;

            return coordinate;
        }

        public static Coordinate GetFromLatitudeLongitude(double latitude, double longitude)
        {
            Coordinate coordinate = new Coordinate(latitude, longitude);
            coordinate.FormatOptions.Format = CoordinateFormatType.Decimal;
            coordinate.FormatOptions.Round = 7;

            return coordinate;
        }
    }
}