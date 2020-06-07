using System;
using System.Collections.Generic;
using TransXChange.Common.Extensions;
using TransXChange.Common.Models;

namespace TransXChange.Common.Utils
{
    public static class CalendarUtils
    {
        public static TXCCalendar Build(TXCXmlOperatingProfile operatingProfile, DateTime startDate, DateTime endDate)
        {
            TXCCalendar result = new TXCCalendar()
            {
                Monday = false,
                Tuesday = false,
                Wednesday = false,
                Thursday = false,
                Friday = false,
                Saturday = false,
                Sunday = false,
                StartDate = startDate,
                EndDate = endDate,
                RunningDates = new List<DateTime>(),
                SupplementRunningDates = new List<DateTime>(),
                SupplementNonRunningDates = new List<DateTime>()
            };

            if (operatingProfile.RegularDayType.DaysOfWeek != null)
            {
                if (operatingProfile.RegularDayType.DaysOfWeek.MondayToFriday != null)
                {
                    result.Monday = true;
                    result.Tuesday = true;
                    result.Wednesday = true;
                    result.Thursday = true;
                    result.Friday = true;
                    result.Saturday = false;
                    result.Sunday = false;
                }
                else if (operatingProfile.RegularDayType.DaysOfWeek.MondayToSaturday != null)
                {
                    result.Monday = true;
                    result.Tuesday = true;
                    result.Wednesday = true;
                    result.Thursday = true;
                    result.Friday = true;
                    result.Saturday = true;
                    result.Sunday = false;
                }
                else if (operatingProfile.RegularDayType.DaysOfWeek.MondayToSunday != null)
                {
                    result.Monday = true;
                    result.Tuesday = true;
                    result.Wednesday = true;
                    result.Thursday = true;
                    result.Friday = true;
                    result.Saturday = true;
                    result.Sunday = true;
                }
                else if (operatingProfile.RegularDayType.DaysOfWeek.Weekend != null)
                {
                    result.Monday = false;
                    result.Tuesday = false;
                    result.Wednesday = false;
                    result.Thursday = false;
                    result.Friday = false;
                    result.Saturday = true;
                    result.Sunday = true;
                }
                else if (operatingProfile.RegularDayType.DaysOfWeek.NotMonday != null)
                {
                    result.Monday = false;
                    result.Tuesday = true;
                    result.Wednesday = true;
                    result.Thursday = true;
                    result.Friday = true;
                    result.Saturday = true;
                    result.Sunday = true;
                }
                else if (operatingProfile.RegularDayType.DaysOfWeek.NotTuesday != null)
                {
                    result.Monday = true;
                    result.Tuesday = false;
                    result.Wednesday = true;
                    result.Thursday = true;
                    result.Friday = true;
                    result.Saturday = true;
                    result.Sunday = true;
                }
                else if (operatingProfile.RegularDayType.DaysOfWeek.NotWednesday != null)
                {
                    result.Monday = true;
                    result.Tuesday = true;
                    result.Wednesday = false;
                    result.Thursday = true;
                    result.Friday = true;
                    result.Saturday = true;
                    result.Sunday = true;
                }
                else if (operatingProfile.RegularDayType.DaysOfWeek.NotThursday != null)
                {
                    result.Monday = true;
                    result.Tuesday = true;
                    result.Wednesday = true;
                    result.Thursday = false;
                    result.Friday = true;
                    result.Saturday = true;
                    result.Sunday = true;
                }
                else if (operatingProfile.RegularDayType.DaysOfWeek.NotFriday != null)
                {
                    result.Monday = true;
                    result.Tuesday = true;
                    result.Wednesday = true;
                    result.Thursday = true;
                    result.Friday = false;
                    result.Saturday = true;
                    result.Sunday = true;
                }
                else if (operatingProfile.RegularDayType.DaysOfWeek.NotSaturday != null)
                {
                    result.Monday = true;
                    result.Tuesday = true;
                    result.Wednesday = true;
                    result.Thursday = true;
                    result.Friday = true;
                    result.Saturday = false;
                    result.Sunday = true;
                }
                else if (operatingProfile.RegularDayType.DaysOfWeek.NotSunday != null)
                {
                    result.Monday = true;
                    result.Tuesday = true;
                    result.Wednesday = true;
                    result.Thursday = true;
                    result.Friday = true;
                    result.Saturday = true;
                    result.Sunday = false;
                }
                else
                {
                    result.Monday = operatingProfile.RegularDayType.DaysOfWeek.Monday.ToBool();
                    result.Tuesday = operatingProfile.RegularDayType.DaysOfWeek.Tuesday.ToBool();
                    result.Wednesday = operatingProfile.RegularDayType.DaysOfWeek.Wednesday.ToBool();
                    result.Thursday = operatingProfile.RegularDayType.DaysOfWeek.Thursday.ToBool();
                    result.Friday = operatingProfile.RegularDayType.DaysOfWeek.Friday.ToBool();
                    result.Saturday = operatingProfile.RegularDayType.DaysOfWeek.Saturday.ToBool();
                    result.Sunday = operatingProfile.RegularDayType.DaysOfWeek.Sunday.ToBool();
                }
            }

            while (startDate <= endDate)
            {
                if (startDate.DayOfWeek == DayOfWeek.Monday && result.Monday)
                {
                    result.RunningDates.Add(startDate);
                }
                else if (startDate.DayOfWeek == DayOfWeek.Tuesday && result.Tuesday)
                {
                    result.RunningDates.Add(startDate);
                }
                else if (startDate.DayOfWeek == DayOfWeek.Wednesday && result.Wednesday)
                {
                    result.RunningDates.Add(startDate);
                }
                else if (startDate.DayOfWeek == DayOfWeek.Thursday && result.Thursday)
                {
                    result.RunningDates.Add(startDate);
                }
                else if (startDate.DayOfWeek == DayOfWeek.Friday && result.Friday)
                {
                    result.RunningDates.Add(startDate);
                }
                else if (startDate.DayOfWeek == DayOfWeek.Saturday && result.Saturday)
                {
                    result.RunningDates.Add(startDate);
                }
                else if (startDate.DayOfWeek == DayOfWeek.Sunday && result.Sunday)
                {
                    result.RunningDates.Add(startDate);
                }

                startDate = startDate.AddDays(1);
            }

            return result;
        }
    }
}