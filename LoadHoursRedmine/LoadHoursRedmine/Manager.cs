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
    public enum TypeView { LoadUser = 0, LoadExperiedUser, LoadYWH, LoadGroup,
                           LoadProject, LoadProjectUser, LoadExperiedProject, LoadIssueDWH, LoadShortIssueDWH,
                           LoadTimeDWH, LoadShortTimeDWH, LoadShortExpProject, LoadShortExpUser };
    public enum Operation { Equal, More, Less };

    public enum TypeSave { WorkHours, HumansMonth };
        
         
    public delegate void UpdateFormInfo();
    public class Manager
    {
        public Issue EmailSaveIssue { get; set; }

        string host = "188.242.201.77";
        string apiKey = "70b1a875928636d8d3895248309344ea2bca6a5f";
        RedmineManager redmineManager;

        public event UpdateFormInfo Update;        
        public List<UserRedmine> listUserRedmine = new List<UserRedmine>();        

        public List<EmailMessage> listEmailMessage = new List<EmailMessage>();

        public List<LoadGroup> listLoadGroup = new List<LoadGroup>();                               

        public List<LoadYWH> listLoadYWH = new List<LoadYWH>();

        public List<LoadUser> listLoadUser = new List<LoadUser>();
        
        public List<LoadProject> listLoadProject = new List<LoadProject>();

        //public ExcelMethods excelMethods = new ExcelMethods();
        public RedmineData redmineData;

        public List<int> listYear { get; }
        
        public Manager(RedmineData redmineData)
        {
            try
            {
                redmineManager = new RedmineManager(host, apiKey);
                this.redmineData = redmineData;
                listYear = new List<int>();
                redmineData.monthValueHours = new MonthValueHours(redmineData.listMonthHours);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error! " + ex.Message);
            }
        }

        public void SetHeadUser(List<LoadIssue> listLoadOpenIssue)
        {
            foreach (LoadIssue loadOpenIssue in listLoadOpenIssue)
            {
                LoadProject loadProject = listLoadProject.Find(x => x.userProject.Id == loadOpenIssue.issue.Project.Id);
                if (loadProject != null)
                {
                    loadOpenIssue.NameHeadUser = loadProject.userProject.NameHeadUser;
                    loadOpenIssue.EmailHeadUser = loadProject.userProject.EmailHeadUser;
                }
            }
        }

        public void SendEmail()
        {
            Email email = new Email();
            foreach (EmailMessage emailMessage in listEmailMessage)
            {                
                email.SendMail(emailMessage.Message, emailMessage.Title, emailMessage.ListToEmail, emailMessage.ListCCEmail);
            }
            MessageBox.Show("Messages are sent!");
        }

        public void SaveDateToRedmineEmailIssue()
        {
            string note = "";
            if (EmailSaveIssue != null)
            {
                listEmailMessage.Sort();

                foreach (EmailMessage emailMessage in listEmailMessage)
                {                    
                    note += "*" + emailMessage.Title + "*";
                    note += "\n";
                    note += emailMessage.Message;                    
                }

                EmailSaveIssue.Notes = note;
                redmineManager.UpdateObject(EmailSaveIssue.Id.ToString(), EmailSaveIssue);
            }
        }

        public void MakeEmailMessages(List<LoadIssue> listLoadOpenIssue, string title)
        {
            //listEmailMessage.Clear();
            string message;
            string addrTo;
            string addrCC;

            foreach (LoadIssue loadIssue in listLoadOpenIssue)
            {
                if (loadIssue.isExperied)
                {
                    LoadUser loadUser = listLoadUser.Find(x => x.user.Id == loadIssue.issue.AssignedTo.Id);
                    if (loadUser != null)
                    {
                        addrTo = loadUser.user.Email;
                        addrCC = loadIssue.EmailHeadUser;

                        message = "Здравствуйте уважаемый(ая) " + loadUser.user.LastName + " " + loadUser.user.FirstName + "." + "\n";
                        message += "\n";
                        message += "У вас есть открытые задания по которым вы являетесь ответственным и по которым есть следующие замечания: " + "\n" +
                            "1. в части задач просрочены сроки выполнения " +
                            " для которых в случае их выполнения необходимо закрыть (поменять статус на - закрыта) или согласовать новую дату завершения;" + "\n" +
                            "2. в части задач необходимо указать актуальные проценты выполнения." + "\n";
                        message += "\n";
                        message += loadIssue.Message;

                        EmailMessage emailMessage = listEmailMessage.Find(x => (x.Id == loadUser.user.Id) & (x.Title.Equals(title)));
                        if (emailMessage != null)
                        {
                            emailMessage.Message += loadIssue.Message;
                        }
                        else
                        {
                            emailMessage = new EmailMessage();
                            emailMessage.ListToEmail.Add(addrTo);
                            emailMessage.ListCCEmail.Add(addrCC);
                            emailMessage.Message = message;
                            emailMessage.Id = loadUser.user.Id;
                            emailMessage.ProjectId = loadIssue.issue.Project.Id;
                            emailMessage.Title = title;
                            listEmailMessage.Add(emailMessage);
                        }
                    }                                        
                }
            }
        }

        public void MakeListLoadUserSave(out List<CSVLoadUserYWH> listCSVLoadUserYWH)
        {
            listCSVLoadUserYWH = new List<CSVLoadUserYWH>();
            CSVLoadUserYWH csvLoadUserYWH;
            listLoadUser.Sort();

            foreach (LoadUser loadUser in listLoadUser)
            {
                foreach (LoadYWH loadYWH in loadUser.listLoadYWH)
                {
                    csvLoadUserYWH = new CSVLoadUserYWH();
                    csvLoadUserYWH.UserName = loadUser.user.LastName + " " + loadUser.user.FirstName;                    
                    csvLoadUserYWH.GroupName = loadUser.GroupName;
                    csvLoadUserYWH.SetMWH(TypeSave.WorkHours, "план", loadYWH);
                    listCSVLoadUserYWH.Add(csvLoadUserYWH);

                    csvLoadUserYWH = new CSVLoadUserYWH();
                    csvLoadUserYWH.UserName = loadUser.user.LastName + " " + loadUser.user.FirstName;
                    csvLoadUserYWH.GroupName = loadUser.GroupName;
                    csvLoadUserYWH.SetMWH(TypeSave.HumansMonth, "план", loadYWH);
                    listCSVLoadUserYWH.Add(csvLoadUserYWH);

                    csvLoadUserYWH = new CSVLoadUserYWH();
                    csvLoadUserYWH.UserName = loadUser.user.LastName + " " + loadUser.user.FirstName;
                    csvLoadUserYWH.GroupName = loadUser.GroupName;
                    csvLoadUserYWH.SetMWH(TypeSave.WorkHours, "факт", loadYWH);
                    listCSVLoadUserYWH.Add(csvLoadUserYWH);

                    csvLoadUserYWH = new CSVLoadUserYWH();
                    csvLoadUserYWH.UserName = loadUser.user.LastName + " " + loadUser.user.FirstName;
                    csvLoadUserYWH.GroupName = loadUser.GroupName;
                    csvLoadUserYWH.SetMWH(TypeSave.HumansMonth, "факт", loadYWH);
                    listCSVLoadUserYWH.Add(csvLoadUserYWH);
                }
            }
        }

        public void MakeListLoadProjectUserSave(out List<CSVLoadProjectUserYWH> listCSVLoadProjectUserYWH)
        {
            listCSVLoadProjectUserYWH = new List<CSVLoadProjectUserYWH>();
            CSVLoadProjectUserYWH csvLoadProjectUserYWH;

            foreach (LoadProject loadProject in listLoadProject)
            {
                foreach (LoadUser loadUser in loadProject.listLoadUser)
                {
                    foreach (LoadYWH loadYWH in loadUser.listLoadYWH)
                    {
                        csvLoadProjectUserYWH = new CSVLoadProjectUserYWH();
                        csvLoadProjectUserYWH.NameProject = loadProject.userProject.Name;
                        csvLoadProjectUserYWH.UserName = loadUser.user.LastName + " " + loadUser.user.FirstName;
                        csvLoadProjectUserYWH.StartDate = loadProject.StartProject.ToShortDateString();
                        csvLoadProjectUserYWH.FinishDate = loadProject.FinishProject.ToShortDateString();
                        csvLoadProjectUserYWH.SetMWH(TypeSave.WorkHours, "план", loadYWH);
                        listCSVLoadProjectUserYWH.Add(csvLoadProjectUserYWH);

                        csvLoadProjectUserYWH = new CSVLoadProjectUserYWH();
                        csvLoadProjectUserYWH.NameProject = loadProject.userProject.Name;
                        csvLoadProjectUserYWH.UserName = loadUser.user.LastName + " " + loadUser.user.FirstName;
                        csvLoadProjectUserYWH.StartDate = loadProject.StartProject.ToShortDateString();
                        csvLoadProjectUserYWH.FinishDate = loadProject.FinishProject.ToShortDateString();
                        csvLoadProjectUserYWH.SetMWH(TypeSave.HumansMonth, "план", loadYWH);
                        listCSVLoadProjectUserYWH.Add(csvLoadProjectUserYWH);

                        csvLoadProjectUserYWH = new CSVLoadProjectUserYWH();
                        csvLoadProjectUserYWH.NameProject = loadProject.userProject.Name;
                        csvLoadProjectUserYWH.UserName = loadUser.user.LastName + " " + loadUser.user.FirstName;
                        csvLoadProjectUserYWH.StartDate = loadProject.StartProject.ToShortDateString();
                        csvLoadProjectUserYWH.FinishDate = loadProject.FinishProject.ToShortDateString();
                        csvLoadProjectUserYWH.SetMWH(TypeSave.WorkHours, "факт", loadYWH);
                        listCSVLoadProjectUserYWH.Add(csvLoadProjectUserYWH);

                        csvLoadProjectUserYWH = new CSVLoadProjectUserYWH();
                        csvLoadProjectUserYWH.NameProject = loadProject.userProject.Name;
                        csvLoadProjectUserYWH.UserName = loadUser.user.LastName + " " + loadUser.user.FirstName;
                        csvLoadProjectUserYWH.StartDate = loadProject.StartProject.ToShortDateString();
                        csvLoadProjectUserYWH.FinishDate = loadProject.FinishProject.ToShortDateString();
                        csvLoadProjectUserYWH.SetMWH(TypeSave.HumansMonth, "факт", loadYWH);
                        listCSVLoadProjectUserYWH.Add(csvLoadProjectUserYWH);
                    }
                }
            }
        }

        public void MakeListLoadProjectSave(out List<CSVLoadProjectYWH> listCSVLoadProjectYWH)
        {
            listCSVLoadProjectYWH = new List<CSVLoadProjectYWH>();
            CSVLoadProjectYWH csvLoadProjectYWH;

            foreach (LoadProject loadProject in listLoadProject)
            {
                foreach (LoadYWH loadYWH in loadProject.listLoadYWH)
                {
                    csvLoadProjectYWH = new CSVLoadProjectYWH();
                    csvLoadProjectYWH.NameProject = loadProject.userProject.Name;
                    csvLoadProjectYWH.StartDate = loadProject.StartProject.ToShortDateString();
                    csvLoadProjectYWH.FinishDate = loadProject.FinishProject.ToShortDateString();
                    csvLoadProjectYWH.SetMWH(TypeSave.WorkHours, "план", loadYWH);
                    listCSVLoadProjectYWH.Add(csvLoadProjectYWH);

                    csvLoadProjectYWH = new CSVLoadProjectYWH();
                    csvLoadProjectYWH.NameProject = loadProject.userProject.Name;
                    csvLoadProjectYWH.StartDate = loadProject.StartProject.ToShortDateString();
                    csvLoadProjectYWH.FinishDate = loadProject.FinishProject.ToShortDateString();
                    csvLoadProjectYWH.SetMWH(TypeSave.HumansMonth, "план", loadYWH);
                    listCSVLoadProjectYWH.Add(csvLoadProjectYWH);

                    csvLoadProjectYWH = new CSVLoadProjectYWH();
                    csvLoadProjectYWH.NameProject = loadProject.userProject.Name;
                    csvLoadProjectYWH.StartDate = loadProject.StartProject.ToShortDateString();
                    csvLoadProjectYWH.FinishDate = loadProject.FinishProject.ToShortDateString();
                    csvLoadProjectYWH.SetMWH(TypeSave.WorkHours, "факт", loadYWH);
                    listCSVLoadProjectYWH.Add(csvLoadProjectYWH);

                    csvLoadProjectYWH = new CSVLoadProjectYWH();
                    csvLoadProjectYWH.NameProject = loadProject.userProject.Name;
                    csvLoadProjectYWH.StartDate = loadProject.StartProject.ToShortDateString();
                    csvLoadProjectYWH.FinishDate = loadProject.FinishProject.ToShortDateString();
                    csvLoadProjectYWH.SetMWH(TypeSave.HumansMonth, "факт", loadYWH);
                    listCSVLoadProjectYWH.Add(csvLoadProjectYWH);
                }
            }
        }

        public void MakeListLoadGroupSave(out List<CSVLoadGroupYWH> listCSVLoadGroupYWH)
        {
            listCSVLoadGroupYWH = new List<CSVLoadGroupYWH>();
            CSVLoadGroupYWH csvLoadGroupYWH;            

            foreach (LoadGroup loadGroup in listLoadGroup)
            {                                               
                foreach (LoadUser loadUser in loadGroup.listLoadUser)
                {
                    if (loadUser.user.LastName.Equals(loadGroup.name))
                    {
                        foreach (LoadYWH loadYWH in loadUser.listLoadYWH)
                        {
                            csvLoadGroupYWH = new CSVLoadGroupYWH();
                            csvLoadGroupYWH.NameGroup = loadGroup.name;
                            csvLoadGroupYWH.CountUser = loadGroup.CountUser;
                            csvLoadGroupYWH.SetMWH(TypeSave.WorkHours, "план", loadYWH);
                            listCSVLoadGroupYWH.Add(csvLoadGroupYWH);

                            csvLoadGroupYWH = new CSVLoadGroupYWH();
                            csvLoadGroupYWH.NameGroup = loadGroup.name;
                            csvLoadGroupYWH.CountUser = loadGroup.CountUser;
                            csvLoadGroupYWH.SetMWH(TypeSave.HumansMonth, "план", loadYWH);
                            listCSVLoadGroupYWH.Add(csvLoadGroupYWH);

                            csvLoadGroupYWH = new CSVLoadGroupYWH();
                            csvLoadGroupYWH.NameGroup = loadGroup.name;
                            csvLoadGroupYWH.CountUser = loadGroup.CountUser;
                            csvLoadGroupYWH.SetMWH(TypeSave.WorkHours, "факт", loadYWH);
                            listCSVLoadGroupYWH.Add(csvLoadGroupYWH);

                            csvLoadGroupYWH = new CSVLoadGroupYWH();
                            csvLoadGroupYWH.NameGroup = loadGroup.name;
                            csvLoadGroupYWH.CountUser = loadGroup.CountUser;
                            csvLoadGroupYWH.SetMWH(TypeSave.HumansMonth, "факт", loadYWH);
                            listCSVLoadGroupYWH.Add(csvLoadGroupYWH);
                        }

                    }
                }
            }
        }       

        public void MakeListLoadYWHSave(out List<CSVLoadYWH> listCSVLoadYWH)
        {
            listCSVLoadYWH = new List<CSVLoadYWH>();
            CSVLoadYWH csvLoadYWH;

            foreach (LoadYWH loadYWH in listLoadYWH)
            {
                csvLoadYWH = new CSVLoadYWH();
                csvLoadYWH.SetMWH(TypeSave.WorkHours, "план", loadYWH);
                listCSVLoadYWH.Add(csvLoadYWH);

                csvLoadYWH = new CSVLoadYWH();
                csvLoadYWH.SetMWH(TypeSave.HumansMonth, "план", loadYWH);
                listCSVLoadYWH.Add(csvLoadYWH);

                csvLoadYWH = new CSVLoadYWH();
                csvLoadYWH.SetMWH(TypeSave.WorkHours, "факт", loadYWH);
                listCSVLoadYWH.Add(csvLoadYWH);

                csvLoadYWH = new CSVLoadYWH();
                csvLoadYWH.SetMWH(TypeSave.HumansMonth, "факт", loadYWH);
                listCSVLoadYWH.Add(csvLoadYWH);
            }
        }

        public void SaveCSVFileListIssue<T>(string fileName, List<T> listToSave)
        {
            fileName = fileName.Replace(":", "_");
            var stream = new StreamWriter(fileName, false, System.Text.Encoding.UTF8);
            var csv = new CsvWriter(stream);
                        
            csv.WriteRecords(listToSave);
            
            stream.Flush();
            stream.Close();            
        }

        public void UpdateForm()
        {
            if (Update != null)
                Update();
        }

        private int GetCountWorkUsers()
        {
            int count = 0;

            foreach (LoadGroup loadGroup in listLoadGroup)
            {
                if (!(loadGroup.name.Contains("Испытатели") | loadGroup.name.Contains("Руководители") | loadGroup.name.Contains("Менеджеры")))
                {
                    count += loadGroup.CountUser;
                }
            }

            return count;
        }

        public void CreateListLoadYWH()
        {
            listLoadYWH.Clear();
            //LoadHours.GetYearsFromListIssue(listYear, listIssue);

            int countWorkUser = GetCountWorkUsers();
            double maxYearHumansHours = countWorkUser * 12;
            double maxMonthHumansHours = countWorkUser;

            foreach (int year in listYear)
            {
                if (year > 0)
                {                    
                    LoadYWH loadYMH = new LoadYWH(redmineData, maxYearHumansHours, year);
                    loadYMH.MakeMonth(maxMonthHumansHours, redmineData.listProject);
                    listLoadYWH.Add(loadYMH);
                }
            }
        }

        public void CreateListLoadUser()
        {
            //listLoadUser.Clear();
            //LoadHours.GetYearsFromListIssue(listYear, listIssue);
                        
            double maxYearHumansHours = 1 * 12;
            double maxMonthHumansHours = 1;

            foreach (LoadUser loadUser in listLoadUser)
            {
                //LoadUser loadUser = new LoadUser(loadUser, listIssue);
                foreach (int year in listYear)
                {                    
                    loadUser.AddYear(maxYearHumansHours, maxMonthHumansHours, year, redmineData.listProject, listLoadUser);
                }

                SetHeadUser(loadUser.listLoadOpenIssue);
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

                LoadUser loadUserGroup = new LoadUser(redmineData, user);
                loadUserGroup.listGroup.Add(userGroupRedmine);
                loadGroup.listLoadUser.Insert(0, loadUserGroup);
            }

            foreach (LoadGroup loadGroup in listLoadGroup)
            {
                int countWorkUser = loadGroup.CountUser;
                double maxYearHumansHours = countWorkUser * 12;
                double maxMonthHumansHours = countWorkUser;

                foreach (LoadUser loadUser in loadGroup.listLoadUser)
                {
                    foreach (int year in listYear)
                    {
                        if (loadUser.listLoadYWH.Count < listYear.Count)
                            loadUser.AddYear(maxYearHumansHours, maxMonthHumansHours, year, redmineData.listProject, loadGroup.listLoadUser);
                    }
                }
            }
        }        

        public void CreateListLoadProject()
        {
            listLoadProject.Clear();

            int countWorkUser = GetCountWorkUsers();
            double maxYearHumansHours = countWorkUser * 12;
            double maxMonthHumansHours = countWorkUser;            

            foreach (Project project in redmineData.listProject)
            {                
                if (LoadHours.IsItemInPlanActiveProject(project))
                {
                    UserProject userProject = new UserProject(redmineData, project.Name, project.Id);
                    LoadProject loadProject = new LoadProject(redmineData, userProject);

                    foreach (int year in listYear)
                    {
                        loadProject.AddYear(maxYearHumansHours, maxMonthHumansHours, year);
                    }

                    listLoadProject.Add(loadProject);
                    SetHeadUser(loadProject.listLoadOpenIssue);
                }
            }
        }                        

        public void GetIssue_ProjectFromRedmine()
        {
            redmineData.listIssue.Clear();
            redmineData.listOpenIssue.Clear();
            redmineData.listProject.Clear();
            redmineData.listMonthHours.Clear();
            try
            {
                NameValueCollection parametr = new NameValueCollection { { "project_id", "*" } };
                foreach (Project project in redmineManager.GetObjects<Project>(parametr))
                {
                    //MessageBox.Show(project.Status.ToString());
                    redmineData.listProject.Add(project);                    
                }
                
                parametr = new NameValueCollection { { "status_id", "open" } };
                foreach (Issue issue in redmineManager.GetObjects<Issue>(parametr))
                {

                    Project project = redmineData.listProject.Find(x => x.Id == issue.Project.Id);

                    if (project != null)
                    {
                        if ((issue.StartDate != null) & (issue.DueDate != null) &
                            (issue.AssignedTo != null))
                        {
                            redmineData.listOpenIssue.Add(issue);
                        }
                    }
                }

                parametr = new NameValueCollection { { "status_id", "*" } };
                foreach (Issue issue in redmineManager.GetObjects<Issue>(parametr))
                {
                    
                    Project project = redmineData.listProject.Find(x => x.Id == issue.Project.Id);

                    if (issue.Id == 936)
                    {
                        Issue issue_jornals = redmineManager.GetObject<Issue>(issue.Id.ToString(), 
                                                                              new NameValueCollection { { "include", "journals" } });
                        this.EmailSaveIssue = issue_jornals;
                    }

                    if (issue.Id == 937)
                    {
                        Issue issue_jornals = redmineManager.GetObject<Issue>(issue.Id.ToString(),
                                                                              new NameValueCollection { { "include", "journals" } });

                        foreach (var journal in issue_jornals.Journals)
                        {
                            string note = journal.Notes;
                            if (!note.Equals(""))
                            {
                                MonthHours monthHours = new MonthHours(note);
                                redmineData.listMonthHours.Add(monthHours);
                            }
                        }
                    }

                    if (project != null)
                    {
                        if ((issue.StartDate != null) & (issue.DueDate != null) &
                            (issue.AssignedTo != null))
                        {
                            redmineData.listIssue.Add(issue);

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
                MessageBox.Show(redmineData.listIssue[(redmineData.listIssue.Count - 1)].Id.ToString());
            }
        LoadHours.GetYearsFromListIssue(listYear, redmineData.listIssue);
        }

        public void GetUser_GroupFromRedmine()
        {
            NameValueCollection parametr;
            redmineData.listUser.Clear();
            listLoadUser.Clear();
            listLoadGroup.Clear();

            try
            {
                parametr = new NameValueCollection { { "user_id", "*" } };
                foreach (User user in redmineManager.GetObjects<User>(parametr))
                {
                    redmineData.listUser.Add(user);
                    LoadUser loadUser = new LoadUser(redmineData, user);
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
                    LoadGroup loadGroup = new LoadGroup(group.Name, group.Id, redmineData.listIssue);
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
            redmineData.listUserTimeEntry.Clear();
            
            NameValueCollection parametr = new NameValueCollection { { "user_id", "*" } };
            foreach (var time in redmineManager.GetObjects<TimeEntry>(parametr))
            {                           
                UserTimeEntry userTimeEntry = new UserTimeEntry(time, redmineData.listIssue, redmineData.listUser);
                redmineData.listUserTimeEntry.Add(userTimeEntry);             
            }          
        }
              
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
