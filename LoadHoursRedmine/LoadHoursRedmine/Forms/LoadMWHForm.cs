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
    public partial class LoadMWHForm : Form
    {
        Manager manager;
               
        private TypeView typeView; // тип окна: 1 - загрузка ФРВ общая, 0 - загрузка ФРВ сотрудников, 2 - загрузка групп
        

        IssueDayForm issueMonthForm;

        public LoadMWHForm(Manager manager, TypeView typeView)
        {
            InitializeComponent();
            this.manager = manager;
            this.typeView = typeView;
            Start();
        }

        private void AutoFitColumn(ListView listView)
        {
            foreach (ColumnHeader column in listView.Columns)
            {
                column.Width = -2;
            }
        }

        private void Start()
        {
            GetYearForReport();
            MakeCaptionColums();
            manager.listLoadUser.Sort();

            if (comboYear.Items.Count > 0)
            {
                string year = comboYear.SelectedItem.ToString();
                ShowLoadMWH(year);
            }            
        }
               
        public void UpdateForm()
        {
            if (comboYear.Items.Count != 0)
            {
                //MessageBox.Show("Hi! " + this.Text);
                string year = comboYear.Text.ToString();
                ShowLoadMWH(year);                
            }
            else
            {
                Start();
            }
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
                            if ((Convert.ToDouble(array[i]) > value) & i >= 7)
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

        private void AddLineLoad(LoadYWH loadYWH, ref List<string> list)
        {
            if (list.Find(x => x.Contains("план")) != null)
            {
                for (int i = 0; i <= 11; i++)
                {
                    string sl = loadYWH.listLoadMWH[i].EstimatedMonthHours.ToString("0.0") + "/" +
                                loadYWH.listLoadMWH[i].EstimatedMontHumans.ToString("0.0");
                    list.Add(sl);

                }
            }
            else
            {
                for (int i = 0; i <= 11; i++)
                {
                    string sl = loadYWH.listLoadMWH[i].FactMonthHours.ToString("0.0") + "/" +
                                loadYWH.listLoadMWH[i].FactMontHumans.ToString("0.0");
                    list.Add(sl);
                }
            }            
        }

        private void GetList(string[] array, ref List<string> list)
        {
            list.Clear();

            foreach (string line in array)
            {
                list.Add(line);
            }           
        }
        
        private void SetLineToListView(List<string> list, LoadYWH loadYWH, ListView listOut)
        {
            string[] the_array = list.Select(i => i.ToString()).ToArray();
            ListViewItem lvi = new ListViewItem(the_array);
            lvi.Tag = loadYWH;
            listOut.Items.Add(lvi);

           
        }

        private void MakeLineToListView(int numberYear, string value, ref List<string> listLine, ListView listOut, 
                                        LoadYWH loadYWH = null, LoadUser loadUser = null, LoadProject loadProject = null)
        {
            LoadYWH loadYWHcur;                        

            if (loadUser != null)
                loadYWHcur = manager.FindLoadYWH(numberYear, loadUser.listLoadYWH);
            else
            {
                if (loadProject != null)
                    loadYWHcur = manager.FindLoadYWH(numberYear, loadProject.listLoadYWH);
                else
                    loadYWHcur = loadYWH;
            }

            if (value.Contains("план"))
            {
                string sl = loadYWHcur.EstimatedYWH.ToString("0.0") + "/" + loadYWHcur.EstimatedYHumans.ToString("0.0");
                string[] lineLoadIssue = { "", value, sl };
                GetList(lineLoadIssue, ref listLine);
            }
            else
            {
                string sl = loadYWHcur.FactYWH.ToString("0.0") + "/" + loadYWHcur.FactYHumans.ToString("0.0");
                string[] lineLoadIssue = { "", value, sl };
                GetList(lineLoadIssue, ref listLine);
            }
           
            AddLineLoad(loadYWHcur, ref listLine);

            SetLineToListView(listLine, loadYWHcur, listOut);
            //------------------------------------------------//
        }

        private void AddLineNameGroup(int numberYear,
                                      ListView listOut,                                       
                                      LoadYWH loadYWH = null, 
                                      LoadUser loadUser = null, LoadGroup loadGroup = null, LoadProject loadProject = null)
        {
            List<string> listLine = new List<string>();
            bool isGroup = false;

            if (loadGroup != null)
            {
                if (loadUser.user.LastName.Equals(loadGroup.name))
                {
                    isGroup = true;
                    string[] lineGroup = { ""/*numGroup.ToString()*/, loadUser.user.LastName, loadGroup.CountUser.ToString() };
                    ListViewItem lvi_group = new ListViewItem(lineGroup);
                    lvi_group.Font = new Font(lvi_group.Font, FontStyle.Bold);
                    lvi_group.Tag = loadUser;
                    listOut.Items.Add(lvi_group);

                    MakeLineToListView(numberYear, "план", ref listLine, listOut, loadUser: loadUser);
                    MakeLineToListView(numberYear, "факт", ref listLine, listOut, loadUser: loadUser);                    
                }                
            }

            if ((loadUser != null) & (!isGroup) & (loadGroup == null))
            {
                string groupName = "";

                if (loadGroup == null)
                {
                    foreach (UserGroupRedmine group in loadUser.listGroup)
                    {
                        if (groupName == "")
                            groupName += group.name;
                        else
                            groupName += ";" + group.name;
                    }
                }

                string[] lineName = { ""/*numUser.ToString()*/, loadUser.user.LastName + " " + loadUser.user.FirstName, groupName };
                ListViewItem lvi_name = new ListViewItem(lineName);
                lvi_name.Font = new Font(lvi_name.Font, FontStyle.Bold);
                lvi_name.Tag = loadUser;
                listOut.Items.Add(lvi_name);

                //вывод на следующей строке данных по факту и плану
                MakeLineToListView(numberYear, "план", ref listLine, listOut, loadUser: loadUser);
                MakeLineToListView(numberYear, "факт", ref listLine, listOut, loadUser: loadUser);
            }

            if (loadProject != null)
            {
                string[] lineName = { ""/*numUser.ToString()*/, loadProject.userProject.Name, "" };
                ListViewItem lvi_name = new ListViewItem(lineName);
                lvi_name.Font = new Font(lvi_name.Font, FontStyle.Bold);
                //lvi_name.Tag = ;
                listOut.Items.Add(lvi_name);

                MakeLineToListView(numberYear, "план", ref listLine, listOut, loadProject: loadProject);
                MakeLineToListView(numberYear, "факт", ref listLine, listOut, loadProject: loadProject);
            }

            if (loadYWH != null)
            {
                string[] lineName = { "", "Планирование план/факт на " + numberYear.ToString(), "" };
                ListViewItem lvi_name = new ListViewItem(lineName);
                lvi_name.Font = new Font(lvi_name.Font, FontStyle.Bold);                
                listOut.Items.Add(lvi_name);

                MakeLineToListView(numberYear, "план", ref listLine, listOut, loadYWH: loadYWH);
                MakeLineToListView(numberYear, "факт", ref listLine, listOut, loadYWH: loadYWH);
            }            
        }

        //private void ShowNameUser()
        //{
        //    listDescription.Items.Clear();
        //    int num = 1;

        //    foreach (LoadUser loadUser in manager.listLoadUser)
        //    {
        //        string nameUser = loadUser.user.LastName + " " + loadUser.user.FirstName;
        //        string groupName = "";

        //        foreach (UserGroupRedmine group in loadUser.listGroup)
        //        {
        //            if (groupName == "")
        //                groupName += group.name;
        //            else
        //                groupName += ";" + group.name;                    
        //        }

        //        string[] line = { num.ToString(), nameUser + "План", groupName};
        //        ListViewItem lvi = new ListViewItem(line);
        //        lvi.Tag = loadUser;
        //        listDescription.Items.Add(lvi);

        //        string[] line2 = { num.ToString(), nameUser + "Факт", groupName };
        //        ListViewItem lvi2 = new ListViewItem(line2);
        //        lvi2.Tag = loadUser;
        //        listDescription.Items.Add(lvi2);
                
        //        num++;
        //    }
        //}

        //private void ShowGroupName()
        //{
        //    listDescription.Items.Clear();
        //    int numGroup = 1;
        //    int numUser;

        //    foreach (LoadGroup loadGroup in manager.listLoadGroup)
        //    {                                
        //        foreach (LoadUser loadUser in loadGroup.listLoadUser)
        //        {
        //            numUser = 1;
        //            if (loadUser.user.LastName.Equals(loadGroup.name))
        //            {
        //                string[] lineGroup = { numGroup.ToString(), loadUser.user.LastName, loadGroup.CountUser.ToString() };
        //                ListViewItem lvi_group = new ListViewItem(lineGroup);
        //                lvi_group.Font = new Font(lvi_group.Font, FontStyle.Bold);
        //                lvi_group.Tag = loadUser;
        //                listDescription.Items.Add(lvi_group);

        //                string[] lineGroup2 = { numGroup.ToString(), loadUser.user.LastName, loadGroup.CountUser.ToString() };
        //                ListViewItem lvi_group2 = new ListViewItem(lineGroup2);
        //                lvi_group2.Font = new Font(lvi_group.Font, FontStyle.Bold);
        //                lvi_group2.Tag = loadUser;
        //                listDescription.Items.Add(lvi_group2);
        //                numGroup++;                        
        //            }
        //            else
        //            {
        //                string[] lineName = { numUser.ToString(), loadUser.user.LastName + " " + loadUser.user.FirstName, " " };
        //                ListViewItem lvi_name = new ListViewItem(lineName);
        //                lvi_name.Tag = loadUser;
        //                listDescription.Items.Add(lvi_name);

        //                string[] lineName2 = { numUser.ToString(), loadUser.user.LastName + " " + loadUser.user.FirstName, " " };
        //                ListViewItem lvi_name2 = new ListViewItem(lineName2);
        //                lvi_name.Tag = loadUser;
        //                listDescription.Items.Add(lvi_name2);

        //                numUser++;
        //            }
        //        }
        //    }
        //}               

        //private void AddLine(LoadYWH loadYWH)
        //{
        //    if (loadYWH != null)
        //    {                
        //        List<string> list = new List<string>();
        //        for (int i = 0; i <= 11; i++)
        //        {
        //            list.Add(loadYWH.listLoadMWH[i].EstimatesMonthHours.ToString("0.0"));
                    
        //        }

        //        string[] the_array = list.Select(i => i.ToString()).ToArray();
        //        ListViewItem lvi = new ListViewItem(the_array);                
        //        listLoad.Items.Add(lvi);

        //        List<string> list_fact = new List<string>();
        //        for (int i = 0; i <= 11; i++)
        //        {
        //            list_fact.Add(loadYWH.listLoadMWH[i].FactMonthHours.ToString("0.0"));
        //        }

        //        string[] the_array_fact = list_fact.Select(i => i.ToString()).ToArray();
        //        ListViewItem lvi_fact = new ListViewItem(the_array_fact);
        //        listLoad.Items.Add(lvi_fact);

        //        //foreach (ColumnHeader column in listViewLoad.Columns)
        //        //{
        //        //    column.Width = -2;
        //        //}
        //    }
        //}

        private void ShowLoadMWH(string year)
        {            
            int numberYear = Convert.ToInt16(year);
            listLoadMWH.Items.Clear();

            LoadYWH loadYWH;

            switch (typeView)
            {
                case TypeView.LoadYWH:
                    loadYWH = manager.FindLoadYWH(numberYear, manager.listLoadYWH);
                    //if (loadYWH != null)
                    //    AddLine(loadYWH);
                    AddLineNameGroup(numberYear, listLoadMWH, loadYWH, null, null, null);
                    break;
                case TypeView.LoadUser:
                    //ShowNameUser();

                    foreach (LoadUser loadUser in manager.listLoadUser)
                    {
                        //loadYWH = manager.FindLoadYWH(numberYear, loadUser.listLoadYWH);
                        //if (loadYWH != null)
                        //    AddLine(loadYWH);
                        AddLineNameGroup(numberYear, listLoadMWH, null, loadUser, null, null);
                    }
                    break;
                case TypeView.LoadGroup:
                    //ShowGroupName();
                    foreach (LoadGroup loadGroup in manager.listLoadGroup)
                    {
                        foreach (LoadUser loadUser in loadGroup.listLoadUser)
                        {
                            //loadYWH = manager.FindLoadYWH(numberYear, loadUser.listLoadYWH);
                            //if (loadYWH != null)
                            //    AddLine(loadYWH);
                            AddLineNameGroup(numberYear, listLoadMWH, null, loadUser, loadGroup, null);
                        }
                    }
                    break;
                case TypeView.LoadProject:
                    foreach (LoadProject loadProject in manager.listLoadProject)
                    {
                        AddLineNameGroup(numberYear, listLoadMWH, null, null, null, loadProject);
                    }
                    break;
            }
            AutoFitColumn(listLoadMWH);
        }

        private void GetYearForReport()
        {
            comboYear.Items.Clear();
            foreach (int year in manager.listYear)
            {
                comboYear.Items.Add(year.ToString());
            }

            if (comboYear.Items.Count > 0)
                comboYear.SelectedIndex = 0;
        }

        private void MakeCaptionColums()
        {
            listLoadMWH.Columns.Clear();
            listLoadMWH.View = View.Details;

            listLoadMWH.Columns.Add("№", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("ФИО специалиста", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("Кол-во", -2, HorizontalAlignment.Left);

            listLoadMWH.Columns.Add("01", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("02", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("03", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("04", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("05", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("06", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("07", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("08", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("09", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("10", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("11", -2, HorizontalAlignment.Left);
            listLoadMWH.Columns.Add("12", -2, HorizontalAlignment.Left);

            listLoadMWH.GridLines = true;
        }        
        

        private void comboYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            string year = comboYear.SelectedItem.ToString();
            ShowLoadMWH(year);
        }

        private void ShowIssueDayForm(LoadMWH loadMWH, int year, int month, TypeView typeView)
        {
            string text = "";

            if (typeView == TypeView.LoadIssueDWH)
                text = "Запланировано на ";
            if (typeView == TypeView.LoadTimeDWH)
                text = "Факт на ";

            issueMonthForm = new IssueDayForm(loadMWH, manager, typeView);
            manager.Update += issueMonthForm.UpdateIssueInfo;
            DateTime dateText = new DateTime(year, month, 1);
            issueMonthForm.Text = text + dateText.ToString("MMMM yyyy");
            issueMonthForm.Show();
        }

        //private void ShowUser_GroupDWH(int year, int month)
        //{
        //    LoadYWH loadYWH;
        //    if (listDescription.Items.Count > 0)
        //    {
        //        for (int i = 0; i < listDescription.SelectedIndices.Count; i++)
        //        {
        //            //MessageBox.Show(((LoadUser)listItems.Items[listItems.SelectedIndices[i]].Tag).user.LastName);
        //            LoadUser loadUser = (LoadUser)listDescription.Items[listDescription.SelectedIndices[i]].Tag;
        //            if (loadUser != null)
        //            {
        //                loadYWH = manager.FindLoadYWH(year, loadUser.listLoadYWH);
        //                if (loadYWH != null)
        //                {
        //                    LoadMWH loadMWH = loadYWH.FindLoadMWH(month);
        //                    if (loadMWH != null)
        //                    {
        //                        ShowIssueDayForm(loadMWH, year, month);
        //                    }
        //                    else
        //                        MessageBox.Show(month.ToString() + " месяца не существует!");
        //                }
        //            }
        //        }
        //    }
        //}

        //private void ShowYearDWH(int year, int month)
        //{
        //    LoadYWH loadYWH = manager.FindLoadYWH(year, manager.listLoadYWH);
        //    if (loadYWH != null)
        //    {
        //        LoadMWH loadMWH = loadYWH.FindLoadMWH(month);
        //        if (loadMWH != null)
        //        {
        //            ShowIssueDayForm(loadMWH, year, month);
        //        }
        //        else
        //            MessageBox.Show(month.ToString() + " месяца не существует!");
        //    }
        //}

        private void listLoad_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (comboYear.Items.Count != 0)
            {
                int colIndex = Convert.ToInt16(e.Column.ToString());
                int month = ++colIndex - 3;
                //MessageBox.Show(colIndex.ToString());
                int year = Convert.ToInt16(comboYear.SelectedItem.ToString());

                if (listLoadMWH.Items.Count > 0)
                {
                    if (listLoadMWH.SelectedIndices.Count > 0)
                    {
                        for (int i = 0; i < listLoadMWH.SelectedIndices.Count; i++)
                        {
                            ListViewItem lvi = listLoadMWH.Items[listLoadMWH.SelectedIndices[i]];
                            if (lvi.Tag is LoadYWH)
                            {
                                LoadYWH loadYWH = (LoadYWH)lvi.Tag;
                                LoadMWH loadMWH = loadYWH.FindLoadMWH(month);
                                if (loadMWH != null)
                                {
                                    //loadMWH.GetListLoadProject();

                                    if (listLoadMWH.Items[listLoadMWH.SelectedIndices[i]].SubItems[1].Text.Contains("план"))
                                    {
                                        //ShowIssueDayForm(loadMWH, year, month, TypeView.LoadShortIssueDWH);
                                        //MessageBox.Show("plan");
                                        ShowIssueDayForm(loadMWH, year, month, TypeView.LoadIssueDWH);
                                    }

                                    if (listLoadMWH.Items[listLoadMWH.SelectedIndices[i]].SubItems[1].Text.Contains("факт"))
                                    {
                                        //ShowIssueDayForm(loadMWH, year, month, TypeView.LoadShortTimeDWH);
                                        //MessageBox.Show("plan");
                                        ShowIssueDayForm(loadMWH, year, month, TypeView.LoadTimeDWH);
                                    }
                                }
                                //MessageBox.Show("i = " + i.ToString() + "lvi.index = " + lvi.Index.ToString());
                            }

                        }
                    }

                }
            }            
        }

        private void ListViewSelectIndices(ListView listViewSet, ListView listViewSource, ListView.SelectedIndexCollection listIndices)
        {
            if (listViewSet.Items.Count > 0)
            {
                for (int i = 0; i < listViewSet.Items.Count; i++)
                {
                    listViewSet.Items[i].Selected = false;
                }

                if (listIndices.Count > 0)
                {
                    for (int i = 0; i < listIndices.Count; i++)
                    {
                        //MessageBox.Show(" i = " + listIndices[i].ToString());                        
                        listViewSet.Items[listIndices[i]].Selected = true;
                        listViewSet.Items[listIndices[i]].EnsureVisible();
                        listViewSource.Items[listIndices[i]].EnsureVisible();
                        listViewSet.Items[listIndices[i]].Focused = true;
                        //listView.Select();
                    }
                }
            }
        }

        private void LoadMWHForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            manager.Update -= this.UpdateForm;
        }
    }
}
