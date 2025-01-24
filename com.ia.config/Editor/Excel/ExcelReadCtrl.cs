using IAConfig.Excel.Export;
using IAConfig.Excel.GenCode;
using IAConfig.Excel.GenCode.Property;
using IAToolkit;
using MemoryPack;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace IAConfig.Excel
{
    public static class ExcelReadCtrl
    {
        private static List<GenConfigInfo> ReadBaseExcel(bool pNeedGenCode = false)
        {
            //导出自定义枚举
            EnumExcelGenCode enumExcelGenCode = new EnumExcelGenCode("基础配置/GenEnumConfig.xlsx");
            List<EnumInfo> enumInfos = pNeedGenCode ? enumExcelGenCode.GenAllClass() : enumExcelGenCode.GetAllEnum();
            ExcelPropertyMap.SetEnumInfos(enumInfos);

            //导出自定义类
            CustomClassExcelGenCode customExcelGenCode = new CustomClassExcelGenCode("基础配置/GenClassConfig.xlsx");
            List<ClassInfo> classInfos =  pNeedGenCode ? customExcelGenCode.GenAllClass() : customExcelGenCode.GetAllClass();
            ExcelPropertyMap.SetClassInfos(classInfos);

            //导出所有表格Json
            GenConfigExcelRead genConfigExcelRead = new GenConfigExcelRead("基础配置/GenConfig.xlsx");
            List<GenConfigInfo> configs = genConfigExcelRead.ReadAllConfigs();
            return configs;
        }


        [MenuItem("Tools/表格/生成代码")]
        public static void GenCode()
        {
            List<GenConfigInfo> configs = ReadBaseExcel(true);

            //生成导出代码
            ExcelGenCode genCode = new ExcelGenCode();
            genCode.GenConfigsGenCodes(configs);

            //生成映射代码
            ExcelGenConfigMappingCode mappingCode = new ExcelGenConfigMappingCode();
            mappingCode.GenMappingCode(configs);
            
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Tools/表格/导出所有")]
        public static void ExportAll()
        {
            List<GenConfigInfo> configs = ReadBaseExcel();
            ExcelExportSystem exportSystem = new ExcelExportSystem();
            exportSystem.ExportAll(configs);
            
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Tools/表格/设置")]
        public static void CreateSetting()
        {
            ExcelReadSetting.CreateSetting();
        }

        [MenuItem("Tools/表格/打开表格目录 &e")]
        public static void OpenExcelRootPath()
        {
            MiscHelper.OpenDirectory(ExcelReadSetting.Setting.ConfigRootPath);
        }
        
        [MenuItem("Tools/表格/测试")]
        public static void Test()
        {
            

        }
        
        /// <summary>
        /// 加载配置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> LoadConfig<T>()
        {
            string savePath = $"{ExcelReadSetting.Setting.GenJsonRootPath}/{$"Tb{typeof(T).Name}"}{ExcelReadSetting.Setting.GenJsonExName}";

            List<T> configs = new List<T>();
            using (FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.Read))
            {
                byte[] byteArray = new byte[fs.Length];
                fs.Read(byteArray, 0, byteArray.Length);
                configs = MemoryPackSerializer.Deserialize<List<T>>(byteArray);
            }

            return configs;
        }
    }
}