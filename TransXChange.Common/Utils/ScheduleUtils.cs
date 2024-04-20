using System;
using System.Collections.Generic;
using System.Linq;
using TransXChange.Common.Models;

namespace TransXChange.Common.Utils
{
    public static class ScheduleUtils
    {
        public static TXCSchedule Build(TXCXmlOperator @operator, TXCXmlService service, TXCXmlJourneyPattern journeyPattern, TXCCalendar calendar)
        {
            TXCSchedule result = new TXCSchedule()
            {
                Id = Guid.NewGuid().ToString(),
                Line = service.Lines.Line.LineName,
                ServiceCode = service.ServiceCode,
                Calendar = calendar,
                Stops = new List<TXCStop>()
            };

            if (service.Description != null)
            {
                result.Description = service.Description.Trim();
            }

            if (journeyPattern.Direction == "inbound")
            {
                result.Direction = "1";
            }
            else
            {
                result.Direction = "0";
            }

            if (service.Mode == "bus")
            {
                result.Mode = "3";
            }
            else if (service.Mode == "coach")
            {
                result.Mode = "3";
            }
            else if (service.Mode == "ferry")
            {
                result.Mode = "4";
            }
            else if (service.Mode == "rail")
            {
                if (@operator.OperatorCode == "EAL")
                {
                    result.Mode = "6";
                }
                else
                {
                    result.Mode = "0";
                }
            }
            else if (service.Mode == "tram")
            {
                result.Mode = "0";
            }
            else if (service.Mode == "underground")
            {
                result.Mode = "1";
            }
            else
            {
                result.Mode = "3";
            }

            if (@operator.NationalOperatorCode != null)
            {
                if (@operator.NationalOperatorCode != "")
                {
                    result.OperatorCode = @operator.NationalOperatorCode;
                    result.OperatorName = @operator.OperatorShortName;
                }
                else
                {
                    result.OperatorCode = "ZZZZ";
                    result.OperatorName = "Unknown NOC Operator";
                }
            }
            else
            {
                result.OperatorCode = "ZZZZ";
                result.OperatorName = "Unknown NOC Operator";
            }

            return result;
        }

        public static bool CheckDuplicate(IEnumerable<TXCSchedule> duplicates, TXCSchedule schedule)
        {
            foreach (TXCSchedule duplicate in duplicates)
            {
                if (schedule.Stops.FirstOrDefault().ATCOCode == duplicate.Stops.FirstOrDefault().ATCOCode && schedule.Stops.FirstOrDefault().DepartureTime == duplicate.Stops.FirstOrDefault().DepartureTime)
                {
                    if (schedule.Stops.LastOrDefault().ATCOCode == duplicate.Stops.LastOrDefault().ATCOCode && schedule.Stops.LastOrDefault().ArrivalTime == duplicate.Stops.LastOrDefault().ArrivalTime)
                    {
                        if (schedule.Line == duplicate.Line)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}