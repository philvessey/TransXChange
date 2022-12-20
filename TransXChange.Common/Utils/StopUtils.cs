using System.Collections.Generic;
using System.Linq;
using TransXChange.Common.Models;

namespace TransXChange.Common.Utils
{
    public static class StopUtils
    {
        public static TXCStop Build(Dictionary<string, NAPTANStop> stops, string reference)
        {
            TXCStop result = new TXCStop()
            {
                ATCOCode = reference,
                NaptanStop = GetNaptan(stops, reference)
            };

            return result;
        }

        public static bool GetFilter(bool includeSchedule, string mode, IEnumerable<string> filters, TXCStop stop)
        {
            if (!includeSchedule)
            {
                if (mode != "all")
                {
                    if (mode == "bus")
                    {
                        if (stop.NaptanStop.StopType == "BCS")
                        {
                            if (!filters.Contains("all"))
                            {
                                foreach (string filter in filters)
                                {
                                    if (stop.NaptanStop.ATCOCode == filter)
                                    {
                                        includeSchedule = true;
                                    }
                                }
                            }
                            else
                            {
                                includeSchedule = true;
                            }
                        }
                        else if (stop.NaptanStop.StopType == "BCT")
                        {
                            if (!filters.Contains("all"))
                            {
                                foreach (string filter in filters)
                                {
                                    if (stop.NaptanStop.ATCOCode == filter)
                                    {
                                        includeSchedule = true;
                                    }
                                }
                            }
                            else
                            {
                                includeSchedule = true;
                            }
                        }
                    }
                    else if (mode == "city-rail")
                    {
                        if (stop.NaptanStop.StopType == "RLY")
                        {
                            if (!filters.Contains("all"))
                            {
                                foreach (string filter in filters)
                                {
                                    if (stop.NaptanStop.ATCOCode == filter)
                                    {
                                        includeSchedule = true;
                                    }
                                }
                            }
                            else
                            {
                                includeSchedule = true;
                            }
                        }
                    }
                    else if (mode == "ferry")
                    {
                        if (stop.NaptanStop.StopType == "FBT")
                        {
                            if (!filters.Contains("all"))
                            {
                                foreach (string filter in filters)
                                {
                                    if (stop.NaptanStop.ATCOCode == filter)
                                    {
                                        includeSchedule = true;
                                    }
                                }
                            }
                            else
                            {
                                includeSchedule = true;
                            }
                        }
                    }
                    else if (mode == "light-rail")
                    {
                        if (stop.NaptanStop.StopType == "PLT")
                        {
                            if (!filters.Contains("all"))
                            {
                                foreach (string filter in filters)
                                {
                                    if (stop.NaptanStop.ATCOCode == filter)
                                    {
                                        includeSchedule = true;
                                    }
                                }
                            }
                            else
                            {
                                includeSchedule = true;
                            }
                        }
                    }
                }
                else
                {
                    if (!filters.Contains("all"))
                    {
                        foreach (string filter in filters)
                        {
                            if (stop.NaptanStop.ATCOCode == filter)
                            {
                                includeSchedule = true;
                            }
                        }
                    }
                    else
                    {
                        includeSchedule = true;
                    }
                }
            }

            return includeSchedule;
        }

        private static NAPTANStop GetNaptan(Dictionary<string, NAPTANStop> stops, string reference)
        {
            NAPTANStop result = new NAPTANStop()
            {
                ATCOCode = reference,
                CommonName = "Unknown NaPTAN Stop",
                StopType = "ZZZ"
            };

            if (stops.ContainsKey(reference))
            {
                result = stops[reference];

                if (string.IsNullOrEmpty(result.Longitude))
                {
                    result.Longitude = CoordinateUtils.GetFromEastingNorthing(double.Parse(result.Easting), double.Parse(result.Northing)).Longitude.ToString();
                }

                if (string.IsNullOrEmpty(result.Latitude))
                {
                    result.Latitude = CoordinateUtils.GetFromEastingNorthing(double.Parse(result.Easting), double.Parse(result.Northing)).Latitude.ToString();
                }
            }

            return result;
        }
    }
}