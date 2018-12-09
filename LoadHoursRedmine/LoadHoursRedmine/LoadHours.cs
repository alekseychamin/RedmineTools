using System;
using System.Collections.Generic;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinRedminePlaning
{
    static class LoadHours
    {

        public static bool IsItemInPlanProject(Object item, List<Project> listProject)
        {
            bool isItemInPlanProject = false;

            if (item is Issue)
            {
                Issue issue = (Issue)item;
                Project project = listProject.Find(x => x.Id == issue.Project.Id);
                if (project != null)
                {
                    foreach (var customField in project.CustomFields)
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

            if (item is UserTimeEntry)
            {
                UserTimeEntry timeEntry = (UserTimeEntry)item;
                Project project = listProject.Find(x => x.Id == timeEntry.time.Project.Id);
                if (project != null)
                {
                    foreach (var customField in project.CustomFields)
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
            return isItemInPlanProject;
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

        public double estimatedIssueHours { get; }
        public double estimatedDayIssueHours { get; }        

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

        private List<Issue> listIssue;
        private List<UserTimeEntry> listUserTimeEntry;

        private double estimatedMonthHours;
        private double factHoursMonth;

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

                if (countWD != 0)
                    estimatedMonthHumans /= countWD * 8;
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

                if (countWD != 0)
                    factMonthHumans /= countWD * 8;
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

        public LoadMWH(int numberMonth, DateTime dateStartMonth,
                       DateTime dateFinishMonth, List<Issue> listIssue, List<UserTimeEntry> listUserTimeEntry)
        {
            this.numberMonth = numberMonth;
            this.listIssue = listIssue;
            this.listUserTimeEntry = listUserTimeEntry;           
            this.dateStartMonth = dateStartMonth;
            this.dateFinishMonth = dateFinishMonth;

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
                    LoadHours.IsItemInPlanProject(issue, listProject))
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
                    LoadHours.IsItemInPlanProject(userTimeEntry, listProject))
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
                        LoadHours.IsItemInPlanProject(userTimeEntry, listProject))
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
                        LoadHours.IsItemInPlanProject(userTimeEntry, listProject) &
                       (userTimeEntry.time.User.Id == loadUser.user.Id))
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
                        LoadHours.IsItemInPlanProject(issue, listProject))
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
                        LoadHours.IsItemInPlanProject(issue, listProject) &
                       (issue.AssignedTo.Id == loadUser.user.Id))
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
                UserProject userProject = new UserProject(Name, Id);
                LoadProject findLoadProject = listLoadProject.Find(x => x.userProject.Id == Id);
                if (findLoadProject == null)
                {
                    LoadProject loadProject = new LoadProject(userProject, listLoadIssue, listLoadTimeEntry);                    
                    listLoadProject.Add(loadProject);
                }
            }

            foreach (LoadTimeEntry loadTime in listLoadTimeEntry)
            {
                string Name = loadTime.userTime.time.Project.Name;
                int Id = loadTime.userTime.time.Project.Id;
                UserProject userProject = new UserProject(Name, Id);
                LoadProject findLoadProject = listLoadProject.Find(x => x.userProject.Id == Id);
                if (findLoadProject == null)
                {
                    LoadProject loadProject = new LoadProject(userProject, listLoadIssue, listLoadTimeEntry);
                    listLoadProject.Add(loadProject);
                }
            }
        }
    }

    public class UserProject
    {
        public string Name { get; }
        public int Id { get; }

        public UserProject(string Name, int Id)
        {
            this.Name = Name;
            this.Id = Id;
        }

    }

    public class LoadProject
    {        
        public UserProject userProject { get; }
        public List<LoadYWH> listLoadYWH { get; }
        public DateTime StartProject { get; }
        public DateTime FinishProject { get; }
                    
        private List<Issue> listIssue;
        private List<UserTimeEntry> listUserTimeEntry;

        private List<LoadIssue> listLoadIssue;
        private List<LoadTimeEntry> listLoadTimeEntry;

        private DateTime startDate, finishDate;

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

        public LoadProject(UserProject project, List<LoadIssue> listLoadIssue, List<LoadTimeEntry> listLoadTimeEntry)
        {
            this.userProject = project;
            this.listLoadIssue = listLoadIssue;
            this.listLoadTimeEntry = listLoadTimeEntry;
        }

        public LoadProject(UserProject project, List<Issue> listIssue, List<UserTimeEntry> listUserTimeEntry)
        {
            this.userProject = project;
            this.listIssue = listIssue;
            this.listUserTimeEntry = listUserTimeEntry;
            this.listLoadYWH = new List<LoadYWH>();

            GetStartFinishDateProject(ref startDate, ref finishDate);
            this.StartProject = startDate;
            this.FinishProject = finishDate;
        }

        public void AddYear(int year)
        {
            LoadYWH loadYWH = new LoadYWH(year, this.listIssue, this.listUserTimeEntry);
            loadYWH.MakeMonth(this.userProject);
            listLoadYWH.Add(loadYWH);
        }        
    }

    public class LoadUser : IComparable
    {
        public User user { get; }

        public List<UserGroupRedmine> listGroup { get; }

        public List<LoadYWH> listLoadYWH { get; }

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

        private List<Issue> listIssue;
        private List<UserTimeEntry> listUserTimeEntry;        

        public LoadUser(User user, List<Issue> listIssue, List<UserTimeEntry> listUserTimeEntry)
        {
            this.user = user;
            this.listLoadYWH = new List<LoadYWH>();
            this.listGroup = new List<UserGroupRedmine>();
            this.listIssue = listIssue;
            this.listUserTimeEntry = listUserTimeEntry;            
        }

        public void AddYear(int year, List<Project> listProject, List<LoadUser> listLoadUser)
        {
            LoadYWH loadYWH = new LoadYWH(year, listIssue, listUserTimeEntry);
            loadYWH.MakeMonth(listProject, this, listLoadUser);
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
        public int NumberYear { get; }
        public List<LoadMWH> listLoadMWH { get; }
        
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

        private List<Issue> listIssue;
        private List<UserTimeEntry> listUserTimeEntry;
        
        public LoadYWH(int numberYear, List<Issue> listIssue, List<UserTimeEntry> listUserTimeEntry)
        {
            this.NumberYear = numberYear;
            this.listIssue = listIssue;
            this.listUserTimeEntry = listUserTimeEntry;            
            listLoadMWH = new List<LoadMWH>();
        }        
                
        // добавить метод который будет из загруженных listLoadIssue по месяцам определять список загрузки специалистов
        public void MakeMonth(List<Project> listProject)
        {
            listLoadMWH.Clear();                        

            for (int month = 1; month <= 12; month++)
            {
                DateTime dateStartMonth = new DateTime(NumberYear, month, 1);
                DateTime dateFinishMonth = dateStartMonth.AddMonths(1).AddDays(-1);
                LoadMWH loadMWH = new LoadMWH(month, dateStartMonth, dateFinishMonth, listIssue, listUserTimeEntry);
                loadMWH.GetListLoadIssue(listProject, NumberYear);
                loadMWH.GetListLoadTime(listProject, NumberYear);
                loadMWH.GetListLoadProject();
                listLoadMWH.Add(loadMWH);
                loadMWH.MakeDWHSum();
            }
        }

        public void MakeMonth(List<Project> listProject, LoadUser loadUser, List<LoadUser> listLoadUser)
        {
            if (loadUser != null)
            {
                listLoadMWH.Clear();

                for (int month = 1; month <= 12; month++)
                {
                    DateTime dateStartMonth = new DateTime(NumberYear, month, 1);
                    DateTime dateFinishMonth = dateStartMonth.AddMonths(1).AddDays(-1);
                    LoadMWH loadMWH = new LoadMWH(month, dateStartMonth, dateFinishMonth, listIssue, listUserTimeEntry);
                    loadMWH.GetListLoadIssue(listProject, NumberYear, loadUser, listLoadUser);
                    loadMWH.GetListLoadTime(listProject, NumberYear, loadUser, listLoadUser);
                    loadMWH.GetListLoadProject();
                    listLoadMWH.Add(loadMWH);
                    loadMWH.MakeDWHSum();
                }
            }
        }

        public void MakeMonth(UserProject userProject)
        {
            listLoadMWH.Clear();

            for (int month = 1; month <= 12; month++)
            {
                DateTime dateStartMonth = new DateTime(NumberYear, month, 1);
                DateTime dateFinishMonth = dateStartMonth.AddMonths(1).AddDays(-1);
                LoadMWH loadMWH = new LoadMWH(month, dateStartMonth, dateFinishMonth, listIssue, listUserTimeEntry);
                loadMWH.GetListLoadIssue(NumberYear, userProject);
                loadMWH.GetListLoadTime(NumberYear, userProject);
                loadMWH.GetListLoadProject();
                listLoadMWH.Add(loadMWH);
                loadMWH.MakeDWHSum();
            }
        }

        public void MakeMonth(UserProject userProject, int month)
        {
            listLoadMWH.Clear();
            
            DateTime dateStartMonth = new DateTime(NumberYear, month, 1);
            DateTime dateFinishMonth = dateStartMonth.AddMonths(1).AddDays(-1);
            LoadMWH loadMWH = new LoadMWH(month, dateStartMonth, dateFinishMonth, listIssue, listUserTimeEntry);
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
