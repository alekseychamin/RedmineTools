using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinRedminePlaning
{    
    class Manager
    {
        private static Manager instance;

        string host = "http://redmine.starsyst.com";
        string apiKey = "70b1a875928636d8d3895248309344ea2bca6a5f";

        RedmineManager redmineManager;
        private int maxMonthHours;
        private int countTotalRecord;
        private int curReadRecord;
        private TextProgressBar progressBar;

        public List<UserRedmine> listUserRedmine = new List<UserRedmine>();        
        public List<MonthHours> listMonthHours = new List<MonthHours>();
        public MonthValueHours monthValueHours;
        public List<Issue> listIssue = new List<Issue>();
        public List<Project> listProject = new List<Project>();
        public List<User> listUser = new List<User>();        
        public int MaxMonthHours
        {
            get
            {
                return maxMonthHours;
            }
        }
        public ExcelMethods excelMethods = new ExcelMethods();

        private Manager(TextProgressBar progressBar)
        {
            this.progressBar = progressBar;
            try
            {                
                redmineManager = new RedmineManager(host, apiKey);
                monthValueHours = new MonthValueHours(listMonthHours);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! " + ex.Message);
            }
        }

        public static Manager GetInstance(TextProgressBar progressBar)
        {
            if (instance == null)
                instance = new Manager(progressBar);

            return instance;
        }

        /*private bool checkName(string[] noNameForReport, string name)
        {
            bool res = false;

            foreach (string noName in noNameForReport)
            {
                if (noName.Equals(name))
                {
                    res = true;
                    break;
                }

            }

            return res;
        }*/
        private void SetInitProgBar(TextProgressBar progressBar, int startValue, int maxValue, int step)
        {
            progressBar.InvokeIfNeeded(delegate { progressBar.Value = startValue; });
            progressBar.InvokeIfNeeded(delegate { progressBar.Maximum = maxValue; });
            progressBar.InvokeIfNeeded(delegate { progressBar.Step = step; });
        }
        public void GetUserFromRedmine(Dictionary<string, string> bossName) //params string[] noNameForReport)
        {
            NameValueCollection parametr;
            countTotalRecord = 0;
            curReadRecord = 0;

            listIssue.Clear();
            listProject.Clear();
            listUserRedmine.Clear();
            listUser.Clear();

            progressBar.InvokeIfNeeded(delegate { progressBar.DisplayStyle = ProgressBarDisplayText.CustomText; });
            progressBar.InvokeIfNeeded(delegate { progressBar.CustomText = "Загрузка записей, подождите пожалуйста."; });
            progressBar.InvokeIfNeeded(delegate { progressBar.Minimum = 0; });

            
            SetInitProgBar(progressBar, 0, 5, 1);

            try
            {
                //parametr = new NameValueCollection { { "user_id", "*" } };
                //countTotalRecord += redmineManager.GetObjects<User>(parametr).Count;
                //progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                //progressBar.InvokeIfNeeded(delegate { progressBar.CustomText = "Список № " + progressBar.Value + "/5"; });

                //parametr = new NameValueCollection { { "group_id", "*" } };
                //countTotalRecord += redmineManager.GetObjects<Group>(parametr).Count;
                //progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                //progressBar.InvokeIfNeeded(delegate { progressBar.CustomText = "Список № " + progressBar.Value + "/5"; });

                //parametr = new NameValueCollection { { "status_id", "*" } };
                //countTotalRecord += redmineManager.GetObjects<Issue>(parametr).Count;
                //progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                //progressBar.InvokeIfNeeded(delegate { progressBar.CustomText = "Список № " + progressBar.Value + "/5"; });

                //parametr = new NameValueCollection { { "project_id", "*" } };
                //countTotalRecord += redmineManager.GetObjects<Project>(parametr).Count;
                //progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                //progressBar.InvokeIfNeeded(delegate { progressBar.CustomText = "Список № " + progressBar.Value + "/5"; });

                //parametr = new NameValueCollection { { "user_id", "*" } };
                //countTotalRecord += redmineManager.GetObjects<TimeEntry>(parametr).Count;
                //progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                //progressBar.InvokeIfNeeded(delegate { progressBar.CustomText = "Список № " + progressBar.Value + "/5"; });

                //progressBar.InvokeIfNeeded(delegate { progressBar.Value = 0; });
                //progressBar.InvokeIfNeeded(delegate { progressBar.Maximum = countTotalRecord; });
                //progressBar.InvokeIfNeeded(delegate { progressBar.Step = 1; });

                parametr = new NameValueCollection { { "user_id", "*" } };
                List<User> listUserRedm = redmineManager.GetObjects<User>(parametr);

                countTotalRecord = listUserRedm.Count;
                SetInitProgBar(progressBar, 0, countTotalRecord, 1);                
                foreach (var user in listUserRedm)
                {
                    UserRedmine userRedmine = new UserRedmine(this.monthValueHours);
                    userRedmine.bossName = bossName;
                    userRedmine.Value = user;
                    listUser.Add(user);
                    curReadRecord++;
                    progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                    progressBar.InvokeIfNeeded(delegate { progressBar.CustomText = "Запись № " + progressBar.Value +
                                                        "/ " + countTotalRecord; });
                    Debug.WriteLine(curReadRecord);

                    bool isNameWorkHour = false;
                    string res = "";
                    foreach (var customField in user.CustomFields)
                    {
                        if (customField.Name.Contains("Учет трудозатратах/месяц"))
                        {
                            res = customField.Values[0].Info;
                            if (res.ToLower().Contains("1"))
                                isNameWorkHour = true;                            
                        }
                    }

                    if (isNameWorkHour)
                    {
                        //userRedmine.listIssue = this.listIssue;
                        userRedmine.listProject = this.listProject;
                        listUserRedmine.Add(userRedmine);
                    }
                    

                    //parametr = new NameValueCollection { { "user_id", user.Id.ToString() } };
                    //int count = redmineManager.GetObjects<TimeEntry>(parametr).Count;
                    //if (count > 0)
                    //{
                    //    UserRedmine userRedmine = new UserRedmine();
                    //    userRedmine.Value = user;
                    //    listUserRedmine.Add(userRedmine);
                    //    Console.WriteLine("Name = {0}, Count time entry = {1}", user.LastName, count);
                    //}
                }

                parametr = new NameValueCollection { { "group_id", "*" } };
                List<Group> listGroupRedm = redmineManager.GetObjects<Group>(parametr);

                countTotalRecord = listGroupRedm.Count;
                SetInitProgBar(progressBar, 0, countTotalRecord, 1);                
                foreach (Group group in listGroupRedm)
                {
                    curReadRecord++;
                    progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                    progressBar.InvokeIfNeeded(delegate {progressBar.CustomText = "Запись № " + progressBar.Value +
                                                        "/ " + countTotalRecord;
                    });
                    Debug.WriteLine(curReadRecord);

                    UserGroup userGroup = new UserGroup(group.Name, group.Id);
                    listUserRedm = redmineManager.GetObjects<User>(new NameValueCollection { { "group_id", group.Id.ToString() } });
                    foreach (User user in listUserRedm)
                    {                        
                        UserRedmine userRedmine = listUserRedmine.Find(x => x.Value.Id == user.Id);
                        if (userRedmine != null)
                        {
                            userRedmine.listUserGroup.Add(userGroup);
                        }
                    }
                }

                parametr = new NameValueCollection { { "status_id", "*" } };
                List<Issue> listIssueRedm = redmineManager.GetObjects<Issue>(parametr);

                countTotalRecord = listIssueRedm.Count;
                SetInitProgBar(progressBar, 0, countTotalRecord, 1);                
                foreach (Issue issue in listIssueRedm)
                {
                    listIssue.Add(issue);
                    curReadRecord++;
                    progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                    progressBar.InvokeIfNeeded(delegate {progressBar.CustomText = "Запись № " + progressBar.Value +
                                                        "/ " + countTotalRecord;
                    });
                    Debug.WriteLine(curReadRecord);
                    if (issue.Id == 1435)
                    {
                        Issue issue_jornals = redmineManager.GetObject<Issue>(issue.Id.ToString(),
                                                                              new NameValueCollection { { "include", "journals" } });

                        foreach (var journal in issue_jornals.Journals)
                        {
                            string note = journal.Notes;
                            if (!note.Equals(""))
                            {
                                MonthHours monthHours = new MonthHours(note);
                                listMonthHours.Add(monthHours);
                            }
                        }
                    }
                }

                parametr = new NameValueCollection { { "project_id", "*" } };
                List<Project> listProjectRedm = redmineManager.GetObjects<Project>(parametr);

                countTotalRecord = listProjectRedm.Count;
                SetInitProgBar(progressBar, 0, countTotalRecord, 1);                
                foreach (Project project in listProjectRedm)
                {
                    listProject.Add(project);
                    curReadRecord++;
                    progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                    progressBar.InvokeIfNeeded(delegate {progressBar.CustomText = "Запись № " + progressBar.Value +
                                                        "/ " + countTotalRecord;
                    });
                    Debug.WriteLine(curReadRecord);
                    Debug.WriteLine("str # 249");
                }                
                
                
                parametr = new NameValueCollection { { "user_id", "*" } };
                List<TimeEntry> listTimeEntryRedm = redmineManager.GetObjects<TimeEntry>(parametr);

                countTotalRecord = listTimeEntryRedm.Count;
                SetInitProgBar(progressBar, 0, countTotalRecord, 1);
                foreach (var time in listTimeEntryRedm)
                {                    
                    UserRedmine userRedmine = listUserRedmine.Find(x => x.Value.Id == time.User.Id);
                    Project project = listProject.Find(x => x.Id == time.Project.Id);
                    curReadRecord++;
                    progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                    progressBar.InvokeIfNeeded(delegate {progressBar.CustomText = "Запись № " + progressBar.Value +
                                                        "/ " + countTotalRecord;
                    });
                    Debug.WriteLine(curReadRecord);
                    Debug.WriteLine("str # 269");
                    if (userRedmine != null)
                    {                        
                        if (project != null)
                        {
                            if (project.IsPublic)
                            {
                                UserTimeEntry userTimeEntry = new UserTimeEntry(userRedmine.listIssue, userRedmine.listProject, 
                                                                                userRedmine, listUser);
                                userTimeEntry.Value = time;
                                userRedmine.listUserTimeEntry.Add(userTimeEntry);
                            }
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error - " + ex.Message);
            }

            listUserRedmine.Sort();
        }

        private bool isExistExcelTimeEntry(UserTimeEntry userTimeEntry, List<ExcelUserTimeEntry> listExcelTimeEntry)
        {
            bool isEqual = false;

            ExcelUserTimeEntry excelUserTimeEntry = listExcelTimeEntry.Find(x => ((x.ProjectName == userTimeEntry.ProjectName) & 
                                                                                  (x.IssueName == userTimeEntry.IssueName) & 
                                                                                  (x.Comment == userTimeEntry.Comment) &
                                                                                  (x.ActivityName == userTimeEntry.ActivityName)));

            if (excelUserTimeEntry != null)
            {
                isEqual = true;                
            }                       

            return isEqual;
        }

        private void FindEqualTimeEntry(UserTimeEntry iUserTimeEntry, 
                                        List<UserTimeEntry> listUserTimeEntry, out ExcelUserTimeEntry excelTimeEntry)
        {            
            excelTimeEntry = new ExcelUserTimeEntry(iUserTimeEntry.ProjectName, iUserTimeEntry.IssueName, 
                                                    iUserTimeEntry.Comment, iUserTimeEntry.ActivityName, 
                                                    iUserTimeEntry.HeadName, iUserTimeEntry.DateStart, 
                                                    iUserTimeEntry.DateFinish, iUserTimeEntry.Hours);

            int iIndex = 0;
            while (iIndex < listUserTimeEntry.Count)
            {
                UserTimeEntry jUserTimeEntry = listUserTimeEntry[iIndex];
                if (((jUserTimeEntry.ProjectName == iUserTimeEntry.ProjectName) &
                    (jUserTimeEntry.IssueName == iUserTimeEntry.IssueName) &
                    (jUserTimeEntry.ActivityName == iUserTimeEntry.ActivityName) &
                    (jUserTimeEntry.Value.Id != iUserTimeEntry.Value.Id)) ||
                    
                    ((jUserTimeEntry.ProjectName == iUserTimeEntry.ProjectName) &
                    (jUserTimeEntry.Comment == iUserTimeEntry.Comment) & 
                    (jUserTimeEntry.ActivityName == iUserTimeEntry.ActivityName) &
                    (jUserTimeEntry.Value.Id != iUserTimeEntry.Value.Id)))
                {                    
                    if (jUserTimeEntry.DateFinish > excelTimeEntry.DateFinish)
                    {
                        excelTimeEntry.DateFinish = jUserTimeEntry.DateFinish;
                    }
                    excelTimeEntry.Hours = excelTimeEntry.Hours + jUserTimeEntry.Hours;
                }
                iIndex++;
            }            
        }

        public void GetExcelTimeEntry(UserRedmine userRedmine)
        {            
            userRedmine.listExcelUserTimeEntry.Clear();

            int iIndex = 0;

            while (iIndex < userRedmine.listMounthUserTimeEntry.Count)
            {                    
                UserTimeEntry iUserTimeEntry = userRedmine.listMounthUserTimeEntry[iIndex];
                if (!isExistExcelTimeEntry(iUserTimeEntry, userRedmine.listExcelUserTimeEntry))
                {
                    ExcelUserTimeEntry excelUserTimeEntry = null;
                    FindEqualTimeEntry(iUserTimeEntry, userRedmine.listMounthUserTimeEntry, out excelUserTimeEntry);

                    if (excelUserTimeEntry != null)
                    {
                        userRedmine.listExcelUserTimeEntry.Add(excelUserTimeEntry);
                    }
                }
                iIndex++;
            }                            
        }

        public void GetMounthUserTimeEntry(int year, int month)
        {
            if (month > 0 & month <= 12)
            {
                this.monthValueHours.CurYear = year;
                this.monthValueHours.CurMonth = month;
                foreach (UserRedmine userRedmine in listUserRedmine)
                {
                    if (userRedmine.BossName.Contains("Першин"))
                    {
                        int a = 0;
                    }

                    userRedmine.listMounthUserTimeEntry.Clear();
                    foreach (UserTimeEntry userTimeEntry in userRedmine.listUserTimeEntry)
                    {
                        if ( (userTimeEntry.DateStart.Month == month) & (userTimeEntry.DateStart.Year == year))
                        {
                            userRedmine.listMounthUserTimeEntry.Add(userTimeEntry);
                        }
                    }
                    userRedmine.listMounthUserTimeEntry.Sort();

                    GetExcelTimeEntry(userRedmine);                    
                }
            }
        }

        public int FindUserRedmine(string fullName, out UserRedmine userRedmine)
        {
            int Id = 0;
            UserRedmine user = null;

            foreach (UserRedmine _userRedmine in listUserRedmine)
            {
                if (_userRedmine.FullName.Equals(fullName))
                {
                    Id = _userRedmine.Value.Id;
                    user = _userRedmine;
                }
            }

            userRedmine = user;
            return Id;
        }
        
    }
}
