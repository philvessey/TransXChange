﻿using System;

namespace TransXChange.Common.Models
{
    public class TXCStop
    {
        public string ATCOCode { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public NAPTANStop NaptanStop { get; set; }
    }
}