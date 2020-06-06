using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.IO;
using TransXChange.Common;
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

            Dictionary<string, NAPTANStop> stops = Naptan.Read(options.Naptan);
            Console.WriteLine(string.Format("READ: National Public Transport Access Nodes (NaPTAN). Found {0:#,##0.##} stops.", stops.Count));

            Dictionary<string, TXCSchedule> originals = Traveline.ReadScotland(stops, options.Traveline, options.Mode, options.Filters, options.Days);
            Console.WriteLine(string.Format("READ: Traveline National Dataset (TNDS). Found {0:#,##0.##} original schedules.", originals.Count));

            Dictionary<string, TXCSchedule> duplicates = Traveline.ScanDuplicate(originals);
            Console.WriteLine(string.Format("READ: Traveline National Dataset (TNDS). Found {0:#,##0.##} duplicate schedules.", duplicates.Count));

            Directory.CreateDirectory(options.Output);
            Console.WriteLine(string.Format("WRITE: {0}", options.Output));

            Gtfs.WriteAgency(originals, duplicates, options.Output);
            Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "agency.txt")));

            Gtfs.WriteCalendar(originals, duplicates, options.Output);
            Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "calendar.txt")));

            Gtfs.WriteCalendarDates(originals, duplicates, options.Output);
            Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "calendar_dates.txt")));

            Gtfs.WriteRoutes(originals, duplicates, options.Output);
            Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "routes.txt")));

            Gtfs.WriteStops(originals, duplicates, options.Output);
            Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "stops.txt")));

            Gtfs.WriteStopTimes(originals, duplicates, options.Output);
            Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "stop_times.txt")));

            Gtfs.WriteTrips(originals, duplicates, options.Output);
            Console.WriteLine(string.Format("WRITE: {0}", Path.Combine(options.Output, "trips.txt")));
        }
    }
}