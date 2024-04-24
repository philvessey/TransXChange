using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TransXChange.Common.Extensions;
using TransXChange.Common.Models;

namespace TransXChange.Common.Helpers
{
    public class GtfsHelpers
    {
        private Dictionary<string, GTFSAgency> _agencies = [];
        private Dictionary<string, GTFSCalendar> _calendars = [];
        private Dictionary<string, GTFSCalendarDate> _calendarDates = [];
        private Dictionary<string, GTFSRoute> _routes = [];
        private Dictionary<string, GTFSStop> _stops = [];
        private Dictionary<string, GTFSStopTime> _stopTimes = [];
        private Dictionary<string, GTFSTrip> _trips = [];

        public void WriteAgency(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareAgency(schedules);

            using StreamWriter writer = new(Path.Combine(path, "agency.txt"));
            using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSAgency>();
            csv.NextRecord();

            foreach (GTFSAgency agency in _agencies.Values)
            {
                csv.WriteRecord(agency);
                csv.NextRecord();
            }
        }

        public void WriteCalendar(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareCalendar(schedules);

            using StreamWriter writer = new(Path.Combine(path, "calendar.txt"));
            using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSCalendar>();
            csv.NextRecord();

            foreach (GTFSCalendar calendar in _calendars.Values)
            {
                csv.WriteRecord(calendar);
                csv.NextRecord();
            }
        }

        public void WriteCalendarDates(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareCalendarDates(schedules);

            using StreamWriter writer = new(Path.Combine(path, "calendar_dates.txt"));
            using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSCalendarDate>();
            csv.NextRecord();

            foreach (GTFSCalendarDate calendarDate in _calendarDates.Values)
            {
                csv.WriteRecord(calendarDate);
                csv.NextRecord();
            }
        }

        public void WriteRoutes(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareRoutes(schedules);

            using StreamWriter writer = new(Path.Combine(path, "routes.txt"));
            using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSRoute>();
            csv.NextRecord();

            foreach (GTFSRoute route in _routes.Values)
            {
                csv.WriteRecord(route);
                csv.NextRecord();
            }
        }

        public void WriteStops(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareStops(schedules);

            using StreamWriter writer = new(Path.Combine(path, "stops.txt"));
            using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSStop>();
            csv.NextRecord();

            foreach (GTFSStop stop in _stops.Values)
            {
                csv.WriteRecord(stop);
                csv.NextRecord();
            }
        }

        public void WriteStopTimes(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareStopTimes(schedules);

            using StreamWriter writer = new(Path.Combine(path, "stop_times.txt"));
            using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSStopTime>();
            csv.NextRecord();

            foreach (GTFSStopTime stopTime in _stopTimes.Values)
            {
                csv.WriteRecord(stopTime);
                csv.NextRecord();
            }
        }

        public void WriteTrips(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareTrips(schedules);

            using StreamWriter writer = new(Path.Combine(path, "trips.txt"));
            using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<GTFSTrip>();
            csv.NextRecord();

            foreach (GTFSTrip trip in _trips.Values)
            {
                csv.WriteRecord(trip);
                csv.NextRecord();
            }
        }

        private void PrepareAgency(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
            {
                GTFSAgency agency = new()
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

                _agencies.TryAdd(agency.AgencyId, agency);
            }

            _agencies = _agencies.OrderBy(a => a.Value.AgencyId).ToDictionary(a => a.Key, a => a.Value);
        }

        private void PrepareCalendar(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
            {
                GTFSCalendar calendar = new()
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

                _calendars.TryAdd(calendar.ServiceId, calendar);
            }

            _calendars = _calendars.OrderBy(c => c.Value.ServiceId).ToDictionary(c => c.Key, c => c.Value);
        }

