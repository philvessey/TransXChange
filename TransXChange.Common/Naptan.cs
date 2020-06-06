using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using TransXChange.Common.Models;

namespace TransXChange.Common
{
    public static class Naptan
    {
        public static Dictionary<string, NAPTANStop> Read(string path)
        {
            Dictionary<string, NAPTANStop> dictionary = new Dictionary<string, NAPTANStop>();

            if (path.EndsWith(".zip"))
            {
                if (File.Exists(path))
                {
                    using ZipArchive archive = ZipFile.Open(path, ZipArchiveMode.Read);

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.Name.EndsWith("Stops.csv"))
                        {
                            using StreamReader reader = new StreamReader(entry.Open());
                            IEnumerable<NAPTANStop> results = new CsvReader(reader, CultureInfo.InvariantCulture).GetRecords<NAPTANStop>();

                            foreach (NAPTANStop stop in results)
                            {
                                dictionary.Add(stop.ATCOCode, stop);
                            }
                        }
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
                        if (entry.EndsWith("Stops.csv"))
                        {
                            using StreamReader reader = new StreamReader(entry);
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