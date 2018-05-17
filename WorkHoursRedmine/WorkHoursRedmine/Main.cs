using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinRedminePlaning
{
    public partial class Main : Form
    {
        Manager manager;
        public Main()
        {
            InitializeComponent();
            manager = new Manager();
                        
            listViewUser.Columns.Add("Id", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("ФИО", -2, HorizontalAlignment.Left);
            //listViewUser.Columns.Add("Статус", -2, HorizontalAlignment.Left);
            //listViewUser.Columns.Add("Открытый", -2, HorizontalAlignment.Left);            
            //listViewUser.Columns.Add("Дата создания", -2, HorizontalAlignment.Left);
            //listViewUser.Columns.Add("Дата обновления", -2, HorizontalAlignment.Left);

            listViewIssues.Columns.Add("Id", -2, HorizontalAlignment.Left);
            listViewIssues.Columns.Add("Название", -2, HorizontalAlignment.Left);
        }

        private void ShowRedmineIssue(int Id)
        {
            if (Id > 0)
            {
                listViewIssues.Items.Clear();
                RedmineProject redmProject = projects.ListProject.Find(x => x.Value.Id == Id);
                if (redmProject != null)
                {
                    foreach (Issue issue in redmProject.ListIssue)
                    {
                        string[] line = { issue.Id.ToString(), issue.Subject };
                        ListViewItem lvi = new ListViewItem(line);
                        listViewIssues.Items.Add(lvi);
                    }
                }
            }
        }

        private void ShowUserRedmine()
        {
            listViewUser.Items.Clear();
            foreach (RedmineUser user in manager.listUserRedmine)
            {
                string[] line = {user.Value.Id.ToString(), user.Value.FirstName + " " + user.Value.LastName};
                ListViewItem lvi = new ListViewItem(line);
                listViewUser.Items.Add(lvi);
            }
        }

        private void but_loadRedmine_Click(object sender, EventArgs e)
        {
            manager.GetUserFromRedmine();
            ShowUserRedmine();
        }        

        private void but_SaveExcel_Click(object sender, EventArgs e)
        {
            //if (manager.Projects.ListProject.Count != 0)
            //    manager.excelMethods.UpdateExcelProject(manager.Projects, @"D:\Project C#\WinRedminePlaning\План УРАС.xlsx");
        }

        private void listViewUser_MouseClick(object sender, MouseEventArgs e)
        {
            if (listViewUser.SelectedItems[0] != null)
            {
                ListViewItem lvi = listViewUser.SelectedItems[0];                
                ShowRedmineIssue(manager.Projects, Convert.ToInt16(lvi.SubItems[0].Text));
                //Console.WriteLine(lvi.SubItems[0].ToString());
            }
        }
    }
}
