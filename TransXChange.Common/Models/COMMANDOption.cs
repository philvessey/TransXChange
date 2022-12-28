using CommandLine;
using System.Collections.Generic;

namespace TransXChange.Common.Models
{
    public class COMMANDOption
    {
        [Option('n', "naptan", Required = true, HelpText = "Path to NaPTAN csv .zip or directory.")]
        public string Naptan { get; set; }

        [Option('t', "traveline", Required = true, HelpText = "Path to regional Traveline TNDS .zip or directory.")]
        public string Traveline { get; set; }

        [Option('o', "output", Required = true, HelpText = "Path to output directory.")]
        public string Output { get; set; }

        [Option('k', "key", Required = true, HelpText = "Key for Nager.Date NuGet package.")]
        public string Key { get; set; }

        [Option("mode", Default = "all", HelpText = "Specify transport mode for schedules. Modes: all, bus, city-rail, ferry, light-rail")]
        public string Mode { get; set; }

        [Option("indexes", Default = new[] { "all" }, Separator = ',', HelpText = "Specify filename indexes for schedules. Separate by comma.")]
        public IEnumerable<string> Indexes { get; set; }

        [Option("filters", Default = new[] { "all" }, Separator = ',', HelpText = "Specify stop filters for schedules. Separate by comma.")]
        public IEnumerable<string> Filters { get; set; }

        [Option("date", Default = "today", HelpText = "Specify date for schedules. Dates: yesterday, today, tomorrow, dd/MM/yyyy")]
        public string Date { get; set; }

        [Option("days", Default = 7, HelpText = "Specify days in advance for schedules. Maximum is 28 days.")]
        public int Days { get; set; }
    }
}