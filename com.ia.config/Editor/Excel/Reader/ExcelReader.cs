using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace IAConfig.Excel
{
    public class ExcelReader
    {
        public static string ExcelRootPath = ExcelReadSetting.Setting.ConfigRootPath;

        public static List<ExcelWorksheet> ReadAllSheets(string pFilePath, out ExcelPackage pPackage, string pSheetName = "")
        {
            string tPath = ExcelRootPath + "/" + pFilePath;
            tPath = Path.GetFullPath(tPath);
            List<ExcelWorksheet> sheets = new List<ExcelWorksheet>();
            FileInfo exFile = new FileInfo(tPath);
            pPackage = new ExcelPackage(exFile);
            foreach (ExcelWorksheet worksheet in pPackage.Workbook.Worksheets)
            {
                if (worksheet != null && worksheet.Dimension != null)
                {
                    if (string.IsNullOrEmpty(pSheetName))
                    {
                        sheets.Add(worksheet);
                    }
                    else
                    {
                        if (worksheet.Name == pSheetName)
                        {
                            sheets.Add(worksheet);
                        }
                    }
                }
            }
            return sheets;
        }

        /// <summary>
        /// 获得单元格值
        /// </summary>
        /// <param name="pSheet">工作簿</param>
        /// <param name="pRow">行数</param>
        /// <param name="pColumn">列数</param>
        /// <returns></returns>
        public static string GetCellValue(ExcelWorksheet pSheet, int pRow, int pColumn)
        {
            object value = pSheet.Cells[pRow, pColumn].Value;
            if (value == null)
            {
                return "";
            }

            return value.ToString();
        }
    }
}