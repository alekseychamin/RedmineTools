using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Types;

namespace NotificationRedmine
{
    class UserRedmine
    {
        string fullName;
        public User Value;
        public string message;
        public List<Issue> ListIssue = new List<Issue>();        

        public string FullName
        {
            get
            {
                return Value.LastName + " " + Value.FirstName;
            }
        }
    }
}
