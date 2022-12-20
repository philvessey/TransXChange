using Nager.Date;
using Nager.Date.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TransXChange.Common.Models;

namespace TransXChange.Common.Utils
{
    public static class BankHolidayUtils
    {
        public static List<PublicHoliday> GetDaysOfOperationEngland(TXCXmlDaysOfOperation daysOfOperation, TXCCalendar calendar, string key)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            if (string.IsNullOrEmpty(DateSystem.LicenseKey))
            {
                DateSystem.LicenseKey = key;
            }

            if (daysOfOperation.AllBankHolidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.AllHolidaysExceptChristmas != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.Christmas != null)
            {
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.DisplacementHolidays != null)
            {
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.EarlyRunOff != null)
            {
                results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.HolidayMondays != null)
            {
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.Holidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.NewYearsDay != null) { results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.NewYearsDayHoliday != null) { results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.GoodFriday != null) { results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.EasterMonday != null) { results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.MayDay != null) { results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.SpringBank != null) { results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.LateSummerBankHolidayNotScotland != null) { results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.ChristmasEve != null) { results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.ChristmasDay != null) { results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.ChristmasDayHoliday != null) { results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.BoxingDay != null) { results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.BoxingDayHoliday != null) { results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.NewYearsEve != null) { results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate)); }

            return results;
        }

        public static List<PublicHoliday> GetDaysOfNonOperationEngland(TXCXmlDaysOfNonOperation daysOfNonOperation, TXCCalendar calendar, string key)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            if (string.IsNullOrEmpty(DateSystem.LicenseKey))
            {
                DateSystem.LicenseKey = key;
            }

            if (daysOfNonOperation.AllBankHolidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.AllHolidaysExceptChristmas != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.Christmas != null)
            {
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.DisplacementHolidays != null)
            {
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.EarlyRunOff != null)
            {
                results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.HolidayMondays != null)
            {
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.Holidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.NewYearsDay != null) { results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.NewYearsDayHoliday != null) { results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.GoodFriday != null) { results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.EasterMonday != null) { results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.MayDay != null) { results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.SpringBank != null) { results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.LateSummerBankHolidayNotScotland != null) { results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.ChristmasEve != null) { results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.ChristmasDay != null) { results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.ChristmasDayHoliday != null) { results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.BoxingDay != null) { results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.BoxingDayHoliday != null) { results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.NewYearsEve != null) { results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate)); }

            return results;
        }

        public static List<PublicHoliday> GetDaysOfOperationScotland(TXCXmlDaysOfOperation daysOfOperation, TXCCalendar calendar, string key)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            if (string.IsNullOrEmpty(DateSystem.LicenseKey))
            {
                DateSystem.LicenseKey = key;
            }

            if (daysOfOperation.AllBankHolidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.AllHolidaysExceptChristmas != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.Christmas != null)
            {
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.DisplacementHolidays != null)
            {
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.EarlyRunOff != null)
            {
                results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.HolidayMondays != null)
            {
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.Holidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.NewYearsDay != null) { results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.NewYearsDayHoliday != null) { results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.Jan2ndScotland != null) { results.AddRange(GetJan2ndScotland(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.Jan2ndScotlandHoliday != null) { results.AddRange(GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.GoodFriday != null) { results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.EasterMonday != null) { results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.MayDay != null) { results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.SpringBank != null) { results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.AugustBankHolidayScotland != null) { results.AddRange(GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.StAndrewsDay != null) { results.AddRange(GetStAndrewsDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.StAndrewsDayHoliday != null) { results.AddRange(GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.ChristmasEve != null) { results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.ChristmasDay != null) { results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.ChristmasDayHoliday != null) { results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.BoxingDay != null) { results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.BoxingDayHoliday != null) { results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.NewYearsEve != null) { results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate)); }

            return results;
        }

        public static List<PublicHoliday> GetDaysOfNonOperationScotland(TXCXmlDaysOfNonOperation daysOfNonOperation, TXCCalendar calendar, string key)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            if (string.IsNullOrEmpty(DateSystem.LicenseKey))
            {
                DateSystem.LicenseKey = key;
            }

            if (daysOfNonOperation.AllBankHolidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.AllHolidaysExceptChristmas != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.Christmas != null)
            {
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.DisplacementHolidays != null)
            {
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.EarlyRunOff != null)
            {
                results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.HolidayMondays != null)
            {
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.Holidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.NewYearsDay != null) { results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.NewYearsDayHoliday != null) { results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.Jan2ndScotland != null) { results.AddRange(GetJan2ndScotland(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.Jan2ndScotlandHoliday != null)  { results.AddRange(GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.GoodFriday != null) { results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.EasterMonday != null) { results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.MayDay != null) { results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.SpringBank != null) { results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.AugustBankHolidayScotland != null) { results.AddRange(GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.StAndrewsDay != null) { results.AddRange(GetStAndrewsDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.StAndrewsDayHoliday != null) { results.AddRange(GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.ChristmasEve != null) { results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.ChristmasDay != null) { results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.ChristmasDayHoliday != null) { results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.BoxingDay != null) { results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.BoxingDayHoliday != null) { results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.NewYearsEve != null) { results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate)); }

            return results;
        }

        public static List<PublicHoliday> GetDaysOfOperationWales(TXCXmlDaysOfOperation daysOfOperation, TXCCalendar calendar, string key)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            if (string.IsNullOrEmpty(DateSystem.LicenseKey))
            {
                DateSystem.LicenseKey = key;
            }

            if (daysOfOperation.AllBankHolidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.AllHolidaysExceptChristmas != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.Christmas != null)
            {
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.DisplacementHolidays != null)
            {
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.EarlyRunOff != null)
            {
                results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.HolidayMondays != null)
            {
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.Holidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfOperation.NewYearsDay != null) { results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.NewYearsDayHoliday != null) { results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.GoodFriday != null) { results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.EasterMonday != null) { results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.MayDay != null) { results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.SpringBank != null) { results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.LateSummerBankHolidayNotScotland != null) { results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.ChristmasEve != null) { results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.ChristmasDay != null) { results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.ChristmasDayHoliday != null) { results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.BoxingDay != null) { results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.BoxingDayHoliday != null) { results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfOperation.NewYearsEve != null) { results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate)); }

            return results;
        }

        public static List<PublicHoliday> GetDaysOfNonOperationWales(TXCXmlDaysOfNonOperation daysOfNonOperation, TXCCalendar calendar, string key)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            if (string.IsNullOrEmpty(DateSystem.LicenseKey))
            {
                DateSystem.LicenseKey = key;
            }

            if (daysOfNonOperation.AllBankHolidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.AllHolidaysExceptChristmas != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.Christmas != null)
            {
                results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.DisplacementHolidays != null)
            {
                results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.EarlyRunOff != null)
            {
                results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.HolidayMondays != null)
            {
                results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.Holidays != null)
            {
                results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate));
            }

            if (daysOfNonOperation.NewYearsDay != null) { results.AddRange(GetNewYearsDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.NewYearsDayHoliday != null) { results.AddRange(GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.GoodFriday != null) { results.AddRange(GetGoodFriday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.EasterMonday != null) { results.AddRange(GetEasterMonday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.MayDay != null) { results.AddRange(GetMayDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.SpringBank != null) { results.AddRange(GetSpringBank(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.LateSummerBankHolidayNotScotland != null) { results.AddRange(GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.ChristmasEve != null) { results.AddRange(GetChristmasEve(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.ChristmasDay != null) { results.AddRange(GetChristmasDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.ChristmasDayHoliday != null) { results.AddRange(GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.BoxingDay != null) { results.AddRange(GetBoxingDay(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.BoxingDayHoliday != null) { results.AddRange(GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate)); }
            if (daysOfNonOperation.NewYearsEve != null) { results.AddRange(GetNewYearsEve(calendar.StartDate, calendar.EndDate)); }

            return results;
        }

        private static List<PublicHoliday> GetNewYearsDay(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 1, 1, "New Year's Day", "New Year's Day", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetNewYearsDayHoliday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHolidays(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "New Year's Day" && h.Counties != null && h.Counties.Contains("GB-ENG")).FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetJan2ndScotland(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 1, 2, "New Year's Day", "New Year's Day", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetJan2ndScotlandHoliday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHolidays(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "New Year's Day" && h.Counties != null && h.Counties.Contains("GB-SCT")).LastOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetGoodFriday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHolidays(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Good Friday").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetEasterMonday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHolidays(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Easter Monday").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetMayDay(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHolidays(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Early May Bank Holiday").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetSpringBank(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHolidays(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Spring Bank Holiday").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetAugustBankHolidayScotland(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHolidays(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Summer Bank Holiday" && h.Counties != null && h.Counties.Contains("GB-SCT")).FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetLateSummerBankHolidayNotScotland(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHolidays(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Summer Bank Holiday" && h.Counties != null && h.Counties.Contains("GB-ENG")).FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetStAndrewsDay(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 11, 30, "Saint Andrew's Day", "Saint Andrew's Day", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetStAndrewsDayHoliday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHolidays(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Saint Andrew's Day").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetChristmasEve(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 12, 24, "Christmas Eve", "Christmas Eve", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetChristmasDay(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 12, 25, "Christmas Day", "Christmas Day", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetChristmasDayHoliday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHolidays(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Christmas Day").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetBoxingDay(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = new PublicHoliday(new DateTime(Math.Max(startDate.Ticks, endDate.Ticks)).Year, 12, 26, "Boxing Day", "St. Stephen's Day", CountryCode.GB);

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetBoxingDayHoliday(DateTime startDate, DateTime endDate)
        {
            List<PublicHoliday> results = new List<PublicHoliday>();

            PublicHoliday holiday = DateSystem.GetPublicHolidays(startDate, endDate, CountryCode.GB).Where(h => h.LocalName == "Boxing Day").FirstOrDefault();

            if (holiday != null)
            {
                results.Add(holiday);
            }

            return results;
        }

        private static List<PublicHoliday> GetNewYearsEve(DateTime startDate, DateTime endDate)
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