using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using TransXChange.Common.Helpers;
using TransXChange.Common.Models;

namespace TransXChange.England
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

                Dictionary<string, TXCSchedule> originals = travelineHelpers.ReadEngland(stops, options.Traveline, options.Mode, options.Filters, options.Days);
                Console.WriteLine(string.Format("READ: Traveline National Dataset (TNDS). Found {0:#,##0.##} original schedules.", originals.Count));

                Dictionary<string, TXCSchedule> duplicates = travelineHelpers.ScanDuplicate(originals);
                Console.WriteLine(string.Format("READ: Traveline National Dataset (TNDS). Found {0:#,##0.##} duplicate schedules.", duplicates.Count));

                Directory.CreateDirectory(options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", options.Output));

                gtfsHelpers.WriteAgency(originals, duplicates, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "agency.txt")));

                gtfsHelpers.WriteCalendar(originals, duplicates, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "calendar.txt")));

                gtfsHelpers.WriteCalendarDates(originals, duplicates, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "calendar_dates.txt")));

                gtfsHelpers.WriteRoutes(originals, duplicates, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "routes.txt")));

                gtfsHelpers.WriteStops(originals, duplicates, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "stops.txt")));

                gtfsHelpers.WriteStopTimes(originals, duplicates, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "stop_times.txt")));

                gtfsHelpers.WriteTrips(originals, duplicates, options.Output);
                Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "trips.txt")));
            }
            catch (Exception exception)
            {
                Console.WriteLine(string.Format("ERROR: {0}", exception.Message));
            }
        }
    }
}