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
    public partial class IssueDayForm : Form
    {
        LoadMWH loadMWH;
        Manager manager;

        TypeView typeView;                        

        public IssueDayForm(LoadMWH loadMWH, Manager manager, TypeView typeView)
        {
            InitializeComponent();
            this.loadMWH = loadMWH;
            this.manager = manager;
            this.typeView = typeView;

            MakeCaptionColumnsDWH();
            ShowLoad_TimeDWH();
        }

        public void UpdateIssueInfo()
        {
            MessageBox.Show("Hi! " + this.Text);
            ShowLoad_TimeDWH();
        }

        private void MakeCaptionColumnsDWH()
        {
            listLoadDWH.Columns.Clear();
            listLoadDWH.View = View.Details;

            if ((typeView == TypeView.LoadIssueDWH) | (typeView == TypeView.LoadTimeDWH))
            {
                listLoadDWH.Columns.Add("Id", -2, HorizontalAlignment.Left);
                listLoadDWH.Columns.Add("Проект", -2, HorizontalAlignment.Left);
                listLoadDWH.Columns.Add("Наименование задачи", -2, HorizontalAlignment.Left);
                listLoadDWH.Columns.Add("Старт", -2, HorizontalAlignment.Left);
                listLoadDWH.Columns.Add("Финиш", -2, HorizontalAlignment.Left);
                listLoadDWH.Columns.Add("Кол-во ч", -2, HorizontalAlignment.Left);
                listLoadDWH.Columns.Add("Исполнитель", -2, HorizontalAlignment.Left);

                DateTime start = loadMWH.dateStartMonth;
                DateTime finish = loadMWH.dateFinishMonth;
                DateTime curDate = start;

                while (curDate.CompareTo(finish) <= 0)
                {
                    listLoadDWH.Columns.Add(curDate.ToString("dd.ddd"));
                    curDate = curDate.AddDays(1);
                }
            }

            if ((typeView == TypeView.LoadShortIssueDWH) | (typeView == TypeView.LoadShortTimeDWH))
            {
                listLoadDWH.Columns.Add("Id", -2, HorizontalAlignment.Left);
                listLoadDWH.Columns.Add("Проект", -2, HorizontalAlignment.Left);
                listLoadDWH.Columns.Add("Кол-во ч", -2, HorizontalAlignment.Left);
            }

            AutoFitColumn(listLoadDWH);                        
            listLoadDWH.GridLines = true;            
        }

        private void SetColorValue(ListView listView, int row, string[] array, 
                                        float value, Color color, Operation operation)
        {
            for (int i = 0; i < array.Length; i++)
            {
                switch (operation)
                {
                    case Operation.Equal:
                        try
                        {
                            if (Convert.ToDouble(array[i]) == value)
                            {
                                listView.Items[row].UseItemStyleForSubItems = false;
                                listView.Items[row].SubItems[i].BackColor = color;
                            }                            
                        }
                        catch
                        {

                        }
                        break;

                    case Operation.More:
                        try
                        {
                            if ((Convert.ToDouble(array[i]) > value) & i >=7 )
                            {
                                listView.Items[row].UseItemStyleForSubItems = false;
                                listView.Items[row].SubItems[i].BackColor = color;
                            }
                        }
                        catch
                        {

                        }
                        break;
                }                 
            }
        }
        
        private void AutoFitColumn(ListView listView)
        {
            foreach (ColumnHeader column in listView.Columns)
            {
                column.Width = -2;
            }
        }

        private void ShowLoad_TimeDWH()
        {
            List<string> list = new List<string>();
            listLoadDWH.Items.Clear();            
            ListViewItem lvi;
            string[] array;
            int iLine = 0;

            loadMWH.listLoadIssue.Sort();
            loadMWH.listLoadTimeEntry.Sort();

            switch (typeView)
            {
                case TypeView.LoadShortIssueDWH:
                    foreach (LoadProject loadProject in loadMWH.listLoadProject)
                    {
                        list.Clear();

                        string[] line = { loadProject.userProject.Id.ToString(),
                                          loadProject.userProject.Name,                                    
                                          loadProject.EstimatedMWH.ToString("0.0") };
                        foreach (string s in line)
                        {
                            list.Add(s);
                        }
                        
                        array = list.Select(i => i.ToString()).ToArray();
                        lvi = new ListViewItem(array);
                        listLoadDWH.Items.Add(lvi);

                        SetColorValue(listLoadDWH, iLine, array, 0, Color.Yellow, Operation.Equal);

                        iLine++;

                    }

                    list.Clear();



                    string[] lineSumShortIssue = { "",
                            "Итого",                            
                            loadMWH.EstimatedMonthHours.ToString("0.0") };

                    foreach (string s in lineSumShortIssue)
                    {
                        list.Add(s);
                    }
                    
                    array = list.Select(i => i.ToString()).ToArray();
                    lvi = new ListViewItem(array);
                    listLoadDWH.Items.Add(lvi);

                    SetColorValue(listLoadDWH, iLine, array, 0, Color.Yellow, Operation.Equal);

                    AutoFitColumn(listLoadDWH);

                    break;

                case TypeView.LoadShortTimeDWH:
                    foreach (LoadProject loadProject in loadMWH.listLoadProject)
                    {
                        list.Clear();

                        string[] line = { loadProject.userProject.Id.ToString(),
                                          loadProject.userProject.Name,
                                          loadProject.FactMWH.ToString("0.0") };
                        foreach (string s in line)
                        {
                            list.Add(s);
                        }

                        array = list.Select(i => i.ToString()).ToArray();
                        lvi = new ListViewItem(array);
                        listLoadDWH.Items.Add(lvi);

                        SetColorValue(listLoadDWH, iLine, array, 0, Color.Yellow, Operation.Equal);

                        iLine++;

                    }

                    list.Clear();



                    string[] lineSumShortTime = { "",
                            "Итого",
                            loadMWH.FactMonthHours.ToString("0.0") };

                    foreach (string s in lineSumShortTime)
                    {
                        list.Add(s);
                    }

                    array = list.Select(i => i.ToString()).ToArray();
                    lvi = new ListViewItem(array);
                    listLoadDWH.Items.Add(lvi);

                    SetColorValue(listLoadDWH, iLine, array, 0, Color.Yellow, Operation.Equal);

                    AutoFitColumn(listLoadDWH);

                    break;

                case TypeView.LoadIssueDWH:
                    foreach (LoadIssue loadIssue in loadMWH.listLoadIssue)
                    {
                        User user = manager.listUser.Find(x => x.Id == loadIssue.issue.AssignedTo.Id);
                        list.Clear();

                        string userName = "";
                        if (user != null)
                            userName = user.LastName + " " + user.FirstName;
                        else
                            userName = loadIssue.issue.AssignedTo.Name;

                        string[] line = { loadIssue.issue.Id.ToString(),
                                    loadIssue.issue.Project.Name,
                                    loadIssue.issue.Subject,
                                    loadIssue.dateStartIssue.ToShortDateString(),
                                    loadIssue.dateFinishIssue.ToShortDateString(),
                                    loadIssue.estimatedIssueHours.ToString("0.0"),
                                    userName };
                        foreach (string s in line)
                        {
                            list.Add(s);
                        }

                        foreach (LoadDWH loadDWH in loadIssue.listLoadDWH)
                        {
                            list.Add(loadDWH.hoursDay.ToString("0.0"));
                        }
                        array = list.Select(i => i.ToString()).ToArray();
                        lvi = new ListViewItem(array);
                        listLoadDWH.Items.Add(lvi);

                        SetColorValue(listLoadDWH, iLine, array, 0, Color.Yellow, Operation.Equal);
                        SetColorValue(listLoadDWH, iLine, array, 8, Color.Red, Operation.More);

                        iLine++;
                    }

                    list.Clear();



                    string[] lineSum = { "",
                            "Итого в день",
                            "",
                            loadMWH.dateStartMonth.ToShortDateString(),
                            loadMWH.dateFinishMonth.ToShortDateString(),
                            loadMWH.EstimatedMonthHours.ToString("0.0"),
                            "" };

                    foreach (string s in lineSum)
                    {
                        list.Add(s);
                    }

                    foreach (LoadDWH loadDWH in loadMWH.listLoadDWH)
                    {
                        list.Add(loadDWH.hoursDay.ToString("0.0"));
                    }

                    array = list.Select(i => i.ToString()).ToArray();
                    lvi = new ListViewItem(array);
                    listLoadDWH.Items.Add(lvi);

                    SetColorValue(listLoadDWH, iLine, array, 0, Color.Yellow, Operation.Equal);                    
                    SetColorValue(listLoadDWH, iLine, array, 8, Color.Red, Operation.More);

                    AutoFitColumn(listLoadDWH);

                    break;

                case TypeView.LoadTimeDWH:

                    foreach (LoadTimeEntry loadTime in loadMWH.listLoadTimeEntry)
                    {
                        User user = manager.listUser.Find(x => x.Id == loadTime.userTime.time.User.Id);
                        list.Clear();

                        string userName = "";
                        if (user != null)
                            userName = user.LastName + " " + user.FirstName;
                        else
                            userName = loadTime.userTime.nameUser;

                        string[] line = { loadTime.userTime.time.Id.ToString(),
                                          loadTime.userTime.nameProject,
                                          loadTime.userTime.subject,
                                          loadTime.dateStart.ToShortDateString(),
                                          loadTime.dateFinish.ToShortDateString(),
                                          loadTime.factMonthHours.ToString("0.0"),
                                          userName };
                        foreach (string s in line)
                        {
                            list.Add(s);
                        }

                        foreach (LoadDWH loadDWH in loadTime.listLoadDWH)
                        {
                            list.Add(loadDWH.factDayHours.ToString("0.0"));
                        }
                        array = list.Select(i => i.ToString()).ToArray();
                        lvi = new ListViewItem(array);
                        listLoadDWH.Items.Add(lvi);

                        SetColorValue(listLoadDWH, iLine, array, 0, Color.Yellow, Operation.Equal);
                        SetColorValue(listLoadDWH, iLine, array, 8, Color.Red, Operation.More);
                        iLine++;
                    }

                    list.Clear();



                    string[] lineSumTime = { "",
                                         "Итого в день",
                                         "",
                                         loadMWH.dateStartMonth.ToShortDateString(),
                                         loadMWH.dateFinishMonth.ToShortDateString(),
                                         loadMWH.FactMonthHours.ToString("0.0"),
                                         "" };

                    foreach (string s in lineSumTime)
                    {
                        list.Add(s);
                    }

                    foreach (LoadDWH loadDWH in loadMWH.listLoadDWH)
                    {
                        list.Add(loadDWH.factDayHours.ToString("0.0"));
                    }

                    array = list.Select(i => i.ToString()).ToArray();
                    lvi = new ListViewItem(array);
                    listLoadDWH.Items.Add(lvi);

                    SetColorValue(listLoadDWH, iLine, array, 0, Color.Yellow, Operation.Equal);

                    AutoFitColumn(listLoadDWH);

                    break;
            }                                               
        }

        private void ListViewSelectIndices(ListView listView, ListView.SelectedIndexCollection listIndices)
        {
            if (listView.Items.Count > 0)
            {
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    listView.Items[i].Selected = false;
                }

                if (listIndices.Count > 0)
                {
                    for (int i = 0; i < listIndices.Count; i++)
                    {
                        //MessageBox.Show(" i = " + listIndices[i].ToString());                        
                        listView.Items[listIndices[i]].Selected = true;
                        //listView.Items[listIndices[i]].Focused = true;
                        //listView.Select();
                    }
                }
            }
        }

        private void listLoadDWH_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            IssueDayForm issueMonthForm;

            switch (typeView)
            {
                case TypeView.LoadIssueDWH:
                    issueMonthForm = new IssueDayForm(loadMWH, manager, TypeView.LoadShortIssueDWH);
                    manager.Update += issueMonthForm.UpdateIssueInfo;
                    issueMonthForm.Text = this.Text;
                    issueMonthForm.Show();
                    break;
                case TypeView.LoadTimeDWH:
                    issueMonthForm = new IssueDayForm(loadMWH, manager, TypeView.LoadShortTimeDWH);
                    manager.Update += issueMonthForm.UpdateIssueInfo;
                    issueMonthForm.Text = this.Text;
                    issueMonthForm.Show();
                    break;
            }
            
        }

        private void IssueDayForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            manager.Update -= this.UpdateIssueInfo;
        }
    }
}
