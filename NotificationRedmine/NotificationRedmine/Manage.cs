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

        public void GetUserIssue()
        {
            if (redmineManager != null)
            {
                NameValueCollection param = new NameValueCollection { {"Id", "*" } };
                foreach (var user in redmineManager.GetObjects<User>(param))
                {
                    UserRedmine userRedmine = new UserRedmine();
                    userRedmine.Value = user;
                    listUserRedmine.Add(userRedmine);
                    Console.WriteLine("User = {0}", userRedmine.Value.LastName.ToString());
                    
                    param = new NameValueCollection { { "assigned_to_id", user.Id.ToString()}, { "status_id", "open" } };
                    foreach (var issue in redmineManager.GetObjects<Issue>(param))
                    {
                        userRedmine.ListIssue.Add(issue);
                        Console.WriteLine("Issue = {0}", issue.Subject.ToString());                                                                        
                    }

                }
            }
        }

        
    }
}
