using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using TransXChange.Common.Models;

namespace TransXChange.Common.Helpers
{
    public class NaptanHelpers
    {
        public static Dictionary<string, NAPTANStop> Read(string path)
        {
            Dictionary<string, NAPTANStop> dictionary = [];

            if (path.EndsWith(".zip"))
            {
                if (File.Exists(path))
                {
                    using ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read);

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.Name.Contains("stops", StringComparison.CurrentCultureIgnoreCase) && entry.Name.ToLower().EndsWith(".csv"))
                        {
                            using StreamReader reader = new(entry.Open());
                            IEnumerable<NAPTANStop> results = new CsvReader(reader, CultureInfo.InvariantCulture).GetRecords<NAPTANStop>();

                            foreach (NAPTANStop stop in results)
                            {
                                dictionary.Add(stop.ATCOCode, stop);
                            }
                        }
                    }
                }
            }
            else if (path.EndsWith(".csv"))
            {
                if (File.Exists(path))
                {
                    using StreamReader reader = new(path);
                    IEnumerable<NAPTANStop> results = new CsvReader(reader, CultureInfo.InvariantCulture).GetRecords<NAPTANStop>();

                    foreach (NAPTANStop stop in results)
                    {
                        dictionary.Add(stop.ATCOCode, stop);
                    }
                }
            }
            else
            {
                if (Directory.Exists(path))
                {
                    string[] entries = Directory.GetFiles(path);

                    foreach (string entry in entries)
                    {
                        if (entry.Contains("stops", StringComparison.CurrentCultureIgnoreCase) && entry.ToLower().EndsWith(".csv"))
                        {
                            using StreamReader reader = new(entry);
                            IEnumerable<NAPTANStop> results = new CsvReader(reader, CultureInfo.InvariantCulture).GetRecords<NAPTANStop>();

                            foreach (NAPTANStop stop in results)
                            {
                                dictionary.Add(stop.ATCOCode, stop);
                            }
                        }
                    }
                }
            }

            return dictionary;
        }
    }
}