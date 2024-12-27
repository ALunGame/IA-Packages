using IAToolkit;
using IAConfig.Excel.GenCode.Property;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace IAConfig.Excel.GenCode
{
    internal class ClassFieldInfo
    {
        public string name;
        public string typeName;
        public string defaultValue;
        public string comment;
    }
    internal class ClassInfo
    {
        public string nameSpace;
        public string className;
        public string classComment;
        public string exName;
        
        public List<ClassFieldInfo> fields = new List<ClassFieldInfo>();
    }
    
    internal class CustomClassExcelGenCode
    {
        private const string namespaceStr = @"
namespace #KEY#
{
    #STR#
}
";

        private const string classCodeStr = @"
    /// <summary>
    /// #ClassComment#
    /// </summary>
    [MemoryPackable]
    public partial class #KEY#
    {
        #STR#
    }
";
        
        private const string propCodeStr = @"
        /// <summary>
        /// #Comment#
        /// </summary>
        public #TYPE# #PRONAME#;
";
        private const string constructFunc1 = @"
        [MemoryPackConstructor]
        public #KEY#(){}
";
        private const string constructFunc2 = @"
        public #KEY#(#PARAM#)
        {
            #STR#
        }
";
        
        private const string constructFunc2Param = "#TYPE# #PRONAME#";
        
        private const string constructFunc2SetParam = @"
            this.#PRONAME#=#PRONAME#;
";
        
        
        private string excelPath;
        private string usingStr = "using MemoryPack;\n";
        private string allClassStr = "";
        public List<string> usingList = new List<string>();

        public CustomClassExcelGenCode(string pExcelPath)
        {
            excelPath = pExcelPath;
        }

        public List<ClassInfo> GenAllClass()
        {
            List<ClassInfo> resClassInfos = new List<ClassInfo>();
            
            ExcelPackage tPackage = null;
            List<ExcelWorksheet> sheets = ExcelReader.ReadAllSheets(excelPath,out tPackage);
            
            for (int i = 0; i < sheets.Count; i++)
            {
                allClassStr = "";
                ExcelWorksheet tSheet = sheets[i];
                List<ClassInfo> classInfos = GetAllClass(tSheet);
                resClassInfos.AddRange(classInfos);
                
                foreach (ClassInfo classInfo in classInfos)
                {
                    GenClassCode(classInfo);
                }
                
                //创建代码
                IOHelper.WriteText(usingStr+allClassStr,ExcelReadSetting.Setting.GenCodeRootPath + $"/TbCustomClass_{i}.cs");
            }
            
            tPackage.Dispose();
            
            return resClassInfos;
        }

        private void GenClassCode(ClassInfo pClassInfo)
        {
            string propStr = "";
            string paramStr = "";
            string setParamStr = "";
            for (int i = 0; i < pClassInfo.fields.Count; i++)
            {
                ClassFieldInfo fieldInfo = pClassInfo.fields[i];
                BaseProperty prop = ExcelPropertyMap.GetPropertyByTypeName(fieldInfo.typeName);
                if (prop == null)
                {
                    Debug.LogError($"导出自定义类失败,不支持该字段类型{fieldInfo.name}-->{fieldInfo.typeName}");
                    return;
                }
                string tStr = Regex.Replace(propCodeStr, "#Comment#", fieldInfo.comment);
                tStr = Regex.Replace(tStr, "#PRONAME#", fieldInfo.name);
                tStr = Regex.Replace(tStr, "#TYPE#", fieldInfo.typeName);
                propStr += tStr;
                
                string tParmStr = Regex.Replace(constructFunc2Param, "#TYPE#", fieldInfo.typeName);
                tParmStr = Regex.Replace(tParmStr, "#PRONAME#", fieldInfo.name);
                paramStr =  paramStr + (i == pClassInfo.fields.Count -1 ? tParmStr : tParmStr+",");;
                
                string tSetParmStr = Regex.Replace(constructFunc2SetParam, "#PRONAME#", fieldInfo.name);
                setParamStr = setParamStr + tSetParmStr;
                
                if (!usingList.Contains(prop.NameSpace))
                {
                    usingStr += prop.NameSpace + "\n";
                    usingList.Add(prop.NameSpace);
                }
            }

            string func1Str = constructFunc1;
            func1Str = Regex.Replace(func1Str, "#KEY#", pClassInfo.className);
            
            string func2Str = constructFunc2;
            func2Str = Regex.Replace(func2Str, "#KEY#", pClassInfo.className);
            func2Str = Regex.Replace(func2Str, "#PARAM#", paramStr);
            func2Str = Regex.Replace(func2Str, "#STR#", setParamStr);

            propStr += func1Str;
            propStr += func2Str;

            string codeStr = Regex.Replace(classCodeStr, "#ClassComment#", pClassInfo.classComment);
            codeStr = Regex.Replace(codeStr, "#KEY#", pClassInfo.className);
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

        public List<ClassInfo> GetAllClass(ExcelWorksheet sheet)
        {
            List<ClassInfo> classInfos = new List<ClassInfo>();
            
            // ExcelPackage tPackage = null;
            // ExcelWorksheet sheet = ExcelReader.ReadAllSheets(excelPath,out tPackage)[0];
            
            //最大行
            int _MaxRowNum = sheet.Dimension.End.Row;
            //最小行
            int _MinRowNum = sheet.Dimension.Start.Row;
            
            //最大列
            int _MaxColumnNum = sheet.Dimension.End.Column;
            //最小列
            int _MinColumnNum = sheet.Dimension.Start.Column;

            ClassInfo currInfo = null;
            for (int row = 2; row <= _MaxRowNum; row++)
            {
                string firsetValue = ExcelReader.GetCellValue(sheet, row, 1);
                if (firsetValue.Contains("##"))
                    continue;
               
                string className = ExcelReader.GetCellValue(sheet, row, 3);
                if (string.IsNullOrEmpty(className))
                {
                    //添加字段
                    if (currInfo != null)
                    {
                        string fieldName = ExcelReader.GetCellValue(sheet, row, 6);
                        if (!string.IsNullOrEmpty(fieldName))
                        {
                            ClassFieldInfo fieldInfo = new ClassFieldInfo();
                            fieldInfo.name = fieldName;
                            fieldInfo.typeName = ExcelReader.GetCellValue(sheet, row, 7);
                            fieldInfo.defaultValue = ExcelReader.GetCellValue(sheet, row, 8);
                            fieldInfo.comment = ExcelReader.GetCellValue(sheet, row, 9);
                            currInfo.fields.Add(fieldInfo);
                        }
                    }

                    if (row == _MaxRowNum && currInfo!=null)
                    {
                        classInfos.Add(currInfo);
                        currInfo = null;
                    }
                    continue;
                }
                else
                {
                    if (currInfo!=null)
                    {
                        classInfos.Add(currInfo);
                        currInfo = null;
                    }
                }
                    
                //新的
                currInfo = new ClassInfo();
                currInfo.nameSpace = ExcelReader.GetCellValue(sheet, row, 2);
                currInfo.className = ExcelReader.GetCellValue(sheet, row, 3);
                currInfo.classComment = ExcelReader.GetCellValue(sheet, row, 4);
                currInfo.exName = ExcelReader.GetCellValue(sheet, row, 5);
                    
                string currFieldName = ExcelReader.GetCellValue(sheet, row, 6);
                if (!string.IsNullOrEmpty(currFieldName))
                {
                    ClassFieldInfo fieldInfo = new ClassFieldInfo();
                    fieldInfo.name = currFieldName;
                    fieldInfo.typeName = ExcelReader.GetCellValue(sheet, row, 7);
                    fieldInfo.defaultValue = ExcelReader.GetCellValue(sheet, row, 8);
                    fieldInfo.comment = ExcelReader.GetCellValue(sheet, row, 9);
                    currInfo.fields.Add(fieldInfo);
                }
            }
            
            //tPackage.Dispose();

            return classInfos;
        }

        public List<ClassInfo> GetAllClass()
        {
            List<ClassInfo> resClassInfos = new List<ClassInfo>();
            
            ExcelPackage tPackage = null;
            List<ExcelWorksheet> sheets = ExcelReader.ReadAllSheets(excelPath,out tPackage);
            
            for (int i = 0; i < sheets.Count; i++)
            {
                ExcelWorksheet tSheet = sheets[i];
                List<ClassInfo> classInfos = GetAllClass(tSheet);
                resClassInfos.AddRange(classInfos);
            }
            
            tPackage.Dispose();
            return resClassInfos;
        }
        
    }
}