using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationRedmine
{
    class Manage
    {
        List<UserRedmine> listUserRedmine = new List<UserRedmine>();
        RedmineManager redmineManager = null;
        Email email;

        struct DayTimeStart
        {
            public DayOfWeek dayOfWeek;
            public string timeStart;
        }

        DayTimeStart[] dayTimeStart = new DayTimeStart[3];

        public Manage(string host, string key)
        {
            dayTimeStart[0].dayOfWeek = DayOfWeek.Monday;
            dayTimeStart[0].timeStart = "9:00";

            dayTimeStart[1].dayOfWeek = DayOfWeek.Wednesday;
            dayTimeStart[1].timeStart = "9:00";

            dayTimeStart[2].dayOfWeek = DayOfWeek.Friday;
            dayTimeStart[2].timeStart = "9:00";

            try
            {
                email = new Email();
                redmineManager = new RedmineManager(host, key);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to redmine host! " + ex.Message);
            }
        }
        
        private bool isDayOfWeek(DayOfWeek dayOfWeek, out string timeStart)
        {
            bool result = false;
            timeStart = "";

            foreach (DayTimeStart dayTime in dayTimeStart)
            {
                if (dayTime.dayOfWeek == dayOfWeek)
                {
                    result = true;
                    timeStart = dayTime.timeStart;
                    break;
                }
            }

            return result;
        }

        private DateTime GetDate(DateTime stepDateTime, string timeStart)
        {                    
            int year = stepDateTime.Year;
            int mounth = stepDateTime.Month;
            int day = stepDateTime.Day;

            string[] timeParts = timeStart.Split(new char[1] { ':' });
            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);

            return new DateTime(year, mounth, day, hour, minute, 0);            
        }

        public TimeSpan GetTimeStart()
        {
            bool done = false;
            string timeStart = "";

            TimeSpan dt = new TimeSpan();

            DateTime stepDateTime = DateTime.Now;
            DateTime currentDateTime = DateTime.Now;

            while (!done)
            {                
                if (isDayOfWeek(stepDateTime.DayOfWeek, out timeStart) && ( (dt = ( GetDate(stepDateTime, timeStart) - currentDateTime)).TotalSeconds > 0) )
                {                    
                    done = true;                    
                }
                else
                    stepDateTime = stepDateTime.AddDays(1);
            }

            Console.WriteLine("near date to start = {0}, dt = {1} hours", GetDate(stepDateTime, timeStart).ToString(), dt.TotalHours);

            return dt;
        }

        public void MakeNotificationToTime()
        {
            GetUserOpenIssue();
            SetNotificationUser();
            ShowNotification();
            SendEmail();
        }

        public void GetUserOpenIssue()
        {
            listUserRedmine.Clear();

            try
            {
                NameValueCollection param = new NameValueCollection { { "Id", "*" } };
                foreach (var user in redmineManager.GetObjects<User>(param))
                {
                    UserRedmine userRedmine = new UserRedmine();
                    userRedmine.Value = user;
                    listUserRedmine.Add(userRedmine);
                }

                param = new NameValueCollection { { "status_id", "open" } };
                foreach (var issue in redmineManager.GetObjects<Issue>(param))
                {
                    UserRedmine userRedmine = null;

                    if (issue.AssignedTo == null)
                    {
                        userRedmine = listUserRedmine.Find(x => x.Value.Id == issue.Author.Id);
                    }
                    else
                    {
                        userRedmine = listUserRedmine.Find(x => x.Value.Id == issue.AssignedTo.Id);
                    }

                    userRedmine.ListIssue.Add(issue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetUserOpenIssue: {0}", ex.Message);
            }
            
        }

        public void SendEmail(params string[] noNameSendMail)
        {
            bool isNameSendMail = true;

            foreach (var userRedmine in listUserRedmine)
            {
                isNameSendMail = true;
                foreach (string name in noNameSendMail)
                {
                    if (userRedmine.FullName.Equals(name))
                    {
                        isNameSendMail = false;
                        break;
                    }
                }

                if (isNameSendMail)
                {

                }
                   

                if (userRedmine.Value.LastName == "Чамин")
                {
                    email.SendMail(userRedmine, "Уведомление о просроченных задачах");
                }
            }
        }

        private bool IsMorePeriod(DateTime issueDate, int days = 0)
        {
            bool isMore = false;

            DateTime now = DateTime.Now;
            TimeSpan dt = (now - issueDate);

            if ((dt.TotalDays > days) & (days != 0))
                isMore = true;            

            return isMore;
        }
        
        private void SetMessage(UserRedmine userRedmine, Issue issue)
        {
            string message = "";

            if (userRedmine.message == "")
            {
                message = "Здравствуйте!" + "\n";
                message += "Уважаемый(ая) " + userRedmine.FullName + "!" + "\n";
                message += "У вас есть в системе открытые просроченные задания, которые в случае их выполнения необходимо закрыть (статус - закрыта) или согласовать новую дату завершения : " + "\n";
                message += "\n";

                userRedmine.message = message;
            }

            if (issue.DueDate != null)
                userRedmine.message += "Проект: " + issue.Project.Name + "-> задание № " + issue.Id + ": " + issue.Subject.Trim() + "->" + 
                                    " дата завершения: " + issue.DueDate.Value.ToShortDateString() + "\n";
            else
                userRedmine.message += "Проект: " + issue.Project.Name + "-> задание № " + issue.Id + ": " + issue.Subject.Trim() + "->" +
                                    " дата завершения: -" + "\n";

            userRedmine.message += "\n";
        }

        public void SetNotificationUser()
        {
            foreach (var userRedmine in listUserRedmine)
            {
                userRedmine.message = "";                

                foreach (var issue in userRedmine.ListIssue)
                {
                    if ((issue.DueDate == null) || (IsMorePeriod(issue.DueDate.Value, 1)) )
                    {
                        SetMessage(userRedmine, issue);
                    }
                }
            }
        }

        public void ShowNotification()
        {
            foreach (var userRedmine in listUserRedmine)
            {
                Console.WriteLine(userRedmine.message);
            }
        }

        
    }
}
