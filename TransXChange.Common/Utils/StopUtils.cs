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
                NaptanStop = GetNaptan(stops, stopPoint.StopPointRef),
                TravelineStop = GetTraveline(stopPoint.StopPointRef, stopPoint.CommonName, stopPoint.LocalityName)
            };
        }

        public static bool GetFilter(bool includeSchedule, string mode, IEnumerable<string> filters, TXCStop stop)
        {
            if (!includeSchedule)
            {
                if (mode != "all")
                {
                    if (mode == "bus")
                    {
                        if (!string.IsNullOrEmpty(stop.NaptanStop.StopType) && stop.NaptanStop.StopType == "BCS")
                        {
                            if (!filters.Contains("all"))
                            {
                                foreach (string filter in filters)
                                {
                                    if (!string.IsNullOrEmpty(stop.NaptanStop.ATCOCode) && stop.NaptanStop.ATCOCode.Contains(filter))
                                    {
                                        includeSchedule = true;
                                    }

                                    if (!string.IsNullOrEmpty(stop.TravelineStop.StopPointReference) && stop.TravelineStop.StopPointReference.Contains(filter))
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
                        else if (!string.IsNullOrEmpty(stop.NaptanStop.StopType) && stop.NaptanStop.StopType == "BCT")
                        {
                            if (!filters.Contains("all"))
                            {
                                foreach (string filter in filters)
                                {
                                    if (!string.IsNullOrEmpty(stop.NaptanStop.ATCOCode) && stop.NaptanStop.ATCOCode.Contains(filter))
                                    {
                                        includeSchedule = true;
                                    }

                                    if (!string.IsNullOrEmpty(stop.TravelineStop.StopPointReference) && stop.TravelineStop.StopPointReference.Contains(filter))
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
                        if (!string.IsNullOrEmpty(stop.NaptanStop.StopType) && stop.NaptanStop.StopType == "RLY")
                        {
                            if (!filters.Contains("all"))
                            {
                                foreach (string filter in filters)
                                {
                                    if (!string.IsNullOrEmpty(stop.NaptanStop.ATCOCode) && stop.NaptanStop.ATCOCode.Contains(filter))
                                    {
                                        includeSchedule = true;
                                    }

                                    if (!string.IsNullOrEmpty(stop.TravelineStop.StopPointReference) && stop.TravelineStop.StopPointReference.Contains(filter))
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
                        if (!string.IsNullOrEmpty(stop.NaptanStop.StopType) && stop.NaptanStop.StopType == "FBT")
                        {
                            if (!filters.Contains("all"))
                            {
                                foreach (string filter in filters)
                                {
                                    if (!string.IsNullOrEmpty(stop.NaptanStop.ATCOCode) && stop.NaptanStop.ATCOCode.Contains(filter))
                                    {
                                        includeSchedule = true;
                                    }

                                    if (!string.IsNullOrEmpty(stop.TravelineStop.StopPointReference) && stop.TravelineStop.StopPointReference.Contains(filter))
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
                        if (!string.IsNullOrEmpty(stop.NaptanStop.StopType) && stop.NaptanStop.StopType == "PLT")
                        {
                            if (!filters.Contains("all"))
                            {
                                foreach (string filter in filters)
                                {
                                    if (!string.IsNullOrEmpty(stop.NaptanStop.ATCOCode) && stop.NaptanStop.ATCOCode.Contains(filter))
                                    {
                                        includeSchedule = true;
                                    }

                                    if (!string.IsNullOrEmpty(stop.TravelineStop.StopPointReference) && stop.TravelineStop.StopPointReference.Contains(filter))
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
                            if (!string.IsNullOrEmpty(stop.NaptanStop.ATCOCode) && stop.NaptanStop.ATCOCode.Contains(filter))
                            {
                                includeSchedule = true;
                            }

                            if (!string.IsNullOrEmpty(stop.TravelineStop.StopPointReference) && stop.TravelineStop.StopPointReference.Contains(filter))
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
            NAPTANStop result = new NAPTANStop();

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

                return result;
            }

            return result;
        }

        private static TRAVELINEStop GetTraveline(string reference, string commonName, string localityName)
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