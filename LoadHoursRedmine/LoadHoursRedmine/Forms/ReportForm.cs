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
        public ReportForm(Manager manager, string title)
        {
            InitializeComponent();
            this.manager = manager;
            this.title = title;
            ShowEmailMessage();
        }

        private void ShowEmailMessage()
        {
            foreach (EmailMessage emailMessage in manager.listEmailMessage)
            {
                if (emailMessage.Title.Equals(title))
                {
                    foreach (string email in emailMessage.ListToEmail)
                    {
                        richTextBox1.Text += "Сообщение пользователю с адресом: " + email;
                    }

                    richTextBox1.Text += "\n";

                    foreach (string email in emailMessage.ListCCEmail)
                    {
                        richTextBox1.Text += "Копия сообщения пользователю с адресом: " + email;
                    }

                    richTextBox1.Text += "\n";
                    richTextBox1.Text += "Тема сообщения " + emailMessage.Title;
                    richTextBox1.Text += "\n";
                    richTextBox1.Text += emailMessage.Message;
                    richTextBox1.Text += "\n";
                }
            }
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }        
    }
}
