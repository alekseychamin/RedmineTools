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
        public User Value;
        public List<Issue> ListIssue = new List<Issue>();        
    }
}
