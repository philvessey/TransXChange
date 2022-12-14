using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TransXChange.Common.Helpers;
using TransXChange.Common.Models;

namespace TransXChange.England.Test
{
    [TestClass]
    public class Scan
    {
        [TestMethod]
        public void Duplicates()
        {
            GtfsHelpers gtfsHelpers = new GtfsHelpers();
            NaptanHelpers naptanHelpers = new NaptanHelpers();
            TravelineHelpers travelineHelpers = new TravelineHelpers();
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/manchester.csv");
            Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, "Data/NW.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "all" }, new[] { "1800EB01341" }, 7);
            Dictionary<string, TXCSchedule> duplicates = travelineHelpers.ScanDuplicate(originals);

            Assert.IsNotNull(duplicates);
        }
    }
}