﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Types;

namespace WinRedminePlaning
{
    class ExcelUserTimeEntry
    {
        public string ProjectName;
        public string IssueName;
        public string Comment;
        public string ActivityName;
        public string HeadName;
        public DateTime DateStart;
        public DateTime DateFinish;
        public decimal Hours;

        public ExcelUserTimeEntry(string ProjectName, string IssueName, 
                                  string Comment, string ActivityName, string HeadName,
                                  DateTime DateStart, DateTime DateFinish, decimal Hours)
        {
            this.ProjectName = ProjectName;
            this.IssueName = IssueName;
            this.Comment = Comment;
            this.ActivityName = ActivityName;
            this.HeadName = HeadName;
            this.DateStart = DateStart;
            this.DateFinish = DateFinish;
            this.Hours = Hours;
        }
    }

    class UserGroup
    {
        public string name;
        public int id;
        public UserGroup(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }
    class UserTimeEntry : IComparable
    {
        public TimeEntry Value;
        public UserRedmine AssignedTo;
        private List<Issue> listIssue;
        private List<Project> listProject;
        public string IssueName
        {
            get
            {
                if (Value != null)
                {
                    if (Value.Issue != null)
                    {
                        if (listIssue != null)
                        {
                            int Id = Value.Issue.Id;
                            Issue issue = listIssue.Find(x => x.Id == Id);
                            if (issue != null)
                            {
                                return issue.Subject;
                            }
                            else
                                return "";
                        }
                        else
                            return "";
                        
                    }
                    else
                        return Comment;
                }
                else
                    return "";
            }
        }
        public string Comment
        {
            get
            {
                if (Value != null)
                {
                    if (Value.Comments != null)
                        return Value.Comments;
                    else
                        return "";
                }
                return "";
            }
        }
        
        public string HeadName
        {
            get
            {
                if (Value != null)
                {
                    if (Value.Project != null)
                    {
                        int Id = Value.Project.Id;
                        Project project = listProject.Find(x => x.Id == Id);
                        if (project != null)
                        {
                            string res = "";

                            foreach (var customField in project.CustomFields)
                            {
                                if (customField.Name.Contains("ТРП"))
                                {
                                    res = customField.Values[0].Info;
                                }
                            }

                            if (res.Contains(AssignedTo.Value.LastName))
                            {
                                res = AssignedTo.BossName;
                            }                                                        

                            if (res == "")
                            {
                                foreach (var customField in Value.CustomFields)
                                {
                                    if (customField.Name.Contains("Руководитель"))
                                    {
                                        res = customField.Values[0].Info;
                                    }
                                }
                            }

                            return res;
                        }
                        else return "";
                    }
                    else return "";
                }
                else return "";
            }
        }

        public string ProjectName
        {
            get
            {
                if (Value != null)
                {
                    if (Value.Project != null)
                    {
                        return Value.Project.Name;
                    }
                    else
                        return "";
                }
                else
                    return "";
            }
        }

        public string ActivityName
        {
            get
            {
                if (Value != null)
                {
                    if (Value.Activity != null)
                    {
                        return Value.Activity.Name;
                    } else
                        return "";
                } else
                    return "";
            }
        }

        public DateTime DateStart
        {
            get
            {
                //return (DateTime)Value.SpentOn;
                string res = "";
                foreach (var customField in Value.CustomFields)
                {
                    if (customField.Name.Contains("Дата старта"))
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
                    return DateFirstWork;
            }
        }

        public DateTime DateFinish
        {
            get
            {
                //DateTime stepDate = DateStart;
                //DateTime lastDate = GetLastWorkDay(stepDate.Year, stepDate.Month);

                //int countDay = (int)Hours / 8;

                //int i = 1;
                //while (i <= countDay)
                //{
                //    if (stepDate.Equals(lastDate))
                //        break;
                //    stepDate = stepDate.AddDays(1);
                //    if ((stepDate.DayOfWeek != DayOfWeek.Saturday) & (stepDate.DayOfWeek != DayOfWeek.Sunday))
                //        i++;
                //}

                //return stepDate;

                string res = "";
                DateTime lastWorkDay = GetLastWorkDay(DateFirstWork.Year, DateFirstWork.Month);

                foreach (var customField in Value.CustomFields)
                {
                    if (customField.Name.Contains("Дата завершения"))
                    {
                        res = customField.Values[0].Info;
                    }
                }

                if (res != "")
                {
                    DateTime date;
                    date = Convert.ToDateTime(res);
                    if (date.CompareTo(lastWorkDay) == 1)
                    {
                        date = lastWorkDay;
                    }
                    return date;
                }
                else
                    return lastWorkDay;

            }
        }

        public DateTime DateFirstWork
        {
            get
            {
                DateTime spentOn = (DateTime)Value.SpentOn;
                //DateTime spentOn = DateStart;
                DateTime dateFirstWork = new DateTime(spentOn.Year, spentOn.Month, 1);

                while ((dateFirstWork.DayOfWeek == DayOfWeek.Saturday) | (dateFirstWork.DayOfWeek == DayOfWeek.Sunday))
                {
                    dateFirstWork = dateFirstWork.AddDays(1);
                }

                return dateFirstWork;
            }
        }

        public decimal Hours
        {
            get
            {
                return Value.Hours;
            }
        }

        public UserTimeEntry(List<Issue> listIssue, List<Project> listProject, UserRedmine AssignedTo)
        {
            this.listIssue = listIssue;
            this.listProject = listProject;
            this.AssignedTo = AssignedTo;
        }

        public DateTime GetLastWorkDay(int year, int month)
        {
            int lastDay = DateTime.DaysInMonth(year, month);
            DateTime lastWorkDay = new DateTime(year, month, lastDay);

            while ((lastWorkDay.DayOfWeek == DayOfWeek.Saturday) || (lastWorkDay.DayOfWeek == DayOfWeek.Sunday))
            {
                lastWorkDay = lastWorkDay.AddDays(-1);
            }

            return lastWorkDay;
        }

        public int CompareTo(object obj)
        {
            UserTimeEntry userTimeEntry = obj as UserTimeEntry;
            return this.DateStart.CompareTo(userTimeEntry.DateStart);
        }
    }
    class UserRedmine : IComparable
    {
        public User Value;
        public string FullName
        {
            get { return Value.LastName + " " + Value.FirstName; }
        }

