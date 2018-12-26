using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinRedminePlaning
{
    public class RedmineData
    {
        public List<Project> listProject = new List<Project>();
        public List<UserTimeEntry> listUserTimeEntry = new List<UserTimeEntry>();
        public List<Issue> listIssue = new List<Issue>();
        public List<Issue> listOpenIssue = new List<Issue>();
        public List<User> listUser = new List<User>();

    }
}
