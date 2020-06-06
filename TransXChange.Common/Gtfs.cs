using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TransXChange.Common.Extensions;
using TransXChange.Common.Models;

namespace TransXChange.Common
{
    public static class Gtfs
    {
        private static Dictionary<string, GTFSAgency> _agencies = new Dictionary<string, GTFSAgency>();
        private static Dictionary<string, GTFSCalendar> _calendars = new Dictionary<string, GTFSCalendar>();
        private static Dictionary<string, GTFSCalendarDate> _calendarDates = new Dictionary<string, GTFSCalendarDate>();
        private static Dictionary<string, GTFSRoute> _routes = new Dictionary<string, GTFSRoute>();
        private static Dictionary<string, GTFSStop> _stops = new Dictionary<string, GTFSStop>();
        private static Dictionary<string, GTFSStopTime> _stopTimes = new Dictionary<string, GTFSStopTime>();
        private static Dictionary<string, GTFSTrip> _trips = new Dictionary<string, GTFSTrip>();

        public static void WriteAgency(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates, string path)
        {
            PrepareAgency(originals, duplicates);

            using StreamWriter writer = new StreamWriter(Path.Combine(path, "agency.txt"));
            using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSAgency>();
            csv.NextRecord();

            foreach (GTFSAgency agency in _agencies.Values)
            {
                csv.WriteRecord(agency);
                csv.NextRecord();
            }
        }

        public static void WriteCalendar(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates, string path)
        {
            PrepareCalendar(originals, duplicates);

            using StreamWriter writer = new StreamWriter(Path.Combine(path, "calendar.txt"));
            using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSCalendar>();
            csv.NextRecord();

            foreach (GTFSCalendar calendar in _calendars.Values)
            {
                csv.WriteRecord(calendar);
                csv.NextRecord();
            }
        }

        public static void WriteCalendarDates(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates, string path)
        {
            PrepareCalendarDates(originals, duplicates);

            using StreamWriter writer = new StreamWriter(Path.Combine(path, "calendar_dates.txt"));
            using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSCalendarDate>();
            csv.NextRecord();

            foreach (GTFSCalendarDate calendarDate in _calendarDates.Values)
            {
                csv.WriteRecord(calendarDate);
                csv.NextRecord();
            }
        }

        public static void WriteRoutes(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates, string path)
        {
            PrepareRoutes(originals, duplicates);

            using StreamWriter writer = new StreamWriter(Path.Combine(path, "routes.txt"));
            using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSRoute>();
            csv.NextRecord();

            foreach (GTFSRoute route in _routes.Values)
            {
                csv.WriteRecord(route);
                csv.NextRecord();
            }
        }

        public static void WriteStops(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates, string path)
        {
            PrepareStops(originals, duplicates);

            using StreamWriter writer = new StreamWriter(Path.Combine(path, "stops.txt"));
            using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSStop>();
            csv.NextRecord();

            foreach (GTFSStop stop in _stops.Values)
            {
                csv.WriteRecord(stop);
                csv.NextRecord();
            }
        }

        public static void WriteStopTimes(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates, string path)
        {
            PrepareStopTimes(originals, duplicates);

            using StreamWriter writer = new StreamWriter(Path.Combine(path, "stop_times.txt"));
            using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSStopTime>();
            csv.NextRecord();

            foreach (GTFSStopTime stopTime in _stopTimes.Values)
            {
                csv.WriteRecord(stopTime);
                csv.NextRecord();
            }
        }

        public static void WriteTrips(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates, string path)
        {
            PrepareTrips(originals, duplicates);

            using StreamWriter writer = new StreamWriter(Path.Combine(path, "trips.txt"));
            using CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSTrip>();
            csv.NextRecord();

            foreach (GTFSTrip trip in _trips.Values)
            {
                csv.WriteRecord(trip);
                csv.NextRecord();
            }
        }

        private static void PrepareAgency(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates)
        {
            foreach (TXCSchedule schedule in originals.Values)
            {
                GTFSAgency agency = new GTFSAgency()
                {
                    AgencyId = schedule.OperatorCode,
                    AgencyName = schedule.OperatorName,
                    AgencyUrl = string.Format("https://www.google.com/search?q={0}", schedule.OperatorName),
                    AgencyTimezone = "Europe/London",
                    AgencyLang = "EN",
                    AgencyPhone = null,
                    AgencyFareUrl = null,
                    AgencyEmail = null,
                };

                string id = agency.AgencyId;

                if (!duplicates.ContainsKey(schedule.Id) && !_agencies.ContainsKey(id))
                {
                    _agencies.Add(id, agency);
                }
            }

            _agencies = _agencies.OrderBy(a => a.Value.AgencyId).ToDictionary(a => a.Key, a => a.Value);
        }

