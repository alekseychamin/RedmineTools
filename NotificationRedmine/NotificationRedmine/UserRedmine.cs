using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redmine.Net.Api.Types;

namespace NotificationRedmine
{
    class UserIssue : IComparable
    {
        public Issue issue;
        public string message = "";

        public int CompareTo(object obj)
        {
            UserIssue orderToCompare = obj as UserIssue;
            if ((orderToCompare.issue.DueDate == null) & (issue.DueDate == null))
            {
                return 0;
            }
            else if ((orderToCompare.issue.DueDate == null) & (issue.DueDate != null))
            {
                return 1;
            }
            else if ((orderToCompare.issue.DueDate != null) & (issue.DueDate == null))
            {
                return -1;
            }
            else if ((orderToCompare.issue.DueDate != null) & (issue.DueDate != null))
            {
                if (orderToCompare.issue.DueDate < issue.DueDate)
                    return 1;
                if (orderToCompare.issue.DueDate > issue.DueDate)
                    return -1;
            }
            return 0;
        }
    }

    class UserRedmine
    {        
        public User Value;
        public string EmailAddress = "";
        public string messageSend = "";
        public bool isNeedToSend = false;        
        public List<UserIssue> ListUserIssue = new List<UserIssue>();        

        public string FullName
        {
            get
            {
                return Value.LastName + " " + Value.FirstName;
            }
        }
    }
}
