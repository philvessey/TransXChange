using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TransXChange.Common.Helpers;
using TransXChange.Common.Models;

namespace TransXChange.Wales.Test
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
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/cardiff.csv");
            Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, "Data/W.zip", Environment.GetEnvironmentVariable("KEY"), "bus",new[] { "all" }, new[] { "5710WDB48395" }, 7);
            Dictionary<string, TXCSchedule> duplicates = travelineHelpers.ScanDuplicate(originals);

            Assert.IsNotNull(duplicates);
        }
    }
}