using IAToolkit;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IAConfig.Excel.GenCode
{
    internal class EnumFieldInfo
    {
        public string name;
        public string comment;
        public string exName;
        public int value;
    }
    
    internal class EnumInfo
    {
        public string nameSpace;
        public string enumName;
        public string enumComment;
        public string exName;
        
        public List<EnumFieldInfo> fields = new List<EnumFieldInfo>();
    }
    
    internal class EnumExcelGenCode
    {
        private const string namespaceStr = @"
namespace #KEY#
{
    #STR#
}
";

        private const string classCodeStr = @"
    /// <summary>
    /// #EnumComment#
    /// </summary>
    public enum #KEY#
    {
        #STR#
    }
";
        
        private const string propCodeStr = @"
        /// <summary>
        /// #Comment#
        /// </summary>
        #PRONAME#,
";
        
        private string excelPath;
        private string allClassStr = "";

        public EnumExcelGenCode(string pExcelPath)
        {
            excelPath = pExcelPath;
        }

        public List<EnumInfo> GenAllClass()
        {
            List<EnumInfo> resEnumInfos = new List<EnumInfo>();
            ExcelPackage tPackage = null;
            List<ExcelWorksheet> sheets = ExcelReader.ReadAllSheets(excelPath,out tPackage);

            for (int i = 0; i < sheets.Count; i++)
            {
                allClassStr = "";
                ExcelWorksheet tSheet = sheets[i];
                List<EnumInfo> enumInfos = GetAllEnum(tSheet);
                resEnumInfos.AddRange(enumInfos);
                
                foreach (EnumInfo enumInfo in enumInfos)
                {
                    GenClassCode(enumInfo);
                }
                
                //创建代码
                IOHelper.WriteText(allClassStr,ExcelReadSetting.Setting.GenCodeRootPath + $"/TbCustomEnum_{i}.cs");
            }
            
            tPackage.Dispose();
            return resEnumInfos;
        }

        private void GenClassCode(EnumInfo pClassInfo)
        {
            string propStr = "";
            foreach (EnumFieldInfo fieldInfo in pClassInfo.fields)
            {
                string tStr = Regex.Replace(propCodeStr, "#Comment#", fieldInfo.comment);
                tStr = Regex.Replace(tStr, "#PRONAME#", fieldInfo.name);
                propStr += tStr;
            }

            string codeStr = Regex.Replace(classCodeStr, "#EnumComment#", pClassInfo.enumComment);
            codeStr = Regex.Replace(codeStr, "#KEY#", pClassInfo.enumName);
            codeStr = Regex.Replace(codeStr, "#STR#", propStr);

            if (string.IsNullOrEmpty(pClassInfo.nameSpace))
            {
                allClassStr += codeStr + "\n";
            }
            else
            {
                string spaceStr = Regex.Replace(namespaceStr, "#KEY#", pClassInfo.nameSpace);
                spaceStr = Regex.Replace(spaceStr, "#STR#", codeStr);
                allClassStr += spaceStr + "\n";
            }
        }

        public List<EnumInfo> GetAllEnum(ExcelWorksheet sheet)
        {
            List<EnumInfo> enumInfos = new List<EnumInfo>();

            //最大行
            int _MaxRowNum = sheet.Dimension.End.Row;
            //最小行
            int _MinRowNum = sheet.Dimension.Start.Row;
            
            //最大列
            int _MaxColumnNum = sheet.Dimension.End.Column;
            //最小列
            int _MinColumnNum = sheet.Dimension.Start.Column;

            EnumInfo enumInfo = null;
            for (int row = 2; row <= _MaxRowNum; row++)
            {
                string firsetValue = ExcelReader.GetCellValue(sheet, row, 1);
                if (firsetValue.Contains("##"))
                    continue;
               
                string className = ExcelReader.GetCellValue(sheet, row, 3);
                if (string.IsNullOrEmpty(className))
                {
                    //添加字段
                    if (enumInfo != null)
                    {
                        string fieldName = ExcelReader.GetCellValue(sheet, row, 6);
                        if (!string.IsNullOrEmpty(fieldName))
                        {
                            EnumFieldInfo fieldInfo = new EnumFieldInfo();
                            fieldInfo.name = fieldName;
                            fieldInfo.comment = ExcelReader.GetCellValue(sheet, row, 7);
                            fieldInfo.exName = ExcelReader.GetCellValue(sheet, row, 8);
                            enumInfo.fields.Add(fieldInfo);
                        }
                    }

                    if (row == _MaxRowNum && enumInfo!=null)
                    {
                        enumInfos.Add(enumInfo);
                        for (int i = 0; i < enumInfo.fields.Count; i++)
                        {
                            enumInfo.fields[i].value = i;
                        }
                        enumInfo = null;
                    }
                    continue;
                }
                else
                {
                    if (enumInfo != null)
                    {
                        enumInfos.Add(enumInfo);
                        for (int i = 0; i < enumInfo.fields.Count; i++)
                        {
                            enumInfo.fields[i].value = i;
                        }
                        enumInfo = null;
                    }
                }
                    
                //新的
                enumInfo = new EnumInfo();
                enumInfo.nameSpace = ExcelReader.GetCellValue(sheet, row, 2);
                enumInfo.enumName = ExcelReader.GetCellValue(sheet, row, 3);
                enumInfo.enumComment = ExcelReader.GetCellValue(sheet, row, 4);
                enumInfo.exName = ExcelReader.GetCellValue(sheet, row, 5);
                    
                string currfieldName = ExcelReader.GetCellValue(sheet, row, 6);
                if (!string.IsNullOrEmpty(currfieldName))
                {
                    EnumFieldInfo fieldInfo = new EnumFieldInfo();
                    fieldInfo.name = currfieldName;
                    fieldInfo.comment = ExcelReader.GetCellValue(sheet, row, 7);
                    fieldInfo.exName = ExcelReader.GetCellValue(sheet, row, 8);
                    enumInfo.fields.Add(fieldInfo);

                    if (row == _MaxRowNum)
                    {
                        enumInfos.Add(enumInfo);
                    }
                }
            }
            
            return enumInfos;
        }

        public List<EnumInfo> GetAllEnum()
        {
            List<EnumInfo> resEnumInfos = new List<EnumInfo>();
            ExcelPackage tPackage = null;
            List<ExcelWorksheet> sheets = ExcelReader.ReadAllSheets(excelPath,out tPackage);
            
            for (int i = 0; i < sheets.Count; i++)
            {
                allClassStr = "";
                ExcelWorksheet tSheet = sheets[i];
                List<EnumInfo> enumInfos = GetAllEnum(tSheet);
                resEnumInfos.AddRange(enumInfos);
            }
            
            tPackage.Dispose();
            return resEnumInfos;
        }
    }
}