        public string ShortName
        {
            get
            {
                string res = Value.LastName + " ";

                string[] name = Value.FirstName.Split(' ');

                for (int i = 0; i < name.Length; i++)
                {
                    name[i] = name[i][0] + ".";
                    res += name[i];
                }

                return res;
            }
        }

        public string GroupName
        {
            get
            {
                string result = "";
                foreach (UserGroup userGroup in listUserGroup)
                {
                    if (result == "")
                        result += userGroup.name;
                    else
                        result += "; " + userGroup.name;
                }
                return result;                
            }
        }
        public decimal TotalMonthHours
        {
            get
            {
                decimal hours = 0;
                foreach (UserTimeEntry userTimeEntry in listMounthUserTimeEntry)
                {
                    hours += userTimeEntry.Hours;
                }
                return hours;
            }
        }

        public decimal TotalSeekHours
        {
            get
            {
                decimal hours = 0;
                foreach (UserTimeEntry userTimeEntry in listMounthUserTimeEntry)
                {
                    if (userTimeEntry.ActivityName.Contains("Больничный"))
                        hours += userTimeEntry.Hours;
                }
                return hours;
            }
        }

        public decimal TotalVacaitionHours
        {
            get
            {
                decimal hours = 0;
                foreach (UserTimeEntry userTimeEntry in listMounthUserTimeEntry)
                {
                    if (userTimeEntry.ActivityName.Contains("Отпуск"))
                        hours += userTimeEntry.Hours;
                }
                return hours;
            }
        }

        public decimal TotalFreeHours
        {
            get
            {
                decimal hours = 0;
                foreach (UserTimeEntry userTimeEntry in listMounthUserTimeEntry)
                {
                    if (userTimeEntry.ActivityName.Contains("Отгул"))
                        hours += userTimeEntry.Hours;
                }
                return hours;
            }
        }

        public decimal TotalTripHours
        {
            get
            {
                decimal hours = 0;
                foreach (UserTimeEntry userTimeEntry in listMounthUserTimeEntry)
                {
                    if (userTimeEntry.ActivityName.Contains("ПНР"))
                        hours += userTimeEntry.Hours;
                }
                return hours;
            }
        }

        public decimal TotalOverOfficeHours
        {
            get
            {
                decimal hours = 0;
                foreach (UserTimeEntry userTimeEntry in listMounthUserTimeEntry)
                {
                    if (userTimeEntry.ActivityName.Contains("Сверхурочные офис"))
                        hours += userTimeEntry.Hours;
                }
                return hours;
            }
        }

        public decimal TotalOverTripHours
        {
            get
            {
                decimal hours = 0;
                foreach (UserTimeEntry userTimeEntry in listMounthUserTimeEntry)
                {
                    if (userTimeEntry.ActivityName.Contains("Сверхурочные ПНР"))
                        hours += userTimeEntry.Hours;
                }
                return hours;
            }
        }

        public decimal TotalOfficeHours
        {
            get
            {
                decimal hours = 0;
                foreach (UserTimeEntry userTimeEntry in listMounthUserTimeEntry)
                {
                    if (!userTimeEntry.ActivityName.Contains("Сверхурочные ПНР") &
                        !userTimeEntry.ActivityName.Contains("Сверхурочные офис") &
                        !userTimeEntry.ActivityName.Contains("ПНР") &
                        !userTimeEntry.ActivityName.Contains("Отгул") &
                        !userTimeEntry.ActivityName.Contains("Отпуск") &
                        !userTimeEntry.ActivityName.Contains("Больничный"))
                        hours += userTimeEntry.Hours;
                }
                return hours;
            }
        }

        public decimal TotalWorkHours
        {
            get
            {
                decimal hours = 0;
                foreach (UserTimeEntry userTimeEntry in listMounthUserTimeEntry)
                {
                    if (!userTimeEntry.ActivityName.Contains("Отгул") &
                        !userTimeEntry.ActivityName.Contains("Отпуск") &
                        !userTimeEntry.ActivityName.Contains("Больничный"))
                        hours += userTimeEntry.Hours;
                }
                return hours;
            }
        }

        public string BossName
        {
            get
            {
                string res = "";

                if (bossName.ContainsKey(GroupName))
                {
                    res = bossName[GroupName];
                }

                return res;
            }
        }

        public int CompareTo(object obj)
        {
            UserRedmine userToCompare = obj as UserRedmine;
            return this.GroupName.CompareTo(userToCompare.GroupName);                
        }

        public List<UserTimeEntry> listUserTimeEntry = new List<UserTimeEntry>();
        public List<UserTimeEntry> listMounthUserTimeEntry = new List<UserTimeEntry>();        
        public List<ExcelUserTimeEntry> listExcelUserTimeEntry = new List<ExcelUserTimeEntry>();
        public List<Issue> listIssue = null; //для сохранения ссылки на список, который хранится в классе manager
        public List<Project> listProject = null; //для сохранения ссылки на список, который хранится в классе manager
        public List<UserGroup> listUserGroup = new List<UserGroup>();
        public Dictionary<string, string> bossName;
    }
}
