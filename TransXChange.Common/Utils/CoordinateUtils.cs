using CoordinateSharp;

namespace TransXChange.Common.Utils
{
    public static class CoordinateUtils
    {
        public static Coordinate GetFromEastingNorthing(double easting, double northing)
        {
            Coordinate coordinate = UniversalTransverseMercator.ConvertUTMtoLatLong(new UniversalTransverseMercator("30U", easting, northing));
            coordinate.FormatOptions.Format = CoordinateFormatType.Decimal;
            coordinate.FormatOptions.Round = 11;

            return coordinate;
        }
    }
}