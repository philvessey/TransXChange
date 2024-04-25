using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TransXChange.Common.Helpers;
using TransXChange.Common.Models;

namespace TransXChange.Wales.Test
{
    [TestClass]
    public class Read
    {
        [TestMethod]
        public void Naptan()
        {
            GtfsHelpers gtfsHelpers = new();
            NaptanHelpers naptanHelpers = new();
            TravelineHelpers travelineHelpers = new();
            
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/cardiff.csv");

            Assert.IsTrue(stops.Count > 0);
        }

        [TestMethod]
        public void Traveline()
        {
            GtfsHelpers gtfsHelpers = new();
            NaptanHelpers naptanHelpers = new();
            TravelineHelpers travelineHelpers = new();
            
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/cardiff.csv");
            Dictionary<string, TXCSchedule> schedules = TravelineHelpers.ReadWales(stops, "Data/W.zip", Environment.GetEnvironmentVariable("KEY"), "bus", ["all"], ["5710WDB48395"], "22/04/2024", 7);

            Assert.IsTrue(schedules.Count > 0);
        }
    }
}