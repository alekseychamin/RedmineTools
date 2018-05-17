using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Types;

namespace WinRedminePlaning
{
    class RedmineUser
    {
        public User Value;
        public List<TimeEntry> listTimeEntry = new List<TimeEntry>();
        public List<Issue> listIssue = new List<Issue>();        
    }
}
