using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinRedminePlaning
{

    public class FieldProp
    {
        public Object value;
        public String name;

        public FieldProp(string name)
        {
            this.name = name;
        }
    }

    public class ExcelIssue
    {
        public FieldProp id = new FieldProp("Id задачи");

    }

    public class ExcelMethods
    {
        private Application applicationExcel;
        private Workbook workBook;
        private Worksheet workSheet;
               
        private int iRowHeader = 8;
        private int iRowData = 2;

        private Dictionary<int, string> column = new Dictionary<int, string>();
        

        public ExcelMethods()
        {
            applicationExcel = new Application();
            column.Add(1, "A");
            column.Add(2, "B");
            column.Add(3, "C");
            column.Add(4, "D");
            column.Add(5, "E");
            column.Add(6, "F");
            column.Add(7, "G");
            column.Add(8, "H");
            column.Add(9, "I");
            column.Add(10, "J");
            column.Add(11, "K");
            column.Add(12, "L");
            column.Add(13, "M");
            column.Add(14, "N");
            column.Add(15, "O");
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
        private int OpenExcel(string fileName)
        {
            int res = 0;

            try
            {
                workBook = applicationExcel.Workbooks.Open(fileName);
                applicationExcel.Visible = true;
                //applicationExcel.ScreenUpdating = false;
                //, 0, false, 5, "", "", false, XlPlatform.xlWindows, 
                //                                        "", true, false, 0, true, false, false);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error - " + ex.Message);
                res = -1;
                return res;
            }
            return res;         
        }
                

        private bool isActivityWork(string activityName, params string[] activityNotWorkingHours)
        {
            bool res = true;

            foreach (string activity in activityNotWorkingHours)
            {
                if (activityName.Contains(activity))
                {
                    res = false;
                    break;
                }
            }

            return res;
        }

        /*public void MakeSheetWorkingHours(UserRedmine userRedmine, string startPath, out string filePrint, params string[] activityNotWorkingHours)
        {
            int iCurRow = 24;
            int num = 1;
            string filename = startPath + @"\Pattern.xls";
            filePrint = null;

            if (userRedmine.listMounthUserIssueEntry.Count != 0)
            {
                if (OpenExcel(filename) == -1)
                    return;

                FindExcelSheet(workBook.Sheets, "Лист1");

                if (workSheet != null)
                {
                    foreach (UserIssueEntry userTimeEntry in userRedmine.listMounthUserIssueEntry)
                    {
                        if (isActivityWork(userTimeEntry.ActivtyName, activityNotWorkingHours))
                        {
                            if (num > 1)
                            {

                                iCurRow++;
                                workSheet.Rows[iCurRow].Insert();
                            }

                            //A = column[1] + iCurRow.ToString();
                            workSheet.Cells[iCurRow, 1].Value2 = num;

                            //A = column[2] + iCurRow.ToString();
                            workSheet.Cells[iCurRow, 2].Value2 = userTimeEntry.IssueName;

                            workSheet.Cells[iCurRow, 3].Value2 = userTimeEntry.ProjectName;

                            //workSheet.Cells[iCurRow, 8].FormulaLocal = string.Format("=СУММ(A1:A4)");

                            workSheet.Cells[iCurRow, 8].Value2 = userTimeEntry.DateFinish.ToShortDateString();

                            workSheet.Cells[iCurRow, 9].Value2 = userTimeEntry.TotalEstimatedHours;

                            workSheet.Cells[iCurRow, 10].Value2 = userTimeEntry.TotalEstimatedHours;

                            workSheet.Cells[iCurRow, 11].Value2 = userTimeEntry.DateFinish.ToShortDateString();

                            workSheet.Cells[iCurRow, 13].Value2 = userTimeEntry.DateFinish.ToShortDateString();

                            workSheet.Cells[iCurRow, 14].Value2 = userTimeEntry.HeadName;

                            num++;

                        }
                        //A = column[1] + iCurRow.ToString();
                        //workSheet.Range[A].EntireRow.Insert(XlInsertShiftDirection.xlShiftDown, XlInsertFormatOrigin.xlFormatFromRightOrBelow);                    
                    }


                    workSheet.Cells[iCurRow + 1, 9].FormulaLocal = string.Format("=СУММ({0}:{1})",
                                                                                             column[9] + iRowData.ToString(), column[9] + iCurRow.ToString());
                    workSheet.Cells[iCurRow + 1, 10].FormulaLocal = string.Format("=СУММ({0}:{1})",
                                                                                 column[10] + iRowData.ToString(), column[10] + iCurRow.ToString());

                    DateTime dateFirstWork = userRedmine.listMounthUserIssueEntry[0].DateFirstWork;                    

                    workSheet.Cells[18, 1].Value2 = dateFirstWork;
                    workSheet.Cells[18, 7].Value2 = dateFirstWork;
                    workSheet.Cells[6, 4].Value2 = dateFirstWork;

                    workSheet.Cells[8, 1].Value2 = "ФИО  специалиста " + userRedmine.FullName;

                    workSheet.Cells[15, 7].Value2 = string.Format("Задание получил______________________/ {0}/", userRedmine.ShortName);

                    workSheet.Cells[15, 1].Value2 = string.Format("Задание выдал______________________/ {0}/", userRedmine.BossName);

                    string dir = startPath + @"\Трудозатраты";

                    Directory.CreateDirectory(dir);

                    filename = string.Format(dir + @"\" + "{0} трудозатраты за {1}.xls", 
                                            userRedmine.ShortName, dateFirstWork.ToString("Y"));
                    workBook.SaveAs(filename);
                    filePrint = filename;
                }
            }

            //applicationExcel.Quit();
            //applicationExcel.ScreenUpdating = true;
            //applicationExcel.Visible = true;
        }*/

        /*public void MakeSheetReportUsers(List<UserRedmine> listUserRedmine, string startPath)
        {
            int iCurRow = 2;
            int num = 1;
            string filename = startPath + @"\Report.xls";

            if (listUserRedmine.Count != 0)
            {
                if (OpenExcel(filename) == -1)
                    return;

                FindExcelSheet(workBook.Sheets, "Лист1");

                if (workSheet != null)
                {
                    foreach (UserRedmine user in listUserRedmine)
                    {
                        if (num > 1)
                        {

                            iCurRow++;
                            workSheet.Rows[iCurRow].Insert();
                        }

                        workSheet.Cells[iCurRow, 1].Value2 = num;
                        workSheet.Cells[iCurRow, 2].Value2 = user.FullName;
                        workSheet.Cells[iCurRow, 3].Value2 = user.GroupName;
                        workSheet.Cells[iCurRow, 4].Value2 = user.TotalMonthHours;
                        workSheet.Cells[iCurRow, 5].Value2 = user.TotalWorkHours;
                        workSheet.Cells[iCurRow, 6].Value2 = user.TotalOfficeHours;
                        workSheet.Cells[iCurRow, 7].Value2 = user.TotalOverOfficeHours;
                        workSheet.Cells[iCurRow, 8].Value2 = user.TotalTripHours;
                        workSheet.Cells[iCurRow, 9].Value2 = user.TotalOverTripHours;
                        workSheet.Cells[iCurRow, 10].Value2 = user.TotalVacaitionHours;
                        workSheet.Cells[iCurRow, 11].Value2 = user.TotalSeekHours;
                        workSheet.Cells[iCurRow, 12].Value2 = user.TotalFreeHours;

                        num++;
                    }

                    string dir = startPath + @"\Отчеты";

                    Directory.CreateDirectory(dir);

                    filename = string.Format(dir + @"\Отчет за {0}.xls", DateTime.Now.ToString("Y"));
                    workBook.SaveAs(filename);
                }
            }

            //applicationExcel.Quit();
        }*/
    }
}
