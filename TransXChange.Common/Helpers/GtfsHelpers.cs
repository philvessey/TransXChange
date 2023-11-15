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
        private Dictionary<string, GTFSAgency> _agencies = new Dictionary<string, GTFSAgency>();
        private Dictionary<string, GTFSCalendar> _calendars = new Dictionary<string, GTFSCalendar>();
        private Dictionary<string, GTFSCalendarDate> _calendarDates = new Dictionary<string, GTFSCalendarDate>();
        private Dictionary<string, GTFSRoute> _routes = new Dictionary<string, GTFSRoute>();
        private Dictionary<string, GTFSStop> _stops = new Dictionary<string, GTFSStop>();
        private Dictionary<string, GTFSStopTime> _stopTimes = new Dictionary<string, GTFSStopTime>();
        private Dictionary<string, GTFSTrip> _trips = new Dictionary<string, GTFSTrip>();

        public void WriteAgency(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareAgency(schedules);

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

        public void WriteCalendar(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareCalendar(schedules);

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

        public void WriteCalendarDates(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareCalendarDates(schedules);

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

        public void WriteRoutes(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareRoutes(schedules);

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

        public void WriteStops(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareStops(schedules);

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

        public void WriteStopTimes(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareStopTimes(schedules);

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

        public void WriteTrips(Dictionary<string, TXCSchedule> schedules, string path)
        {
            PrepareTrips(schedules);

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

        private void PrepareAgency(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
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

                if (!_agencies.ContainsKey(id))
                {
                    _agencies.Add(id, agency);
                }
            }

            _agencies = _agencies.OrderBy(a => a.Value.AgencyId).ToDictionary(a => a.Key, a => a.Value);
        }

        private void PrepareCalendar(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
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

                if (!_calendars.ContainsKey(id))
                {
                    _calendars.Add(id, calendar);
                }
            }

            _calendars = _calendars.OrderBy(c => c.Value.ServiceId).ToDictionary(c => c.Key, c => c.Value);
        }

        private void PrepareCalendarDates(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
            {
                for (int i = 0; i < schedule.Calendar.SupplementRunningDates.Count; i++)
                {
                    GTFSCalendarDate calendarDate = new GTFSCalendarDate()
                    {
                        ServiceId = string.Format("{0}-{1}-{2}-{3}", schedule.ServiceCode, string.Format("{0}{1}{2}", schedule.Calendar.StartDate.ToString("yyyy"), schedule.Calendar.StartDate.ToString("MM"), schedule.Calendar.StartDate.ToString("dd")), string.Format("{0}{1}{2}", schedule.Calendar.EndDate.ToString("yyyy"), schedule.Calendar.EndDate.ToString("MM"), schedule.Calendar.EndDate.ToString("dd")), string.Format("{0}{1}{2}{3}{4}{5}{6}", schedule.Calendar.Monday.ToInt().ToString(), schedule.Calendar.Tuesday.ToInt().ToString(), schedule.Calendar.Wednesday.ToInt().ToString(), schedule.Calendar.Thursday.ToInt().ToString(), schedule.Calendar.Friday.ToInt().ToString(), schedule.Calendar.Saturday.ToInt().ToString(), schedule.Calendar.Sunday.ToInt().ToString())),
                        Date = string.Format("{0}{1}{2}", schedule.Calendar.SupplementRunningDates[i].ToString("yyyy"), schedule.Calendar.SupplementRunningDates[i].ToString("MM"), schedule.Calendar.SupplementRunningDates[i].ToString("dd")),
                        ExceptionType = "1"
                    };

                    string id = string.Format("{0}-{1}", calendarDate.ServiceId, calendarDate.Date);

                    if (!_calendarDates.ContainsKey(id))
                    {
                        _calendarDates.Add(id, calendarDate);
                    }
                }

                for (int i = 0; i < schedule.Calendar.SupplementNonRunningDates.Count; i++)
                {
                    GTFSCalendarDate calendarDate = new GTFSCalendarDate()
                    {
                        ServiceId = string.Format("{0}-{1}-{2}-{3}", schedule.ServiceCode, string.Format("{0}{1}{2}", schedule.Calendar.StartDate.ToString("yyyy"), schedule.Calendar.StartDate.ToString("MM"), schedule.Calendar.StartDate.ToString("dd")), string.Format("{0}{1}{2}", schedule.Calendar.EndDate.ToString("yyyy"), schedule.Calendar.EndDate.ToString("MM"), schedule.Calendar.EndDate.ToString("dd")), string.Format("{0}{1}{2}{3}{4}{5}{6}", schedule.Calendar.Monday.ToInt().ToString(), schedule.Calendar.Tuesday.ToInt().ToString(), schedule.Calendar.Wednesday.ToInt().ToString(), schedule.Calendar.Thursday.ToInt().ToString(), schedule.Calendar.Friday.ToInt().ToString(), schedule.Calendar.Saturday.ToInt().ToString(), schedule.Calendar.Sunday.ToInt().ToString())),
                        Date = string.Format("{0}{1}{2}", schedule.Calendar.SupplementNonRunningDates[i].ToString("yyyy"), schedule.Calendar.SupplementNonRunningDates[i].ToString("MM"), schedule.Calendar.SupplementNonRunningDates[i].ToString("dd")),
                        ExceptionType = "2"
                    };

                    string id = string.Format("{0}-{1}", calendarDate.ServiceId, calendarDate.Date);

                    if (!_calendarDates.ContainsKey(id))
                    {
                        _calendarDates.Add(id, calendarDate);
                    }
                }
            }

            _calendarDates = _calendarDates.OrderBy(c => c.Value.ServiceId).ToDictionary(c => c.Key, c => c.Value);
        }

        private void PrepareRoutes(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
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

                if (!_routes.ContainsKey(id))
                {
                    _routes.Add(id, route);
                }
            }

            _routes = _routes.OrderBy(r => r.Value.RouteId).ToDictionary(r => r.Key, r => r.Value);
        }

        private void PrepareStops(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
            {
                for (int i = 0; i < schedule.Stops.Count; i++)
                {
                    GTFSStop stop = new GTFSStop()
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
                            if (schedule.Stops[i].NaptanStop.Indicator.ToLower().StartsWith("bay"))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.Indicator.ToLower().Split(" ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.Indicator.ToLower().StartsWith("stance"))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.Indicator.ToLower().Split(" ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.Indicator.ToLower().StartsWith("stand"))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.Indicator.ToLower().Split(" ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.Indicator.ToLower().StartsWith("stop"))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.Indicator.ToLower().Split(" ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.CommonName.ToLower().Contains("/"))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.CommonName.ToLower().Split("/").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.CommonName.ToLower().Contains("bay "))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.CommonName.ToLower().Split("bay ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.CommonName.ToLower().Contains("stance "))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.CommonName.ToLower().Split("stance ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.CommonName.ToLower().Contains("stand "))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.CommonName.ToLower().Split("stand ").LastOrDefault().Trim();
                            }
                            else if (schedule.Stops[i].NaptanStop.CommonName.ToLower().Contains("stop "))
                            {
                                stop.PlatformCode = schedule.Stops[i].NaptanStop.CommonName.ToLower().Split("stop ").LastOrDefault().Trim();
                            }
                        }
                        else if (schedule.Stops[i].NaptanStop.StopType == "LPL" || schedule.Stops[i].NaptanStop.StopType == "PLT" || schedule.Stops[i].NaptanStop.StopType == "RPL")
                        {
                            stop.PlatformCode = schedule.Stops[i].NaptanStop.ATCOCode.Substring(schedule.Stops[i].NaptanStop.ATCOCode.Length - 1).ToLower();
                        }
                    }
                    else
                    {
                        stop.PlatformCode = string.Empty;
                    }

                    string id = stop.StopId;

                    if (!_stops.ContainsKey(id))
                    {
                        _stops.Add(id, stop);
                    }
                }
            }

            _stops = _stops.OrderBy(s => s.Value.StopId).ToDictionary(s => s.Key, s => s.Value);
        }

        private void PrepareStopTimes(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
            {
                TimeSpan timeSpan = new TimeSpan();

                for (int i = 0; i < schedule.Stops.Count; i++)
                {
                    GTFSStopTime stopTime = new GTFSStopTime()
                    {
                        TripId = schedule.Id,
                        StopSequence = Convert.ToString(i + 1)
                    };

                    if (schedule.Stops[i].DepartureTime < timeSpan)
                    {
                        schedule.Stops[i].ArrivalTime = schedule.Stops[i].ArrivalTime.Add(new TimeSpan(24, 0, 0));
                        schedule.Stops[i].DepartureTime = schedule.Stops[i].DepartureTime.Add(new TimeSpan(24, 0, 0));

                        stopTime.ArrivalTime = Math.Round(schedule.Stops[i].ArrivalTime.TotalHours, 0).ToString() + schedule.Stops[i].ArrivalTime.ToString(@"hh\:mm\:ss").Substring(2, 6);
                        stopTime.DepartureTime = Math.Round(schedule.Stops[i].DepartureTime.TotalHours, 0).ToString() + schedule.Stops[i].DepartureTime.ToString(@"hh\:mm\:ss").Substring(2, 6);

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

                    string id = Guid.NewGuid().ToString();

                    _stopTimes.Add(id, stopTime);
                }
            }

            _stopTimes = _stopTimes.OrderBy(s => s.Value.TripId).ToDictionary(s => s.Key, s => s.Value);
        }

        private void PrepareTrips(Dictionary<string, TXCSchedule> schedules)
        {
            foreach (TXCSchedule schedule in schedules.Values)
            {
                GTFSTrip trip = new GTFSTrip()
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

                string id = Guid.NewGuid().ToString();

                _trips.Add(id, trip);
            }

            _trips = _trips.OrderBy(t => t.Value.TripId).ToDictionary(t => t.Key, t => t.Value);
        }
    }
}