        private static void PrepareCalendar(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates)
        {
            foreach (TXCSchedule schedule in originals.Values)
            {
                GTFSCalendar calendar = new GTFSCalendar()
                {
                    ServiceId = string.Format("{0}-{1}-{2}-{3}", schedule.ServiceCode, string.Format("{0}{1}{2}", schedule.Calendar.StartDate.ToString("yyyy"), schedule.Calendar.StartDate.ToString("MM"), schedule.Calendar.StartDate.ToString("dd")), string.Format("{0}{1}{2}", schedule.Calendar.EndDate.ToString("yyyy"), schedule.Calendar.EndDate.ToString("MM"), schedule.Calendar.EndDate.ToString("dd")), string.Format("{0}{1}{2}{3}{4}{5}{6}", schedule.Calendar.Monday.ToInt().ToString(), schedule.Calendar.Tuesday.ToInt().ToString(), schedule.Calendar.Wednesday.ToInt().ToString(), schedule.Calendar.Thursday.ToInt().ToString(), schedule.Calendar.Friday.ToInt().ToString(), schedule.Calendar.Saturday.ToInt().ToString(), schedule.Calendar.Sunday.ToInt().ToString())),
                    Monday = schedule.Calendar.Monday.ToInt().ToString(),
                    Tuesday = schedule.Calendar.Tuesday.ToInt().ToString(),
                    Wednesday = schedule.Calendar.Wednesday.ToInt().ToString(),
                    Thursday = schedule.Calendar.Thursday.ToInt().ToString(),
                    Friday = schedule.Calendar.Friday.ToInt().ToString(),
                    Saturday = schedule.Calendar.Saturday.ToInt().ToString(),
                    Sunday = schedule.Calendar.Sunday.ToInt().ToString(),
                    StartDate = string.Format("{0}{1}{2}", schedule.Calendar.StartDate.ToString("yyyy"), schedule.Calendar.StartDate.ToString("MM"), schedule.Calendar.StartDate.ToString("dd")),
                    EndDate = string.Format("{0}{1}{2}", schedule.Calendar.EndDate.ToString("yyyy"), schedule.Calendar.EndDate.ToString("MM"), schedule.Calendar.EndDate.ToString("dd"))
                };

                string id = calendar.ServiceId;

                if (!duplicates.ContainsKey(schedule.Id) && !_calendars.ContainsKey(id))
                {
                    _calendars.Add(id, calendar);
                }
            }

            _calendars = _calendars.OrderBy(c => c.Value.ServiceId).ToDictionary(c => c.Key, c => c.Value);
        }

