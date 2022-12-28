using System;
using System.Globalization;

namespace TransXChange.Common.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime GetScheduleDate(DateTime result, string date)
        {
            if (DateTime.TryParse(date, CultureInfo.CreateSpecificCulture("en-GB"), out DateTime now))
            {
                result = now;
            }

            if (date == "yesterday")
            {
                result = result.AddDays(-1);
            }

            if (date == "tomorrow")
            {
                result = result.AddDays(1);
            }

            return result;
        }
        
        public static DateTime? GetStartDate(DateTime? result, DateTime now, int days)
        {
            days = Validate(days);

            if (result == null)
            {
                result = now;
            }

            if (result.Value < now)
            {
                result = now;
            }

            if (result.Value > now)
            {
                if (result.Value.Subtract(now).TotalDays > days)
                {
                    return null;
                }
            }

            return result;
        }

        public static DateTime? GetEndDate(DateTime? result, DateTime now, int days)
        {
            days = Validate(days);

            if (result == null)
            {
                result = now.AddDays(days);
            }

            if (result.Value < now)
            {
                return null;
            }

            if (result.Value > now)
            {
                if (result.Value.Subtract(now).TotalDays > days)
                {
                    result = now.AddDays(days);
                }
            }

            return result;
        }

        public static DateTime? GetHolidayDate(DateTime? result, DateTime now, int days)
        {
            days = Validate(days);

            if (result == null)
            {
                return null;
            }

            if (result.Value < now)
            {
                return null;
            }

            if (result.Value > now)
            {
                if (result.Value.Subtract(now).TotalDays > days)
                {
                    return null;
                }
            }

            return result;
        }

        private static int Validate(int days)
        {
            if (days < 1)
            {
                days = 1;
            }

            if (days > 28)
            {
                days = 28;
            }

            return days - 1;
        }
    }
}