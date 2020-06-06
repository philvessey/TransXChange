using Nager.Date;
using Nager.Date.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TransXChange.Common.Utils
{
    public static class BankHolidayUtils
    {
        public static List<PublicHoliday> GetNewYearsDay(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 1, 1, "New Year's Day", "New Year's Day", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetNewYearsDayHoliday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHoliday(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "New Year's Day" && h.Counties.Contains("GB-ENG")).FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetJan2ndScotland(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 1, 2, "New Year's Day", "New Year's Day", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetJan2ndScotlandHoliday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHoliday(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "New Year's Day" && h.Counties.Contains("GB-SCT")).LastOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetGoodFriday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHoliday(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Good Friday").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetEasterMonday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHoliday(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Easter Monday").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetMayDay(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHoliday(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Early May Bank Holiday").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetSpringBank(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHoliday(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Spring Bank Holiday").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetAugustBankHolidayScotland(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHoliday(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Summer Bank Holiday" && h.Counties.Contains("GB-SCT")).FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetLateSummerBankHolidayNotScotland(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHoliday(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Summer Bank Holiday" && h.Counties.Contains("GB-ENG")).FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetStAndrewsDay(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 11, 30, "Saint Andrew's Day", "Saint Andrew's Day", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetStAndrewsDayHoliday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHoliday(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Saint Andrew's Day").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetChristmasEve(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 12, 24, "Christmas Eve", "Christmas Eve", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetChristmasDay(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 12, 25, "Christmas Day", "Christmas Day", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetChristmasDayHoliday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHoliday(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Christmas Day").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetBoxingDay(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 12, 26, "Boxing Day", "St. Stephen's Day", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetBoxingDayHoliday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHoliday(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Boxing Day").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        public static List<PublicHoliday> GetNewYearsEve(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 12, 31, "New Year's Eve", "New Year's Eve", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }
    }
}