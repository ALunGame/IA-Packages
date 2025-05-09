using IAToolkit;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IAConfig.Excel.GenCode
{
    public class ExcelGenConfigMappingCode
    {
        private const string FileStr = @"
using System;
using System.Collections.Generic;
using IAFramework;
using MemoryPack;
#USINGNAME#

namespace IAConfig
{
    public static partial class Config
    {
        /// <summary>
        /// 加载字节数组方法，注入
        /// </summary>
        public static Func<string, byte[]> LoadBytesFunc;

        #CNFSTR#

        public static void Preload()
        {
#PRELOAD#
        }
    }
}";

        private const string PreloadCode = @"
            var tPreLoad#NAME# = Config.#NAME#;";

        private const string RelaodCode = @"
            if(#NAME#!= null)
				#NAME#.Clear();";
        
        private const string CnfCode = @"
        private static #CLASS# #NAME01# = null;
        /// <summary>
        /// #DISPLAYNAME#
        /// </summary>
        public static #CLASS# #NAME02#
        {
            get
            {
                if (#NAME01# == null)
                {
                    #NAME01# = new #CLASS#();
                    #NAME01#.Load();
                }
                return #NAME01#;
            }
        }";


        public void GenMappingCode(List<GenConfigInfo> pInfos)
        {
            string resStr = FileStr;
            
            //命名空间
            string usingNameStr = "";
            string cnfStr = "";
            string preloadValue = "";
            string reloadValue = "";
            List<string> usingNames = new List<string>();
            foreach (var item in pInfos)
            {
                cnfStr += GenCnfCode(item);

                if (!usingNames.Contains(item.nameSpace))
                {
                    usingNames.Add(item.nameSpace);
                    string str = string.Format("using {0};\n", item.nameSpace);
                    usingNameStr = usingNameStr + str;
                }

                if (item.needPreload)
                {
                    //Preload
                    string preloadStr = PreloadCode;
                    preloadStr = Regex.Replace(preloadStr, "#NAME#", item.className) + "\n";
                    preloadValue += preloadStr;
                }

                //Reload
                // string reloadStr = RelaodCode;
                // reloadStr = Regex.Replace(reloadStr, "#NAME#", "_" + item.className) + "\n";
                // reloadValue += reloadStr;
            }
            resStr = Regex.Replace(resStr, "#USINGNAME#", usingNameStr);
            resStr = Regex.Replace(resStr, "#PRELOAD#", preloadValue);
            resStr = Regex.Replace(resStr, "#CNFSTR#", cnfStr);
            // resStr = Regex.Replace(resStr, "#RELOADVALUE#", reloadValue);

            //生成
            IOHelper.WriteText(resStr, ExcelReadSetting.RunningRootPath+"/Config.cs");
        }
        
        private string GenCnfCode(GenConfigInfo pInfo)
        {
            string className = pInfo.className;
            string classFileName = pInfo.GetFileName();

            string resStr = CnfCode;
            resStr = Regex.Replace(resStr, "#DISPLAYNAME#", pInfo.comment);
            resStr = Regex.Replace(resStr, "#CLASS#",  ExcelGenCode.Tb+className);
            resStr = Regex.Replace(resStr, "#TYPE#", pInfo.className);
            resStr = Regex.Replace(resStr, "#NAME01#", "_"+pInfo.className);
            resStr = Regex.Replace(resStr, "#NAME02#", pInfo.className);
            resStr = Regex.Replace(resStr, "#NAME03#", pInfo.GetFileNameNoExName());
            resStr += "\n";
            return resStr;
        }
    }
}