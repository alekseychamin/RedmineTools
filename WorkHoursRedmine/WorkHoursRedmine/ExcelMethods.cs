using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinRedminePlaning
{

    class FieldProp
    {
        public Object value;
        public String name;

        public FieldProp(string name)
        {
            this.name = name;
        }
    }

    class ExcelIssue
    {
        public FieldProp id = new FieldProp("Id задачи");

    }

    class ExcelMethods
    {
        private Application applicationExcel;
        private Workbook workBook;
        private Worksheet workSheet;

        private Dictionary<string, int> colName;
        private List<string> listKey;

        private int iRowHeader = 8;
        private int iRowData = 10;

        private List<ExcelIssue> listExcelIssue;

        public ExcelMethods()
        {
            applicationExcel = new Application();

            listKey = new List<string>();
            listKey.Add("Проект");

            colName = new Dictionary<string, int>();            
            
            colName.Add("Проект", 0);
            colName.Add("Id задачи", 0);
            colName.Add("Дата начала", 0);
            colName.Add("Дата завершения", 0);
            colName.Add("Готовность", 0);
            colName.Add("Длительность", 0);
            colName.Add("Оценка временных затрат", 0);
            colName.Add("Назначена", 0);
            colName.Add("Статус", 0);
            colName.Add("Приоритет", 0);
            colName.Add("Трекер", 0);
            colName.Add("Версия", 0);
            colName.Add("Обновлено", 0);
        }
        
        private void FindExcelSheet(Sheets sheets, string name)
        {
            int index = -1;

            int i = 1;

            while (i <= sheets.Count)
            {
                string sName = sheets[i].Name;
                if (sName.Equals(name))
                    index = i;
                i++;
            }

            if (index != -1)
                workSheet = sheets[index];
            else
                workSheet = null;

        }
        private void OpenExcel(string fileName)
        {
            workBook = applicationExcel.Workbooks.Open(fileName, 0, false, 5, "", "", false, XlPlatform.xlWindows, 
                                                        "", true, false, 0, true, false, false);                       
        }

        private void DeclareColumn()
        {
            if (workSheet != null)
            {
                for (int i = 1; i <= colName.Count; i++)
                {
                    string key = workSheet.Cells[iRowHeader, i].Text.ToString();
                    int col = 0;
                    if (colName.TryGetValue(key, out col))
                    {
                        colName[key] = i;
                        Console.WriteLine("Name column = {0}, index column = {1} ", key, i.ToString());
                    }
                }
            }
        }

        public void GetExcelIssue()
        {

        }

        public void UpdateExcelProject(RedmineProjects redmineProjects, string fileName)
        {
            OpenExcel(fileName);
            FindExcelSheet(workBook.Sheets, "График");
            DeclareColumn();

            if (workSheet != null)
            {
                
            }
            applicationExcel.Quit();
        }
    }
}
