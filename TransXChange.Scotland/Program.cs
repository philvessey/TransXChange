using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using TransXChange.Common.Helpers;
using TransXChange.Common.Models;

namespace TransXChange.Scotland
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
                GtfsHelpers gtfsHelpers = new GtfsHelpers();
                NaptanHelpers naptanHelpers = new NaptanHelpers();
                TravelineHelpers travelineHelpers = new TravelineHelpers();

                Dictionary<string, NAPTANStop> stops = naptanHelpers.Read(options.Naptan);
                Console.WriteLine(string.Format("READ: National Public Transport Access Nodes (NaPTAN). Found {0:#,##0.##} stops.", stops.Count));

                Dictionary<string, TXCSchedule> schedules = travelineHelpers.ReadScotland(stops, options.Traveline, options.Key, options.Mode, options.Indexes, options.Filters, options.Days);
                Console.WriteLine(string.Format("READ: Traveline National Dataset (TNDS). Found {0:#,##0.##} schedules.", schedules.Count));

                Directory.CreateDirectory(options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", options.Output));

                gtfsHelpers.WriteAgency(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "agency.txt")));

                gtfsHelpers.WriteCalendar(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "calendar.txt")));

                gtfsHelpers.WriteCalendarDates(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "calendar_dates.txt")));

                gtfsHelpers.WriteRoutes(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "routes.txt")));

                gtfsHelpers.WriteStops(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "stops.txt")));

                gtfsHelpers.WriteStopTimes(schedules, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "stop_times.txt")));

                gtfsHelpers.WriteTrips(schedules, options.Output);
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