        private void PrepareCalendarDates(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
            {
                for (int i = 0; i < schedule.Calendar.SupplementRunningDates.Count; i++)
                {
                    GTFSCalendarDate calendarDate = new()
                    {
                        ServiceId = string.Format("{0}-{1}-{2}-{3}", schedule.ServiceCode, string.Format("{0}{1}{2}", schedule.Calendar.StartDate.ToString("yyyy"), schedule.Calendar.StartDate.ToString("MM"), schedule.Calendar.StartDate.ToString("dd")), string.Format("{0}{1}{2}", schedule.Calendar.EndDate.ToString("yyyy"), schedule.Calendar.EndDate.ToString("MM"), schedule.Calendar.EndDate.ToString("dd")), string.Format("{0}{1}{2}{3}{4}{5}{6}", schedule.Calendar.Monday.ToInt().ToString(), schedule.Calendar.Tuesday.ToInt().ToString(), schedule.Calendar.Wednesday.ToInt().ToString(), schedule.Calendar.Thursday.ToInt().ToString(), schedule.Calendar.Friday.ToInt().ToString(), schedule.Calendar.Saturday.ToInt().ToString(), schedule.Calendar.Sunday.ToInt().ToString())),
                        Date = string.Format("{0}{1}{2}", schedule.Calendar.SupplementRunningDates[i].ToString("yyyy"), schedule.Calendar.SupplementRunningDates[i].ToString("MM"), schedule.Calendar.SupplementRunningDates[i].ToString("dd")),
                        ExceptionType = "1"
                    };

                    _calendarDates.TryAdd(string.Format("{0}-{1}", calendarDate.ServiceId, calendarDate.Date), calendarDate);
                }

                for (int i = 0; i < schedule.Calendar.SupplementNonRunningDates.Count; i++)
                {
                    GTFSCalendarDate calendarDate = new()
                    {
                        ServiceId = string.Format("{0}-{1}-{2}-{3}", schedule.ServiceCode, string.Format("{0}{1}{2}", schedule.Calendar.StartDate.ToString("yyyy"), schedule.Calendar.StartDate.ToString("MM"), schedule.Calendar.StartDate.ToString("dd")), string.Format("{0}{1}{2}", schedule.Calendar.EndDate.ToString("yyyy"), schedule.Calendar.EndDate.ToString("MM"), schedule.Calendar.EndDate.ToString("dd")), string.Format("{0}{1}{2}{3}{4}{5}{6}", schedule.Calendar.Monday.ToInt().ToString(), schedule.Calendar.Tuesday.ToInt().ToString(), schedule.Calendar.Wednesday.ToInt().ToString(), schedule.Calendar.Thursday.ToInt().ToString(), schedule.Calendar.Friday.ToInt().ToString(), schedule.Calendar.Saturday.ToInt().ToString(), schedule.Calendar.Sunday.ToInt().ToString())),
                        Date = string.Format("{0}{1}{2}", schedule.Calendar.SupplementNonRunningDates[i].ToString("yyyy"), schedule.Calendar.SupplementNonRunningDates[i].ToString("MM"), schedule.Calendar.SupplementNonRunningDates[i].ToString("dd")),
                        ExceptionType = "2"
                    };

                    _calendarDates.TryAdd(string.Format("{0}-{1}", calendarDate.ServiceId, calendarDate.Date), calendarDate);
                }
            }

            _calendarDates = _calendarDates.OrderBy(c => c.Value.ServiceId).ToDictionary(c => c.Key, c => c.Value);
        }

        private void PrepareRoutes(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
            {
                GTFSRoute route = new()
                {
                    RouteId = schedule.ServiceCode,
                    AgencyId = schedule.OperatorCode,
                    RouteShortName = schedule.Line,
                    RouteLongName = schedule.Description,
                    RouteType = schedule.Mode
                };

                _routes.TryAdd(route.RouteId, route);
            }

            _routes = _routes.OrderBy(r => r.Value.RouteId).ToDictionary(r => r.Key, r => r.Value);
        }

