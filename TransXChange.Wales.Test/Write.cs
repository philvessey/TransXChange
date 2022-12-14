using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TransXChange.Common.Helpers;
using TransXChange.Common.Models;

namespace TransXChange.Wales.Test
{
    [TestClass]
    public class Write
    {
        [TestMethod]
        public void Agency()
        {
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/cardiff.csv");
            Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, "Data/W.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "5710WDB48395" }, 7);
            Dictionary<string, TXCSchedule> duplicates = travelineHelpers.ScanDuplicate(originals);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteAgency(originals, duplicates, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "agency.txt")));
        }

        [TestMethod]
        public void Calendar()
        {
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/cardiff.csv");
            Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, "Data/W.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "5710WDB48395" }, 7);
            Dictionary<string, TXCSchedule> duplicates = travelineHelpers.ScanDuplicate(originals);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteCalendar(originals, duplicates, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "calendar.txt")));
        }

        [TestMethod]
        public void CalendarDates()
        {
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/cardiff.csv");
            Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, "Data/W.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "5710WDB48395" }, 7);
            Dictionary<string, TXCSchedule> duplicates = travelineHelpers.ScanDuplicate(originals);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteCalendarDates(originals, duplicates, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "calendar_dates.txt")));
        }

        [TestMethod]
        public void Routes()
        {
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/cardiff.csv");
            Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, "Data/W.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "5710WDB48395" }, 7);
            Dictionary<string, TXCSchedule> duplicates = travelineHelpers.ScanDuplicate(originals);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteRoutes(originals, duplicates, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "routes.txt")));
        }

        [TestMethod]
        public void Stops()
        {
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/cardiff.csv");
            Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, "Data/W.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "5710WDB48395" }, 7);
            Dictionary<string, TXCSchedule> duplicates = travelineHelpers.ScanDuplicate(originals);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteStops(originals, duplicates, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "stops.txt")));
        }

        [TestMethod]
        public void StopTimes()
        {
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/cardiff.csv");
            Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, "Data/W.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "5710WDB48395" }, 7);
            Dictionary<string, TXCSchedule> duplicates = travelineHelpers.ScanDuplicate(originals);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteStopTimes(originals, duplicates, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "stop_times.txt")));
        }

        [TestMethod]
        public void Trips()
        {
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/cardiff.csv");
            Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, "Data/W.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "5710WDB48395" }, 7);
            Dictionary<string, TXCSchedule> duplicates = travelineHelpers.ScanDuplicate(originals);

            DirectoryInfo localDirectory = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 8).Select(s => s[new Random().Next(s.Length)]).ToArray())));
            gtfsHelpers.WriteTrips(originals, duplicates, localDirectory.FullName);

            Assert.IsTrue(File.Exists(Path.Combine(localDirectory.FullName, "trips.txt")));
        }
    }
}