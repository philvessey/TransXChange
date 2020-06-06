using System;
using System.Collections.Generic;

namespace TransXChange.Common.Models
{
    public class TXCCalendar
    {
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<DateTime> RunningDates { get; set; }
        public List<DateTime> SupplementRunningDates { get; set; }
        public List<DateTime> SupplementNonRunningDates { get; set; }
    }
}