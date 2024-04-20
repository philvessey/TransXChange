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
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/edinburgh.csv");

            Assert.IsTrue(stops.Count > 0);
        }

        [TestMethod]
        public void Traveline()
        {
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> schedules = travelineHelpers.ReadScotland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "6200206531" }, "22/04/2024", 7);

            Assert.IsTrue(schedules.Count > 0);
        }
    }
}