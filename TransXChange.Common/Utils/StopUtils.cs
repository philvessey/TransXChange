using System;
using System.Collections.Generic;
using System.Linq;
using TransXChange.Common.Models;

namespace TransXChange.Common.Utils
{
    public static class StopUtils
    {
        public static TXCStop Build(Dictionary<string, NAPTANStop> stops, TXCXmlAnnotatedStopPointRef stopPoint)
        {
            return new TXCStop()
            {
                ATCOCode = stopPoint.StopPointRef,
                NaptanStop = GetNaptan(stops, stopPoint.StopPointRef, stopPoint.CommonName ?? "Unknown NaPTAN Stop", stopPoint.LocalityName ?? "Unknown NaPTAN Locality"),
                TravelineStop = GetTraveline(stops, stopPoint.StopPointRef, stopPoint.CommonName ?? "Unknown Traveline Stop", stopPoint.LocalityName ?? "Unknown Traveline Locality")
            };
        }

        public static bool CheckFilter(IEnumerable<string> filters, List<TXCStop> stops)
        {
            foreach (TXCStop stop in stops)
            {
                if (!string.IsNullOrEmpty(stop.NaptanStop.StopType))
                {
                    if (!filters.Contains("all"))
                    {
                        foreach (string filter in filters)
                        {
                            if (stop.NaptanStop.ATCOCode.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                            {
                                return true;
                            }
                            else if (stop.NaptanStop.CommonName.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                            {
                                return true;
                            }
                            else if (stop.NaptanStop.LocalityName.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    if (!filters.Contains("all"))
                    {
                        foreach (string filter in filters)
                        {
                            if (stop.TravelineStop.StopPointReference.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                            {
                                return true;
                            }
                            else if (stop.TravelineStop.CommonName.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                            {
                                return true;
                            }
                            else if (stop.TravelineStop.LocalityName.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        public static bool CheckMode(string mode, List<TXCStop> stops)
        {
            foreach (TXCStop stop in stops)
            {
                if (mode != "all")
                {
                    if (mode == "bus")
                    {
                        if (!string.IsNullOrEmpty(stop.NaptanStop.StopType))
                        {
                            if (stop.NaptanStop.StopType == "BCS" || stop.NaptanStop.StopType == "BCT")
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (mode == "city-rail")
                    {
                        if (!string.IsNullOrEmpty(stop.NaptanStop.StopType))
                        {
                            if (stop.NaptanStop.StopType == "RLY")
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (mode == "ferry")
                    {
                        if (!string.IsNullOrEmpty(stop.NaptanStop.StopType))
                        {
                            if (stop.NaptanStop.StopType == "FBT")
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else if (mode == "light-rail")
                    {
                        if (!string.IsNullOrEmpty(stop.NaptanStop.StopType))
                        {
                            if (stop.NaptanStop.StopType == "PLT")
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            
            return false;
        }

        private static NAPTANStop GetNaptan(Dictionary<string, NAPTANStop> stops, string reference, string commonName, string localityName)
        {
            NAPTANStop result = new()
            {
                ATCOCode = reference,
                CommonName = commonName,
                LocalityName = localityName
            };

            if (stops.TryGetValue(reference, out NAPTANStop value))
            {
                result = value;

                if (string.IsNullOrEmpty(result.Longitude))
                {
                    result.Longitude = CoordinateUtils.GetFromEastingNorthing(double.Parse(result.Easting), double.Parse(result.Northing)).Longitude.ToString();
                }

                if (string.IsNullOrEmpty(result.Latitude))
                {
                    result.Latitude = CoordinateUtils.GetFromEastingNorthing(double.Parse(result.Easting), double.Parse(result.Northing)).Latitude.ToString();
                }

                return result;
            }

            return result;
        }

        private static TRAVELINEStop GetTraveline(Dictionary<string, NAPTANStop> stops, string reference, string commonName, string localityName)
        {
            return new TRAVELINEStop()
            {
                StopPointReference = reference,
                CommonName = commonName,
                LocalityName = localityName
            };
        }
    }
}