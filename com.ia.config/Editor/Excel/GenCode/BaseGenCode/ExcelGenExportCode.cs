using IAConfig.Excel.GenCode.Property;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace IAConfig.Excel.GenCode
{
    /// <summary>
    /// 创建导出代码
    /// </summary>
    internal class ExcelGenExportCode
    {
        private string className;
        public ExcelGenExportCode(string pClassName)
        {
            className = pClassName;
        }

        public string GenExportCode(List<ExcelWorksheet> pSheets,List<BaseProperty> pProps,Dictionary<string, List<int>> pOutPropColDict)
        {
            List<Dictionary<string, List<string>>> propValuelist = Export(pSheets, pProps, pOutPropColDict);
            string codeStr = @"
        private void Export_#CLASSNAME#(GenConfigInfo pGenInfo,List<BaseProperty> pProps,List<Dictionary<string, List<string>>> propValuelist)
        {
            List<#CLASSNAME#> cnfs = new List<#CLASSNAME#>();
            foreach (var propDict in propValuelist)
            {
                #CLASSNAME# cnf = new #CLASSNAME#();
#CODESTR#
                cnfs.Add(cnf);
            }
                
            pGenInfo.SaveJson(cnfs);    
        }
";
            string setPropStr = "";
            foreach (BaseProperty prop in pProps)
            {
                setPropStr += prop.CreateExportStr("cnf","values") + "\n";
            }

            string resStr = codeStr;
            resStr = Regex.Replace(resStr, "#CLASSNAME#", className);
            resStr = Regex.Replace(resStr, "#CODESTR#", setPropStr);

            return resStr;
        }

        public static List<Dictionary<string, List<string>>> Export(List<ExcelWorksheet> pSheets,List<BaseProperty> pProps,Dictionary<string, List<int>> pOutPropColDict)
        {
            //字段值
            List<Dictionary<string, List<string>>> propValuelist = new List<Dictionary<string, List<string>>>();
            foreach (ExcelWorksheet sheet in pSheets)
            {
                //最大行
                int _MaxRowNum = sheet.Dimension.End.Row;
                //最小行
                int _MinRowNum = sheet.Dimension.Start.Row;

                for (int i = 2; i <= _MaxRowNum; i++)
                {
                    string firstValue = ExcelReader.GetCellValue(sheet,i, 1).ToString();
                    //特殊标记
                    if (firstValue.Contains("##"))
                        continue;
                    //类字段
                    Dictionary<string, List<string>> propValueDict = new Dictionary<string, List<string>>();
                    foreach (BaseProperty prop in pProps)
                    {
                        List<string> propValues = new List<string>();
                        List<int> valueColList = pOutPropColDict[prop.name];
                        for (int j = 0; j < valueColList.Count; j++)
                        {
                            int col = valueColList[j];
                            propValues.Add(ExcelReader.GetCellValue(sheet,i,col));
                        }

                        if (propValueDict.ContainsKey(prop.name))
                        {
                            Debug.LogError($"生成导出代码失败，字段名重复{sheet.Name} -->{prop.name}");
                            return propValuelist;
                        }
                        propValueDict.Add(prop.name,propValues);
                    }
                    
                    propValuelist.Add(propValueDict);
                }
            }

            return propValuelist;
        }
    }
}