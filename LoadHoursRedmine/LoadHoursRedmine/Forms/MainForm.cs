﻿using Redmine.Net.Api.Types;
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
        //Dictionary<string, string> bossName = new Dictionary<string, string>();
        PrintForm printForm;
        LoadMWHForm mwhLoadForm;

        //string[] activityNotWorkHours = new string[3] { "Отпуск", "Больничный", "Отгул" };

        public MainForm()
        {
            InitializeComponent();
            RedmineData redmineData = new RedmineData();
            manager = new Manager(redmineData);
        }

        private void GetDateFromRedmine(bool redmineProject, bool selectedProject, bool bothProject, string textProject)
        {
            manager.GetIssue_ProjectFromRedmine(redmineProject, selectedProject, bothProject, textProject);
            manager.GetUser_GroupFromRedmine();
            manager.GetTimeEntryFromRedmine();
        }
        private void but_loadYWH_Click(object sender, EventArgs e)
        {
            mwhLoadForm = new LoadMWHForm(manager, TypeView.LoadYWH, TypeView.LoadYWH);

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
            mwhLoadForm = new LoadMWHForm(manager, TypeView.LoadUser, TypeView.LoadUser);

            manager.Update += mwhLoadForm.UpdateForm;

            mwhLoadForm.Text = "ФРВ специалистов";
            mwhLoadForm.Show();
        }

        private void but_loadGroup_Click(object sender, EventArgs e)
        {
            mwhLoadForm = new LoadMWHForm(manager, TypeView.LoadGroup, TypeView.LoadGroup);

            manager.Update += mwhLoadForm.UpdateForm;

            mwhLoadForm.Text = "ФРВ групп";
            mwhLoadForm.Show();
        }

        private void but_loadProject(object sender, EventArgs e)
        {
            mwhLoadForm = new LoadMWHForm(manager, TypeView.LoadProject, TypeView.LoadProject);

            manager.Update += mwhLoadForm.UpdateForm;

            mwhLoadForm.Text = "Проекты ФРВ";
            mwhLoadForm.Show();
        }

        private void but_updateRedmineData(object sender, EventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            string textProject = rTProject.Text;
            bool redmineProject = rBRedmineProject.Checked;
            bool selProject = rBSelectedProject.Checked;
            bool bothProject = rBBothProject.Checked;

            GetDateFromRedmine(redmineProject, selProject, bothProject, textProject);

            manager.CreateListLoadYWH();
            manager.CreateListLoadUser();
            manager.CreateListLoadGroup();
            manager.CreateListLoadProject();

            manager.UpdateForm();

            watch.Stop();
            MessageBox.Show("Данные загружены за " + (watch.ElapsedMilliseconds / 1000).ToString() + " сек");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<CSVLoadYWH> listCSVLoadYWH;
            List<CSVLoadGroupYWH> listCSVLoadGroupYWH;
            List<CSVLoadProjectYWH> listCSVLoadProjectYWH;
            List<CSVLoadProjectUserYWH> listCSVLoadProjectUserYWH;
            List<CSVLoadUserYWH> listCSVLoadUserYWH;

            manager.MakeListLoadYWHSave(out listCSVLoadYWH);
            manager.SaveCSVFileListIssue<CSVLoadYWH>("ФРВ на год от " + DateTime.Now.ToString() + ".csv", listCSVLoadYWH);

            manager.MakeListLoadGroupSave(out listCSVLoadGroupYWH);
            manager.SaveCSVFileListIssue<CSVLoadGroupYWH>("ФРВ групп от " + DateTime.Now.ToString() + ".csv", listCSVLoadGroupYWH);

            manager.MakeListLoadProjectSave(out listCSVLoadProjectYWH);
            manager.SaveCSVFileListIssue<CSVLoadProjectYWH>("ФРВ проектов от " + DateTime.Now.ToString() + ".csv", listCSVLoadProjectYWH);

            manager.MakeListLoadProjectUserSave(out listCSVLoadProjectUserYWH);
            manager.SaveCSVFileListIssue<CSVLoadProjectUserYWH>("ФРВ проектов спец от " + DateTime.Now.ToString() + ".csv", listCSVLoadProjectUserYWH);

            manager.MakeListLoadUserSave(out listCSVLoadUserYWH);
            manager.SaveCSVFileListIssue<CSVLoadUserYWH>("ФРВ специалистов от " + DateTime.Now.ToString() + ".csv", listCSVLoadUserYWH);
        }

        private void but_LoadExperiedUser(object sender, EventArgs e)
        {
            mwhLoadForm = new LoadMWHForm(manager, TypeView.LoadExperiedUser, TypeView.LoadExperiedUser);

            manager.Update += mwhLoadForm.UpdateForm;

            mwhLoadForm.Text = "Просроченные задания специалистов";
            mwhLoadForm.Show();
        }

        private void but_LoadExperiedProject(object sender, EventArgs e)
        {
            mwhLoadForm = new LoadMWHForm(manager, TypeView.LoadExperiedProject, TypeView.LoadExperiedProject);

            manager.Update += mwhLoadForm.UpdateForm;

            mwhLoadForm.Text = "Просроченные проекты";
            mwhLoadForm.Show();
        }

        private void but_LoadProjectUser(object sender, EventArgs e)
        {
            mwhLoadForm = new LoadMWHForm(manager, TypeView.LoadProjectUser, TypeView.LoadProjectUser);

            manager.Update += mwhLoadForm.UpdateForm;

            mwhLoadForm.Text = "Проекты по специалистам";
            mwhLoadForm.Show();
        }

        private void but_ReportIssue(object sender, EventArgs e)
        {
            manager.MakeReportIssue();
            ReportForm report = new ReportForm(manager, "", TypeView.ReportIssue);
            report.Text = "Отчет о задачах с задержкой";
            report.Show();
        }

        private void but_ReportProject(object sender, EventArgs e)
        {
            manager.MakeReportProject();
            ReportForm report = new ReportForm(manager, "", TypeView.ReportProject);
            report.Text = "Отчет о проектах";
            report.Show();
        }
    }
}
