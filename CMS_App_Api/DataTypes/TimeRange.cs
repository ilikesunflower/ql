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
        
        public string Format { get; }
        
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

        public TimeRange(int inputType, string dataTime)
        {
            if (2 == inputType)
            {
                const string format = "MM/yyyy";
                DateTime dataT = DateTime.ParseExact(dataTime, format, CultureInfo.InvariantCulture);
                Start = new DateTime(dataT.Year, dataT.Month, 1, 0, 0, 0);
                End = new DateTime(dataT.Year, dataT.Month, DateTime.DaysInMonth(dataT.Year, dataT.Month), 23, 59, 59);
            }
            else
            {
                const string format = "dd/MM/yyyy";
                DateTime dataT = DateTime.ParseExact(dataTime, format, CultureInfo.InvariantCulture);
                Start = dataT;
                End = dataT;
            }
        }

        public TimeRange(string dataTime)
        {
            CultureInfo enUs = new CultureInfo("en-US");
            if (DateTime.TryParseExact(dataTime, "dd/MM/yyyy", enUs, DateTimeStyles.None, out var dayMonthYear))
            {
                Start = dayMonthYear;
                End = dayMonthYear;
            }
            else
            {
                if (DateTime.TryParseExact(dataTime, "MM/yyyy", enUs, DateTimeStyles.None, out var monthYear))
                {
                    var timeEnd =  new DateTime(monthYear.Year, monthYear.Month, DateTime.DaysInMonth(monthYear.Year, monthYear.Month), 0, 0, 0);
                    if(timeEnd > DateTime.Now) return;
                    Start = new DateTime(monthYear.Year, monthYear.Month, 1, 0, 0, 0);
                    End =  new DateTime(monthYear.Year, monthYear.Month, DateTime.DaysInMonth(monthYear.Year, monthYear.Month), 23, 59, 59);;
                }
                else
                {
                    if (!DateTime.TryParseExact(dataTime, "yyyy", enUs, DateTimeStyles.None, out var year)) return;
                    Start = new DateTime(year.Year, 1, 1, 0, 0, 0);
                    End = new DateTime(year.Year, 12, 31, 23, 59, 59);
                }
            }
        }
        public TimeRange(string dataTime, out string format)
        {
            CultureInfo enUs = new CultureInfo("en-US");
            if (DateTime.TryParseExact(dataTime, "dd/MM/yyyy", enUs, DateTimeStyles.None, out var dayMonthYear))
            {
                Start = dayMonthYear;
                End = dayMonthYear;
                format = "dd/MM/yyyy";
            }
            else
            {
                if (DateTime.TryParseExact(dataTime, "MM/yyyy", enUs, DateTimeStyles.None, out var monthYear))
                {
                    Start = new DateTime(monthYear.Year, monthYear.Month, 1, 0, 0, 0);
                    End = new DateTime(monthYear.Year, monthYear.Month,
                        DateTime.DaysInMonth(monthYear.Year, monthYear.Month), 23, 59, 59);
                        
                    format = "MM/yyyy";
                }
                else
                {
                    if (!DateTime.TryParseExact(dataTime, "yyyy", enUs, DateTimeStyles.None, out var year))
                    {
                        format = null;
                        return;
                    }

                    Start = new DateTime(year.Year, 1, 1, 0, 0, 0);
                    End = new DateTime(year.Year, 12, 31, 23, 59, 59);
                    format = "yyyy";
                }
            }
        }
        public TimeRange(int year)
        {
            Start = new DateTime(year, 1, 1, 0, 0, 0);
            End = new DateTime(year, 12, 31, 23, 59, 59);
        }
        public TimeRange(string fromDate, string toDate)
        {
            CultureInfo enUs = new CultureInfo("en-US");
            if (
                DateTime.TryParseExact(fromDate, "dd/MM/yyyy", enUs, DateTimeStyles.None, out var start) &&
                DateTime.TryParseExact(toDate, "dd/MM/yyyy", enUs, DateTimeStyles.None, out var end)
            )
            {
                Start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                End = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
            }
            else
            {
                if (
                    DateTime.TryParseExact(fromDate, "MM/yyyy", enUs, DateTimeStyles.None, out var startMonthYear) &&
                    DateTime.TryParseExact(toDate, "MM/yyyy", enUs, DateTimeStyles.None, out var endMonthYear)
                )
                {
                    Start = new DateTime(startMonthYear.Year, startMonthYear.Month, 1, 0, 0, 0);
                    End = new DateTime(endMonthYear.Year, endMonthYear.Month,
                        DateTime.DaysInMonth(endMonthYear.Year, endMonthYear.Month), 23, 59, 59);
                }
                else
                {
                    if (!DateTime.TryParseExact(fromDate, "yyyy", enUs, DateTimeStyles.None, out var startYear) ||
                        !DateTime.TryParseExact(toDate, "yyyy", enUs, DateTimeStyles.None, out var endYear))
                        return;
                    Start = new DateTime(startYear.Year, 1, 1, 0, 0, 0);
                    End = new DateTime(endYear.Year, 12, 31, 23, 59, 59);
                }
            }
        }

        public TimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public TimeRange SamePeriod()
        {
            DateTime samePeriodStart = new DateTime(Start.Year - 1, Start.Month, Start.Day, 0, 0, 0);
            DateTime samePeriodEnd = new DateTime(End.Year - 1, End.Month, End.Day, 23, 59, 59);
            return new TimeRange(samePeriodStart, samePeriodEnd);
        }
        
        public TimeRange FromStartYear()
        {
            DateTime samePeriodStart = new DateTime(Start.Year , 1, 1, 0, 0, 0);
            DateTime samePeriodEnd = new DateTime(End.Year , End.Month, End.Day, 23, 59, 59);
            return new TimeRange(samePeriodStart, samePeriodEnd);
        }
        public TimeRange GetLastMonth()
        {
            int yearS = Start.Year;
            int yearE = End.Year;
            int lastMonthS = Start.Month - 1;
            int lastMonthE = End.Month - 1;

            if (lastMonthS == 0)
            {
                lastMonthS = 12;
                yearS -= 1;
            }
            if (lastMonthE == 0)
            {
                lastMonthE = 12;
                yearE -= 1;
            }
            
            DateTime samePeriodStart = new DateTime(yearS , lastMonthS, 1, 0, 0, 0);
            DateTime samePeriodEnd = new DateTime(yearE , lastMonthE, DateTime.DaysInMonth(yearE, lastMonthE), 23, 59, 59);
            return new TimeRange(samePeriodStart,samePeriodEnd);
        }
        public List<NumberDayOfWeek> GetNumberWeek()
        {
            List<NumberDayOfWeek> numberDayOfWeeks = new List<NumberDayOfWeek>();
            if (Start > End)
            {
                return numberDayOfWeeks;
            }
            DateTime start = new DateTime(Start.Ticks);
            do
            {
                if (start.DayOfWeek == DayOfWeek.Sunday)
                {
                    start = start.AddDays(1);
                    continue;
                }
                int week = CmsFunction.GetWeekFromDateTime(start);

                var curren = numberDayOfWeeks.FirstOrDefault(f => f.Week == week);
                if (curren == null)
                {
                    curren = new NumberDayOfWeek()
                    {
                        Week = week,
                        NumberOfDay = 0
                    };
                    numberDayOfWeeks.Add(curren);
                }
                
                curren.NumberOfDay += 1;
                
                start = start.AddDays(1);
            } while (start < End);
            return numberDayOfWeeks;
        }
    }

    public class NumberDayOfWeek
    {
        public int Week { get; set; }
        public int NumberOfDay { get; set; }
    }
}