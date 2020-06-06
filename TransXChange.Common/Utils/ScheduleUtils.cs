using System;
using System.Collections.Generic;
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
    }
}