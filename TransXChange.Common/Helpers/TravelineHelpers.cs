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
        public Dictionary<string, TXCSchedule> ReadEngland(Dictionary<string, NAPTANStop> stops, string path, string key, string mode, IEnumerable<string> indexes, IEnumerable<string> filters, string date, int days)
        {
            Dictionary<string, TXCSchedule> dictionary = new Dictionary<string, TXCSchedule>();

            if (path.EndsWith(".zip"))
            {
                DateTime scheduleDate = DateTimeUtils.GetScheduleDate(DateTime.Now.ToZonedDateTime("Europe/London").Date, date);

                if (File.Exists(path))
                {
                    using ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read);

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (!indexes.Contains("all"))
                        {
                            foreach (string index in indexes)
                            {
                                if (entry.Name.StartsWith(index) && entry.Name.EndsWith(".xml"))
                                {
                                    using StreamReader reader = new StreamReader(entry.Open());
                                    TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                    if (xml.VehicleJourneys == null)
                                    {
                                        continue;
                                    }

                                    foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                    {
                                        bool includeSchedule = false;

                                        DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                        DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationEngland(daysOfOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationEngland(daysOfNonOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                        TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                        TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                        List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                        TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                        List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                        for (int i = 0; i < patternTimings?.Count; i++)
                                        {
                                            TXCStop stop = new TXCStop();

                                            if (i == 0)
                                            {
                                                string activity = "pickUp";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                                departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i > 0)
                                            {
                                                string activity = "pickUpAndSetDown";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i - 1].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i == patternTimings.Count - 1)
                                            {
                                                string activity = "setDown";

                                                if (patternTimings[i].To.Activity != null)
                                                {
                                                    activity = patternTimings[i].To.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                        }

                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                        if (includeSchedule)
                                        {
                                            dictionary.Add(schedule.Id, schedule);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (entry.Name.EndsWith(".xml"))
                            {
                                using StreamReader reader = new StreamReader(entry.Open());
                                TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                if (xml.VehicleJourneys == null)
                                {
                                    continue;
                                }

                                foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                {
                                    bool includeSchedule = false;

                                    DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                    DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationEngland(daysOfOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationEngland(daysOfNonOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                    TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                    TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                    List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                    TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                    List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                    for (int i = 0; i < patternTimings?.Count; i++)
                                    {
                                        TXCStop stop = new TXCStop();

                                        if (i == 0)
                                        {
                                            string activity = "pickUp";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                            departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i > 0)
                                        {
                                            string activity = "pickUpAndSetDown";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i - 1].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i == patternTimings.Count - 1)
                                        {
                                            string activity = "setDown";

                                            if (patternTimings[i].To.Activity != null)
                                            {
                                                activity = patternTimings[i].To.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                    }

                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                    if (includeSchedule)
                                    {
                                        dictionary.Add(schedule.Id, schedule);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                DateTime scheduleDate = DateTimeUtils.GetScheduleDate(DateTime.Now.ToZonedDateTime("Europe/London").Date, date);

                if (Directory.Exists(path))
                {
                    string[] entries = Directory.GetFiles(path);

                    foreach (string entry in entries)
                    {
                        if (!indexes.Contains("all"))
                        {
                            foreach (string index in indexes)
                            {
                                if (entry.StartsWith(index) && entry.EndsWith(".xml"))
                                {
                                    using StreamReader reader = new StreamReader(entry);
                                    TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                    if (xml.VehicleJourneys == null)
                                    {
                                        continue;
                                    }

                                    foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                    {
                                        bool includeSchedule = false;

                                        DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                        DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationEngland(daysOfOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationEngland(daysOfNonOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                        TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                        TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                        List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                        TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                        List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                        for (int i = 0; i < patternTimings?.Count; i++)
                                        {
                                            TXCStop stop = new TXCStop();

                                            if (i == 0)
                                            {
                                                string activity = "pickUp";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                                departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i > 0)
                                            {
                                                string activity = "pickUpAndSetDown";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i - 1].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i == patternTimings.Count - 1)
                                            {
                                                string activity = "setDown";

                                                if (patternTimings[i].To.Activity != null)
                                                {
                                                    activity = patternTimings[i].To.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                        }

                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                        if (includeSchedule)
                                        {
                                            dictionary.Add(schedule.Id, schedule);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (entry.EndsWith(".xml"))
                            {
                                using StreamReader reader = new StreamReader(entry);
                                TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                if (xml.VehicleJourneys == null)
                                {
                                    continue;
                                }

                                foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                {
                                    bool includeSchedule = false;

                                    DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                    DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationEngland(daysOfOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationEngland(daysOfNonOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                    TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                    TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                    List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                    TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                    List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                    for (int i = 0; i < patternTimings?.Count; i++)
                                    {
                                        TXCStop stop = new TXCStop();

                                        if (i == 0)
                                        {
                                            string activity = "pickUp";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                            departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i > 0)
                                        {
                                            string activity = "pickUpAndSetDown";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i - 1].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i == patternTimings.Count - 1)
                                        {
                                            string activity = "setDown";

                                            if (patternTimings[i].To.Activity != null)
                                            {
                                                activity = patternTimings[i].To.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                    }

                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                    if (includeSchedule)
                                    {
                                        dictionary.Add(schedule.Id, schedule);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return dictionary;
        }

        public Dictionary<string, TXCSchedule> ReadScotland(Dictionary<string, NAPTANStop> stops, string path, string key, string mode, IEnumerable<string> indexes, IEnumerable<string> filters, string date, int days)
        {
            Dictionary<string, TXCSchedule> dictionary = new Dictionary<string, TXCSchedule>();

            if (path.EndsWith(".zip"))
            {
                DateTime scheduleDate = DateTimeUtils.GetScheduleDate(DateTime.Now.ToZonedDateTime("Europe/London").Date, date);

                if (File.Exists(path))
                {
                    using ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read);

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (!indexes.Contains("all"))
                        {
                            foreach (string index in indexes)
                            {
                                if (entry.Name.StartsWith(index) && entry.Name.EndsWith(".xml"))
                                {
                                    using StreamReader reader = new StreamReader(entry.Open());
                                    TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                    if (xml.VehicleJourneys == null)
                                    {
                                        continue;
                                    }

                                    foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                    {
                                        bool includeSchedule = false;

                                        DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                        DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationScotland(daysOfOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationScotland(daysOfNonOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                        TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                        TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                        List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                        TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                        List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                        for (int i = 0; i < patternTimings?.Count; i++)
                                        {
                                            TXCStop stop = new TXCStop();

                                            if (i == 0)
                                            {
                                                string activity = "pickUp";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                                departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i > 0)
                                            {
                                                string activity = "pickUpAndSetDown";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i - 1].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i == patternTimings.Count - 1)
                                            {
                                                string activity = "setDown";

                                                if (patternTimings[i].To.Activity != null)
                                                {
                                                    activity = patternTimings[i].To.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                        }

                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                        if (includeSchedule)
                                        {
                                            dictionary.Add(schedule.Id, schedule);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (entry.Name.EndsWith(".xml"))
                            {
                                using StreamReader reader = new StreamReader(entry.Open());
                                TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                if (xml.VehicleJourneys == null)
                                {
                                    continue;
                                }

                                foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                {
                                    bool includeSchedule = false;

                                    DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                    DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationScotland(daysOfOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationScotland(daysOfNonOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                    TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                    TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                    List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                    TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                    List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                    for (int i = 0; i < patternTimings?.Count; i++)
                                    {
                                        TXCStop stop = new TXCStop();

                                        if (i == 0)
                                        {
                                            string activity = "pickUp";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                            departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i > 0)
                                        {
                                            string activity = "pickUpAndSetDown";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i - 1].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i == patternTimings.Count - 1)
                                        {
                                            string activity = "setDown";

                                            if (patternTimings[i].To.Activity != null)
                                            {
                                                activity = patternTimings[i].To.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                    }

                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                    if (includeSchedule)
                                    {
                                        dictionary.Add(schedule.Id, schedule);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                DateTime scheduleDate = DateTimeUtils.GetScheduleDate(DateTime.Now.ToZonedDateTime("Europe/London").Date, date);

                if (Directory.Exists(path))
                {
                    string[] entries = Directory.GetFiles(path);

                    foreach (string entry in entries)
                    {
                        if (!indexes.Contains("all"))
                        {
                            foreach (string index in indexes)
                            {
                                if (entry.StartsWith(index) && entry.EndsWith(".xml"))
                                {
                                    using StreamReader reader = new StreamReader(entry);
                                    TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                    if (xml.VehicleJourneys == null)
                                    {
                                        continue;
                                    }

                                    foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                    {
                                        bool includeSchedule = false;

                                        DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                        DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationScotland(daysOfOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationScotland(daysOfNonOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                        TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                        TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                        List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                        TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                        List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                        for (int i = 0; i < patternTimings?.Count; i++)
                                        {
                                            TXCStop stop = new TXCStop();

                                            if (i == 0)
                                            {
                                                string activity = "pickUp";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                                departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i > 0)
                                            {
                                                string activity = "pickUpAndSetDown";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i - 1].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i == patternTimings.Count - 1)
                                            {
                                                string activity = "setDown";

                                                if (patternTimings[i].To.Activity != null)
                                                {
                                                    activity = patternTimings[i].To.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                        }

                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                        if (includeSchedule)
                                        {
                                            dictionary.Add(schedule.Id, schedule);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (entry.EndsWith(".xml"))
                            {
                                using StreamReader reader = new StreamReader(entry);
                                TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                if (xml.VehicleJourneys == null)
                                {
                                    continue;
                                }

                                foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                {
                                    bool includeSchedule = false;

                                    DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                    DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationScotland(daysOfOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationScotland(daysOfNonOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                    TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                    TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                    List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                    TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                    List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                    for (int i = 0; i < patternTimings?.Count; i++)
                                    {
                                        TXCStop stop = new TXCStop();

                                        if (i == 0)
                                        {
                                            string activity = "pickUp";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                            departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i > 0)
                                        {
                                            string activity = "pickUpAndSetDown";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i - 1].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i == patternTimings.Count - 1)
                                        {
                                            string activity = "setDown";

                                            if (patternTimings[i].To.Activity != null)
                                            {
                                                activity = patternTimings[i].To.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                    }

                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                    if (includeSchedule)
                                    {
                                        dictionary.Add(schedule.Id, schedule);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return dictionary;
        }

        public Dictionary<string, TXCSchedule> ReadWales(Dictionary<string, NAPTANStop> stops, string path, string key, string mode, IEnumerable<string> indexes, IEnumerable<string> filters, string date, int days)
        {
            Dictionary<string, TXCSchedule> dictionary = new Dictionary<string, TXCSchedule>();

            if (path.EndsWith(".zip"))
            {
                DateTime scheduleDate = DateTimeUtils.GetScheduleDate(DateTime.Now.ToZonedDateTime("Europe/London").Date, date);

                if (File.Exists(path))
                {
                    using ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read);

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (!indexes.Contains("all"))
                        {
                            foreach (string index in indexes)
                            {
                                if (entry.Name.StartsWith(index) && entry.Name.EndsWith(".xml"))
                                {
                                    using StreamReader reader = new StreamReader(entry.Open());
                                    TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                    if (xml.VehicleJourneys == null)
                                    {
                                        continue;
                                    }

                                    foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                    {
                                        bool includeSchedule = false;

                                        DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                        DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationWales(daysOfOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationWales(daysOfNonOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                        TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                        TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                        List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                        TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                        List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                        for (int i = 0; i < patternTimings?.Count; i++)
                                        {
                                            TXCStop stop = new TXCStop();

                                            if (i == 0)
                                            {
                                                string activity = "pickUp";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                                departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i > 0)
                                            {
                                                string activity = "pickUpAndSetDown";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i - 1].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i == patternTimings.Count - 1)
                                            {
                                                string activity = "setDown";

                                                if (patternTimings[i].To.Activity != null)
                                                {
                                                    activity = patternTimings[i].To.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                        }

                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                        if (includeSchedule)
                                        {
                                            dictionary.Add(schedule.Id, schedule);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (entry.Name.EndsWith(".xml"))
                            {
                                using StreamReader reader = new StreamReader(entry.Open());
                                TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                if (xml.VehicleJourneys == null)
                                {
                                    continue;
                                }

                                foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                {
                                    bool includeSchedule = false;

                                    DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                    DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationWales(daysOfOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationWales(daysOfNonOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                    TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                    TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                    List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                    TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                    List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                    for (int i = 0; i < patternTimings?.Count; i++)
                                    {
                                        TXCStop stop = new TXCStop();

                                        if (i == 0)
                                        {
                                            string activity = "pickUp";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                            departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i > 0)
                                        {
                                            string activity = "pickUpAndSetDown";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i - 1].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i == patternTimings.Count - 1)
                                        {
                                            string activity = "setDown";

                                            if (patternTimings[i].To.Activity != null)
                                            {
                                                activity = patternTimings[i].To.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                    }

                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                    if (includeSchedule)
                                    {
                                        dictionary.Add(schedule.Id, schedule);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                DateTime scheduleDate = DateTimeUtils.GetScheduleDate(DateTime.Now.ToZonedDateTime("Europe/London").Date, date);

                if (Directory.Exists(path))
                {
                    string[] entries = Directory.GetFiles(path);

                    foreach (string entry in entries)
                    {
                        if (!indexes.Contains("all"))
                        {
                            foreach (string index in indexes)
                            {
                                if (entry.StartsWith(index) && entry.EndsWith(".xml"))
                                {
                                    using StreamReader reader = new StreamReader(entry);
                                    TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                    if (xml.VehicleJourneys == null)
                                    {
                                        continue;
                                    }

                                    foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                    {
                                        bool includeSchedule = false;

                                        DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                        DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationWales(daysOfOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationWales(daysOfNonOperation, calendar, key);

                                                for (int i = 0; i < publicHolidays.Count; i++)
                                                {
                                                    DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                                startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                                endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                        TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                        TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                        List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                        TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                        List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                        for (int i = 0; i < patternTimings?.Count; i++)
                                        {
                                            TXCStop stop = new TXCStop();

                                            if (i == 0)
                                            {
                                                string activity = "pickUp";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                                departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i > 0)
                                            {
                                                string activity = "pickUpAndSetDown";

                                                if (patternTimings[i].From.Activity != null)
                                                {
                                                    activity = patternTimings[i].From.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i - 1].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                if (patternTimings[i].From.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            if (i == patternTimings.Count - 1)
                                            {
                                                string activity = "setDown";

                                                if (patternTimings[i].To.Activity != null)
                                                {
                                                    activity = patternTimings[i].To.Activity;
                                                }

                                                arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                                departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                                if (patternTimings[i].To.WaitTime != null)
                                                {
                                                    if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                    {
                                                        departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                    }
                                                    else
                                                    {
                                                        departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                    }
                                                }

                                                stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                                stop.Activity = activity;
                                                stop.ArrivalTime = arrivalTime.Value;
                                                stop.DepartureTime = departureTime.Value;

                                                schedule.Stops.Add(stop);
                                            }

                                            includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                        }

                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                        includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                        if (includeSchedule)
                                        {
                                            dictionary.Add(schedule.Id, schedule);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (entry.EndsWith(".xml"))
                            {
                                using StreamReader reader = new StreamReader(entry);
                                TXCXmlTransXChange xml = new XmlSerializer(typeof(TXCXmlTransXChange)).Deserialize(reader) as TXCXmlTransXChange;

                                if (xml.VehicleJourneys == null)
                                {
                                    continue;
                                }

                                foreach (TXCXmlVehicleJourney vehicleJourney in xml.VehicleJourneys.VehicleJourney)
                                {
                                    bool includeSchedule = false;

                                    DateTime? startDate = DateTimeUtils.GetStartDate(xml.Services.Service.OperatingPeriod.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                    DateTime? endDate = DateTimeUtils.GetEndDate(xml.Services.Service.OperatingPeriod.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfOperationWales(daysOfOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            List<PublicHoliday> publicHolidays = BankHolidayUtils.GetDaysOfNonOperationWales(daysOfNonOperation, calendar, key);

                                            for (int i = 0; i < publicHolidays.Count; i++)
                                            {
                                                DateTime? holidayDate = DateTimeUtils.GetHolidayDate(publicHolidays[i].Date, scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                            startDate = DateTimeUtils.GetStartDate(daysOfNonOperation.DateRange.StartDate.ToDateTimeFromTraveline(), scheduleDate, days);
                                            endDate = DateTimeUtils.GetEndDate(daysOfNonOperation.DateRange.EndDate.ToDateTimeFromTraveline(), scheduleDate, days);

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
                                    TimeSpan? arrivalTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();
                                    TimeSpan? departureTime = vehicleJourney.DepartureTime.ToTimeSpanFromTraveline();

                                    List<TXCXmlAnnotatedStopPointRef> stopPoints = xml.StopPoints.AnnotatedStopPointRef;

                                    TXCXmlJourneyPatternSection patternSection = xml.JourneyPatternSections?.JourneyPatternSection.Where(s => s.Id == journeyPattern.JourneyPatternSectionRefs).FirstOrDefault();
                                    List<TXCXmlJourneyPatternTimingLink> patternTimings = patternSection?.JourneyPatternTimingLink;

                                    for (int i = 0; i < patternTimings?.Count; i++)
                                    {
                                        TXCStop stop = new TXCStop();

                                        if (i == 0)
                                        {
                                            string activity = "pickUp";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(TimeSpan.Zero);
                                            departureTime = departureTime.Value.Add(TimeSpan.Zero);

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i > 0)
                                        {
                                            string activity = "pickUpAndSetDown";

                                            if (patternTimings[i].From.Activity != null)
                                            {
                                                activity = patternTimings[i].From.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i - 1].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i - 1].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i - 1].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            if (patternTimings[i].From.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].From.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].From.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].From.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        if (i == patternTimings.Count - 1)
                                        {
                                            string activity = "setDown";

                                            if (patternTimings[i].To.Activity != null)
                                            {
                                                activity = patternTimings[i].To.Activity;
                                            }

                                            arrivalTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));
                                            departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].RunTime) > TimeSpan.Zero ? XmlConvert.ToTimeSpan(patternTimings[i].RunTime) : TimeSpan.FromMinutes(1));

                                            if (patternTimings[i].To.WaitTime != null)
                                            {
                                                if (XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime) > TimeSpan.Zero)
                                                {
                                                    departureTime = departureTime.Value.Add(XmlConvert.ToTimeSpan(patternTimings[i].To.WaitTime));
                                                }
                                                else
                                                {
                                                    departureTime = departureTime.Value.Add(TimeSpan.FromMinutes(1));
                                                }
                                            }

                                            stop = StopUtils.Build(stops, stopPoints.Where(s => s.StopPointRef == patternTimings[i].To.StopPointRef).FirstOrDefault() ?? new TXCXmlAnnotatedStopPointRef() { StopPointRef = patternTimings[i].To.StopPointRef });
                                            stop.Activity = activity;
                                            stop.ArrivalTime = arrivalTime.Value;
                                            stop.DepartureTime = departureTime.Value;

                                            schedule.Stops.Add(stop);
                                        }

                                        includeSchedule = StopUtils.GetFilter(includeSchedule, mode, filters, stop);
                                    }

                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.RunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.RunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);
                                    includeSchedule = ScheduleUtils.GetDuplicate(includeSchedule, dictionary.Values.Where(s => s.Calendar.SupplementNonRunningDates.Intersect(schedule.Calendar.SupplementNonRunningDates).Any() && s.Id != schedule.Id), schedule);

                                    if (includeSchedule)
                                    {
                                        dictionary.Add(schedule.Id, schedule);
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