        private static void PrepareCalendarDates(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates)
        {
            foreach (TXCSchedule schedule in originals.Values)
            {
                for (int i = 1; i <= schedule.Calendar.SupplementRunningDates.Count; i++)
                {
                    GTFSCalendarDate calendarDate = new GTFSCalendarDate()
                    {
                        ServiceId = string.Format("{0}-{1}-{2}-{3}", schedule.ServiceCode, string.Format("{0}{1}{2}", schedule.Calendar.StartDate.ToString("yyyy"), schedule.Calendar.StartDate.ToString("MM"), schedule.Calendar.StartDate.ToString("dd")), string.Format("{0}{1}{2}", schedule.Calendar.EndDate.ToString("yyyy"), schedule.Calendar.EndDate.ToString("MM"), schedule.Calendar.EndDate.ToString("dd")), string.Format("{0}{1}{2}{3}{4}{5}{6}", schedule.Calendar.Monday.ToInt().ToString(), schedule.Calendar.Tuesday.ToInt().ToString(), schedule.Calendar.Wednesday.ToInt().ToString(), schedule.Calendar.Thursday.ToInt().ToString(), schedule.Calendar.Friday.ToInt().ToString(), schedule.Calendar.Saturday.ToInt().ToString(), schedule.Calendar.Sunday.ToInt().ToString())),
                        Date = string.Format("{0}{1}{2}", schedule.Calendar.SupplementRunningDates[i - 1].ToString("yyyy"), schedule.Calendar.SupplementRunningDates[i - 1].ToString("MM"), schedule.Calendar.SupplementRunningDates[i - 1].ToString("dd")),
                        ExceptionType = "1"
                    };

                    string id = string.Format("{0}-{1}", calendarDate.ServiceId, calendarDate.Date);

                    if (!duplicates.ContainsKey(schedule.Id) && !_calendarDates.ContainsKey(id))
                    {
                        _calendarDates.Add(id, calendarDate);
                    }
                }

                for (int i = 1; i <= schedule.Calendar.SupplementNonRunningDates.Count; i++)
                {
                    GTFSCalendarDate calendarDate = new GTFSCalendarDate()
                    {
                        ServiceId = string.Format("{0}-{1}-{2}-{3}", schedule.ServiceCode, string.Format("{0}{1}{2}", schedule.Calendar.StartDate.ToString("yyyy"), schedule.Calendar.StartDate.ToString("MM"), schedule.Calendar.StartDate.ToString("dd")), string.Format("{0}{1}{2}", schedule.Calendar.EndDate.ToString("yyyy"), schedule.Calendar.EndDate.ToString("MM"), schedule.Calendar.EndDate.ToString("dd")), string.Format("{0}{1}{2}{3}{4}{5}{6}", schedule.Calendar.Monday.ToInt().ToString(), schedule.Calendar.Tuesday.ToInt().ToString(), schedule.Calendar.Wednesday.ToInt().ToString(), schedule.Calendar.Thursday.ToInt().ToString(), schedule.Calendar.Friday.ToInt().ToString(), schedule.Calendar.Saturday.ToInt().ToString(), schedule.Calendar.Sunday.ToInt().ToString())),
                        Date = string.Format("{0}{1}{2}", schedule.Calendar.SupplementNonRunningDates[i - 1].ToString("yyyy"), schedule.Calendar.SupplementNonRunningDates[i - 1].ToString("MM"), schedule.Calendar.SupplementNonRunningDates[i - 1].ToString("dd")),
                        ExceptionType = "2"
                    };

                    string id = string.Format("{0}-{1}", calendarDate.ServiceId, calendarDate.Date);

                    if (!duplicates.ContainsKey(schedule.Id) && !_calendarDates.ContainsKey(id))
                    {
                        _calendarDates.Add(id, calendarDate);
                    }
                }
            }

            _calendarDates = _calendarDates.OrderBy(c => c.Value.ServiceId).ToDictionary(c => c.Key, c => c.Value);
        }

        private static void PrepareRoutes(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates)
        {
            foreach (TXCSchedule schedule in originals.Values)
            {
                GTFSRoute route = new GTFSRoute()
                {
                    RouteId = schedule.ServiceCode,
                    AgencyId = schedule.OperatorCode,
                    RouteShortName = schedule.Line,
                    RouteLongName = schedule.Description,
                    RouteType = schedule.Mode
                };

                string id = route.RouteId;

                if (!duplicates.ContainsKey(schedule.Id) && !_routes.ContainsKey(id))
                {
                    _routes.Add(id, route);
                }
            }

            _routes = _routes.OrderBy(r => r.Value.RouteId).ToDictionary(r => r.Key, r => r.Value);
        }

        private static void PrepareStops(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates)
        {
            foreach (TXCSchedule schedule in originals.Values)
            {
                for (int i = 1; i <= schedule.Stops.Count; i++)
                {
                    GTFSStop stop = new GTFSStop()
                    {
                        StopId = schedule.Stops[i - 1].NaptanStop.ATCOCode,
                        StopCode = schedule.Stops[i - 1].NaptanStop.NaptanCode,
                        StopName = schedule.Stops[i - 1].NaptanStop.CommonName,
                        StopLat = schedule.Stops[i - 1].NaptanStop.Latitude,
                        StopLon = schedule.Stops[i - 1].NaptanStop.Longitude,
                        LocationType = "0",
                        StopTimezone = "Europe/London"
                    };

                    string id = stop.StopId;

                    if (!duplicates.ContainsKey(schedule.Id) && !_stops.ContainsKey(id))
                    {
                        _stops.Add(id, stop);
                    }
                }
            }

            _stops = _stops.OrderBy(s => s.Value.StopId).ToDictionary(s => s.Key, s => s.Value);
        }

