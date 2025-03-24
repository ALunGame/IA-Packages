using System.IO;
using IAConfig.Excel.GenCode;
using UnityEditor;
using UnityEngine;

namespace IAConfig.Excel
{
    /// <summary>
    /// 配置设置
    /// </summary>
    public class ExcelReadSetting : ScriptableObject
    {
        public const string EditorRootPath = "Assets/IAConfig/Editor";
        public const string RunningRootPath = "Assets/IAConfig/Runtime";
        
        /// <summary>
        /// 表格根目录
        /// </summary>
        public string ConfigRootPath = "";
        
        /// <summary>
        /// 代码生成目录
        /// </summary>
        public string GenCodeRootPath = "";
        
        /// <summary>
        /// Json生成目录
        /// </summary>
        public string GenJsonRootPath = "";

        /// <summary>
        /// Json文件后缀名
        /// </summary>
        public string GenJsonExName = ".txt";
        
        /// <summary>
        /// 获取Table代码生成目录
        /// </summary>
        /// <returns></returns>
        public string GetGenTableCodeRootPath()
        {
            string dirPath = GenCodeRootPath + "/Table/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            return dirPath;
        }
        
        /// <summary>
        /// 获取Cfg代码生成目录
        /// </summary>
        /// <returns></returns>
        public string GetGenCfgCodeRootPath()
        {
            string dirPath = GenCodeRootPath + "/Cfg/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            return dirPath;
        }

        public string GetExportConfigPath(string pClassName)
        {
            string savePath = $"{GenJsonRootPath}/{GetExportConfigFileName(pClassName)}";
            return savePath;
        }
        
        public string GetExportConfigFileName(string pClassName)
        {
            return $"{ExcelGenCode.Tb}{pClassName}{GenJsonExName}";
        }

        #region Static

        private static ExcelReadSetting setting;
        public static ExcelReadSetting Setting
        {
            get
            {
                if (setting == null)
                {
                    string[] tileAssetPath = new string[] { GetSettingPath() };
                    string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", tileAssetPath);
                    foreach (var guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        ExcelReadSetting asset = AssetDatabase.LoadAssetAtPath<ExcelReadSetting>(path);
                        if (asset != null)
                        {
                            setting = asset;
                            break;
                        }
                    }
                }
                return setting;
            }
        }
        
        public static void CreateSetting()
        {
            if (Setting!=null)
            {
                Selection.activeObject = Setting;
                return;
            }
            if (!Directory.Exists(GetSettingPath()))
            {
                Directory.CreateDirectory(GetSettingPath());
            }
            ExcelReadSetting setting = CreateInstance<ExcelReadSetting>();
            setting.name = "配置设置";
            AssetDatabase.CreateAsset(setting, GetSettingPath() + "/配置设置.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = setting;
        }

        public static string GetSettingPath()
        {
            return EditorRootPath + "/Setting";
        }
        #endregion
    }
}