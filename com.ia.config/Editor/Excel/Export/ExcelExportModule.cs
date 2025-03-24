using IAConfig.Excel.GenCode;
using IAConfig.Excel.GenCode.Property;
using IAEngine;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using UnityEngine;

//#region AutoGenUsing
//using IAUI;
//using Gameplay.Map;
//using UnityEngine;
//using Gameplay.Map;
//using Gameplay.Map;
//using Gameplay.Map;
//using Gameplay;
//#endregion AutoGenUsing

namespace IAConfig.Excel.Export
{
    internal partial class ExcelExportModule
    {
        private Dictionary<string, Action<GenConfigInfo, List<BaseProperty>, List<Dictionary<string, List<string>>>>>
            exportFuncDict =
                new Dictionary<string,
                    Action<GenConfigInfo, List<BaseProperty>, List<Dictionary<string, List<string>>>>>();
        
        private bool _init = false;
        private void init()
        {
            if (_init)
            {
                return;
            }
            _init = true;
        }
        
        public BaseProperty GetProp(List<BaseProperty> pProps, string pPropName)
        {
            foreach (BaseProperty baseProperty in pProps)
            {
                if (baseProperty.name == pPropName)
                {
                    return baseProperty;
                }
            }

            return null;
        }

        private List<Dictionary<string, List<string>>> GetAllPropValuelist(List<BaseProperty> pProps,List<ExcelWorksheet> pSheets,Dictionary<string, List<int>> pPropColDict)
        {
            List<Dictionary<string, List<string>>> propValuelist = new List<Dictionary<string, List<string>>>();
            foreach (ExcelWorksheet pSheet in pSheets)
            {
                if (pSheet == null || pSheet.Dimension == null)
                {
                    continue;
                }
                //最大行
                int _MaxRowNum = pSheet.Dimension.End.Row;
                //最小行
                int _MinRowNum = pSheet.Dimension.Start.Row;
            
                //最大列
                int _MaxColumnNum = pSheet.Dimension.End.Column;
                //最小列
                int _MinColumnNum = pSheet.Dimension.Start.Column;

                bool hasDefault = false;
                int defaultRow = 0;
                for (int row = 2; row <= _MaxRowNum; row++)
                {
                    string firstValue = ExcelReader.GetCellValue(pSheet,row, 1).ToString();
                    //特殊标记
                    if (firstValue.Contains("##"))
                    {
                        if (firstValue == "##default")
                        {
                            hasDefault = true;
                            defaultRow = row;
                        }
                        continue;
                    }

                    bool isSuccess = true;
                    
                    Dictionary<string, List<string>> propValueDict = new Dictionary<string, List<string>>();
                    foreach (string propName in pPropColDict.Keys)
                    {
                        List<int> colList = pPropColDict[propName];
                        List<string> values = new List<string>();
                        BaseProperty prop = GetProp(pProps, propName);
                        for (int i = 0; i < colList.Count; i++)
                        {
                            int col = colList[i];
                            string value =  ExcelReader.GetCellValue(pSheet,row, col).ToString();
                            if (hasDefault && string.IsNullOrEmpty(value))
                            {
                                value = ExcelReader.GetCellValue(pSheet,defaultRow, col).ToString();
                            }
                            if (string.IsNullOrEmpty(value))
                            {
                                if (prop.isKey)
                                {
                                    isSuccess = false;
                                    break;
                                }
                                else
                                {
                                    if (!prop.CanCatch(value))
                                    {
                                        isSuccess = false;
                                        Debug.LogWarning($"表格导出出错，类型不匹配{value}--->{prop.TypeName}:{prop.name} Sheet:{pSheet.Name} Col:{col} Row:{row}");
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (!prop.CanCatch(value))
                                {
                                    isSuccess = false;
                                    Debug.LogWarning($"表格导出出错，类型不匹配{value}--->{prop.TypeName}:{prop.name} Sheet:{pSheet.Name} Col:{col} Row:{row}");
                                    break;
                                } 
                            }
                            values.Add(value);
                        }

                        if (isSuccess)
                        {
                            propValueDict.Add(propName,values);
                        }
                        else
                        {
                            break;
                        }
                        
                    }

                    if (isSuccess)
                    {
                        propValuelist.Add(propValueDict);
                    }
                }
            }
            return propValuelist;
        }

        public void ExportAll(List<GenConfigInfo> pConfigs)
        {
            init();
            foreach (GenConfigInfo info in pConfigs)
            {
                ExcelPackage tPackage = null;
                
                List<ExcelWorksheet> sheets = new List<ExcelWorksheet>();
                foreach (string filePath in info.filePaths)
                    sheets.AddRange(ExcelReader.ReadAllSheets(filePath,out tPackage, info.sheetName));

                if (!sheets.IsLegal())
                {
                    Debug.LogError($"导出失败，没有对应工作簿:{info.filePaths[0]}-->{info.sheetName}");
                }
                List<BaseProperty> props = ExcelGenCode.GetPropsByCommonExcel(sheets[0], out var propDict);
                exportFuncDict[info.className].Invoke(info,props,GetAllPropValuelist(props,sheets,propDict));
                
                tPackage.Dispose();
            }
        }

        public List<T> Export<T>(GenConfigInfo pInfo)
        {
            init();
            ExcelPackage tPackage = null;
            
            List<ExcelWorksheet> sheets = new List<ExcelWorksheet>();
            foreach (string filePath in pInfo.filePaths)
                sheets.AddRange(ExcelReader.ReadAllSheets(filePath,out tPackage, pInfo.sheetName));
            
            List<BaseProperty> props = ExcelGenCode.GetPropsByCommonExcel(sheets[0], out var propDict);
            exportFuncDict[pInfo.className].Invoke(pInfo,props,GetAllPropValuelist(props,sheets,propDict));
            
            tPackage.Dispose();

            return pInfo.resValue as List<T>;
        }
    }
}



















































































































































