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
    public partial class MainForm : Form
    {
        Manager manager;
        //Dictionary<int, string> mounth = new Dictionary<int, string>();
        Dictionary<string, string> bossName = new Dictionary<string, string>();
        PrintForm printForm;        
        LoadMWHForm mwhLoadForm;

        string[] activityNotWorkHours = new string[3] { "Отпуск", "Больничный", "Отгул" };

        public MainForm()
        {
            InitializeComponent();            

            manager = new Manager();                                   
        }
        
        private void GetDateFromRedmine()
        {
            manager.GetIssue_ProjectFromRedmine();            
            manager.GetUser_GroupFromRedmine();
            manager.GetTimeEntryFromRedmine();
        }
        private void but_loadYWH_Click(object sender, EventArgs e)
        {                      
            
            mwhLoadForm = new LoadMWHForm(manager, TypeView.LoadYWH);
            
            manager.Update += mwhLoadForm.UpdateForm;

            mwhLoadForm.Text = "Годовой ФРВ";
            mwhLoadForm.Show();
        }
        

        private void butReportExcel_Click(object sender, EventArgs e)
        {
            //manager.excelMethods.MakeSheetReportUsers(manager.listUserRedmine, Application.StartupPath);
        }

        private void but_loadUser_Click(object sender, EventArgs e)
        {            
                       
            mwhLoadForm = new LoadMWHForm(manager, TypeView.LoadUser);

            manager.Update += mwhLoadForm.UpdateForm;

            mwhLoadForm.Text = "ФРВ специалистов";
            mwhLoadForm.Show();
        }

        private void but_loadGroup_Click(object sender, EventArgs e)
        {            
            
            mwhLoadForm = new LoadMWHForm(manager, TypeView.LoadGroup);

            manager.Update += mwhLoadForm.UpdateForm;

            mwhLoadForm.Text = "ФРВ групп";
            mwhLoadForm.Show();
        }

        private void but_loadProject(object sender, EventArgs e)
        {            
            
            mwhLoadForm = new LoadMWHForm(manager, TypeView.LoadProject);

            manager.Update += mwhLoadForm.UpdateForm;

            mwhLoadForm.Text = "Проекты ФРВ";
            mwhLoadForm.Show();
        }

        private void but_updateRedmineData(object sender, EventArgs e)
        {
            GetDateFromRedmine();            

            manager.CreateListLoadYWH();
            manager.CreateListLoadUser();
            manager.CreateListLoadGroup();
            manager.CreateListLoadProject();

            manager.UpdateForm();
            //manager.SaveCSVFileListIssue("test.csv");
            MessageBox.Show("Data is updated!");
        }
    }
}
