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
    public partial class ReportForm : Form
    {
        Manager manager;
        string title;
        public ReportForm(Manager manager, string title, TypeView typeView)
        {
            InitializeComponent();
            this.manager = manager;
            this.title = title;

            switch (typeView)
            {
                case TypeView.ReportProject:
                    ShowReportProject();
                    break;
                case TypeView.ReportIssue:
                    ShowReportIssue();
                    break;
                case TypeView.ReportEmail:
                    ShowReportEmailMessage();
                    break;
            }            
        }

        private void ShowReportEmailMessage()
        {
            foreach (EmailMessage emailMessage in manager.listEmailMessage)
            {
                if (emailMessage.Title.Equals(title))
                {
                    foreach (string email in emailMessage.ListToEmail)
                    {
                        richReportText.Text += "Сообщение пользователю с адресом: " + email;
                    }

                    richReportText.Text += "\n";

                    foreach (string email in emailMessage.ListCCEmail)
                    {
                        richReportText.Text += "Копия сообщения пользователю с адресом: " + email;
                    }

                    richReportText.Text += "\n";
                    richReportText.Text += "Тема сообщения " + emailMessage.Title;
                    richReportText.Text += "\n";
                    richReportText.Text += emailMessage.Message;
                    richReportText.Text += "\n";
                }
            }
        }

        private void ShowReportIssue()
        {
            foreach (ProjectInfo projectInfo in manager.listProjectInfo)
            {
                richReportText.Text += projectInfo.ProjectName;
                richReportText.Text += "\n";
                richReportText.Text += projectInfo.Info;
                richReportText.Text += "\n";
            }
        }

        private void ShowReportProject()
        {
            foreach (ProjectInfo projectInfo in manager.listProjectInfo)
            {
                richReportText.SelectionFont = new Font(richReportText.Font, FontStyle.Bold);
                richReportText.AppendText(projectInfo.ProjectName + "\n");
                richReportText.SelectionFont = new Font(richReportText.Font, FontStyle.Regular);

                foreach (LoadInfoYWH loadInfoYWH in projectInfo.listLoadInfoYWH)
                {
                    richReportText.SelectionFont = new Font(richReportText.Font, FontStyle.Regular);
                    richReportText.SelectionColor = Color.Black;
                    richReportText.AppendText("Год: " + loadInfoYWH.NumberYear + "\n");
                    richReportText.AppendText("Запланировано, ч: " + loadInfoYWH.PlanTotalHours.ToString("0.0") +"\n");
                    richReportText.AppendText("Потрачено, ч: " + loadInfoYWH.FactTotalHours.ToString("0.0") + "\n");
                    
                    if (loadInfoYWH.LeftHours < 0)
                    {
                        richReportText.SelectionFont = new Font(richReportText.Font, FontStyle.Regular);
                        richReportText.SelectionColor = Color.Red;
                        richReportText.AppendText("Остаток, ч: " + loadInfoYWH.LeftHours.ToString("0.0") + "\n");
                        
                    } else
                    {
                        richReportText.SelectionFont = new Font(richReportText.Font, FontStyle.Regular);
                        richReportText.SelectionColor = Color.Green;
                        richReportText.AppendText("Остаток, ч: " + loadInfoYWH.LeftHours.ToString("0.0") + "\n");
                    }

                }

                richReportText.AppendText("\n");
            }
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }        
    }
}
