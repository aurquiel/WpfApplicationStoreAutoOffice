using LogLibraryClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using excel = Microsoft.Office.Interop.Excel;

namespace Excel
{
    public static class ManageExcel
    {
        public static async Task<bool> Create(DataTable dataTable, string path)
        {
            return await Task.Run(() => ShellCreate(dataTable, path));
        }

        private static bool ShellCreate(DataTable dataTable, string path)
        {
            DataTable auxDataTable = dataTable.Copy();
            excel.Application XlObj = new excel.Application();
            XlObj.Visible = false;
            excel._Workbook WbObj = (excel.Workbook)(XlObj.Workbooks.Add(""));
            excel._Worksheet WsObj = (excel.Worksheet)WbObj.ActiveSheet;
            object misValue = System.Reflection.Missing.Value;

            try
            {
                int row = 1; int col = 1;
                foreach (DataColumn column in auxDataTable.Columns)
                {
                    //adding columns
                    WsObj.Cells[row, col] = column.ColumnName;
                    col++;
                }

                //reset column and row variables
                col = 1;
                row++;
                for (int i = 0; i < auxDataTable.Rows.Count; i++)
                {
                    //adding data
                    foreach (var cell in auxDataTable.Rows[i].ItemArray)
                    {
                        WsObj.Cells[row, col] = cell;
                        col++;
                    }
                    col = 1;
                    row++;
                }

                WbObj.SaveAs(path, excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                WbObj.Close(true, misValue, misValue);

                return true;
            }
            catch(Exception ex)
            {
                WbObj.Close(true, misValue, misValue);
                Logger.WriteToLog("Metodo: " + ex.TargetSite + ", Error: " + ex.Message.ToLower());
                return false;
            }
        }


    }
}
