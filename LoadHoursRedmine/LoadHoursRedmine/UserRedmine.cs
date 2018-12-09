using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Types;

namespace WinRedminePlaning
{
    public class UserGroupRedmine
    {
        public string name { get; }
        public int id { get; }
        public UserGroupRedmine(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }
    public class UserIssueEntry : IComparable
    {
        public Issue issue;
        public Project project;
        public UserRedmine AssignedTo;
        
        public string IssueName
        {
            get
            {
                if (issue != null)
                {
                    return issue.Subject;

                    /*if (listIssue != null)
                    {
                        int Id = Value.Id;
                        Issue issue = listIssue.Find(x => x.Id == Id);
                        if (issue != null)
                        {
                            if (issue.Subject != "")
                                return issue.Subject;
                            else
                                return "";
                        }
                        else
                            return "";
                    }
                    else
                        return "";*/
                }
                else
                    return "";
            }
        }        
        
        public string HeadName
        {
            get
            {
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
                        foreach (var customField in issue.CustomFields)
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
        }

        public string ProjectName
        {
            get
            {
                if (project != null)
                {
                    return project.Name;
                }
                else
                    return "";
            }
        }

        //public string ProjectName
        //{
        //    get
        //    {
        //        if (issue != null)
        //        {
        //            if (issue.Project != null)
        //            {
        //                return issue.Project.Name;
        //            }
        //            else
        //                return "";
        //        }
        //        else
        //            return "";
        //    }
        //}        

        public DateTime DateStart
        {
            get
            {
                //return (DateTime)Value.SpentOn;
                if (issue != null)
                {
                    if (issue.StartDate != null)
                        return (DateTime)issue.StartDate;
                    else
                        return new DateTime();
                }
                else
                    return new DateTime();                               
            }
        }

        public DateTime DateFinish
        {
            get
            {
                if (issue != null)
                {
                    if (issue.DueDate != null)
                        return (DateTime)issue.DueDate;
                    else
                        return new DateTime();
                }
                else
                    return new DateTime();
            }
        }

        public DateTime DateFirstWork
        {
            get
            {
                DateTime spentOn = (DateTime)issue.DueDate;
                //DateTime spentOn = DateStart;
                DateTime dateFirstWork = new DateTime(spentOn.Year, spentOn.Month, 1);

                while ((dateFirstWork.DayOfWeek == DayOfWeek.Saturday) | (dateFirstWork.DayOfWeek == DayOfWeek.Sunday))
                {
                    dateFirstWork = dateFirstWork.AddDays(1);
                }

                return dateFirstWork;
            }
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

        public float EstimatedHours
        {
            get
            {
                if (issue.EstimatedHours != null)
                    return (float)issue.EstimatedHours;
                else
                    return 0;
            }
        }

        public UserIssueEntry(Issue issue, Project project, UserRedmine AssignedTo)
        {
            this.issue = issue;
            this.project = project;            
            this.AssignedTo = AssignedTo;
        }        

        public int CompareTo(object obj)
        {
            UserIssueEntry userIssueEntry = obj as UserIssueEntry;
            return this.DateStart.CompareTo(userIssueEntry.DateStart);
        }
    }
    public class UserRedmine : IComparable
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
                foreach (UserGroupRedmine userGroup in listUserGroup)
                {
                    if (result == "")
                        result += userGroup.name;
                    else
                        result += "; " + userGroup.name;
                }
                return result;                
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

        public List<UserIssueEntry> listUserIssueEntry = new List<UserIssueEntry>();
        public List<UserIssueEntry> listMounthUserIssueEntry = new List<UserIssueEntry>();
        public List<Issue> listIssue = null; //для сохранения ссылки на список, который хранится в классе manager
        public List<Project> listProject = null; //для сохранения ссылки на список, который хранится в классе manager
        public List<UserGroupRedmine> listUserGroup = new List<UserGroupRedmine>();
        public Dictionary<string, string> bossName;
    }
}
