using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TransXChange.Common.Helpers;
using TransXChange.Common.Models;

namespace TransXChange.Scotland.Test
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
            
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");

            Assert.IsTrue(stops.Count > 0);
        }

        [TestMethod]
        public void Traveline()
        {
            GtfsHelpers gtfsHelpers = new();
            NaptanHelpers naptanHelpers = new();
            TravelineHelpers travelineHelpers = new();
            
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = TravelineHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", ["all"], ["6200206531"], "22/04/2024", 7);

            Assert.IsTrue(schedules.Count > 0);
        }
    }
}