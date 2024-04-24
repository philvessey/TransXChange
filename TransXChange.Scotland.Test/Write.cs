using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TransXChange.Common.Helpers;
using TransXChange.Common.Models;

namespace TransXChange.Scotland.Test
{
    [TestClass]
    public class Write
    {
        [TestMethod]
        public void Agency()
        {
            GtfsHelpers gtfsHelpers = new();
            NaptanHelpers naptanHelpers = new();
            TravelineHelpers travelineHelpers = new();
            
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TravelineHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "6200206531" }, "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteAgency(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "agency.txt")));
        }

        [TestMethod]
        public void Calendar()
        {
            GtfsHelpers gtfsHelpers = new();
            NaptanHelpers naptanHelpers = new();
            TravelineHelpers travelineHelpers = new();
            
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TravelineHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "6200206531" }, "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteCalendar(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "calendar.txt")));
        }

        [TestMethod]
        public void CalendarDates()
        {
            GtfsHelpers gtfsHelpers = new();
            NaptanHelpers naptanHelpers = new();
            TravelineHelpers travelineHelpers = new();
            
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TravelineHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "6200206531" }, "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteCalendarDates(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "calendar_dates.txt")));
        }

        [TestMethod]
        public void Routes()
        {
            GtfsHelpers gtfsHelpers = new();
            NaptanHelpers naptanHelpers = new();
            TravelineHelpers travelineHelpers = new();
            
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TravelineHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "6200206531" }, "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteRoutes(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "routes.txt")));
        }

        [TestMethod]
        public void Stops()
        {
            GtfsHelpers gtfsHelpers = new();
            NaptanHelpers naptanHelpers = new();
            TravelineHelpers travelineHelpers = new();
            
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TravelineHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "6200206531" }, "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteStops(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "stops.txt")));
        }

        [TestMethod]
        public void StopTimes()
        {
            GtfsHelpers gtfsHelpers = new();
            NaptanHelpers naptanHelpers = new();
            TravelineHelpers travelineHelpers = new();
            
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TravelineHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "6200206531" }, "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteStopTimes(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "stop_times.txt")));
        }

        [TestMethod]
        public void Trips()
        {
            GtfsHelpers gtfsHelpers = new();
            NaptanHelpers naptanHelpers = new();
            TravelineHelpers travelineHelpers = new();
            
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TravelineHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "6200206531" }, "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteTrips(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "trips.txt")));
        }
    }
}