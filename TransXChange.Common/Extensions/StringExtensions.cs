using System;

namespace TransXChange.Common.Extensions
{
    public static class StringExtensions
    {
        public static bool ToBool(this string baseString)
        {
            if (baseString != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DateTime? ToDateTimeFromTransXChange(this string baseString)
        {
            if (baseString != null)
            {
                int year = int.Parse(baseString[..4]);
                int month = int.Parse(baseString.Substring(5, 2));
                int day = int.Parse(baseString.Substring(8, 2));

                return new DateTime(year, month, day);
            }
            else
            {
                return null;
            }
        }

        public static TimeSpan? ToTimeSpanFromTransXChange(this string baseString)
        {
            if (baseString != null)
            {
                int hour = int.Parse(baseString[..2]);
                int minute = int.Parse(baseString.Substring(3, 2));
                int second = int.Parse(baseString.Substring(6, 2));

                return new TimeSpan(hour, minute, second);
            }
            else
            {
                return null;
            }
        }
    }
}