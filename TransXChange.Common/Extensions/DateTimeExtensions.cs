using NodaTime;
using NodaTime.Extensions;
using System;

namespace TransXChange.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToZonedDateTime(this DateTime dateTime, string timezone)
        {
            Instant instant = dateTime.ToUniversalTime().ToInstant();
            ZonedDateTime zoned = instant.InZone(DateTimeZoneProviders.Tzdb[timezone]);

            return zoned.ToDateTimeUnspecified();
        }
    }
}