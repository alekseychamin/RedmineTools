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

        private void ClearLists()
        {
            listIssue.Clear();
            listProject.Clear();
            listUserRedmine.Clear();
            listUser.Clear();
        }
        /// <summary>
        /// разработка метода с применением технологии LINQ
        /// </summary>
        public void GetDataFromRedmine()
        {
            NameValueCollection parametr;
            ClearLists();

            progressBar.InvokeIfNeeded(delegate { progressBar.DisplayStyle = ProgressBarDisplayText.CustomText; });
            progressBar.InvokeIfNeeded(delegate { progressBar.CustomText = "Загрузка записей, подождите пожалуйста."; });
            progressBar.InvokeIfNeeded(delegate { progressBar.Minimum = 0; });

            SetInitProgBar(progressBar, 0, 5, 1);

            try
            {
                // получение списка пользователей redmine
                parametr = new NameValueCollection { { "user_id", "*" } };
                List<User> listUserRedm = redmineManager.GetObjects<User>(parametr);
                progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });

                // получение списка групп пользователей redmine
                parametr = new NameValueCollection { { "group_id", "*" } };
                List<Group> listGroupRedm = redmineManager.GetObjects<Group>(parametr);
                progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });

                // получение списка задач из redmine
                parametr = new NameValueCollection { { "created_on", ">=2019-01-01" } };//{ "status_id", "*" }
                List<Issue> listIssueRedm = redmineManager.GetObjects<Issue>(parametr);
                progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });

                // получение списка проектов из redmine
                parametr = new NameValueCollection { { "project_id", "*" } };
                List<Project> listProjectRedm = redmineManager.GetObjects<Project>(parametr);
                progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });

                // получение списка трудозатра из redmine
                parametr = new NameValueCollection { { "spent_on", ">=2019-01-01" } };//{ "user_id", "*" }
                List<TimeEntry> listTimeEntryRedm = redmineManager.GetObjects<TimeEntry>(parametr);
                progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string GetFirstDateCurYear()
        {
            int year = DateTime.Now.Year;
            DateTime firstDateCurYear = new DateTime(year, 1, 1);

            return firstDateCurYear.ToString("yyyy-MM-dd");
        }

        public void GetUserFromRedmine(Dictionary<string, string> bossName) //params string[] noNameForReport)
        {
            NameValueCollection parametr;
            countTotalRecord = 0;
            curReadRecord = 0;

            ClearLists();

            progressBar.InvokeIfNeeded(delegate { progressBar.DisplayStyle = ProgressBarDisplayText.CustomText; });
            progressBar.InvokeIfNeeded(delegate { progressBar.CustomText = "Загрузка записей, подождите пожалуйста."; });
            progressBar.InvokeIfNeeded(delegate { progressBar.Minimum = 0; });


            //SetInitProgBar(progressBar, 0, 5, 1);

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
                    if (user.IsCustomFieldEqual("Учет трудозатратах/месяц"))
                    {
                        listUser.Add(user);
                        //userRedmine.listIssue = this.listIssue;

                        UserRedmine userRedmine = new UserRedmine(this.monthValueHours);
                        userRedmine.bossName = bossName;
                        userRedmine.Value = user;
                        userRedmine.listProject = this.listProject;

                        curReadRecord++;
                        progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                        progressBar.InvokeIfNeeded(delegate
                        {
                            progressBar.CustomText = "Запись № " + progressBar.Value +
                          "/ " + countTotalRecord;
                        });

                        listUserRedmine.Add(userRedmine);

                        Debug.WriteLine(curReadRecord);
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
                    progressBar.InvokeIfNeeded(delegate
                    {
                        progressBar.CustomText = "Запись № " + progressBar.Value +
                       "/ " + countTotalRecord;
                    });


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

                parametr = new NameValueCollection { { "created_on", ">=" + GetFirstDateCurYear() } };//{ "status_id", "*" }

                List<Issue> listIssueRedm = redmineManager.GetObjects<Issue>(parametr);

                countTotalRecord = listIssueRedm.Count;
                SetInitProgBar(progressBar, 0, countTotalRecord, 1);
                foreach (Issue issue in listIssueRedm)
                {
                    listIssue.Add(issue);
                    curReadRecord++;
                    progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                    progressBar.InvokeIfNeeded(delegate
                    {
                        progressBar.CustomText = "Запись № " + progressBar.Value +
                       "/ " + countTotalRecord;
                    });
                    Debug.WriteLine(curReadRecord);
                    Debug.WriteLine(issue.CreatedOn.Value);
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
                    progressBar.InvokeIfNeeded(delegate
                    {
                        progressBar.CustomText = "Запись № " + progressBar.Value +
                       "/ " + countTotalRecord;
                    });
                }


                parametr = new NameValueCollection { { "spent_on", ">=" + GetFirstDateCurYear() } };//{ "user_id", "*" }
                List<TimeEntry> listTimeEntryRedm = redmineManager.GetObjects<TimeEntry>(parametr);

                //List<TimeEntry> listTimeEntryUnique = new List<TimeEntry>();

                TimeEntry timeUniqe;

                for (int i = 0; i < listTimeEntryRedm.Count; i++)
                {
                    TimeEntry timeI = listTimeEntryRedm[i];
                    timeUniqe = timeI;

                    int j = i + 1;
                    while (j < listTimeEntryRedm.Count)
                    {
                        TimeEntry timeJ = listTimeEntryRedm[j];
                        if ((timeI.User.Id == timeJ.User.Id) &
                             (timeI.Project.Id == timeJ.Project.Id) &
                             (timeI.Activity.Id == timeJ.Activity.Id) &
                             (timeI.Id != timeJ.Id) &
                             (timeI.SpentOn.Value.Date.Month == timeJ.SpentOn.Value.Date.Month))
                        {

                            // TODO добавить поиск даты старта и даты финиша суммарной задчи timeI
                            DateTime startDateTimeI = timeI.GetDateValue("Дата старта", TypeDates.Start);
                            DateTime finishDateTimeI = timeI.GetDateValue("Дата завершения", TypeDates.Finish);

                            DateTime startDateTimeJ = timeJ.GetDateValue("Дата старта", TypeDates.Start);
                            DateTime finishDateTimeJ = timeJ.GetDateValue("Дата завершения", TypeDates.Finish);

                            if (startDateTimeI.CompareTo(startDateTimeJ) > 0)
                            {
                                timeI.SetDateValue("Дата старта", startDateTimeJ);
                            }

                            if (finishDateTimeI.CompareTo(finishDateTimeJ) < 0)
                            {
                                timeI.SetDateValue("Дата завершения", finishDateTimeJ);
                            }

                            timeI.Hours += timeJ.Hours;
                            listTimeEntryRedm.Remove(timeJ);
                        }
                        else j++;
                    }
                }

                countTotalRecord = listTimeEntryRedm.Count;
                SetInitProgBar(progressBar, 0, countTotalRecord, 1);

                var qTimeEntryAll = from time in listTimeEntryRedm
                                    from userRedmine in listUserRedmine
                                    from project in listProject
                                    where time.User.Id == userRedmine.Value.Id
                                    where ((time.Project.Id == project.Id) & project.IsPublic)
                                    select new
                                    {
                                        listIssue = userRedmine.listIssue,
                                        listProject = userRedmine.listProject,
                                        userRedmine = userRedmine,
                                        listUser = listUser,
                                        Value = time
                                    };                

                foreach (var time in qTimeEntryAll)
                {                    
                    UserTimeEntry userTimeEntry = new UserTimeEntry(time.listIssue, time.listProject,
                                                                    time.userRedmine, time.listUser);
                    userTimeEntry.Value = time.Value;
                    time.userRedmine.listUserTimeEntry.Add(userTimeEntry);
                }

                //foreach (var time in listTimeEntryRedm)
                //{                    
                //    UserRedmine userRedmine = listUserRedmine.Find(x => x.Value.Id == time.User.Id);
                //    Project project = listProject.Find(x => x.Id == time.Project.Id);
                //    curReadRecord++;
                //    progressBar.InvokeIfNeeded(delegate { progressBar.PerformStep(); });
                //    progressBar.InvokeIfNeeded(delegate {progressBar.CustomText = "Запись № " + progressBar.Value +
                //                                        "/ " + countTotalRecord;
                //    });                    
                //    if (userRedmine != null)
                //    {                        
                //        if (project != null)
                //        {
                //            if (project.IsPublic)
                //            {
                //                UserTimeEntry userTimeEntry = new UserTimeEntry(userRedmine.listIssue, userRedmine.listProject, 
                //                                                                userRedmine, listUser);
                //                userTimeEntry.Value = time;
                //                userRedmine.listUserTimeEntry.Add(userTimeEntry);
                //            }
                //        }                        
                //    }
                //}
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
                if ((jUserTimeEntry.ProjectName == iUserTimeEntry.ProjectName) &
                    (jUserTimeEntry.ActivityName == iUserTimeEntry.ActivityName) &
                    (jUserTimeEntry.Value.Id != iUserTimeEntry.Value.Id))
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
                        if ((userTimeEntry.DateStart.Month == month) & (userTimeEntry.DateStart.Year == year))
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
