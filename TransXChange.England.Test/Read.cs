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
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/manchester.csv");
            
            Assert.IsTrue(stops.Count > 0);
        }

        [TestMethod]
        public void TransXChange()
        {
            Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read("Data/manchester.csv");
            Dictionary<string, TXCSchedule> schedules = TransXChangeHelpers.ReadEngland(stops, "Data/NW.zip", Environment.GetEnvironmentVariable("KEY"), "bus", ["all"], ["1800EB01341"], "22/04/2024", 7);
            
            Assert.IsTrue(schedules.Count > 0);
        }
    }
}