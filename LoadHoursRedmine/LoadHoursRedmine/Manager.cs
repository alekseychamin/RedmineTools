using CsvHelper;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinRedminePlaning
{
    public enum TypeView { LoadUser = 0, LoadYWH, LoadGroup, LoadProject, LoadIssueDWH, LoadShortIssueDWH, LoadTimeDWH, LoadShortTimeDWH };
    public enum Operation { Equal, More, Less };

    public delegate void UpdateFormInfo();
    public class Manager
    {
        string host = "188.242.201.77";
        string apiKey = "70b1a875928636d8d3895248309344ea2bca6a5f";
        RedmineManager redmineManager;

        public event UpdateFormInfo Update;

        public List<UserRedmine> listUserRedmine = new List<UserRedmine>();
        public List<Project> listProject = new List<Project>();
        //public List<IssueRelation> listIssueRelation = new List<IssueRelation>();

        public List<LoadGroup> listLoadGroup = new List<LoadGroup>();
        public List<Issue> listIssue = new List<Issue>();
        public List<User> listUser = new List<User>();
        public List<UserTimeEntry> listUserTimeEntry = new List<UserTimeEntry>();        
        public List<LoadYWH> listLoadYWH = new List<LoadYWH>();
        public List<LoadUser> listLoadUser = new List<LoadUser>();
        public List<LoadProject> listLoadProject = new List<LoadProject>();

        public ExcelMethods excelMethods = new ExcelMethods();

        public List<int> listYear { get; }
        
        public Manager()
        {
            try
            {
                redmineManager = new RedmineManager(host, apiKey);
                listYear = new List<int>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! " + ex.Message);
            }
        }

        public void SaveCSVFileListIssue(string fileName)
        {
            var stream = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
            var csv = new CsvWriter(stream);
            List<Issue> listOut = new List<Issue>();            
            
            //foreach (Issue issue in listIssue)
            //{
                
            
            ////    //Project project = listProject.Find(x => x.Id == issue.Project.Id);
            ////    //if (project != null)
            ////    //    listOut.Add(issue);
            ////    //string line = issue.Project.Name + ";" + issue.Project.Id;
            ////    //csv.WriteField(line);
            ////    //csv.NextRecord();
            //}            
            csv.WriteRecords(listIssue);            
            stream.Flush();
            stream.Close();            
        }

        public void UpdateForm()
        {
            if (Update != null)
                Update();
        }

        public void CreateListLoadYWH()
        {
            listLoadYWH.Clear();
            //LoadHours.GetYearsFromListIssue(listYear, listIssue);

            foreach (int year in listYear)
            {
                if (year > 0)
                {
                    LoadYWH loadYMH = new LoadYWH(year, listIssue, listUserTimeEntry);
                    loadYMH.MakeMonth(listProject);
                    listLoadYWH.Add(loadYMH);
                }
            }
        }

        public void CreateListLoadUser()
        {
            //listLoadUser.Clear();
            //LoadHours.GetYearsFromListIssue(listYear, listIssue);
            
            foreach (LoadUser loadUser in listLoadUser)
            {
                //LoadUser loadUser = new LoadUser(loadUser, listIssue);
                foreach (int year in listYear)
                {                    
                    loadUser.AddYear(year, listProject, listLoadUser);
                }

                //listLoadUser.Add(loadUser);
            }
        }       
        
        public void CreateListLoadGroup()
        {
            foreach (LoadGroup loadGroup in listLoadGroup)
            {
                User user = new User();
                user.LastName = loadGroup.name;
                user.Id = loadGroup.id;
                UserGroupRedmine userGroupRedmine = new UserGroupRedmine(loadGroup.name, loadGroup.id);

                LoadUser loadUserGroup = new LoadUser(user, listIssue, listUserTimeEntry);
                loadUserGroup.listGroup.Add(userGroupRedmine);
                loadGroup.listLoadUser.Insert(0, loadUserGroup);
            }

            foreach (LoadGroup loadGroup in listLoadGroup)
            {
                foreach (LoadUser loadUser in loadGroup.listLoadUser)
                {
                    foreach (int year in listYear)
                    {
                        loadUser.AddYear(year, listProject, loadGroup.listLoadUser);
                    }
                }
            }
        }
        
        public void CreateListLoadProject()
        {
            listLoadProject.Clear();

            foreach (Project project in listProject)
            {
                bool isPlaned = false;
                foreach (var customField in project.CustomFields)
                {
                    if (customField.Name.Contains("Учет при планировании"))
                    {
                        string res = customField.Values[0].Info;
                        isPlaned = (res.Contains("1"));
                    }
                }

                if ((project.Status == ProjectStatus.Active) & (isPlaned))
                {
                    UserProject userProject = new UserProject(project.Name, project.Id);
                    LoadProject loadProject = new LoadProject(userProject, listIssue, listUserTimeEntry);

                    foreach (int year in listYear)
                    {
                        loadProject.AddYear(year);
                    }

                    listLoadProject.Add(loadProject);
                }
            }
        }

        //public void GetGroupFromRedmine()
        //{
        //    listLoadGroup.Clear();
        //    try
        //    {
        //        NameValueCollection parametr = new NameValueCollection { { "group_id", "*" } };
        //        foreach (Group group in redmineManager.GetObjects<Group>(parametr))
        //        {
        //            LoadGroup loadGroup = new LoadGroup(group.Name, group.Id, listIssue);
        //            listLoadGroup.Add(loadGroup);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error - " + ex.Message);
        //    }
        //}                               

        public void GetIssue_ProjectFromRedmine()
        {
            listIssue.Clear();
            listProject.Clear();
            try
            {
                NameValueCollection parametr = new NameValueCollection { { "project_id", "*" } };
                foreach (Project project in redmineManager.GetObjects<Project>(parametr))
                {
                    //MessageBox.Show(project.Status.ToString());
                    listProject.Add(project);                    
                }

                parametr = new NameValueCollection { { "status_id", "*" } };
                foreach (Issue issue in redmineManager.GetObjects<Issue>(parametr))
                {
                    
                    Project project = listProject.Find(x => x.Id == issue.Project.Id);

                    if (project != null)
                    {
                        if ((issue.StartDate != null) & (issue.DueDate != null) &
                            (issue.AssignedTo != null))
                        {
                            listIssue.Add(issue);

                            //if (!issue.Status.Name.Contains("Закрыта") & !issue.Status.Name.Contains("Решена") &
                            //    !issue.Status.Name.Contains("Отклонена"))
                            //{
                            //    foreach (IssueRelation issueRelation in redmineManager.GetObjects<IssueRelation>(new NameValueCollection { { "issue_id", issue.Id.ToString() } }))
                            //    {
                            //        if (issueRelation != null)
                            //            issue.Relations.Add(issueRelation);
                            //    }
                            //}

                        }
                    }
                    //else
                    //    listIssue.Add(issue);

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error - " + ex.Message);
                MessageBox.Show(listIssue[(listIssue.Count - 1)].Id.ToString());
            }

    LoadHours.GetYearsFromListIssue(listYear, listIssue);
        }

        public void GetUser_GroupFromRedmine()
        {
            NameValueCollection parametr;
            listUser.Clear();
            listLoadUser.Clear();
            listLoadGroup.Clear();

            try
            {
                parametr = new NameValueCollection { { "user_id", "*" } };
                foreach (User user in redmineManager.GetObjects<User>(parametr))
                {
                    listUser.Add(user);
                    LoadUser loadUser = new LoadUser(user, listIssue, listUserTimeEntry);
                    listLoadUser.Add(loadUser);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error - " + ex.Message);
            }

            try
            {
                parametr = new NameValueCollection { { "group_id", "*" } };
                foreach (Group group in redmineManager.GetObjects<Group>(parametr))
                {
                    UserGroupRedmine userGroup = new UserGroupRedmine(group.Name, group.Id);
                    LoadGroup loadGroup = new LoadGroup(group.Name, group.Id, listIssue);
                    LoadGroup findLoadGroup = listLoadGroup.Find(x => x.id == loadGroup.id);

                    if (findLoadGroup == null)
                    {
                        listLoadGroup.Add(loadGroup);
                        findLoadGroup = loadGroup;
                    }

                    foreach (User user in redmineManager.GetObjects<User>(new NameValueCollection { { "group_id", group.Id.ToString() } }))
                    {
                        LoadUser loadUser = listLoadUser.Find(x => x.user.Id == user.Id);                        

                        if (loadUser != null)
                        {
                            loadUser.listGroup.Add(userGroup);

                            LoadUser findLoadUser = findLoadGroup.listLoadUser.Find(x => x.user.Id == loadUser.user.Id);
                            if (findLoadUser == null)
                                findLoadGroup.listLoadUser.Add(loadUser);
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error -" + ex.Message);
            }

        }

        public void GetTimeEntryFromRedmine()
        {
            listUserTimeEntry.Clear();

            NameValueCollection parametr = new NameValueCollection { { "user_id", "*" } };
            foreach (var time in redmineManager.GetObjects<TimeEntry>(parametr))
            {
                UserTimeEntry userTimeEntry = new UserTimeEntry(time, listIssue, listUser);
                listUserTimeEntry.Add(userTimeEntry);
            }          
        }

        //public void GetUserFromRedmine(Dictionary<string, string> bossName) //params string[] noNameForReport)
        //{
        //    listIssue.Clear();
        //    listProject.Clear();
        //    listUserRedmine.Clear();

        //    NameValueCollection parametr = new NameValueCollection { { "user_id", "*" } };
        //    try
        //    {
        //        foreach (var user in redmineManager.GetObjects<User>(parametr))
        //        {
        //            UserRedmine userRedmine = new UserRedmine();
        //            userRedmine.bossName = bossName;
        //            userRedmine.Value = user;

        //            userRedmine.listIssue = this.listIssue;
        //            userRedmine.listProject = this.listProject;
        //            listUserRedmine.Add(userRedmine);                                      
                    

        //            //parametr = new NameValueCollection { { "user_id", user.Id.ToString() } };
        //            //int count = redmineManager.GetObjects<TimeEntry>(parametr).Count;
        //            //if (count > 0)
        //            //{
        //            //    UserRedmine userRedmine = new UserRedmine();
        //            //    userRedmine.Value = user;
        //            //    listUserRedmine.Add(userRedmine);
        //            //    Console.WriteLine("Name = {0}, Count time entry = {1}", user.LastName, count);
        //            //}
        //        }

        //        parametr = new NameValueCollection { { "group_id", "*" } };
        //        foreach (Group group in redmineManager.GetObjects<Group>(parametr))
        //        {
        //            UserGroupRedmine userGroup = new UserGroupRedmine(group.Name, group.Id);
        //            foreach (User user in redmineManager.GetObjects<User>(new NameValueCollection { { "group_id", group.Id.ToString() } }))
        //            {
        //                UserRedmine userRedmine = listUserRedmine.Find(x => x.Value.Id == user.Id);
        //                if (userRedmine != null)
        //                {
        //                    userRedmine.listUserGroup.Add(userGroup);
        //                }
        //            }
        //        }

        //        parametr = new NameValueCollection { { "status_id", "*" } };
        //        foreach (Issue issue in redmineManager.GetObjects<Issue>(parametr))
        //        {
        //            listIssue.Add(issue);
        //        }

        //        parametr = new NameValueCollection { { "project_id", "*" } };
        //        foreach (Project project in redmineManager.GetObjects<Project>(parametr))
        //        {                    
        //            listProject.Add(project);
        //        }

        //        parametr = new NameValueCollection { { "user_id", "*" } };
        //        foreach (var issue in redmineManager.GetObjects<Issue>(parametr))
        //        {
        //            if (issue.AssignedTo != null)
        //            {
        //                UserRedmine userRedmine = listUserRedmine.Find(x => x.Value.Id == issue.AssignedTo.Id);
        //                Project project = listProject.Find(x => x.Id == issue.Project.Id);

        //                if (userRedmine != null)
        //                {
        //                    if (project != null)
        //                    {                                                               
        //                        if ((issue.StartDate != null) & (issue.DueDate != null))
        //                        {
        //                            UserIssueEntry userIssueEntry = new UserIssueEntry(issue, project, userRedmine);                                        
        //                            userRedmine.listUserIssueEntry.Add(userIssueEntry);
        //                        }
                                
        //                    }
        //                    userRedmine.listUserIssueEntry.Sort();
        //                }                        
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error - " + ex.Message);
        //    }

        //    listUserRedmine.Sort();
        //}

        /*public void GetMounthUserTimeEntry(int year, int mounth)
        {
            if (mounth > 0 & mounth <= 12)
            {
                foreach (UserRedmine userRedmine in listUserRedmine)
                {
                    if (userRedmine.BossName.Contains("Першин"))
                    {
                        int a = 0;
                    }

                    userRedmine.listMounthUserIssueEntry.Clear();
                    foreach (UserIssueEntry userTimeEntry in userRedmine.listUserIssueEntry)
                    {
                        if ( (userTimeEntry.DateStart.Month == mounth) & (userTimeEntry.DateStart.Year == year))
                        {
                            userRedmine.listMounthUserIssueEntry.Add(userTimeEntry);
                        }
                    }
                    userRedmine.listMounthUserIssueEntry.Sort();
                }
            }
        }
        */

        public LoadYWH FindLoadYWH(int numberYear, List<LoadYWH> listLoadYWH)
        {
            LoadYWH loadYWH = null;

            loadYWH = listLoadYWH.Find(x => x.NumberYear == numberYear);

            return loadYWH;
        }

        public LoadYWH FindLoadYWH(int numberYear, User user)
        {
            LoadYWH loadYWH = null;
            LoadUser loadUser = null;

            loadUser = listLoadUser.Find(x => x.user.Id == user.Id);
            
            if (loadUser != null)
                loadYWH = loadUser.listLoadYWH.Find(x => x.NumberYear == numberYear);

            return loadYWH;
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
