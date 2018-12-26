using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinRedminePlaning
{
    class Manager
    {
        string host = "188.242.201.77";
        string apiKey = "70b1a875928636d8d3895248309344ea2bca6a5f";
        RedmineManager redmineManager;

        public List<UserRedmine> listUserRedmine = new List<UserRedmine>();
        public List<Issue> listIssue = new List<Issue>();
        public List<Project> listProject = new List<Project>();

        public ExcelMethods excelMethods = new ExcelMethods();

        public Manager()
        {
            try
            {
                redmineManager = new RedmineManager(host, apiKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! " + ex.Message);
            }
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
                
        public void GetUserFromRedmine(Dictionary<string, string> bossName) //params string[] noNameForReport)
        {
            listIssue.Clear();
            listProject.Clear();
            listUserRedmine.Clear();

            NameValueCollection parametr = new NameValueCollection { { "user_id", "*" } };
            try
            {
                foreach (var user in redmineManager.GetObjects<User>(parametr))
                {
                    UserRedmine userRedmine = new UserRedmine();
                    userRedmine.bossName = bossName;
                    userRedmine.Value = user;

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
                foreach (Group group in redmineManager.GetObjects<Group>(parametr))
                {
                    UserGroup userGroup = new UserGroup(group.Name, group.Id);
                    foreach (User user in redmineManager.GetObjects<User>(new NameValueCollection { { "group_id", group.Id.ToString() } }))
                    {
                        UserRedmine userRedmine = listUserRedmine.Find(x => x.Value.Id == user.Id);
                        if (userRedmine != null)
                        {
                            userRedmine.listUserGroup.Add(userGroup);
                        }
                    }
                }

                parametr = new NameValueCollection { { "status_id", "*" } };
                foreach (Issue issue in redmineManager.GetObjects<Issue>(parametr))
                {
                    listIssue.Add(issue);

                }

                parametr = new NameValueCollection { { "project_id", "*" } };
                foreach (Project project in redmineManager.GetObjects<Project>(parametr))
                {
                    listProject.Add(project);
                }

                parametr = new NameValueCollection { { "user_id", "*" } };
                foreach (var time in redmineManager.GetObjects<TimeEntry>(parametr))
                {
                    UserRedmine userRedmine = listUserRedmine.Find(x => x.Value.Id == time.User.Id);
                    Project project = listProject.Find(x => x.Id == time.Project.Id);                    

                    if (userRedmine != null)
                    {                        
                        if (project != null)
                        {
                            if (project.IsPublic)
                            {
                                UserTimeEntry userTimeEntry = new UserTimeEntry(userRedmine.listIssue, userRedmine.listProject, 
                                                                                userRedmine);
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
                                                                                  (x.Comment == userTimeEntry.Comment)));

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

        public void GetMounthUserTimeEntry(int year, int mounth)
        {
            if (mounth > 0 & mounth <= 12)
            {
                foreach (UserRedmine userRedmine in listUserRedmine)
                {
                    if (userRedmine.BossName.Contains("Першин"))
                    {
                        int a = 0;
                    }

                    userRedmine.listMounthUserTimeEntry.Clear();
                    foreach (UserTimeEntry userTimeEntry in userRedmine.listUserTimeEntry)
                    {
                        if ( (userTimeEntry.DateStart.Month == mounth) & (userTimeEntry.DateStart.Year == year))
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
