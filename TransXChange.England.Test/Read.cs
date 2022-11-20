using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TransXChange.Common.Helpers;
using TransXChange.Common.Models;

namespace TransXChange.England.Test
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
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/manchester.csv");

            Assert.IsNotNull(stops);
        }

        [TestMethod]
        public void Traveline()
        {
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/manchester.csv");
            Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, "Data/NW.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "1800EB01341" }, 7);

            Assert.IsNotNull(originals);
        }
    }
}