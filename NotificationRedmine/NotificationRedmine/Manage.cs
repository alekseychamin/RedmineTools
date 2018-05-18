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

        public Manage(string host, string key)
        {
            try
            {
                redmineManager = new RedmineManager(host, key);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to redmine host! " + ex.Message);
            }
        }
        
        public void GetUserOpenIssue()
        {
            if (redmineManager != null)
            {
                NameValueCollection param = new NameValueCollection { {"Id", "*" } };
                foreach (var user in redmineManager.GetObjects<User>(param))
                {
                    UserRedmine userRedmine = new UserRedmine();
                    userRedmine.Value = user;
                    listUserRedmine.Add(userRedmine);                    

                    param = new NameValueCollection { { "status_id", "open" } };
                    foreach (var issue in redmineManager.GetObjects<Issue>(param))
                    {
                        if ( (issue.AssignedTo == null) && (issue.Author.Id == userRedmine.Value.Id) || 
                             ( (issue.AssignedTo != null) && (issue.AssignedTo.Id == userRedmine.Value.Id) ))
                        {
                            userRedmine.ListIssue.Add(issue);
                        }                                                                
                    }                    
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
                message = "Уважаемый(ая) " + userRedmine.Value.LastName + " " + userRedmine.Value.FirstName + "!" + "\n";
                message += "У вас имеются открытые просроченные задания, которые в случае их выполнения необходимо закрыть или согласовать новую дату завершения : " + "\n";
                
                userRedmine.message = message;
            }

            userRedmine.message += "Проект: " + issue.Project.Name + " Id задачи: " + issue.Id + " задание: " + issue.Subject + "\n";
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