        private void PrepareStops(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
            {
                for (int i = 0; i < schedule.Stops.Count; i++)
                {
                    GTFSStop stop = new()
                    {
                        StopTimezone = "Europe/London"
                    };

                    if (!string.IsNullOrEmpty(schedule.Stops[i].NaptanStop.StopType))
                    {
                        stop.StopId = schedule.Stops[i].NaptanStop.ATCOCode;
                    }
                    else
                    {
                        stop.StopId = schedule.Stops[i].TravelineStop.StopPointReference;
                    }

                    if (!string.IsNullOrEmpty(schedule.Stops[i].NaptanStop.StopType))
                    {
                        stop.StopCode = schedule.Stops[i].NaptanStop.NaptanCode;
                    }
                    else
                    {
                        stop.StopCode = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(schedule.Stops[i].NaptanStop.StopType))
                    {
                        stop.StopName = schedule.Stops[i].NaptanStop.CommonName;
                    }
                    else
                    {
                        stop.StopName = schedule.Stops[i].TravelineStop.CommonName;
                    }

                    if (!string.IsNullOrEmpty(schedule.Stops[i].NaptanStop.StopType))
                    {
                        stop.StopDesc = schedule.Stops[i].NaptanStop.LocalityName;
                    }
                    else
                    {
                        stop.StopDesc = schedule.Stops[i].TravelineStop.LocalityName;
                    }

                    if (!string.IsNullOrEmpty(schedule.Stops[i].NaptanStop.StopType))
                    {
                        stop.StopLat = schedule.Stops[i].NaptanStop.Latitude;
                    }
                    else
                    {
                        stop.StopLat = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(schedule.Stops[i].NaptanStop.StopType))
                    {
                        stop.StopLon = schedule.Stops[i].NaptanStop.Longitude;
                    }
                    else
                    {
                        stop.StopLon = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(schedule.Stops[i].NaptanStop.StopType))
                    {
                        if (schedule.Stops[i].NaptanStop.StopType == "BST" || schedule.Stops[i].NaptanStop.StopType == "FER" || schedule.Stops[i].NaptanStop.StopType == "GAT" || schedule.Stops[i].NaptanStop.StopType == "LCB" || schedule.Stops[i].NaptanStop.StopType == "MET" || schedule.Stops[i].NaptanStop.StopType == "RLY")
                        {
                            stop.LocationType = "1";
                        }
                        else if (schedule.Stops[i].NaptanStop.StopType == "AIR" || schedule.Stops[i].NaptanStop.StopType == "BCE" || schedule.Stops[i].NaptanStop.StopType == "FTD" || schedule.Stops[i].NaptanStop.StopType == "LSE" || schedule.Stops[i].NaptanStop.StopType == "RSE" || schedule.Stops[i].NaptanStop.StopType == "TMU")
                        {
                            stop.LocationType = "2";
                        }
                        else
                        {
                            stop.LocationType = "0";
                        }
                    }
                    else
                    {
                        stop.LocationType = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(schedule.Stops[i].NaptanStop.StopType))
                    {
                        if (schedule.Stops[i].NaptanStop.StopType == "PLT" || schedule.Stops[i].NaptanStop.StopType == "RPL")
                        {
                            stop.WheelchairBoarding = "1";
                        }
                        else
                        {
                            stop.WheelchairBoarding = "0";
                        }
                    }
                    else
                    {
                        stop.WheelchairBoarding = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(schedule.Stops[i].NaptanStop.StopType))
                    {
                        if (schedule.Stops[i].NaptanStop.StopType == "BCS" || schedule.Stops[i].NaptanStop.StopType == "BCQ")
                        {
                            if (schedule.Stops[i].NaptanStop.Indicator.StartsWith("bay", StringComparison.CurrentCultureIgnoreCase))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.Indicator.ToLower().Split(" ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.Indicator.StartsWith("stance", StringComparison.CurrentCultureIgnoreCase))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.Indicator.ToLower().Split(" ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.Indicator.StartsWith("stand", StringComparison.CurrentCultureIgnoreCase))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.Indicator.ToLower().Split(" ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.Indicator.StartsWith("stop", StringComparison.CurrentCultureIgnoreCase))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.Indicator.ToLower().Split(" ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.CommonName.Contains("/", StringComparison.CurrentCultureIgnoreCase))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.CommonName.ToLower().Split("/").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.CommonName.Contains("bay ", StringComparison.CurrentCultureIgnoreCase))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.CommonName.ToLower().Split("bay ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.CommonName.Contains("stance ", StringComparison.CurrentCultureIgnoreCase))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.CommonName.ToLower().Split("stance ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.CommonName.Contains("stand ", StringComparison.CurrentCultureIgnoreCase))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.CommonName.ToLower().Split("stand ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.CommonName.Contains("stop ", StringComparison.CurrentCultureIgnoreCase))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.CommonName.ToLower().Split("stop ").LastOrDefault().Trim();
                            }
                        }
                        else if (schedule.Stops[i].NaptanStop.StopType == "LPL" || schedule.Stops[i].NaptanStop.StopType == "PLT" || schedule.Stops[i].NaptanStop.StopType == "RPL")
                        {
                            stop.PlatformCode = schedule.Stops[i].NaptanStop.ATCOCode[^1..].ToLower();
                        }
                    }
                    else
                    {
                        stop.PlatformCode = string.Empty;
                    }

                    _stops.TryAdd(stop.StopId, stop);
                }
            }

            _stops = _stops.OrderBy(s => s.Value.StopId).ToDictionary(s => s.Key, s => s.Value);
        }

