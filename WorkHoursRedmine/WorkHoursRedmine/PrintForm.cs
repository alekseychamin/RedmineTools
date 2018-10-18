using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinRedminePlaning
{
    public partial class PrintForm : Form
    {
        int num;
        public PrintForm()
        {
            InitializeComponent();
            num = 1;
            listFiles.View = View.Details;
            listFiles.Columns.Add("№", -2, HorizontalAlignment.Left);
            listFiles.Columns.Add("Имя файла", -2, HorizontalAlignment.Left);
        }

        public void AddFilePrint(string filePrint)
        {
            if (filePrint != null)
            {
                string[] line = { num.ToString(), filePrint };
                ListViewItem lvi = new ListViewItem(line);
                listFiles.Items.Add(lvi);
                num++;

                foreach (ColumnHeader column in listFiles.Columns)
                {
                    column.Width = -2;
                }
            }
        }

        private void PrintDocxFile(string filePath)
        {            
            var info = new ProcessStartInfo(filePath);
            info.Verb = "Print";
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(info);        
    }

        private void butPrint_Click(object sender, EventArgs e)
        {
            string filePath;

            foreach (ListViewItem lvi in listFiles.Items)
            {
                filePath = lvi.SubItems[1].Text;
                PrintDocxFile(filePath);
            }
        }
    }
}
