using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using TransXChange.Common.Helpers;
using TransXChange.Common.Models;

namespace TransXChange.Wales
{
    internal class Program
    {
        static void Main(string[] args) => new Program().Parse(args);

        public void Parse(string[] args)
        {
            Console.WriteLine("");
            Parser.Default.ParseArguments<COMMANDOption>(args).WithParsed(Run);
            Console.WriteLine("");
        }

        private void Run(COMMANDOption options)
        {
            Console.WriteLine(HeadingInfo.Default);
            Console.WriteLine(CopyrightInfo.Default);
            Console.WriteLine("");

            try
            {
                Dictionary<string, NAPTANStop> stops = NaptanHelpers.Read(options.Naptan);
                Console.WriteLine(string.Format("READ: NaPTAN. Found {0:#,##0.##} stops.", stops.Count));

                Dictionary<string, TXCSchedule> schedules = TransXChangeHelpers.ReadWales(stops, options.TransXChange, options.Key, options.Mode, options.Indexes, options.Filters, options.Date, options.Days);
                Console.WriteLine(string.Format("READ: TransXChange. Found {0:#,##0.##} schedules.", schedules.Count));

                Directory.CreateDirectory(options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", options.Output));

                GtfsHelpers.WriteAgency(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "agency.txt")));

                GtfsHelpers.WriteCalendar(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "calendar.txt")));

                GtfsHelpers.WriteCalendarDates(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "calendar_dates.txt")));

                GtfsHelpers.WriteRoutes(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "routes.txt")));

                GtfsHelpers.WriteStops(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "stops.txt")));

                GtfsHelpers.WriteStopTimes(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "stop_times.txt")));

                GtfsHelpers.WriteTrips(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "trips.txt")));
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(string.Format("ERROR: {0}", exception.Message));
                Console.WriteLine("");

                Environment.Exit(1);
            }
        }
    }
}