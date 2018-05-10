using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinRedminePlaning
{
    class RedmineProject
    {
        private Project value = null;
        private List<Issue> listIssue = new List<Issue>();
        public Project Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }        
        public List<Issue> ListIssue
        {
            get
            {
                return listIssue;
            }
        }

        public void AddIssue(Issue issue)
        {
            if ((value != null) && (issue != null))
            {
                if (issue.Project.Id == value.Id)
                {
                    listIssue.Add(issue);
                }
            }
        }
    }

    class RedmineProjects
    {
        private List<RedmineProject> listProject = new List<RedmineProject>();
        public List<RedmineProject> ListProject
        {
            get
            {
                return listProject;
            }
        }

        public void AddProject(Project project)
        {
            if (project != null)
            {
                RedmineProject findProject = null;
                findProject = listProject.Find(x => x.Value.Id == project.Id);
                if (findProject == null)
                {
                    RedmineProject redmProject = new RedmineProject();
                    redmProject.Value = project;                    
                    listProject.Add(redmProject);
                }
            }
        }        
    }    
}
