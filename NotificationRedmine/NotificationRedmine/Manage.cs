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
            public string dayOfWeek;
            public string timeStart;
        }

        public Manage(string host, string key)
        {
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
        
        public void GetUserOpenIssue()
        {
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
                message = "Уважаемый(ая) " + userRedmine.FullName + "!" + "\n";
                message += "У вас имеются открытые просроченные задания, которые в случае их выполнения необходимо закрыть или согласовать новую дату завершения : " + "\n";
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
