using Nager.Date.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TransXChange.Common.Extensions;
using TransXChange.Common.Models;
using TransXChange.Common.Utils;

namespace TransXChange.Common.Helpers
{
    public class TravelineHelpers
    {
        public Dictionary<string, TXCSchedule> ReadEngland(Dictionary<string, NAPTANStop> stops, string path, string mode, IEnumerable<string> filters, double days)
        {
            Dictionary<string, TXCSchedule> dictionary = new Dictionary<string, TXCSchedule>();

            if (path.EndsWith(".zip"))
            {
                DateTime now = DateTime.Now.Date;

                if (File.Exists(path))
                {
                    using ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read);

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.Name.EndsWith(".xml"))
                        {
                            using StreamReader reader = new StreamReader(entry.Open());
                            TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                            foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                            {
                                bool includeSchedule = false;

                                DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTime(), now, days);
                                DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTime(), now, days);

                                if (startDate == null || endDate == null)
                                {
                                    continue;
                                }

                                if (startDate > endDate || endDate < startDate)
                                {
                                    continue;
                                }

                                string journeyPatternReference = vehicleJourney.JourneyPatternRef;

                                if (journeyPatternReference == null)
                                {
                                    continue;
                                }

                                TXCXmlJourneyPattern journeyPattern = xml.Services.Service.StandardService.JourneyPattern.Where(p => p.Id == journeyPatternReference).FirstOrDefault();

                                if (journeyPattern == null)
                                {
                                    journeyPattern = xml.Services.Service.StandardService.JourneyPattern.FirstOrDefault();
                                }

                                TXCXmlOperatingProfile operatingProfile = vehicleJourney.OperatingProfile;

                                if (operatingProfile == null)
                                {
                                    operatingProfile = xml.Services.Service.OperatingProfile;
                                }

                                TXCCalendar calendar = CalendarUtils.Build(operatingProfile, startDate.Value, endDate.Value);

                                if (operatingProfile.BankHolidayOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.BankHolidayOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.BankHolidayOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.LateSummerBankHolidayNotScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (!calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfNonOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.LateSummerBankHolidayNotScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (operatingProfile.SpecialDaysOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.SpecialDaysOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.SpecialDaysOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (!calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }
                                }

                                calendar.RunningDates = calendar.RunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementRunningDates = calendar.SupplementRunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementNonRunningDates = calendar.SupplementNonRunningDates.Distinct().OrderBy(d => d).ToList();

                                TXCSchedule schedule = ScheduleUtils.Build(xml.Operators.Operator, xml.Services.Service, journeyPattern, calendar);
                                TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpan();
                                TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpan();

                                TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                for (int i = 1; i <= patternTimings?.Count; i++)
                                {
                                    TXCStop stop = new TXCStop();

                                    if (i == 1)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else if (i == patternTimings.Count)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].To.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }

                                    if (stops.ContainsKey(stop.ATCOCode))
                                    {
                                        stop.NaptanStop = stops[stop.ATCOCode];
                                    }
                                    else
                                    {
                                        stop.NaptanStop = new NAPTANStop() { ATCOCode = stop.ATCOCode, CommonName = "Unknown NaPTAN Stop", StopType = "ZZZ" };
                                    }

                                    schedule.Stops.Add(stop);

                                    arrivalTime = arrivalTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));
                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));

                                    if (mode != "all")
                                    {
                                        if (mode == "bus")
                                        {
                                            if (stop.NaptanStop.StopType == "BCT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "city-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "RLY")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "ferry")
                                        {
                                            if (stop.NaptanStop.StopType == "FBT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "light-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "PLT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string filter in filters)
                                        {
                                            if (filter != "all")
                                            {
                                                if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                {
                                                    includeSchedule = true;
                                                }
                                            }
                                            else
                                            {
                                                includeSchedule = true;
                                            }
                                        }
                                    }
                                }

                                if (includeSchedule)
                                {
                                    dictionary.Add(schedule.Id, schedule);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                DateTime now = DateTime.Now.Date;

                if (Directory.Exists(path))
                {
                    string[] entries = Directory.GetFiles(path);

                    foreach (string entry in entries)
                    {
                        if (entry.EndsWith(".xml"))
                        {
                            using StreamReader reader = new StreamReader(entry);
                            TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                            foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                            {
                                bool includeSchedule = false;

                                DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTime(), now, days);
                                DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTime(), now, days);

                                if (startDate == null || endDate == null)
                                {
                                    continue;
                                }

                                if (startDate > endDate || endDate < startDate)
                                {
                                    continue;
                                }

                                string journeyPatternReference = vehicleJourney.JourneyPatternRef;

                                if (journeyPatternReference == null)
                                {
                                    continue;
                                }

                                TXCXmlJourneyPattern journeyPattern = xml.Services.Service.StandardService.JourneyPattern.Where(p => p.Id == journeyPatternReference).FirstOrDefault();

                                if (journeyPattern == null)
                                {
                                    journeyPattern = xml.Services.Service.StandardService.JourneyPattern.FirstOrDefault();
                                }

                                TXCXmlOperatingProfile operatingProfile = vehicleJourney.OperatingProfile;

                                if (operatingProfile == null)
                                {
                                    operatingProfile = xml.Services.Service.OperatingProfile;
                                }

                                TXCCalendar calendar = CalendarUtils.Build(operatingProfile, startDate.Value, endDate.Value);

                                if (operatingProfile.BankHolidayOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.BankHolidayOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.BankHolidayOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.LateSummerBankHolidayNotScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (!calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfNonOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.LateSummerBankHolidayNotScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (operatingProfile.SpecialDaysOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.SpecialDaysOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.SpecialDaysOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (!calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }
                                }

                                calendar.RunningDates = calendar.RunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementRunningDates = calendar.SupplementRunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementNonRunningDates = calendar.SupplementNonRunningDates.Distinct().OrderBy(d => d).ToList();

                                TXCSchedule schedule = ScheduleUtils.Build(xml.Operators.Operator, xml.Services.Service, journeyPattern, calendar);
                                TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpan();
                                TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpan();

                                TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                for (int i = 1; i <= patternTimings?.Count; i++)
                                {
                                    TXCStop stop = new TXCStop();

                                    if (i == 1)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else if (i == patternTimings.Count)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].To.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }

                                    if (stops.ContainsKey(stop.ATCOCode))
                                    {
                                        stop.NaptanStop = stops[stop.ATCOCode];
                                    }
                                    else
                                    {
                                        stop.NaptanStop = new NAPTANStop() { ATCOCode = stop.ATCOCode, CommonName = "Unknown NaPTAN Stop", StopType = "ZZZ" };
                                    }

                                    schedule.Stops.Add(stop);

                                    arrivalTime = arrivalTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));
                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));

                                    if (mode != "all")
                                    {
                                        if (mode == "bus")
                                        {
                                            if (stop.NaptanStop.StopType == "BCT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "city-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "RLY")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "ferry")
                                        {
                                            if (stop.NaptanStop.StopType == "FBT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "light-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "PLT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string filter in filters)
                                        {
                                            if (filter != "all")
                                            {
                                                if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                {
                                                    includeSchedule = true;
                                                }
                                            }
                                            else
                                            {
                                                includeSchedule = true;
                                            }
                                        }
                                    }
                                }

                                if (includeSchedule)
                                {
                                    dictionary.Add(schedule.Id, schedule);
                                }
                            }
                        }
                    }
                }
            }

            return dictionary;
        }

        public Dictionary<string, TXCSchedule> ReadScotland(Dictionary<string, NAPTANStop> stops, string path, string mode, IEnumerable<string> filters, double days)
        {
            Dictionary<string, TXCSchedule> dictionary = new Dictionary<string, TXCSchedule>();

            if (path.EndsWith(".zip"))
            {
                DateTime now = DateTime.Now.Date;

                if (File.Exists(path))
                {
                    using ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read);

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.Name.EndsWith(".xml"))
                        {
                            using StreamReader reader = new StreamReader(entry.Open());
                            TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                            foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                            {
                                bool includeSchedule = false;

                                DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTime(), now, days);
                                DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTime(), now, days);

                                if (startDate == null || endDate == null)
                                {
                                    continue;
                                }

                                if (startDate > endDate || endDate < startDate)
                                {
                                    continue;
                                }

                                string journeyPatternReference = vehicleJourney.JourneyPatternRef;

                                if (journeyPatternReference == null)
                                {
                                    continue;
                                }

                                TXCXmlJourneyPattern journeyPattern = xml.Services.Service.StandardService.JourneyPattern.Where(p => p.Id == journeyPatternReference).FirstOrDefault();

                                if (journeyPattern == null)
                                {
                                    journeyPattern = xml.Services.Service.StandardService.JourneyPattern.FirstOrDefault();
                                }

                                TXCXmlOperatingProfile operatingProfile = vehicleJourney.OperatingProfile;

                                if (operatingProfile == null)
                                {
                                    operatingProfile = xml.Services.Service.OperatingProfile;
                                }

                                TXCCalendar calendar = CalendarUtils.Build(operatingProfile, startDate.Value, endDate.Value);

                                if (operatingProfile.BankHolidayOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.BankHolidayOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.BankHolidayOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Jan2ndScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Jan2ndScotlandHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.AugustBankHolidayScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.StAndrewsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.StAndrewsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (!calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfNonOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Jan2ndScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Jan2ndScotlandHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.AugustBankHolidayScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.StAndrewsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.StAndrewsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (operatingProfile.SpecialDaysOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.SpecialDaysOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.SpecialDaysOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (!calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }
                                }

                                calendar.RunningDates = calendar.RunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementRunningDates = calendar.SupplementRunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementNonRunningDates = calendar.SupplementNonRunningDates.Distinct().OrderBy(d => d).ToList();

                                TXCSchedule schedule = ScheduleUtils.Build(xml.Operators.Operator, xml.Services.Service, journeyPattern, calendar);
                                TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpan();
                                TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpan();

                                TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                for (int i = 1; i <= patternTimings?.Count; i++)
                                {
                                    TXCStop stop = new TXCStop();

                                    if (i == 1)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else if (i == patternTimings.Count)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].To.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }

                                    if (stops.ContainsKey(stop.ATCOCode))
                                    {
                                        stop.NaptanStop = stops[stop.ATCOCode];
                                    }
                                    else
                                    {
                                        stop.NaptanStop = new NAPTANStop() { ATCOCode = stop.ATCOCode, CommonName = "Unknown NaPTAN Stop", StopType = "ZZZ" };
                                    }

                                    schedule.Stops.Add(stop);

                                    arrivalTime = arrivalTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));
                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));

                                    if (mode != "all")
                                    {
                                        if (mode == "bus")
                                        {
                                            if (stop.NaptanStop.StopType == "BCT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "city-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "RLY")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "ferry")
                                        {
                                            if (stop.NaptanStop.StopType == "FBT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "light-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "PLT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string filter in filters)
                                        {
                                            if (filter != "all")
                                            {
                                                if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                {
                                                    includeSchedule = true;
                                                }
                                            }
                                            else
                                            {
                                                includeSchedule = true;
                                            }
                                        }
                                    }
                                }

                                if (includeSchedule)
                                {
                                    dictionary.Add(schedule.Id, schedule);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                DateTime now = DateTime.Now.Date;

                if (Directory.Exists(path))
                {
                    string[] entries = Directory.GetFiles(path);

                    foreach (string entry in entries)
                    {
                        if (entry.EndsWith(".xml"))
                        {
                            using StreamReader reader = new StreamReader(entry);
                            TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                            foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                            {
                                bool includeSchedule = false;

                                DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTime(), now, days);
                                DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTime(), now, days);

                                if (startDate == null || endDate == null)
                                {
                                    continue;
                                }

                                if (startDate > endDate || endDate < startDate)
                                {
                                    continue;
                                }

                                string journeyPatternReference = vehicleJourney.JourneyPatternRef;

                                if (journeyPatternReference == null)
                                {
                                    continue;
                                }

                                TXCXmlJourneyPattern journeyPattern = xml.Services.Service.StandardService.JourneyPattern.Where(p => p.Id == journeyPatternReference).FirstOrDefault();

                                if (journeyPattern == null)
                                {
                                    journeyPattern = xml.Services.Service.StandardService.JourneyPattern.FirstOrDefault();
                                }

                                TXCXmlOperatingProfile operatingProfile = vehicleJourney.OperatingProfile;

                                if (operatingProfile == null)
                                {
                                    operatingProfile = xml.Services.Service.OperatingProfile;
                                }

                                TXCCalendar calendar = CalendarUtils.Build(operatingProfile, startDate.Value, endDate.Value);

                                if (operatingProfile.BankHolidayOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.BankHolidayOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.BankHolidayOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Jan2ndScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Jan2ndScotlandHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.AugustBankHolidayScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.StAndrewsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.StAndrewsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (!calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfNonOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Jan2ndScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Jan2ndScotlandHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetJan2ndScotlandHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.AugustBankHolidayScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetAugustBankHolidayScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.StAndrewsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.StAndrewsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetStAndrewsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (operatingProfile.SpecialDaysOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.SpecialDaysOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.SpecialDaysOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (!calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }
                                }

                                calendar.RunningDates = calendar.RunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementRunningDates = calendar.SupplementRunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementNonRunningDates = calendar.SupplementNonRunningDates.Distinct().OrderBy(d => d).ToList();

                                TXCSchedule schedule = ScheduleUtils.Build(xml.Operators.Operator, xml.Services.Service, journeyPattern, calendar);
                                TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpan();
                                TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpan();

                                TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                for (int i = 1; i <= patternTimings?.Count; i++)
                                {
                                    TXCStop stop = new TXCStop();

                                    if (i == 1)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else if (i == patternTimings.Count)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].To.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }

                                    if (stops.ContainsKey(stop.ATCOCode))
                                    {
                                        stop.NaptanStop = stops[stop.ATCOCode];
                                    }
                                    else
                                    {
                                        stop.NaptanStop = new NAPTANStop() { ATCOCode = stop.ATCOCode, CommonName = "Unknown NaPTAN Stop", StopType = "ZZZ" };
                                    }

                                    schedule.Stops.Add(stop);

                                    arrivalTime = arrivalTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));
                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));

                                    if (mode != "all")
                                    {
                                        if (mode == "bus")
                                        {
                                            if (stop.NaptanStop.StopType == "BCT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "city-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "RLY")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "ferry")
                                        {
                                            if (stop.NaptanStop.StopType == "FBT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "light-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "PLT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string filter in filters)
                                        {
                                            if (filter != "all")
                                            {
                                                if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                {
                                                    includeSchedule = true;
                                                }
                                            }
                                            else
                                            {
                                                includeSchedule = true;
                                            }
                                        }
                                    }
                                }

                                if (includeSchedule)
                                {
                                    dictionary.Add(schedule.Id, schedule);
                                }
                            }
                        }
                    }
                }
            }

            return dictionary;
        }

        public Dictionary<string, TXCSchedule> ReadWales(Dictionary<string, NAPTANStop> stops, string path, string mode, IEnumerable<string> filters, double days)
        {
            Dictionary<string, TXCSchedule> dictionary = new Dictionary<string, TXCSchedule>();

            if (path.EndsWith(".zip"))
            {
                DateTime now = DateTime.Now.Date;

                if (File.Exists(path))
                {
                    using ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read);

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.Name.EndsWith(".xml"))
                        {
                            using StreamReader reader = new StreamReader(entry.Open());
                            TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                            foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                            {
                                bool includeSchedule = false;

                                DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTime(), now, days);
                                DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTime(), now, days);

                                if (startDate == null || endDate == null)
                                {
                                    continue;
                                }

                                if (startDate > endDate || endDate < startDate)
                                {
                                    continue;
                                }

                                string journeyPatternReference = vehicleJourney.JourneyPatternRef;

                                if (journeyPatternReference == null)
                                {
                                    continue;
                                }

                                TXCXmlJourneyPattern journeyPattern = xml.Services.Service.StandardService.JourneyPattern.Where(p => p.Id == journeyPatternReference).FirstOrDefault();

                                if (journeyPattern == null)
                                {
                                    journeyPattern = xml.Services.Service.StandardService.JourneyPattern.FirstOrDefault();
                                }

                                TXCXmlOperatingProfile operatingProfile = vehicleJourney.OperatingProfile;

                                if (operatingProfile == null)
                                {
                                    operatingProfile = xml.Services.Service.OperatingProfile;
                                }

                                TXCCalendar calendar = CalendarUtils.Build(operatingProfile, startDate.Value, endDate.Value);

                                if (operatingProfile.BankHolidayOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.BankHolidayOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.BankHolidayOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.LateSummerBankHolidayNotScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (!calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfNonOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.LateSummerBankHolidayNotScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (operatingProfile.SpecialDaysOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.SpecialDaysOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.SpecialDaysOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (!calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }
                                }

                                calendar.RunningDates = calendar.RunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementRunningDates = calendar.SupplementRunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementNonRunningDates = calendar.SupplementNonRunningDates.Distinct().OrderBy(d => d).ToList();

                                TXCSchedule schedule = ScheduleUtils.Build(xml.Operators.Operator, xml.Services.Service, journeyPattern, calendar);
                                TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpan();
                                TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpan();

                                TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                for (int i = 1; i <= patternTimings?.Count; i++)
                                {
                                    TXCStop stop = new TXCStop();

                                    if (i == 1)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else if (i == patternTimings.Count)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].To.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }

                                    if (stops.ContainsKey(stop.ATCOCode))
                                    {
                                        stop.NaptanStop = stops[stop.ATCOCode];
                                    }
                                    else
                                    {
                                        stop.NaptanStop = new NAPTANStop() { ATCOCode = stop.ATCOCode, CommonName = "Unknown NaPTAN Stop", StopType = "ZZZ" };
                                    }

                                    schedule.Stops.Add(stop);

                                    arrivalTime = arrivalTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));
                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));

                                    if (mode != "all")
                                    {
                                        if (mode == "bus")
                                        {
                                            if (stop.NaptanStop.StopType == "BCT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "city-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "RLY")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "ferry")
                                        {
                                            if (stop.NaptanStop.StopType == "FBT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "light-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "PLT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string filter in filters)
                                        {
                                            if (filter != "all")
                                            {
                                                if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                {
                                                    includeSchedule = true;
                                                }
                                            }
                                            else
                                            {
                                                includeSchedule = true;
                                            }
                                        }
                                    }
                                }

                                if (includeSchedule)
                                {
                                    dictionary.Add(schedule.Id, schedule);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                DateTime now = DateTime.Now.Date;

                if (Directory.Exists(path))
                {
                    string[] entries = Directory.GetFiles(path);

                    foreach (string entry in entries)
                    {
                        if (entry.EndsWith(".xml"))
                        {
                            using StreamReader reader = new StreamReader(entry);
                            TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                            foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                            {
                                bool includeSchedule = false;

                                DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTime(), now, days);
                                DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTime(), now, days);

                                if (startDate == null || endDate == null)
                                {
                                    continue;
                                }

                                if (startDate > endDate || endDate < startDate)
                                {
                                    continue;
                                }

                                string journeyPatternReference = vehicleJourney.JourneyPatternRef;

                                if (journeyPatternReference == null)
                                {
                                    continue;
                                }

                                TXCXmlJourneyPattern journeyPattern = xml.Services.Service.StandardService.JourneyPattern.Where(p => p.Id == journeyPatternReference).FirstOrDefault();

                                if (journeyPattern == null)
                                {
                                    journeyPattern = xml.Services.Service.StandardService.JourneyPattern.FirstOrDefault();
                                }

                                TXCXmlOperatingProfile operatingProfile = vehicleJourney.OperatingProfile;

                                if (operatingProfile == null)
                                {
                                    operatingProfile = xml.Services.Service.OperatingProfile;
                                }

                                TXCCalendar calendar = CalendarUtils.Build(operatingProfile, startDate.Value, endDate.Value);

                                if (operatingProfile.BankHolidayOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.BankHolidayOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.BankHolidayOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.LateSummerBankHolidayNotScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (!calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        List<PublicHoliday> publicHolidays = new List<PublicHoliday>();

                                        if (daysOfNonOperation.AllBankHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.AllHolidaysExceptChristmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Christmas != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.DisplacementHolidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EarlyRunOff != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.HolidayMondays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.Holidays != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.GoodFriday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetGoodFriday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.EasterMonday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetEasterMonday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.MayDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetMayDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.SpringBank != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetSpringBank(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.LateSummerBankHolidayNotScotland != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetLateSummerBankHolidayNotScotland(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.ChristmasDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetChristmasDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDay != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDay(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.BoxingDayHoliday != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetBoxingDayHoliday(calendar.StartDate, calendar.EndDate));
                                        }

                                        if (daysOfNonOperation.NewYearsEve != null)
                                        {
                                            publicHolidays.AddRange(BankHolidayUtils.GetNewYearsEve(calendar.StartDate, calendar.EndDate));
                                        }

                                        for (int i = 1; i <= publicHolidays.Count; i++)
                                        {
                                            DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i - 1].Date, now, days);

                                            if (holidayDate != null)
                                            {
                                                if (calendar.RunningDates.Contains(holidayDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(holidayDate.Value);
                                                }
                                            }
                                        }
                                    }
                                }

                                if (operatingProfile.SpecialDaysOperation != null)
                                {
                                    TXCXmlDaysOfOperation daysOfOperation = operatingProfile.SpecialDaysOperation.DaysOfOperation;
                                    TXCXmlDaysOfNonOperation daysOfNonOperation = operatingProfile.SpecialDaysOperation.DaysOfNonOperation;

                                    if (daysOfOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (!calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }

                                    if (daysOfNonOperation != null)
                                    {
                                        startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTime(), now, days);
                                        endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTime(), now, days);

                                        if (startDate != null && endDate != null)
                                        {
                                            while (startDate.Value <= endDate.Value)
                                            {
                                                if (calendar.RunningDates.Contains(startDate.Value))
                                                {
                                                    calendar.SupplementNonRunningDates.Add(startDate.Value);
                                                }

                                                startDate = startDate.Value.AddDays(1);
                                            }
                                        }
                                    }
                                }

                                calendar.RunningDates = calendar.RunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementRunningDates = calendar.SupplementRunningDates.Distinct().OrderBy(d => d).ToList();
                                calendar.SupplementNonRunningDates = calendar.SupplementNonRunningDates.Distinct().OrderBy(d => d).ToList();

                                TXCSchedule schedule = ScheduleUtils.Build(xml.Operators.Operator, xml.Services.Service, journeyPattern, calendar);
                                TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpan();
                                TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpan();

                                TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                for (int i = 1; i <= patternTimings?.Count; i++)
                                {
                                    TXCStop stop = new TXCStop();

                                    if (i == 1)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else if (i == patternTimings.Count)
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].To.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }
                                    else
                                    {
                                        stop.ATCOCode = patternTimings[i - 1].From.StopPointRef;
                                        stop.ArrivalTime = arrivalTime.Value;
                                        stop.DepartureTime = departureTime.Value;
                                    }

                                    if (stops.ContainsKey(stop.ATCOCode))
                                    {
                                        stop.NaptanStop = stops[stop.ATCOCode];
                                    }
                                    else
                                    {
                                        stop.NaptanStop = new NAPTANStop() { ATCOCode = stop.ATCOCode, CommonName = "Unknown NaPTAN Stop", StopType = "ZZZ" };
                                    }

                                    schedule.Stops.Add(stop);

                                    arrivalTime = arrivalTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));
                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime));

                                    if (mode != "all")
                                    {
                                        if (mode == "bus")
                                        {
                                            if (stop.NaptanStop.StopType == "BCT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "city-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "RLY")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "ferry")
                                        {
                                            if (stop.NaptanStop.StopType == "FBT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                        else if (mode == "light-rail")
                                        {
                                            if (stop.NaptanStop.StopType == "PLT")
                                            {
                                                foreach (string filter in filters)
                                                {
                                                    if (filter != "all")
                                                    {
                                                        if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                        {
                                                            includeSchedule = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        includeSchedule = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (string filter in filters)
                                        {
                                            if (filter != "all")
                                            {
                                                if (stop.NaptanStop.ATCOCode.Contains(filter))
                                                {
                                                    includeSchedule = true;
                                                }
                                            }
                                            else
                                            {
                                                includeSchedule = true;
                                            }
                                        }
                                    }
                                }

                                if (includeSchedule)
                                {
                                    dictionary.Add(schedule.Id, schedule);
                                }
                            }
                        }
                    }
                }
            }

            return dictionary;
        }

        public Dictionary<string, TXCSchedule> ScanDuplicate(Dictionary<string, TXCSchedule> originals)
        {
            Dictionary<string, TXCSchedule> dictionary = new Dictionary<string, TXCSchedule>();

            foreach (TXCSchedule schedule in originals.Values)
            {
                if (!dictionary.ContainsKey(schedule.Id))
                {
                    for (int i = 1; i <= schedule.Calendar.RunningDates.Count; i++)
                    {
                        IEnumerable<TXCSchedule> duplicates = originals.Values.Where(s => s.Calendar.RunningDates.Contains(schedule.Calendar.RunningDates[i - 1]) && s.Id != schedule.Id);

                        foreach (TXCSchedule duplicate in duplicates)
                        {
                            if (!dictionary.ContainsKey(duplicate.Id))
                            {
                                if (schedule.Stops.FirstOrDefault().ATCOCode == duplicate.Stops.FirstOrDefault().ATCOCode && schedule.Stops.FirstOrDefault().DepartureTime == duplicate.Stops.FirstOrDefault().DepartureTime)
                                {
                                    if (schedule.Stops.LastOrDefault().ATCOCode == duplicate.Stops.LastOrDefault().ATCOCode && schedule.Stops.LastOrDefault().ArrivalTime == duplicate.Stops.LastOrDefault().ArrivalTime)
                                    {
                                        if (schedule.Line == duplicate.Line)
                                        {
                                            if (!dictionary.ContainsKey(duplicate.Id))
                                            {
                                                dictionary.Add(duplicate.Id, duplicate);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 1; i <= schedule.Calendar.SupplementRunningDates.Count; i++)
                    {
                        IEnumerable<TXCSchedule> duplicates = originals.Values.Where(s => s.Calendar.SupplementRunningDates.Contains(schedule.Calendar.RunningDates[i - 1]) && s.Id != schedule.Id);

                        foreach (TXCSchedule duplicate in duplicates)
                        {
                            if (!dictionary.ContainsKey(duplicate.Id))
                            {
                                if (schedule.Stops.FirstOrDefault().ATCOCode == duplicate.Stops.FirstOrDefault().ATCOCode && schedule.Stops.FirstOrDefault().DepartureTime == duplicate.Stops.FirstOrDefault().DepartureTime)
                                {
                                    if (schedule.Stops.LastOrDefault().ATCOCode == duplicate.Stops.LastOrDefault().ATCOCode && schedule.Stops.LastOrDefault().ArrivalTime == duplicate.Stops.LastOrDefault().ArrivalTime)
                                    {
                                        if (schedule.Line == duplicate.Line)
                                        {
                                            if (!dictionary.ContainsKey(duplicate.Id))
                                            {
                                                dictionary.Add(duplicate.Id, duplicate);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    for (int i = 1; i <= schedule.Calendar.SupplementNonRunningDates.Count; i++)
                    {
                        IEnumerable<TXCSchedule> duplicates = originals.Values.Where(s => s.Calendar.SupplementNonRunningDates.Contains(schedule.Calendar.RunningDates[i - 1]) && s.Id != schedule.Id);

                        foreach (TXCSchedule duplicate in duplicates)
                        {
                            if (!dictionary.ContainsKey(duplicate.Id))
                            {
                                if (schedule.Stops.FirstOrDefault().ATCOCode == duplicate.Stops.FirstOrDefault().ATCOCode && schedule.Stops.FirstOrDefault().DepartureTime == duplicate.Stops.FirstOrDefault().DepartureTime)
                                {
                                    if (schedule.Stops.LastOrDefault().ATCOCode == duplicate.Stops.LastOrDefault().ATCOCode && schedule.Stops.LastOrDefault().ArrivalTime == duplicate.Stops.LastOrDefault().ArrivalTime)
                                    {
                                        if (schedule.Line == duplicate.Line)
                                        {
                                            if (!dictionary.ContainsKey(duplicate.Id))
                                            {
                                                dictionary.Add(duplicate.Id, duplicate);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return dictionary;
        }
    }
}