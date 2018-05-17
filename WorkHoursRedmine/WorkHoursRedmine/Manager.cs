using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinRedminePlaning
{
    class Manager
    {
        string host = "188.242.201.77";
        string apiKey = "70b1a875928636d8d3895248309344ea2bca6a5f";
        RedmineManager redmineManager;

        public List<RedmineUser> listUserRedmine = new List<RedmineUser>();
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

        public void GetUserFromRedmine()
        {
            foreach (var user in redmineManager.GetObjects<User>(new NameValueCollection { { "id", "*" } }))
            {
                RedmineUser redmUser = new RedmineUser();
                redmUser.Value = user;
                listUserRedmine.Add(redmUser);

                foreach (var time in redmineManager.GetObjects<TimeEntry>(new NameValueCollection { { user.Id.ToString(), "*"} }))
                {
                    redmUser.listTimeEntry.Add(time);
                }
                foreach (var issue in redmineManager.GetObjects<Issue>(new NameValueCollection { {user.Id.ToString(), "*" } }))
                {
                    redmUser.listIssue.Add(issue);
                }
            }
        }

        public RedmineUser FindUser(int Id)
        {
            RedmineUser redmUser = null;



            return redmUser;
        }


        
    }
}
