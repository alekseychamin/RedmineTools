using System;
using System.Collections.Generic;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace WinRedminePlaning
{
    static class LoadHours
    {

        public static bool IsOutDate(DateTime issueDate, int days = 0)
        {
            bool isMore = false;

            DateTime now = DateTime.Now;
            TimeSpan dt = (now - issueDate);

            if ((dt.TotalDays > days) & (days != 0))
                isMore = true;

            return isMore;
        }

        public static bool IsOutPercent(Issue issue, out float planPercent)
        {
            bool result = false;
            planPercent = 0;
            float curPercent = (float)issue.DoneRatio;
            DateTime curDate = DateTime.Now;
            TimeSpan dt1, dt2;

            if ((issue.DueDate != null) && ((curDate < issue.DueDate) & (curDate > issue.StartDate)))
            {
                dt1 = curDate - (DateTime)issue.StartDate;
                dt2 = (DateTime)issue.DueDate - (DateTime)issue.StartDate;

                if (dt2.TotalSeconds != 0)
                    planPercent = 100 * (float)(dt1.TotalSeconds / dt2.TotalSeconds);
                else
                    planPercent = 0;
            }
            else if (curDate >= issue.DueDate) planPercent = 100;
            else planPercent = 0;


            if ((planPercent - curPercent) > 10)
            {
                result = true;
            }

            return result;
        }

        public static bool IsItemInPlanActiveProject(Object item, List<Project> listProject = null)
        {
            bool isItemInPlanProject = false;
            bool isActiveProject = false;

            if (item is Project)
            {
                Project project = (Project)item;
                isActiveProject = (project.Status == ProjectStatus.Active);
                foreach (var customField in project.CustomFields)
                {
                    if (customField.Name.Equals("Учет при планировании"))
                    {
                        string res = customField.Values[0].Info;
                        if (res.Contains("1"))
                        {
                            isItemInPlanProject = true;                            
                            break;
                        }
                    }
                }
            }

            if (item is Issue)
            {
                Issue issue = (Issue)item;
                Project project = listProject.Find(x => x.Id == issue.Project.Id);
                if (project != null)
                {
                    isActiveProject = (project.Status == ProjectStatus.Active);
                    foreach (var customField in project.CustomFields)
                    {
                        if (customField.Name.Equals("Учет при планировании"))
                        {
                            string res = customField.Values[0].Info;
                            if (res.Contains("1"))
                            {
                                isItemInPlanProject = true;
                                break;
                            }
                        }
                    }
                }                
            }

            if (item is UserTimeEntry)
            {
                UserTimeEntry timeEntry = (UserTimeEntry)item;
                Project project = listProject.Find(x => x.Id == timeEntry.time.Project.Id);
                if (project != null)
                {
                    isActiveProject = (project.Status == ProjectStatus.Active);
                    foreach (var customField in project.CustomFields)
                    {
                        if (customField.Name.Equals("Учет при планировании"))
                        {
                            string res = customField.Values[0].Info;
                            if (res.Contains("1"))
                            {
                                isItemInPlanProject = true;
                                break;
                            }
                        }
                    }
                }
            }
            return (isItemInPlanProject & isActiveProject);
        }        

        public static bool IsItemInMonth(Object item, DateTime dateStartMonth, DateTime dateFinishMonth)
        {
            bool result = false;

            DateTime dateStartItem = DateTime.MinValue, dateFinishItem = DateTime.MinValue;

            if (item is Issue)
            {
                Issue issue = (Issue)item;
                dateStartItem = (DateTime)issue.StartDate;
                dateFinishItem = (DateTime)issue.DueDate;
            }

            if (item is UserTimeEntry)
            {
                UserTimeEntry userTimeEntry = (UserTimeEntry)item;
                dateStartItem = userTimeEntry.dateStartTimeEntry;
                dateFinishItem = userTimeEntry.dateFinishTimeEntry;
            }            

            if ( ((dateStartItem.CompareTo(dateStartMonth) < 0) & (dateFinishItem.CompareTo(dateStartMonth)) < 0) ||
                 ((dateStartItem.CompareTo(dateFinishMonth) > 0) & (dateFinishItem.CompareTo(dateFinishMonth)) > 0) )
                result = false;
            else
                result = true;

            return result;
        }        

        public static int GetAmountWD(object item, DateTime dateStart, DateTime dateFinish)
        {
            int result = 0;
            bool overload = false;

            if (item is UserTimeEntry)
            {
                UserTimeEntry userTimeEntry = (UserTimeEntry)item;
                if (userTimeEntry.time.Activity.Name.Contains("Сверх"))
                {
                    overload = true;
                }
            }             

            if (overload)
            {
                result = (int)(dateFinish - dateStart).TotalDays + 1;
            }
            else
            {
                DateTime curDate = dateStart;

                while (curDate.CompareTo(dateFinish) <= 0)
                {
                    if ((curDate.DayOfWeek == DayOfWeek.Monday) ||
                        (curDate.DayOfWeek == DayOfWeek.Tuesday) ||
                        (curDate.DayOfWeek == DayOfWeek.Wednesday ||
                        (curDate.DayOfWeek == DayOfWeek.Thursday) ||
                        curDate.DayOfWeek == DayOfWeek.Friday))
                        result++;
                    curDate = curDate.AddDays(1);
                }
            }

            return result;
        }
        public static void GetYearsFromListIssue(List<int> listYear, List<Issue> listIssue)
        {
            if (listYear != null)
            {
                listYear.Clear();
                int dateYear, index;

                foreach (var issue in listIssue)
                {
                    dateYear = ((DateTime)issue.StartDate).Year;
                    index = listYear.FindIndex(x => x == dateYear);
                    if (index == -1)
                        listYear.Add(dateYear);

                    dateYear = ((DateTime)issue.DueDate).Year;
                    index = listYear.FindIndex(x => x == dateYear);
                    if (index == -1)
                        listYear.Add(dateYear);

                }
                listYear.Sort();
            }
        }

        public static void MakeDWH(DateTime dateStartMonth, DateTime dateFinishMonth, double estimateDayHours, 
                                   double factDayHours, DateTime dateStart, DateTime dateFinish, 
                                   List<LoadDWH> listLoadDWH)
        {
            DateTime curDate = dateStartMonth;
            LoadDWH loadDWH;

            while (curDate.CompareTo(dateFinishMonth) <= 0)
            {
                if ((curDate.DayOfWeek == DayOfWeek.Monday) ||
                    (curDate.DayOfWeek == DayOfWeek.Tuesday) ||
                    (curDate.DayOfWeek == DayOfWeek.Wednesday ||
                    (curDate.DayOfWeek == DayOfWeek.Thursday) ||
                    curDate.DayOfWeek == DayOfWeek.Friday))
                {
                    if ((curDate.CompareTo(dateStart) >= 0) &
                        (curDate.CompareTo(dateFinish) <= 0))
                    {
                        loadDWH = new LoadDWH(curDate, estimateDayHours, factDayHours);
                    }
                    else
                        loadDWH = new LoadDWH(curDate, 0, 0);
                }
                else
                {
                    loadDWH = new LoadDWH(curDate, 0, 0);
                }

                listLoadDWH.Add(loadDWH);
                curDate = curDate.AddDays(1);
            }
        }
        public static void SetSFHours(object item, DateTime dateStartMonth, DateTime dateFinishMonth,
                                      DateTime dateStartItem, DateTime dateFinishItem, float hoursItem,
                                      out DateTime dateStart, out DateTime dateFinish,
                                      out float hours, out float hoursDay)
        {
            // определение даты старта загрузки задания в текущем месяце
            dateStart = DateTime.MinValue;
            if (dateStartItem.CompareTo(dateStartMonth) <= 0)
                dateStart = dateStartMonth;
            else if (dateStartItem.CompareTo(dateStartMonth) > 0)
                dateStart = dateStartItem;

            // определение даты финиша загрузки задания в текущем месяце
            dateFinish = DateTime.MinValue;
            if (dateFinishItem.CompareTo(dateFinishMonth) >= 0)
                dateFinish = dateFinishMonth;
            else if (dateFinishItem.CompareTo(dateFinishMonth) < 0)
                dateFinish = dateFinishItem;

            // определение временных интервалов (необходимо добавить метод определения количества рабочих дней)                        
            //TimeSpan interval = (DateTime)issue.DueDate - (DateTime)issue.StartDate;
            int amountWH = LoadHours.GetAmountWD(item, dateStartItem, dateFinishItem);

            if (hoursItem == 0)
                hoursItem = 0;
            if (amountWH != 0)
                hoursDay = hoursItem / amountWH;
            else
                hoursDay = 0;

            amountWH = LoadHours.GetAmountWD(item, dateStart, dateFinish);
            hours = hoursDay * amountWH;
            //hours = hoursItem;
        }
    }

    public class ProjectInfo
    {
        public string ProjectName;
        public string Info;
    }

    public class MonthValueHours
    {
        public int CurYear { get; set; }
        public int CurMonth { get; set; }
        private List<MonthHours> listMonthHours;
        public int Value
        {
            get
            {
                int value = 160;

                if (listMonthHours != null)
                {
                    MonthHours monthHours = listMonthHours.Find(x => x.Year == CurYear);
                    if (monthHours != null)
                    {
                        if ((CurMonth > 0) & (CurMonth <= 12))
                        {
                            value = monthHours.Value[CurMonth];
                        }
                    }
                }
                return value;
            }
        }
        public MonthValueHours(List<MonthHours> listMonthHours)
        {
            this.listMonthHours = listMonthHours;
        }
    }
    public class MonthHours
    {
        public int Year { get; set; }
        public Dictionary<int, int> Value { get; }
        public MonthHours(string data)
        {
            Value = new Dictionary<int, int>();

            if (!data.Equals(""))
            {
                string[] array = data.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    int year = Convert.ToInt16(array[0]);
                    this.Year = year;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                for (int i = 1; i < array.Length; i++)
                {
                    string[] pair = array[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                    try
                    {
                        int key = Convert.ToInt16(pair[0]);
                        int hours = Convert.ToInt16(pair[1]);
                        Value.Add(key, hours);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }

    public class EmailMessage : IComparable
    {
        public List<string> ListToEmail { get; set; }
        public List<string> ListCCEmail { get; set; }
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }

        public EmailMessage()
        {
            ListToEmail = new List<string>();
            ListCCEmail = new List<string>();            
        }

        public int CompareTo(object obj)
        {
            EmailMessage emailMessage = obj as EmailMessage;
            return this.ProjectId.CompareTo(emailMessage.ProjectId);
        }
    }

    public class CSVLoadUserYWH : CSVLoadYWH
    {
        public string UserName { get; set; }        
        public string GroupName { get; set; }

    }
    
    public class CSVLoadProjectUserYWH : CSVLoadProjectYWH
    {
        public string UserName { get; set; }
    }

    public class CSVLoadProjectYWH :  CSVLoadYWH
    {
        public string NameProject { get; set; }
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
    }

    public class CSVLoadGroupYWH : CSVLoadYWH
    {
        public string NameGroup { get; set; }
        public int CountUser { get; set; }
    }

    public class CSVLoadYWH
    {
        public string plan_fact { get; set; }
        public int Year { get; set; }
        public double totalYWH_HWM { get; set; }
        public string measure { get; set; }
        public string dateReport { get; set; }

        public double m_1 { get; set; }
        public double m_2 { get; set; }
        public double m_3 { get; set; }
        public double m_4 { get; set; }
        public double m_5 { get; set; }
        public double m_6 { get; set; }
        public double m_7 { get; set; }
        public double m_8 { get; set; }
        public double m_9 { get; set; }
        public double m_10 { get; set; }
        public double m_11 { get; set; }
        public double m_12 { get; set; }

        public void SetMWH(TypeSave typeSave, string type, LoadYWH loadYWH)
        {
            this.plan_fact = type;
            this.Year = loadYWH.NumberYear;
            this.dateReport = DateTime.Now.ToString();

            if (type.Contains("план"))
            {
                switch (typeSave)
                {
                    case TypeSave.WorkHours:
                        this.totalYWH_HWM = loadYWH.EstimatedYWH;
                        this.measure = "час";
                        this.m_1 = loadYWH.listLoadMWH[0].EstimatedMonthHours;
                        this.m_2 = loadYWH.listLoadMWH[1].EstimatedMonthHours;
                        this.m_3 = loadYWH.listLoadMWH[2].EstimatedMonthHours;
                        this.m_4 = loadYWH.listLoadMWH[3].EstimatedMonthHours;
                        this.m_5 = loadYWH.listLoadMWH[4].EstimatedMonthHours;
                        this.m_6 = loadYWH.listLoadMWH[5].EstimatedMonthHours;
                        this.m_7 = loadYWH.listLoadMWH[6].EstimatedMonthHours;
                        this.m_8 = loadYWH.listLoadMWH[7].EstimatedMonthHours;
                        this.m_9 = loadYWH.listLoadMWH[8].EstimatedMonthHours;
                        this.m_10 = loadYWH.listLoadMWH[9].EstimatedMonthHours;
                        this.m_11 = loadYWH.listLoadMWH[10].EstimatedMonthHours;
                        this.m_12 = loadYWH.listLoadMWH[11].EstimatedMonthHours;
                        break;

                    case TypeSave.HumansMonth:
                        this.totalYWH_HWM = loadYWH.EstimatedYHumans;
                        this.measure = "чел.мес";
                        this.m_1 = loadYWH.listLoadMWH[0].EstimatedMontHumans;
                        this.m_2 = loadYWH.listLoadMWH[1].EstimatedMontHumans;
                        this.m_3 = loadYWH.listLoadMWH[2].EstimatedMontHumans;
                        this.m_4 = loadYWH.listLoadMWH[3].EstimatedMontHumans;
                        this.m_5 = loadYWH.listLoadMWH[4].EstimatedMontHumans;
                        this.m_6 = loadYWH.listLoadMWH[5].EstimatedMontHumans;
                        this.m_7 = loadYWH.listLoadMWH[6].EstimatedMontHumans;
                        this.m_8 = loadYWH.listLoadMWH[7].EstimatedMontHumans;
                        this.m_9 = loadYWH.listLoadMWH[8].EstimatedMontHumans;
                        this.m_10 = loadYWH.listLoadMWH[9].EstimatedMontHumans;
                        this.m_11 = loadYWH.listLoadMWH[10].EstimatedMontHumans;
                        this.m_12 = loadYWH.listLoadMWH[11].EstimatedMontHumans;
                        break;
                }                
            }

            if (type.Contains("факт"))
            {
                switch (typeSave)
                {
                    case TypeSave.WorkHours:
                        this.totalYWH_HWM = loadYWH.FactYWH;
                        this.measure = "час";
                        this.m_1 = loadYWH.listLoadMWH[0].FactMonthHours;
                        this.m_2 = loadYWH.listLoadMWH[1].FactMonthHours;
                        this.m_3 = loadYWH.listLoadMWH[2].FactMonthHours;
                        this.m_4 = loadYWH.listLoadMWH[3].FactMonthHours;
                        this.m_5 = loadYWH.listLoadMWH[4].FactMonthHours;
                        this.m_6 = loadYWH.listLoadMWH[5].FactMonthHours;
                        this.m_7 = loadYWH.listLoadMWH[6].FactMonthHours;
                        this.m_8 = loadYWH.listLoadMWH[7].FactMonthHours;
                        this.m_9 = loadYWH.listLoadMWH[8].FactMonthHours;
                        this.m_10 = loadYWH.listLoadMWH[9].FactMonthHours;
                        this.m_11 = loadYWH.listLoadMWH[10].FactMonthHours;
                        this.m_12 = loadYWH.listLoadMWH[11].FactMonthHours;
                        break;
                    case TypeSave.HumansMonth:
                        this.totalYWH_HWM = loadYWH.FactYHumans;
                        this.measure = "чел.мес";
                        this.m_1 = loadYWH.listLoadMWH[0].FactMontHumans;
                        this.m_2 = loadYWH.listLoadMWH[1].FactMontHumans;
                        this.m_3 = loadYWH.listLoadMWH[2].FactMontHumans;
                        this.m_4 = loadYWH.listLoadMWH[3].FactMontHumans;
                        this.m_5 = loadYWH.listLoadMWH[4].FactMontHumans;
                        this.m_6 = loadYWH.listLoadMWH[5].FactMontHumans;
                        this.m_7 = loadYWH.listLoadMWH[6].FactMontHumans;
                        this.m_8 = loadYWH.listLoadMWH[7].FactMontHumans;
                        this.m_9 = loadYWH.listLoadMWH[8].FactMontHumans;
                        this.m_10 = loadYWH.listLoadMWH[9].FactMontHumans;
                        this.m_11 = loadYWH.listLoadMWH[10].FactMontHumans;
                        this.m_12 = loadYWH.listLoadMWH[11].FactMontHumans;
                        break;
                }                
            }
        }

    }
    public interface IListLoadIssue
    {
        void GetListLoadIssue(List<Project> listProject, int numberYear);
    }
    public class LoadDWH
    {
        public DateTime date { get; }
        // добавить лист перечня задач на день listLoadIssue
        public double hoursDay { get; }
        public double factDayHours { get; }

        public LoadDWH(DateTime date, double estimatedDayHours = 0, double factDayHours = 0)
        {
            this.date = date;
            this.hoursDay = estimatedDayHours;
            this.factDayHours = factDayHours;
        }

    }  

    public class LoadGroup
    {
        private List<Issue> listIssue;
        public string name { get; }
        public int id { get; }
        public List<LoadUser> listLoadUser { get; set; }        
        public int CountUser
        {
            get
            {
                int count = 0;
                foreach (LoadUser loadUser in listLoadUser)
                {
                    if ((!loadUser.user.LastName.ToLower().Contains("оценка")) & 
                        (!loadUser.user.LastName.Equals(name)))
                        count++;
                }
                return count;
            }
        }

        public LoadGroup(string name, int id, List<Issue> listIssue)
        {
            this.name = name;
            this.id = id;
            this.listIssue = listIssue;

            listLoadUser = new List<LoadUser>();            
        }

    }

    public class UserTimeEntry
    {
        public TimeEntry time { get; }
        public string nameUser { get; }
        public string subject { get; }
        public string nameProject { get; }
        public DateTime dateStartTimeEntry { get; }
        public DateTime dateFinishTimeEntry { get; }

        private DateTime GetSFDateTimeEntry(TimeEntry time, string caption)
        {
            DateTime result = (DateTime)time.SpentOn;

            string res = "";
            foreach (var customField in time.CustomFields)
            {
                if (customField.Name.Contains(caption))
                {
                    res = customField.Values[0].Info;
                }
            }

            if (res != "")
                result = Convert.ToDateTime(res);

            return result;
        }

        public UserTimeEntry(TimeEntry time, List<Issue> listIssue, List<User> listUser)
        {
            this.time = time;

            if (time.Issue != null)
            {
                Issue issue = listIssue.Find(x => x.Id == time.Issue.Id);
                if (issue != null)
                    subject = issue.Subject;
                else
                    subject = time.Comments;
            }
            else
            {
                subject = time.Comments;
            }

            User user = listUser.Find(x => x.Id == time.User.Id);
            if (user != null)
                nameUser = user.LastName + " " + user.FirstName;
            else
                nameUser = "";

            nameProject = time.Project.Name;

            dateStartTimeEntry = GetSFDateTimeEntry(time, "Дата старта");
            dateFinishTimeEntry = GetSFDateTimeEntry(time, "Дата завершения");

        }
    }
    public class LoadTimeEntry: IComparable
    {
        public UserTimeEntry userTime { get; }
        
        public List<LoadDWH> listLoadDWH { get; }
        public DateTime dateStart { get; }
        public DateTime dateFinish { get; }
        public float factMonthHours { get; }
        public float hoursDay { get; }

        

        public LoadTimeEntry(UserTimeEntry userTime, DateTime dateStartMonth, DateTime dateFinishMonth)
        {
            this.userTime = userTime;             
            listLoadDWH = new List<LoadDWH>();

            

            // определение даты старат задачи и финиша, при условиях даты начала и конца месяца
            // для определения длительности задач, дата старта и финиша которых раньше даты начала месяца и
            // больше даты окончания месяца                     

            

            float hoursTimeEntry;
            DateTime _dateStartTimeEntry, _dateFinishTimeEntry;
            float _hoursTimeEntry, _hoursDayTimeEntry;

            if (userTime.time.Hours == 0)
                hoursTimeEntry = 0;
            else
                hoursTimeEntry = (float)userTime.time.Hours;

            LoadHours.SetSFHours(userTime, dateStartMonth, dateFinishMonth, 
                                 userTime.dateStartTimeEntry, userTime.dateFinishTimeEntry, hoursTimeEntry,
                                 out _dateStartTimeEntry, out _dateFinishTimeEntry, out _hoursTimeEntry, out _hoursDayTimeEntry);

            this.dateStart = _dateStartTimeEntry;
            this.dateFinish = _dateFinishTimeEntry;
            this.factMonthHours = _hoursTimeEntry;
            this.hoursDay = _hoursDayTimeEntry;

            LoadHours.MakeDWH(dateStartMonth, dateFinishMonth, 0, _hoursDayTimeEntry,
                              this.dateStart, this.dateFinish, this.listLoadDWH);


        }

        public int CompareTo(object obj)
        {
            LoadTimeEntry loadTime = obj as LoadTimeEntry;
            return this.userTime.nameProject.CompareTo(loadTime.userTime.nameProject);
        }
    }

    public class LoadIssue : IComparable
    {
        public Issue issue { get; }
        public DateTime dateStartIssue { get; }
        public DateTime dateFinishIssue { get; }

        public List<LoadDWH> listLoadDWH { get; }

        public string Name { get; }

        public double estimatedIssueHours { get; }
        public double estimatedDayIssueHours { get; }

        public string NameHeadUser { get; set; }

        public string EmailHeadUser { get; set; }

        public bool isExperied = false;
        public string Message = "";         
        private int duration;              

        public LoadIssue(Issue issue, DateTime dateStartMonth, DateTime dateFinishMonth)
        {            
            this.issue = issue;            
            listLoadDWH = new List<LoadDWH>();

            // определение даты старат задачи и финиша, при условиях даты начала и конца месяца
            // для определения длительности задач, дата старта и финиша которых раньше даты начала месяца и
            // больше даты окончания месяца

            DateTime dateStartIssue = (DateTime)issue.StartDate;
            DateTime dateFinishIssue = (DateTime)issue.DueDate;

            float estimatedHours;
            DateTime _dateStartIssue, _dateFinishIssue;
            float estimatedIssueHours, estimatedDayIssueHours;

            if (issue.EstimatedHours == null)
                estimatedHours = 0;
            else
                estimatedHours = (float)issue.EstimatedHours;

            LoadHours.SetSFHours(issue, dateStartMonth, dateFinishMonth, dateStartIssue, dateFinishIssue, estimatedHours,
                                 out _dateStartIssue, out _dateFinishIssue, out estimatedIssueHours, out estimatedDayIssueHours);

            this.dateStartIssue = _dateStartIssue;
            this.dateFinishIssue = _dateFinishIssue;
            this.estimatedIssueHours = estimatedIssueHours;
            this.estimatedDayIssueHours = estimatedDayIssueHours;

            LoadHours.MakeDWH(dateStartMonth, dateFinishMonth, estimatedDayIssueHours, 0, 
                              this.dateStartIssue, this.dateFinishIssue, this.listLoadDWH);                                           
        }

        public LoadIssue(Issue issue)
        {
            this.issue = issue;            
            this.DetermineLoadExpIssue();
        }        

        private void DetermineLoadExpIssue()
        {
            float percent;

            if (LoadHours.IsOutDate(issue.DueDate.Value, 1))
            {
                this.isExperied = true;
                SetMessage(1);
            }

            if (LoadHours.IsOutPercent(issue, out percent))
            {
                this.isExperied = true;
                SetMessage(2, percent);
            }
        }

        private void SetMessage(int type, float planPercent = 0)
        {
            if (this.Message == "")
            {                
                this.Message += "Проект: " + this.issue.Project.Name + "-> задание № " + this.issue.Id + ": " + this.issue.Subject.Trim() + "\n" + "->" +
                                        " дата начала: " + this.issue.StartDate.Value.ToShortDateString() + " -> дата завершения: " + this.issue.DueDate.Value.ToShortDateString() + "\n";                
            }

            switch (type)
            {
                case 1:
                    this.Message += "Дата завершения задачи просрочена на текущую дату." + "\n";                    
                    break;

                case 2:
                    this.Message += "Текущий процент выполнения по задаче не соответствует плановому " + "\n" + "-> плановый % выполнения = " + planPercent.ToString("0") + "%" + " -> текущий % выполнения = " + this.issue.DoneRatio + "%" + "\n";
                    break;
            }

            this.Message += "ссылка на задание в Redmine -> " + "http://188.242.201.77/issues/" + this.issue.Id.ToString() + "\n";
            this.Message += "\n";
        }

        public int CompareTo(object obj)
        {
            LoadIssue loadIssue = obj as LoadIssue;
            return this.issue.Project.Name.CompareTo(loadIssue.issue.Project.Name);
        }
    }

    public class LoadMWH : IListLoadIssue
    {
        public int numberMonth { get; }

        //private List<LoadUser> listLoadUser;

        public List<LoadDWH> listLoadDWH { get; } /// <summary>
        ///  в данном списке хранаться количество итоговых часов по дня в течение месяца
        /// </summary>
        public List<LoadIssue> listLoadIssue { get; }
        public List<LoadTimeEntry> listLoadTimeEntry { get; }

        public List<LoadProject> listLoadProject { get; }

        public DateTime dateStartMonth { get; }
        public DateTime dateFinishMonth { get; }
        public DateTime dateStartLoad { get; }
        public DateTime dateFinishLoad { get; }

        public RedmineData redmineData;

        private List<Issue> listIssue;
        private List<Project> listProject;
        private List<UserTimeEntry> listUserTimeEntry;

        private double estimatedMonthHours;
        private double factHoursMonth;
        private double maxHumansHours;
        private double minHumansHours = 0;
        public int monthHours { get; }

        public Color MonthEstimatedColor
        {
            get
            {
                Color color = Color.White;

                if (EstimatedMontHumans > maxHumansHours)
                    color = Color.Red;
                if (EstimatedMonthHours == minHumansHours)
                    color = Color.Yellow;

                return color;
            }
        }

        public Color MonthFactColor
        {
            get
            {
                Color color = Color.White;

                if (FactMontHumans > maxHumansHours)
                    color = Color.Red;
                if (FactMonthHours == minHumansHours)
                    color = Color.Yellow;

                return color;
            }
        }

        public double EstimatedMontHumans
        {
            get
            {
                double estimatedMonthHumans = 0;
                int countWD = LoadHours.GetAmountWD(null, this.dateStartMonth, this.dateFinishMonth);

                foreach (LoadIssue loadIssue in listLoadIssue)
                {
                    estimatedMonthHumans += loadIssue.estimatedIssueHours;
                }

                if (monthHours != 0)
                    estimatedMonthHumans /= monthHours;
                else estimatedMonthHumans = 0;

                return estimatedMonthHumans;
            }
        }

        public double FactMontHumans
        {
            get
            {
                double factMonthHumans = 0;
                int countWD = LoadHours.GetAmountWD(null, this.dateStartMonth, this.dateFinishMonth);

                foreach (LoadTimeEntry loadTimeEntry in listLoadTimeEntry)
                {
                    factMonthHumans += loadTimeEntry.factMonthHours;
                }

                if (monthHours != 0)
                    factMonthHumans /= monthHours;
                else factMonthHumans = 0;

                return factMonthHumans;
            }
        }

        public double EstimatedMonthHours
        {
            get
            {
                estimatedMonthHours = 0;
                foreach (LoadIssue loadIssue in listLoadIssue)
                {
                    estimatedMonthHours = estimatedMonthHours + loadIssue.estimatedIssueHours;
                }
                return estimatedMonthHours;
            }
        }

        public double FactMonthHours
        {
            get
            {
                factHoursMonth = 0;

                foreach (LoadTimeEntry loadTimeEntry in listLoadTimeEntry)
                {
                    factHoursMonth += loadTimeEntry.factMonthHours;
                }

                return factHoursMonth;
            }
        }

        public LoadMWH(RedmineData redmineData, double maxHumansHours, int numberMonth, DateTime dateStartMonth,
                       DateTime dateFinishMonth)
        {
            this.maxHumansHours = maxHumansHours;
            this.numberMonth = numberMonth;
            this.redmineData = redmineData;
            this.listIssue = redmineData.listIssue;
            this.listProject = redmineData.listProject;
            this.listUserTimeEntry = redmineData.listUserTimeEntry;           
            this.dateStartMonth = dateStartMonth;
            this.dateFinishMonth = dateFinishMonth;

            redmineData.monthValueHours.CurMonth = numberMonth;
            redmineData.monthValueHours.CurYear = dateStartMonth.Year;
            this.monthHours = redmineData.monthValueHours.Value;

            listLoadDWH = new List<LoadDWH>();
            listLoadIssue = new List<LoadIssue>();
            listLoadTimeEntry = new List<LoadTimeEntry>();
            listLoadProject = new List<LoadProject>();        
        }

        public void MakeDWHSum()
        {
            DateTime curDate = this.dateStartMonth;
            double sumEstimatedDayHours = 0;
            double sumFactDaysHours = 0;
            listLoadDWH.Clear();

            while (curDate.CompareTo(this.dateFinishMonth) <= 0)
            {
                sumEstimatedDayHours = 0;
                sumFactDaysHours = 0;

                foreach (LoadIssue loadIssue in this.listLoadIssue)
                {
                    LoadDWH loadDWH = loadIssue.listLoadDWH.Find(x => x.date.Equals(curDate));
                    if (loadDWH != null)                    
                        sumEstimatedDayHours += loadDWH.hoursDay;                                            
                }

                foreach (LoadTimeEntry loadTime in this.listLoadTimeEntry)
                {
                    LoadDWH loadDWH = loadTime.listLoadDWH.Find(x => x.date.Equals(curDate));
                    if (loadDWH != null)
                        sumFactDaysHours += loadDWH.factDayHours;
                }

                LoadDWH curLoadDWH = new LoadDWH(curDate, sumEstimatedDayHours, sumFactDaysHours);
                listLoadDWH.Add(curLoadDWH);
                curDate = curDate.AddDays(1);
            }            
        }

        public void GetListLoadIssue(List<Project> listProject, int numberYear)
        {
            listLoadIssue.Clear();           

            foreach (var issue in listIssue)
            {
                if (LoadHours.IsItemInMonth(issue, dateStartMonth, dateFinishMonth) & 
                    LoadHours.IsItemInPlanActiveProject(issue, listProject))
                {                    
                    LoadIssue loadIssue = new LoadIssue(issue, dateStartMonth, dateFinishMonth);
                    listLoadIssue.Add(loadIssue);
                }
            }
        }       

        public void GetListLoadTime(List<Project> listProject, int numberYear)
        {
            listLoadTimeEntry.Clear();

            foreach (UserTimeEntry userTimeEntry in listUserTimeEntry)
            {
                if (LoadHours.IsItemInMonth(userTimeEntry, dateStartMonth, dateFinishMonth) &
                    LoadHours.IsItemInPlanActiveProject(userTimeEntry, listProject))
                {
                    LoadTimeEntry loadTimeEntry = new LoadTimeEntry(userTimeEntry, dateStartMonth, dateFinishMonth);
                    listLoadTimeEntry.Add(loadTimeEntry);
                }
            }
        }        

        public void GetListLoadTime(List<Project> listProject, int numberYear, LoadUser loadUser, List<LoadUser> listLoadUser)
        {
            listLoadTimeEntry.Clear();

            bool isGroup = false;
            if (loadUser.user.LastName.Equals(loadUser.GroupName))
                isGroup = true;

            foreach (var userTimeEntry in listUserTimeEntry)
            {
                if (isGroup)
                {
                    if (LoadHours.IsItemInMonth(userTimeEntry, dateStartMonth, dateFinishMonth) &
                        LoadHours.IsItemInPlanActiveProject(userTimeEntry, listProject))
                    {
                        LoadUser findLoadUser = listLoadUser.Find(x => x.user.Id == userTimeEntry.time.User.Id);
                        if (findLoadUser != null)
                        {
                            LoadTimeEntry loadTimeEntry = new LoadTimeEntry(userTimeEntry, dateStartMonth, dateFinishMonth);
                            listLoadTimeEntry.Add(loadTimeEntry);
                        }
                    }
                }
                else
                {
                    if (LoadHours.IsItemInMonth(userTimeEntry, dateStartMonth, dateFinishMonth) &
                        LoadHours.IsItemInPlanActiveProject(userTimeEntry, listProject) &
                       (userTimeEntry.time.User.Id == loadUser.user.Id))
                    {
                        LoadTimeEntry loadTimeEntry = new LoadTimeEntry(userTimeEntry, dateStartMonth, dateFinishMonth);
                        listLoadTimeEntry.Add(loadTimeEntry);
                    }
                }
            }
        }

        public void GetListLoadTime(List<Project> listProject, LoadProject loadProject, 
                                    int numberYear, LoadUser loadUser, List<LoadUser> listLoadUser)
        {
            listLoadTimeEntry.Clear();

            bool isGroup = false;
            if (loadUser.user.LastName.Equals(loadUser.GroupName))
                isGroup = true;

            foreach (var userTimeEntry in listUserTimeEntry)
            {
                if (isGroup)
                {
                    if (LoadHours.IsItemInMonth(userTimeEntry, dateStartMonth, dateFinishMonth) &
                        LoadHours.IsItemInPlanActiveProject(userTimeEntry, listProject))
                    {
                        LoadUser findLoadUser = listLoadUser.Find(x => x.user.Id == userTimeEntry.time.User.Id);
                        if (findLoadUser != null)
                        {
                            LoadTimeEntry loadTimeEntry = new LoadTimeEntry(userTimeEntry, dateStartMonth, dateFinishMonth);
                            listLoadTimeEntry.Add(loadTimeEntry);
                        }
                    }
                }
                else
                {
                    if (LoadHours.IsItemInMonth(userTimeEntry, dateStartMonth, dateFinishMonth) &
                        LoadHours.IsItemInPlanActiveProject(userTimeEntry, listProject) &
                       (userTimeEntry.time.User.Id == loadUser.user.Id) & (userTimeEntry.time.Project.Id == loadProject.userProject.Id))
                    {
                        LoadTimeEntry loadTimeEntry = new LoadTimeEntry(userTimeEntry, dateStartMonth, dateFinishMonth);
                        listLoadTimeEntry.Add(loadTimeEntry);
                    }
                }
            }
        }

        public void GetListLoadIssue(List<Project> listProject, int numberYear, LoadUser loadUser, List<LoadUser> listLoadUser)
        {
            listLoadIssue.Clear();

            bool isGroup = false;
            if (loadUser.user.LastName.Equals(loadUser.GroupName))
                isGroup = true;

            foreach (var issue in listIssue)
            {
                if (isGroup)
                {
                    if (LoadHours.IsItemInMonth(issue, dateStartMonth, dateFinishMonth) &
                        LoadHours.IsItemInPlanActiveProject(issue, listProject))
                    {
                        LoadUser findLoadUser = listLoadUser.Find(x => x.user.Id == issue.AssignedTo.Id);
                        if (findLoadUser != null)
                        {
                            LoadIssue loadIssue = new LoadIssue(issue, dateStartMonth, dateFinishMonth);
                            listLoadIssue.Add(loadIssue);
                        }
                    }
                }
                else
                {
                    if (LoadHours.IsItemInMonth(issue, dateStartMonth, dateFinishMonth) &
                        LoadHours.IsItemInPlanActiveProject(issue, listProject) &
                       (issue.AssignedTo.Id == loadUser.user.Id))
                    {
                        LoadIssue loadIssue = new LoadIssue(issue, dateStartMonth, dateFinishMonth);
                        listLoadIssue.Add(loadIssue);
                    }
                }
            }
        }

        public void GetListLoadIssue(List<Project> listProject, LoadProject loadProject, 
                                     int numberYear, LoadUser loadUser, List<LoadUser> listLoadUser)
        {
            listLoadIssue.Clear();

            bool isGroup = false;
            if (loadUser.user.LastName.Equals(loadUser.GroupName))
                isGroup = true;

            foreach (var issue in listIssue)
            {
                if (isGroup)
                {
                    if (LoadHours.IsItemInMonth(issue, dateStartMonth, dateFinishMonth) &
                        LoadHours.IsItemInPlanActiveProject(issue, listProject))
                    {
                        LoadUser findLoadUser = listLoadUser.Find(x => x.user.Id == issue.AssignedTo.Id);
                        if (findLoadUser != null)
                        {
                            LoadIssue loadIssue = new LoadIssue(issue, dateStartMonth, dateFinishMonth);
                            listLoadIssue.Add(loadIssue);
                        }
                    }
                }
                else
                {
                    if (LoadHours.IsItemInMonth(issue, dateStartMonth, dateFinishMonth) &
                        LoadHours.IsItemInPlanActiveProject(issue, listProject) &
                       (issue.AssignedTo.Id == loadUser.user.Id) & (issue.Project.Id == loadProject.userProject.Id))
                    {
                        LoadIssue loadIssue = new LoadIssue(issue, dateStartMonth, dateFinishMonth);
                        listLoadIssue.Add(loadIssue);
                    }
                }
            }
        }

        public void GetListLoadIssue(int numberYear, UserProject userProject)
        {
            listLoadIssue.Clear();
            foreach (Issue issue in listIssue)
            {
                if (LoadHours.IsItemInMonth(issue, dateStartMonth, dateFinishMonth) &                    
                   (issue.Project.Id == userProject.Id))
                {
                    LoadIssue loadIssue = new LoadIssue(issue, dateStartMonth, dateFinishMonth);
                    listLoadIssue.Add(loadIssue);
                }
            }
        }

        public void GetListLoadTime(int numberYear, UserProject userProject)
        {
            listLoadTimeEntry.Clear();
            foreach (var userTimeEntry in listUserTimeEntry)
            {
                if (LoadHours.IsItemInMonth(userTimeEntry, dateStartMonth, dateFinishMonth) &                    
                   (userTimeEntry.time.Project.Id == userProject.Id))
                {
                    LoadTimeEntry loadTimeEntry = new LoadTimeEntry(userTimeEntry, dateStartMonth, dateFinishMonth);
                    listLoadTimeEntry.Add(loadTimeEntry);
                }
            }
        }

        public void GetListLoadProject()
        {
            listLoadProject.Clear();

            foreach (LoadIssue loadIssue in listLoadIssue)
            {
                string Name = loadIssue.issue.Project.Name;
                int Id = loadIssue.issue.Project.Id;                
                LoadProject findLoadProject = listLoadProject.Find(x => x.userProject.Id == Id);
                if (findLoadProject == null)
                {
                    UserProject userProject = new UserProject(redmineData, Name, Id);
                    LoadProject loadProject = new LoadProject(redmineData, userProject, this.listLoadIssue, this.listLoadTimeEntry);
                    listLoadProject.Add(loadProject);
                }
            }

            foreach (LoadTimeEntry loadTime in listLoadTimeEntry)
            {
                string Name = loadTime.userTime.time.Project.Name;
                int Id = loadTime.userTime.time.Project.Id;                
                LoadProject findLoadProject = listLoadProject.Find(x => x.userProject.Id == Id);
                if (findLoadProject == null)
                {
                    UserProject userProject = new UserProject(redmineData, Name, Id);
                    LoadProject loadProject = new LoadProject(redmineData, userProject, this.listLoadIssue, this.listLoadTimeEntry);
                    listLoadProject.Add(loadProject);
                }
            }
        }
    }

    public class UserProject
    {
        public string Name { get; }
        public int Id { get; }
        public string EmailHeadUser
        {
            get
            {
                string emailHeadUser = "";

                if (headUser != null)
                    emailHeadUser = headUser.Email;

                return emailHeadUser;
            }
        }
        public string NameHeadUser
        {
            get
            {
                string nameHeadUser = "";

                if (headUser != null)
                    nameHeadUser = headUser.LastName + " " + headUser.FirstName;

                return nameHeadUser;
            }
        }
        private User headUser;
        public RedmineData redmineData;

        public UserProject(RedmineData redmineData, string Name, int Id)
        {
            this.Name = Name;
            this.Id = Id;
            this.redmineData = redmineData;
            GetHeadUser();
        }
        
        private void GetHeadUser()
        {
            Project project = redmineData.listProject.Find(x => x.Id == this.Id);
            if (project != null)
            {
                foreach (var customField in project.CustomFields)
                {
                    if (customField.Name.Equals("ТРП"))
                    {
                        string res = customField.Values[0].Info;

                        User user = redmineData.listUser.Find(x => x.Id.ToString().Equals(res));
                        if (user != null)
                        {
                            headUser = user;
                        }

                        //string[] names = res.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); 
                        //if (names.Length == 3)
                        //{
                        //    User user = redmineData.listUser.Find(x => x.LastName.Equals(names[2]) & 
                        //                                          x.FirstName.Equals(names[0] + " " + names[1]));
                        //    if (user != null)
                        //    {
                        //        headUser = user;
                        //    }
                        //}
                    }
                }
            }
        }        
    }

    public class LoadProject
    {        
        public UserProject userProject { get; }
        public List<LoadYWH> listLoadYWH { get; }
        public List<LoadUser> listLoadUser { get; }
        public List<LoadIssue> listLoadOpenIssue { get; }
        public DateTime StartProject { get; }
        public DateTime FinishProject { get; }
        public double PercentFinance(int year)
        {
        
            double percent = 0;
            double fact = 0;
            double estim = 0;

            LoadYWH loadYWH = listLoadYWH.Find(x => x.NumberYear == year);
            
            if (loadYWH != null)
            {
                fact += loadYWH.FactYWH;
                estim += loadYWH.EstimatedYWH;
            }

            if (estim != 0)                
                percent = (fact / estim) * 100;                

            return percent;
        
        }
        public double PercentWork(int year)
        {                        
            double percent = 0;
            double fact = 0;
            double estim = 0;

            LoadYWH loadYWH = listLoadYWH.Find(x => x.NumberYear == year);

            if (loadYWH != null)
            {
                estim += loadYWH.EstimatedYWH;                    
            }

            foreach (Issue issue in listIssue)
            {
                if (issue.Project.Id == this.userProject.Id)
                {
                    if (issue.EstimatedHours != null)
                        fact += (double)(issue.EstimatedHours.Value * issue.DoneRatio / 100);
                }
            }

            if (estim != 0)
                percent = (fact / estim) * 100;

            return percent;
            
        }

        public RedmineData redmineData;
                    
        private List<Issue> listIssue;
        private List<Issue> listOpenIssue;
        private List<UserTimeEntry> listUserTimeEntry;

        private List<LoadIssue> listLoadIssue;
        private List<LoadTimeEntry> listLoadTimeEntry;

        private DateTime startDate, finishDate;

        public bool isExperied
        {
            get
            {
                bool isEx = false;

                foreach (LoadIssue loadIssue in listLoadOpenIssue)
                {
                    if (loadIssue.isExperied)
                    {
                        isEx = true;
                        break;
                    }
                }

                return isEx;
            }
        }        

        public double EstimatedMWH
        {
            get
            {
                double estimatedMWH = 0;

                foreach (LoadIssue loadIssue in listLoadIssue)
                {
                    if (loadIssue.issue.Project.Id == userProject.Id)
                    {
                        estimatedMWH += loadIssue.estimatedIssueHours;
                    }
                }

                return estimatedMWH;
            }
        }

        public double FactMWH
        {
            get
            {
                double factMWH = 0;

                foreach (LoadTimeEntry loadTime in listLoadTimeEntry)
                {
                    if (loadTime.userTime.time.Project.Id == userProject.Id)
                    {
                        factMWH += loadTime.factMonthHours;
                    }
                }

                return factMWH;
            }
        }

        private void GetStartFinishDateProject(ref DateTime startDate, ref DateTime finishDate)
        {            
            startDate = DateTime.MaxValue;
            finishDate = DateTime.MinValue;

            foreach (Issue issue in listIssue)
            {
                if (issue.Project.Id == this.userProject.Id)
                {
                    DateTime start = (DateTime)issue.StartDate;
                    DateTime finish = (DateTime)issue.DueDate;

                    if (startDate.CompareTo(start) >=0)
                    {
                        startDate = start;
                    }
                    if (finishDate.CompareTo(finish) <=0)
                    {
                        finishDate = finish;
                    }                    
                }
            }            
        }

        public LoadProject(RedmineData redmineData, UserProject project, List<LoadIssue> listLoadIssue, List<LoadTimeEntry> listLoadTimeEntry)
        {
            this.userProject = project;
            this.redmineData = redmineData;
            this.listLoadIssue = listLoadIssue;
            this.listLoadTimeEntry = listLoadTimeEntry;
        }

        public LoadProject(RedmineData redmineData, UserProject project)
        {
            this.userProject = project;
            this.redmineData = redmineData;
            this.listIssue = redmineData.listIssue;
            this.listOpenIssue = redmineData.listOpenIssue;
            this.listUserTimeEntry = redmineData.listUserTimeEntry;
            this.listLoadYWH = new List<LoadYWH>();
            this.listLoadUser = new List<LoadUser>();
            this.listLoadOpenIssue = new List<LoadIssue>();

            GetStartFinishDateProject(ref startDate, ref finishDate);
            GetLoadOpenIssue();
            CreateListLoadUser();
            this.StartProject = startDate;
            this.FinishProject = finishDate;
        }

        private void CreateListLoadUser()
        {            
            foreach (Issue issue in redmineData.listIssue)
            {
                if (issue.Project.Id == userProject.Id)
                {
                    User user = redmineData.listUser.Find(x => x.Id == issue.AssignedTo.Id);                    
                    LoadUser loadUser = this.listLoadUser.Find(x => x.user.Id == issue.AssignedTo.Id);
                    if ((user != null) & (loadUser == null))
                    {
                        loadUser = new LoadUser(redmineData, user);
                        this.listLoadUser.Add(loadUser);
                    }
                }
            }            
        }

        private void GetLoadOpenIssue()
        {
            foreach (Issue issue in listOpenIssue)
            {
                if (issue.Project.Id == this.userProject.Id)
                {
                    LoadIssue loadIssue = new LoadIssue(issue);
                    listLoadOpenIssue.Add(loadIssue);
                }
            }
        }

        public void AddYear(double maxYHH, double maxMYH, int year)
        {
            LoadYWH loadYWH = new LoadYWH(redmineData, maxYHH, year);
            loadYWH.MakeMonth(maxMYH, this.userProject);
            listLoadYWH.Add(loadYWH);

            double maxYearHumansHours = 1 * 12;
            double maxMonthHumansHours = 1;

            foreach (LoadUser loadUser in listLoadUser)
            {                
                loadUser.AddYear(maxYearHumansHours, maxMonthHumansHours, year, redmineData.listProject, this, listLoadUser);                
            }
        }        
    }

    public class LoadUser : IComparable
    {
        public User user { get; }

        public bool isExperied
        {
            get
            {                
                bool isEx = false;
                             
                foreach (LoadIssue loadIssue in listLoadOpenIssue)
                {
                    if (loadIssue.isExperied)
                    {
                        isEx = true;
                        break;
                    }
                }                

                return isEx;
            }
        }        

        public List<UserGroupRedmine> listGroup { get; }

        public List<LoadYWH> listLoadYWH { get; }

        public List<LoadIssue> listLoadOpenIssue { get; }

        public string GroupName
        {
            get
            {
                string nameGroup = "";

                foreach (UserGroupRedmine group in listGroup)
                {
                    nameGroup += group.name;
                }

                return nameGroup;
            }
        }
        public RedmineData redmineData;        
        private List<Issue> listIssue;
        private List<Issue> listOpenIssue;
        private List<UserTimeEntry> listUserTimeEntry;        

        public LoadUser(RedmineData redmineData, User user)
        {
            this.user = user;
            this.listLoadYWH = new List<LoadYWH>();
            this.listGroup = new List<UserGroupRedmine>();
            this.listLoadOpenIssue = new List<LoadIssue>();            
            this.redmineData = redmineData;
            this.listIssue = redmineData.listIssue;
            this.listOpenIssue = redmineData.listOpenIssue;
            this.listUserTimeEntry = redmineData.listUserTimeEntry;
            GetLoadOpenIssue();
        }

        private void GetLoadOpenIssue()
        {
            listLoadOpenIssue.Clear();

            foreach (Issue issue in listOpenIssue)
            {
                if (issue.AssignedTo.Id == this.user.Id)
                {
                    LoadIssue loadIssue = new LoadIssue(issue);
                    listLoadOpenIssue.Add(loadIssue);
                }
            }
        }

        public void AddYear(double maxYHH, double maxMYH, int year, List<Project> listProject, List<LoadUser> listLoadUser)
        {
            LoadYWH loadYWH = new LoadYWH(redmineData, maxYHH, year);
            loadYWH.MakeMonth(maxMYH, listProject, this, listLoadUser);
            listLoadYWH.Add(loadYWH);                        
        }

        public void AddYear(double maxYHH, double maxMYH, int year, List<Project> listProject, 
                            LoadProject loadProject, List<LoadUser> listLoadUser)
        {
            LoadYWH loadYWH = new LoadYWH(redmineData, maxYHH, year);
            loadYWH.MakeMonth(maxMYH, listProject, loadProject, this, listLoadUser);
            listLoadYWH.Add(loadYWH);
        }

        public int CompareTo(object obj)
        {
            LoadUser loadUser = obj as LoadUser;
            return this.GroupName.CompareTo(loadUser.GroupName);
        }
    }

    public class LoadYWH
    {
        private double maxHumansHours;
        private double minHumansHours = 0;
        public int NumberYear { get; }
        public List<LoadMWH> listLoadMWH { get; }
        public RedmineData redmineData;

        public Color YearEstimatedColor
        {
            get
            {
                Color color = Color.White;

                if (EstimatedYHumans > maxHumansHours)
                    color = Color.Red;
                if (EstimatedYHumans == minHumansHours)
                    color = Color.Yellow;

                return color;
            }
        }
        public Color YearFactColor
        {
            get
            {
                Color color = Color.White;

                if (FactYHumans > maxHumansHours)
                    color = Color.Red;
                if (FactYHumans == minHumansHours)
                    color = Color.Yellow;

                return color;
            }
        }

        public double EstimatedYWH
        {
            get
            {
                double planYWH = 0;

                foreach (LoadMWH loadMWH in listLoadMWH)
                {
                    planYWH += loadMWH.EstimatedMonthHours;
                }

                return planYWH;
            }
        }

        public double EstimatedYHumans
        {
            get
            {
                double planYHumans = 0;

                foreach (LoadMWH loadMWH in listLoadMWH)
                {
                    planYHumans += loadMWH.EstimatedMontHumans;
                }

                return planYHumans;
            }
        }

        public double FactYHumans
        {
            get
            {
                double factYHumans = 0;

                foreach (LoadMWH loadMWH in listLoadMWH)
                {
                    factYHumans += loadMWH.FactMontHumans;
                }

                return factYHumans;
            }
        }

        public double FactYWH
        {
            get
            {
                double factYWH = 0;
                
                foreach (LoadMWH loadMWH in listLoadMWH)
                {
                    factYWH += loadMWH.FactMonthHours;
                }

                return factYWH;
            }
        }
        
        public double EstimatedPercentYHumans
        {
            get
            {
                double res = 0;

                double estYH = EstimatedYHumans;

                if ((estYH != 0) & (maxHumansHours != 0))
                    res = (estYH / maxHumansHours) * 100; 

                return res;
            }
        }

        public double FactPercentYHumans
        {
            get
            {
                double res = 0;

                double factYH = FactYHumans;

                if ((factYH != 0) & (maxHumansHours != 0))
                    res = (factYH / maxHumansHours) * 100;

                return res;
            }
        }

        private List<Issue> listIssue;
        private List<UserTimeEntry> listUserTimeEntry;
        
        public LoadYWH(RedmineData redmineData, double maxHumansHours, int numberYear)
        {
            this.maxHumansHours = maxHumansHours;
            this.NumberYear = numberYear;
            this.redmineData = redmineData;
            this.listIssue = redmineData.listIssue;
            this.listUserTimeEntry = redmineData.listUserTimeEntry;            
            listLoadMWH = new List<LoadMWH>();
        }        
                
        // добавить метод который будет из загруженных listLoadIssue по месяцам определять список загрузки специалистов
        public void MakeMonth(double maxHumansHours, List<Project> listProject)
        {
            listLoadMWH.Clear();                        

            for (int month = 1; month <= 12; month++)
            {
                DateTime dateStartMonth = new DateTime(NumberYear, month, 1);
                DateTime dateFinishMonth = dateStartMonth.AddMonths(1).AddDays(-1);
                LoadMWH loadMWH = new LoadMWH(redmineData, maxHumansHours, month, dateStartMonth, dateFinishMonth);
                loadMWH.GetListLoadIssue(listProject, NumberYear);
                loadMWH.GetListLoadTime(listProject, NumberYear);
                loadMWH.GetListLoadProject();
                listLoadMWH.Add(loadMWH);
                loadMWH.MakeDWHSum();
            }
        }        

        public void MakeMonth(double maxHumansHours, List<Project> listProject, LoadUser loadUser, List<LoadUser> listLoadUser)
        {
            if (loadUser != null)
            {
                listLoadMWH.Clear();

                for (int month = 1; month <= 12; month++)
                {
                    DateTime dateStartMonth = new DateTime(NumberYear, month, 1);
                    DateTime dateFinishMonth = dateStartMonth.AddMonths(1).AddDays(-1);
                    LoadMWH loadMWH = new LoadMWH(redmineData, maxHumansHours, month, dateStartMonth, dateFinishMonth);
                    loadMWH.GetListLoadIssue(listProject, NumberYear, loadUser, listLoadUser);
                    loadMWH.GetListLoadTime(listProject, NumberYear, loadUser, listLoadUser);
                    loadMWH.GetListLoadProject();
                    listLoadMWH.Add(loadMWH);
                    loadMWH.MakeDWHSum();
                }
            }
        }

        public void MakeMonth(double maxHumansHours, List<Project> listProject, LoadProject loadProject, 
                              LoadUser loadUser, List<LoadUser> listLoadUser)
        {
            if (loadUser != null)
            {
                listLoadMWH.Clear();

                for (int month = 1; month <= 12; month++)
                {
                    DateTime dateStartMonth = new DateTime(NumberYear, month, 1);
                    DateTime dateFinishMonth = dateStartMonth.AddMonths(1).AddDays(-1);
                    LoadMWH loadMWH = new LoadMWH(redmineData, maxHumansHours, month, dateStartMonth, dateFinishMonth);
                    loadMWH.GetListLoadIssue(listProject, loadProject, NumberYear, loadUser, listLoadUser);
                    loadMWH.GetListLoadTime(listProject, loadProject, NumberYear, loadUser, listLoadUser);
                    loadMWH.GetListLoadProject();
                    listLoadMWH.Add(loadMWH);
                    loadMWH.MakeDWHSum();
                }
            }
        }

        public void MakeMonth(double maxHumansHours, UserProject userProject)
        {
            listLoadMWH.Clear();

            for (int month = 1; month <= 12; month++)
            {
                DateTime dateStartMonth = new DateTime(NumberYear, month, 1);
                DateTime dateFinishMonth = dateStartMonth.AddMonths(1).AddDays(-1);
                LoadMWH loadMWH = new LoadMWH(redmineData, maxHumansHours, month, dateStartMonth, dateFinishMonth);
                loadMWH.GetListLoadIssue(NumberYear, userProject);
                loadMWH.GetListLoadTime(NumberYear, userProject);
                loadMWH.GetListLoadProject();
                listLoadMWH.Add(loadMWH);
                loadMWH.MakeDWHSum();
            }
        }

        public void MakeMonth(double maxHumansHours, UserProject userProject, int month)
        {
            listLoadMWH.Clear();
            
            DateTime dateStartMonth = new DateTime(NumberYear, month, 1);
            DateTime dateFinishMonth = dateStartMonth.AddMonths(1).AddDays(-1);
            LoadMWH loadMWH = new LoadMWH(redmineData, maxHumansHours, month, dateStartMonth, dateFinishMonth);
            loadMWH.GetListLoadIssue(NumberYear, userProject);
            loadMWH.GetListLoadTime(NumberYear, userProject);
            loadMWH.GetListLoadProject();
            listLoadMWH.Add(loadMWH);
            loadMWH.MakeDWHSum();
            
        }

        public LoadMWH FindLoadMWH(int month)
        {
            LoadMWH loadMWH = null;

            loadMWH = listLoadMWH.Find(x => x.numberMonth == month);

            return loadMWH;
        }
    }    
}
