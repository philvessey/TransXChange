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
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TransXChangeHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", ["all"], ["6200206531"], "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            GtfsHelpers.WriteAgency(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "agency.txt")));
        }

        [TestMethod]
        public void Calendar()
        {
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TransXChangeHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", ["all"], ["6200206531"], "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            GtfsHelpers.WriteCalendar(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "calendar.txt")));
        }

        [TestMethod]
        public void CalendarDates()
        {
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TransXChangeHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", ["all"], ["6200206531"], "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            GtfsHelpers.WriteCalendarDates(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "calendar_dates.txt")));
        }

        [TestMethod]
        public void Routes()
        {
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TransXChangeHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", ["all"], ["6200206531"], "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            GtfsHelpers.WriteRoutes(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "routes.txt")));
        }

        [TestMethod]
        public void Stops()
        {
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TransXChangeHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", ["all"], ["6200206531"], "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            GtfsHelpers.WriteStops(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "stops.txt")));
        }

        [TestMethod]
        public void StopTimes()
        {
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TransXChangeHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", ["all"], ["6200206531"], "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            GtfsHelpers.WriteStopTimes(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "stop_times.txt")));
        }

        [TestMethod]
        public void Trips()
        {
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TransXChangeHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", ["all"], ["6200206531"], "22/04/2024", 7);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            GtfsHelpers.WriteTrips(schedules, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "trips.txt")));
        }
    }
}