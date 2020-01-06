using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinRedminePlaning
{
    public enum TypeDates { Start, Finish };
    static class ExtensionTimeEntry
    {
        private static DateTime FirstDateWork(TimeEntry time)
        {
            DateTime spentOn = (DateTime)time.SpentOn;
            //DateTime spentOn = DateStart;
            DateTime dateFirstWork = new DateTime(spentOn.Year, spentOn.Month, 1);

            while ((dateFirstWork.DayOfWeek == DayOfWeek.Saturday) | (dateFirstWork.DayOfWeek == DayOfWeek.Sunday))
            {
                dateFirstWork = dateFirstWork.AddDays(1);
            }

            return dateFirstWork;
        }

        private static  DateTime LastDateWork(TimeEntry time)
        {
            DateTime firstDateWork = FirstDateWork(time);

            int lastDay = DateTime.DaysInMonth(firstDateWork.Year, firstDateWork.Month);
            DateTime lastWorkDay = new DateTime(firstDateWork.Year, firstDateWork.Month, lastDay);

            while ((lastWorkDay.DayOfWeek == DayOfWeek.Saturday) || (lastWorkDay.DayOfWeek == DayOfWeek.Sunday))
            {
                lastWorkDay = lastWorkDay.AddDays(-1);
            }

            return lastWorkDay;
        }

        public static void SetDateValue(this TimeEntry time, string value, DateTime date)
        {        
            foreach (var customField in time.CustomFields)
            {
                if (customField.Name.Contains(value))
                {
                    customField.Values[0].Info = date.ToString();
                    break;
                }
            }
        }

        public static DateTime GetDateValue(this TimeEntry time, string value, TypeDates typeDates)
        {
            
            string res = "";

            foreach (var customField in time.CustomFields)
            {
                if (customField.Name.Contains(value))
                {
                    res = customField.Values[0].Info;
                }
            }

            if (res != "")
            {
                DateTime date;
                date = Convert.ToDateTime(res);
                return date;
            }
            else
            {
                switch (typeDates)
                {
                    case TypeDates.Start:
                        return FirstDateWork(time);                        
                    case TypeDates.Finish:
                        return LastDateWork(time);                        
                }
                
            }

            return DateTime.Now;
        }        
    }
}
