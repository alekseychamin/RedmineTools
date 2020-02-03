using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinRedminePlaning
{
    public partial class MainForm : Form
    {
        Manager manager;
        Dictionary<int, string> mounth = new Dictionary<int, string>();
        Dictionary<string, string> bossName = new Dictionary<string, string>();
        PrintForm printForm;

        string[] activityNotWorkHours = new string[3] { "Отпуск", "Больничный", "Отгул" };

        public MainForm()
        {
            InitializeComponent();
            int curYear = DateTime.Now.Year;
            textYear.Text = curYear.ToString();
            TextProgressBar progressBar = new TextProgressBar();
            progressBar.Dock = DockStyle.Fill;
            panelBar.Controls.Add(progressBar);
            manager = Manager.GetInstance(progressBar, textYear);

            ToolStripMenuItem emailSendMenuItem = new ToolStripMenuItem("Отправить сообщение специалистам");
            contextMenuStrip1.Items.Add(emailSendMenuItem);
            emailSendMenuItem.Click += emailSend_Click;

            mounth.Add(1, "январь");
            mounth.Add(2, "февраль");
            mounth.Add(3, "март");
            mounth.Add(4, "апрель");
            mounth.Add(5, "май");
            mounth.Add(6, "июнь");
            mounth.Add(7, "июль");
            mounth.Add(8, "август");
            mounth.Add(9, "сентябрь");
            mounth.Add(10, "октябрь");
            mounth.Add(11, "ноябрь");
            mounth.Add(12, "декабрь");

            bossName.Add("Испытатель", "Гульнев А.В.");
            bossName.Add("Конструктор", "Чамин А.Н.");
            bossName.Add("Программист ПЛК", "Чамин А.Н.");
            bossName.Add("Программист SCADA", "Чамин А.Н.");
            bossName.Add("Схемотехник", "Чамин А.Н.");
            bossName.Add("ТРП", "Чамин А.Н.");
            bossName.Add("Менеджер", "Першин П.И.");
            bossName.Add("Руководитель", "Першин П.И.");
            bossName.Add("Руководитель; ТРП", "Першин П.И.");
            bossName.Add("Испытатель; ТРП", "Гульнев А.В.");

            comboMounth.DataSource = new BindingSource(mounth, null);
            comboMounth.DisplayMember = "Value";
            comboMounth.ValueMember = "Key";
            comboMounth.SelectedIndex = DateTime.Now.Month - 1;

            listViewUser.Columns.Add("№", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("ФИО", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("Группа", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("Итого мес. ч.", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("Итого раб. ч.", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("Офис раб. ч.", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("Офис сверх. ур. ч.", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("ПНР ч.", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("ПНР сверх ур. ч.", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("Отпуск ч.", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("Бол-ных. ч.", -2, HorizontalAlignment.Left);
            listViewUser.Columns.Add("Отгул ч.", -2, HorizontalAlignment.Left);

            //listViewUser.Columns.Add("Статус", -2, HorizontalAlignment.Left);
            //listViewUser.Columns.Add("Открытый", -2, HorizontalAlignment.Left);            
            //listViewUser.Columns.Add("Дата создания", -2, HorizontalAlignment.Left);
            //listViewUser.Columns.Add("Дата обновления", -2, HorizontalAlignment.Left);

            listViewTimeEntry.Columns.Add("№", -2, HorizontalAlignment.Left);
            listViewTimeEntry.Columns.Add("Проект", -2, HorizontalAlignment.Left);
            listViewTimeEntry.Columns.Add("Активность", -2, HorizontalAlignment.Left);
            listViewTimeEntry.Columns.Add("Задача", -2, HorizontalAlignment.Left);
            listViewTimeEntry.Columns.Add("Старт", -2, HorizontalAlignment.Left);
            listViewTimeEntry.Columns.Add("Финиш", -2, HorizontalAlignment.Left);
            listViewTimeEntry.Columns.Add("Кол-во ч.", -2, HorizontalAlignment.Left);
            listViewTimeEntry.Columns.Add("Ком-рий", -2, HorizontalAlignment.Left);
        }

        private void emailSend_Click(object sender, EventArgs e)
        {
            if (listViewTimeEntry.Items.Count > 0)
            {
                if (listViewTimeEntry.SelectedIndices.Count > 0)
                {
                    for (int i = 0; i < listViewTimeEntry.SelectedIndices.Count; i++)
                    {

                    }
                }
            }
        }        

        private void ShowUserTimeEntry(int Id)
        {
            if (Id > 0)
            {
                listViewTimeEntry.Items.Clear();
                UserRedmine userRedmine = manager.listUserRedmine.Find(x => x.Value.Id == Id);
                if (userRedmine != null)
                {
                    int num = 1;
                    foreach (UserTimeEntry userTimeEntry in userRedmine.listMounthUserTimeEntry)
                    {
                        string[] line = { num.ToString(),
                                          userTimeEntry.ProjectName,
                                          userTimeEntry.ActivityName,
                                          userTimeEntry.IssueName,
                                          userTimeEntry.DateStart.ToShortDateString(),
                                          userTimeEntry.DateFinish.ToShortDateString(),
                                          userTimeEntry.Hours.ToString(),
                                          userTimeEntry.Comment };
                        ListViewItem lvi = new ListViewItem(line);
                        listViewTimeEntry.Items.Add(lvi);
                        num++;
                    }

                    foreach (ColumnHeader column in listViewTimeEntry.Columns)
                    {
                        column.Width = -2;
                    }
                }
                //RedmineProject redmProject = projects.ListProject.Find(x => x.Value.Id == Id);
                //if (redmProject != null)
                //{
                //    foreach (Issue issue in redmProject.ListIssue)
                //    {
                //        string[] line = { issue.Id.ToString(), issue.Subject };
                //        ListViewItem lvi = new ListViewItem(line);
                //        listViewTimeEntry.Items.Add(lvi);
                //    }
                //}
            }
        }

        private void ShowUserRedmineThread()
        {
            //ShowUserRedmine();
            listViewUser.InvokeIfNeeded(ShowUserRedmine);
        }

        private void ShowUserRedmine()
        {
            listViewUser.Items.Clear();
            int num = 1;
            int iLine = 0;
            foreach (UserRedmine user in manager.listUserRedmine)
            {
                string[] line = { num.ToString(),
                                  user.FullName,
                                  user.GroupName,
                                  user.TotalMonthHours.ToString(),
                                  user.TotalWorkHours.ToString(),
                                  user.TotalOfficeHours.ToString(),
                                  user.TotalOverOfficeHours.ToString(),
                                  user.TotalTripHours.ToString(),
                                  user.TotalOverTripHours.ToString(),
                                  user.TotalVacaitionHours.ToString(),
                                  user.TotalSeekHours.ToString(),
                                  user.TotalFreeHours.ToString()};
                ListViewItem lvi = new ListViewItem(line);
                listViewUser.Items.Add(lvi);
                listViewUser.Items[iLine].UseItemStyleForSubItems = false;
                listViewUser.Items[iLine].SubItems[3].BackColor = user.CellColor;
                num++;
                iLine++;
            }

            foreach (ColumnHeader column in listViewUser.Columns)
            {
                column.Width = -2;
            }
        }

        public void ShowUserName(string name)
        {
            labelUserName.InvokeIfNeeded(delegate { labelUserName.Text = name; });            
        }

        public void ShowMonthHours(int hours)
        {
            lab_MonthHours.InvokeIfNeeded(delegate { lab_MonthHours.Text = "Кол-во раб. часов: " + hours.ToString(); });            
        }
        private void GetDataFromRedmine(object mounth)
        {
            //var watch = System.Diagnostics.Stopwatch.StartNew();

            but_loadRedmine.InvokeIfNeeded(delegate { but_loadRedmine.Enabled = false; }) ;
            panelBar.InvokeIfNeeded(delegate { panelBar.Visible = true; });
            //int mounth = ((KeyValuePair<int, string>)comboMounth.SelectedItem).Key;
            
            manager.GetUserFromRedmine(bossName); //new string[2] { "Арбузов Владимир Леонидович", "Мазилкин Денис Александрович" });
            //manager.GetDataFromRedmine(); // проверка метода для получения данных из redmine с помощью LINQ
            //manager.GetMounthUserTimeEntry(DateTime.Now.Year, (int)mounth);
            manager.GetMounthUserTimeEntry(int.Parse(textYear.Text), (int)mounth);

            ShowUserRedmineThread();
            ShowMonthHours(manager.monthValueHours.Value);
            ShowUserName("Пользователь: -");

            panelBar.InvokeIfNeeded(delegate { panelBar.Visible = false; });
            but_loadRedmine.InvokeIfNeeded(delegate { but_loadRedmine.Enabled = true; });

            //watch.Stop();
            //MessageBox.Show("Данные загружены за " + (watch.ElapsedMilliseconds / 1000).ToString() + " сек");
        }
        private void but_loadRedmine_Click(object sender, EventArgs e)
        {
            

            int mounth = ((KeyValuePair<int, string>)comboMounth.SelectedItem).Key;
            ParameterizedThreadStart getDataFromRedmine = new ParameterizedThreadStart(GetDataFromRedmine);
            Thread threadGetData = new Thread(getDataFromRedmine);
            threadGetData.IsBackground = true;
            threadGetData.Start(mounth);

            //int mounth = ((KeyValuePair<int, string>)comboMounth.SelectedItem).Key;
            //but_loadRedmine.Enabled = false;
            //manager.GetUserFromRedmine(bossName); //new string[2] { "Арбузов Владимир Леонидович", "Мазилкин Денис Александрович" });
            //manager.GetMounthUserTimeEntry(DateTime.Now.Year, mounth);
            //lab_MonthHours.Text = "Кол-во раб. часов: " + manager.monthValueHours.Value.ToString();
            //ShowUserRedmine();
            //labelUserName.Text = "Пользователь: -";
            //ShowUserName("Пользователь: -");

            
            //but_loadRedmine.Enabled = true;
        }

        private void but_SaveExcel_Click(object sender, EventArgs e)
        {
            UserRedmine userRedmine = null;
            string filePrint = null;

            if (listViewUser.Items.Count != 0)
            {
                try
                {
                    printForm.Show();
                }
                catch (NullReferenceException)
                {
                    printForm = new PrintForm();
                }
                catch (ObjectDisposedException)
                {
                    printForm = new PrintForm();
                }

                finally
                {
                    printForm.Show();
                }


                while (listViewUser.SelectedItems.Count > 0)
                {
                    ListViewItem lvi = listViewUser.SelectedItems[0];
                    string fullName = lvi.SubItems[1].Text;
                    manager.FindUserRedmine(fullName, out userRedmine);

                    if (userRedmine != null)
                    {
                        if (userRedmine.TotalMonthHours > 0)
                        {
                            manager.excelMethods.MakeSheetWorkingHours(userRedmine, Application.StartupPath, out filePrint, activityNotWorkHours);
                            printForm.AddFilePrint(filePrint);
                        }
                    }

                    lvi.Selected = false;

                }
            }


        }

        private void listViewUser_MouseClick(object sender, MouseEventArgs e)
        {
            if (listViewUser.SelectedItems[0] != null)
            {
                ListViewItem lvi = listViewUser.SelectedItems[0];
                string fullName = lvi.SubItems[1].Text;
                UserRedmine userRedmine;
                ShowUserTimeEntry(manager.FindUserRedmine(fullName, out userRedmine));
                //labelUserName.Text = "Пользователь: " + userRedmine.FullName;
                ShowUserName("Пользователь: " + userRedmine.FullName);
            }
        }

        private void comboMounth_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int mounth = ((KeyValuePair<int, string>)comboMounth.SelectedItem).Key;

            //manager.GetMounthUserTimeEntry(DateTime.Now.Year, mounth);
            manager.GetMounthUserTimeEntry(int.Parse(textYear.Text), mounth);
            //lab_MonthHours.Text = "Кол-во раб. часов: " + manager.monthValueHours.Value.ToString();
            ShowMonthHours(manager.monthValueHours.Value);
            listViewTimeEntry.Items.Clear();
            ShowUserRedmine();

        }

        private void listViewUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListViewItem lvi = (sender as ListView).FocusedItem;
            if (lvi != null)
            {
                string fullName = lvi.SubItems[1].Text;
                UserRedmine userRedmine;
                ShowUserTimeEntry(manager.FindUserRedmine(fullName, out userRedmine));
                //labelUserName.Text = "Пользователь: " + userRedmine.FullName;
                ShowUserName("Пользователь: " + userRedmine.FullName);
            }
        }

        private void butReportExcel_Click(object sender, EventArgs e)
        {
            manager.excelMethods.MakeSheetReportUsers(manager.listUserRedmine, Application.StartupPath);
        }
    }
}
