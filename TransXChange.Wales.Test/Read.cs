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
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/cardiff.csv");

            Assert.IsTrue(stops.Count > 0);
        }

        [TestMethod]
        public void Traveline()
        {
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/cardiff.csv");
            Dictionary<string, TXCSchedule> schedules = travelineHelpers.ReadWales(stops, "Data/W.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "5710WDB48395" }, "13/11/2023", 7);

            Assert.IsTrue(schedules.Count > 0);
        }
    }
}