        private static void PrepareStopTimes(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates)
        {
            foreach (TXCSchedule schedule in originals.Values)
            {
                TimeSpan timeSpan = new TimeSpan();

                for (int i = 1; i <= schedule.Stops.Count; i++)
                {
                    GTFSStopTime stopTime = new GTFSStopTime()
                    {
                        TripId = schedule.Id,
                        StopId = schedule.Stops[i - 1].NaptanStop.ATCOCode,
                        StopSequence = i.ToString()
                    };

                    if (schedule.Stops[i - 1].DepartureTime < timeSpan)
                    {
                        schedule.Stops[i - 1].ArrivalTime = schedule.Stops[i - 1].ArrivalTime.Add(new TimeSpan(24, 0, 0));
                        schedule.Stops[i - 1].DepartureTime = schedule.Stops[i - 1].DepartureTime.Add(new TimeSpan(24, 0, 0));

                        stopTime.ArrivalTime = Math.Round(schedule.Stops[i - 1].ArrivalTime.TotalHours, 0).ToString() + schedule.Stops[i - 1].ArrivalTime.ToString(@"hh\:mm\:ss").Substring(2, 6);
                        stopTime.DepartureTime = Math.Round(schedule.Stops[i - 1].DepartureTime.TotalHours, 0).ToString() + schedule.Stops[i - 1].DepartureTime.ToString(@"hh\:mm\:ss").Substring(2, 6);

                        timeSpan = schedule.Stops[i - 1].DepartureTime;
                    }
                    else
                    {
                        stopTime.ArrivalTime = schedule.Stops[i - 1].ArrivalTime.ToString(@"hh\:mm\:ss");
                        stopTime.DepartureTime = schedule.Stops[i - 1].DepartureTime.ToString(@"hh\:mm\:ss");

                        timeSpan = schedule.Stops[i - 1].DepartureTime;
                    }

                    if (i == 1)
                    {
                        stopTime.PickupType = "0";
                        stopTime.DropOffType = "1";
                    }
                    else if (i == schedule.Stops.Count)
                    {
                        stopTime.PickupType = "1";
                        stopTime.DropOffType = "0";
                    }
                    else
                    {
                        stopTime.PickupType = "0";
                        stopTime.DropOffType = "0";
                    }

                    string id = Guid.NewGuid().ToString();

                    if (!duplicates.ContainsKey(schedule.Id))
                    {
                        _stopTimes.Add(id, stopTime);
                    }
                }
            }

            _stopTimes = _stopTimes.OrderBy(s => s.Value.TripId).ToDictionary(s => s.Key, s => s.Value);
        }

        private static void PrepareTrips(Dictionary<string, TXCSchedule> originals, Dictionary<string, TXCSchedule> duplicates)
        {
            foreach (TXCSchedule schedule in originals.Values)
            {
                GTFSTrip trip = new GTFSTrip()
                {
                    RouteId = schedule.ServiceCode,
                    ServiceId = string.Format("{0}-{1}-{2}-{3}", schedule.ServiceCode, string.Format("{0}{1}{2}", schedule.Calendar.StartDate.ToString("yyyy"), schedule.Calendar.StartDate.ToString("MM"), schedule.Calendar.StartDate.ToString("dd")), string.Format("{0}{1}{2}", schedule.Calendar.EndDate.ToString("yyyy"), schedule.Calendar.EndDate.ToString("MM"), schedule.Calendar.EndDate.ToString("dd")), string.Format("{0}{1}{2}{3}{4}{5}{6}", schedule.Calendar.Monday.ToInt().ToString(), schedule.Calendar.Tuesday.ToInt().ToString(), schedule.Calendar.Wednesday.ToInt().ToString(), schedule.Calendar.Thursday.ToInt().ToString(), schedule.Calendar.Friday.ToInt().ToString(), schedule.Calendar.Saturday.ToInt().ToString(), schedule.Calendar.Sunday.ToInt().ToString())),
                    TripId = schedule.Id,
                    TripHeadsign = schedule.Stops.LastOrDefault().NaptanStop.CommonName,
                    DirectionId = schedule.Direction
                };

                string id = Guid.NewGuid().ToString();

                if (!duplicates.ContainsKey(schedule.Id))
                {
                    _trips.Add(id, trip);
                }
            }

            _trips = _trips.OrderBy(t => t.Value.TripId).ToDictionary(t => t.Key, t => t.Value);
        }
    }
}