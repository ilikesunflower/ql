using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CMS_Lib.Util;

namespace CMS.DataTypes
{
    public class TimeRange
    {
        public DateTime Start { get; set; } = DateTime.Now;
        public DateTime End { get; set;} = DateTime.Now;

        private string Format { get; }
        
        public TimeRange(string timeFlow, string fromDate, string toDate)
        {
            DateTime start;
            DateTime end;
            string format;
            
            if ("days" == timeFlow)
            {
                 format = "dd/MM/yyyy";
                start = DateTime.ParseExact(fromDate, format, CultureInfo.InvariantCulture);
                end = DateTime.ParseExact(toDate, format, CultureInfo.InvariantCulture);
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
            }
            else if ("months" == timeFlow)
            {
                 format = "MM/yyyy";
                start = DateTime.ParseExact(fromDate, format, CultureInfo.InvariantCulture);
                end = DateTime.ParseExact(toDate, format, CultureInfo.InvariantCulture);
                start = new DateTime(start.Year, start.Month, 1, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, DateTime.DaysInMonth(end.Year, end.Month), 23, 59, 59);
            }
            else
            {
                format = "yyyy";
                start = new DateTime(Convert.ToInt32(fromDate), 1, 1, 1, 0, 0, 0);
                end = new DateTime(Convert.ToInt32(toDate), 12, 31, 23, 59, 59);
            }

            Start = start;
            End = end;
            Format = format;
        }

        public TimeRange(string fromDate, string toDate)
        {
            DateTime  start = DateTime.ParseExact(fromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime  end = DateTime.ParseExact(toDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
            end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
            Start = start;
            End = end;
        }
        public TimeRange(DateTime? fromDate, DateTime? toDate)
        {
            if (fromDate.HasValue && toDate.HasValue)
            {
                DateTime   start = new DateTime(fromDate.Value.Year, fromDate.Value.Month, fromDate.Value.Day, 0, 0, 0);
                DateTime  end = new DateTime(toDate.Value.Year, toDate.Value.Month, toDate.Value.Day, 23, 59, 59);
                Start = start;
                End = end;
            }
        }
    }
}