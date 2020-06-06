﻿using CommandLine;
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

        [Option('m', "mode", Default = "all", HelpText = "Specify transport mode. Modes: all, bus, city-rail, ferry, light-rail")]
        public string Mode { get; set; }

        [Option('f', "filters", Default = new[] { "all" }, Separator = ',', HelpText = "Specify stop filters. Separate by comma.")]
        public IEnumerable<string> Filters { get; set; }

        [Option('d', "days", Default = 7, HelpText = "Specify days in advance for schedules. Maximum is 28 days.")]
        public double Days { get; set; }
    }
}