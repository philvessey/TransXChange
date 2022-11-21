using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TransXChange.Common.Helpers;
using TransXChange.Common.Models;

namespace TransXChange.Scotland.Test
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
            
            Dictionary<string, NAPTANStop> stops = naptanHelpers.Read("Data/edinburgh.csv");
            Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, "Data/S.zip", Environment.GetEnvironmentVariable("KEY"), "bus", new[] { "6200206531" }, 7);
            Dictionary<string, TXCSchedule> duplicates = travelineHelpers.ScanDuplicate(originals);

            Assert.IsNotNull(duplicates);
        }
    }
}