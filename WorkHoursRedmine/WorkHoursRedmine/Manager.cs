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
        public ExcelMethods excelMethods = new ExcelMethods();

        RedmineProjects projects = new RedmineProjects();

        public RedmineProjects Projects
        {
            get
            {
                return projects;
            }
        }

        public void LoadFromRedmine()
        {
            var redmineManager = new RedmineManager(host, apiKey);
            var parameters = new NameValueCollection { { "status_id", "*" } };

            projects.ListProject.Clear();

            foreach (var redmineProject in redmineManager.GetObjects<Project>(999, 0))
            {
                projects.AddProject(redmineProject);
                int count = projects.ListProject.Count;

                foreach (var redmineIssue in redmineManager.GetObjects<Issue>(999, 0)) 
                {                    
                    projects.ListProject[count - 1].AddIssue(redmineIssue);
                    Console.WriteLine(redmineIssue.Id.ToString());
                }

            }

            int i = 0;
            while (i < projects.ListProject.Count)
            {
                if (projects.ListProject[i].ListIssue.Count == 0)
                    projects.ListProject.RemoveAt(i);
                else
                    i++;
            }
            
        }


        
    }
}