        private void PrepareStopTimes(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
            {
                TimeSpan timeSpan = new();

                for (int i = 0; i < schedule.Stops.Count; i++)
                {
                    GTFSStopTime stopTime = new()
                    {
                        TripId = schedule.Id,
                        StopSequence = Convert.ToString(i + 1)
                    };

                    if (schedule.Stops[i].DepartureTime < timeSpan)
                    {
                        schedule.Stops[i].ArrivalTime = schedule.Stops[i].ArrivalTime.Add(new TimeSpan(24, 0, 0));
                        schedule.Stops[i].DepartureTime = schedule.Stops[i].DepartureTime.Add(new TimeSpan(24, 0, 0));

                        stopTime.ArrivalTime = string.Concat(Math.Round(schedule.Stops[i].ArrivalTime.TotalHours, 0).ToString(), schedule.Stops[i].ArrivalTime.ToString(@"hh\:mm\:ss").AsSpan(2, 6));
                        stopTime.DepartureTime = string.Concat(Math.Round(schedule.Stops[i].DepartureTime.TotalHours, 0).ToString(), schedule.Stops[i].DepartureTime.ToString(@"hh\:mm\:ss").AsSpan(2, 6));

                        timeSpan = schedule.Stops[i].DepartureTime;
                    }
                    else
                    {
                        stopTime.ArrivalTime = schedule.Stops[i].ArrivalTime.ToString(@"hh\:mm\:ss");
                        stopTime.DepartureTime = schedule.Stops[i].DepartureTime.ToString(@"hh\:mm\:ss");

                        timeSpan = schedule.Stops[i].DepartureTime;
                    }

                    if (!string.IsNullOrEmpty(schedule.Stops[i].NaptanStop.StopType))
                    {
                        stopTime.StopId = schedule.Stops[i].NaptanStop.ATCOCode;
                    }
                    else
                    {
                        stopTime.StopId = schedule.Stops[i].TravelineStop.StopPointReference;
                    }

                    if (schedule.Stops[i].Activity == "pickUp")
                    {
                        stopTime.PickupType = "0";
                        stopTime.DropOffType = "1";
                    }
                    else if (schedule.Stops[i].Activity == "pickUpAndSetDown")
                    {
                        stopTime.PickupType = "0";
                        stopTime.DropOffType = "0";
                    }
                    else if (schedule.Stops[i].Activity == "setDown")
                    {
                        stopTime.PickupType = "1";
                        stopTime.DropOffType = "0";
                    }
                    else
                    {
                        stopTime.PickupType = "1";
                        stopTime.DropOffType = "1";
                    }

                    _stopTimes.Add(Guid.NewGuid().ToString(), stopTime);
                }
            }

            _stopTimes = _stopTimes.OrderBy(s => s.Value.TripId).ToDictionary(s => s.Key, s => s.Value);
        }

        private void PrepareTrips(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
            {
                GTFSTrip trip = new()
                {
                    RouteId = schedule.ServiceCode,
                    ServiceId = string.Format("{0}-{1}-{2}-{3}", schedule.ServiceCode, string.Format("{0}{1}{2}", schedule.Calendar.StartDate.ToString("yyyy"), schedule.Calendar.StartDate.ToString("MM"), schedule.Calendar.StartDate.ToString("dd")), string.Format("{0}{1}{2}", schedule.Calendar.EndDate.ToString("yyyy"), schedule.Calendar.EndDate.ToString("MM"), schedule.Calendar.EndDate.ToString("dd")), string.Format("{0}{1}{2}{3}{4}{5}{6}", schedule.Calendar.Monday.ToInt().ToString(), schedule.Calendar.Tuesday.ToInt().ToString(), schedule.Calendar.Wednesday.ToInt().ToString(), schedule.Calendar.Thursday.ToInt().ToString(), schedule.Calendar.Friday.ToInt().ToString(), schedule.Calendar.Saturday.ToInt().ToString(), schedule.Calendar.Sunday.ToInt().ToString())),
                    TripId = schedule.Id,
                    DirectionId = schedule.Direction
                };

                if (!string.IsNullOrEmpty(schedule.Stops.LastOrDefault().NaptanStop.StopType))
                {
                    trip.TripHeadsign = schedule.Stops.LastOrDefault().NaptanStop.CommonName;
                }
                else
                {
                    trip.TripHeadsign = schedule.Stops.LastOrDefault().TravelineStop.CommonName;
                }

                _trips.Add(Guid.NewGuid().ToString(), trip);
            }

            _trips = _trips.OrderBy(t => t.Value.TripId).ToDictionary(t => t.Key, t => t.Value);
        }
    }
}