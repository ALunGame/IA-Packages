using System.Collections.Generic;
using System.Text.RegularExpressions;
using IAConfig.Excel.GenCode.Property;
using UnityEngine;

namespace IAConfig.Excel.GenCode
{
    /// <summary>
    /// 创建配置代码
    /// </summary>
    internal class ExcelGenTbClassCode
    {
        private const string namespaceStr = @"
namespace #KEY#
{
    #STR#
}
";

        private const string classCodeStr = @"
    public class #KEY#
    {
        #STR#
    }
";

        private const string loadFun1Str = @"
        public void Load()
        {
            Byte[] byteArray = IAConfig.Config.LoadBytesFunc?.Invoke(""Tb#CLASSNAME#"");
            List<#CLASSNAME#> configs = MemoryPack.MemoryPackSerializer.Deserialize<List<#CLASSNAME#>>(byteArray);
            AddConfig(configs);
        }
";
        private const string reLoadFun2Str = @"
        public void ReLoad()
        {
            Clear();
            Load();
        }
";
        
        private const string fun1Str = @"
        public void AddConfig(#FUNC1PARAM#)
        {
            #FUNC1VALUE#
        }
";
        private const string fun2Str = @"
        public void AddConfig(List<#CLASSNAME#> configs)
        {
            foreach (var item in configs)
            {
                #CLASSNAME# config = item;
                AddConfig(#FUNC2PARAM#);
            }
        }
";
        
        private string nameSpace;
        private string className;

        public ExcelGenTbClassCode(string pNameSpace,string pClassName)
        {
            nameSpace = pNameSpace;
            className = pClassName;
        }
        
        public string GenTbCode(List<BaseProperty> pProps)
        {
            string usingStrs = "";
            List<string> usingCheckList = new List<string>();
            //找到所有的配置键
            List<BaseProperty> keyProps = new List<BaseProperty>();
            foreach (var info in pProps)
            {
                if (info.isKey)
                {
                    if (!info.CanBeKey)
                    {
                        Debug.LogError($"生成代码出错，该属性不可当作键{info.name}--{className}");
                        return "";
                    }
                    keyProps.Add(info);
                }

                if (!usingCheckList.Contains(info.NameSpace))
                {
                    usingStrs += info.NameSpace + "\n";
                    usingCheckList.Add(info.NameSpace);
                }
                
            }

            string classDefStr = ExcelGenCode.Tb + className + " : Dictionary<#KEY#, #VALUE#>, IAConfig.IConfigItem";
            classDefStr = Regex.Replace(classDefStr, "#KEY#", keyProps[0].TypeName);
            classDefStr = Regex.Replace(classDefStr, "#VALUE#", GenValueStr(keyProps));

            string classCodeContentStr = "";
            classCodeContentStr += Regex.Replace(loadFun1Str, "#CLASSNAME#", className);
            classCodeContentStr += reLoadFun2Str;
            classCodeContentStr += GenFunc1(fun1Str, keyProps, className);
            classCodeContentStr += GenFunc2(fun2Str, keyProps, className);

            string resCodeStr = classCodeStr;
            resCodeStr = Regex.Replace(resCodeStr, "#STR#", classCodeContentStr);
            resCodeStr = Regex.Replace(resCodeStr, "#KEY#", classDefStr);
            
            string resStr = "using System.Collections.Generic;\n" + usingStrs + namespaceStr;
            resStr = Regex.Replace(resStr, "#STR#", resCodeStr);
            resStr = Regex.Replace(resStr, "#KEY#", nameSpace);
            
            return resStr; 
        }
        
        private string GenValueStr(List<BaseProperty> keys)
        {
            if (keys.Count <= 1)
                return className;
            string resValue = "";
            for (int i = 1; i < keys.Count; i++)
            {
                BaseProperty tmpField = keys[i];
                string formatStr = "Dictionary<{0}, ";
                resValue += string.Format(formatStr, tmpField.TypeName);
            }
            resValue += className;
            for (int i = 1; i < keys.Count; i++)
            {
                resValue += ">";
            }
            return resValue;
        }
        
        #region FUNC1

        private static string GenFunc1(string resStr, List<BaseProperty> keyFields, string cnfType)
        {
            //函数参数
            string func1Param = "";
            for (int i = 0; i < keyFields.Count; i++)
            {
                BaseProperty fieldInfo = keyFields[i];
                func1Param = func1Param + (fieldInfo.TypeName + " key" + (i + 1) + ",") + " ";
            }
            func1Param = func1Param + cnfType + " config";
            resStr = Regex.Replace(resStr, "#FUNC1PARAM#", func1Param);
            
            //函数内容
            string fun1ValueStr = GenFunc1Value(keyFields, cnfType);
            resStr = Regex.Replace(resStr, "#FUNC1VALUE#", fun1ValueStr);
            return resStr;
        }
        

        public static string GenFunc1Value(List<BaseProperty> keyFields, string cnfType)
        {
            string genValueStr(int index, List<BaseProperty> keyFields, string cnfType)
            {
                if (index == keyFields.Count - 1)
                {
                    return "config";
                }

                string resValue = "new ";
                for (int i = index + 1; i < keyFields.Count; i++)
                {
                    string formatStr = "Dictionary<{0}, ";
                    BaseProperty tmpField = keyFields[i];
                    resValue += string.Format(formatStr, tmpField.TypeName);
                }
                resValue += cnfType;
                for (int i = index; i < keyFields.Count - 1; i++)
                {
                    resValue += ">";

                }
                resValue += "()";
                return resValue;
            }

            string currStr = "";
            for (int i = 0; i < keyFields.Count; i++)
            {
                string valueStr = @"
            if (!this#VALUE1#ContainsKey(#VALUE2#))
            {
                this#VALUE1#Add(#VALUE2#, #VALUE3#);
            }";

                string value1 = "";
                for (int j = 1; j <= i; j++)
                {
                    value1 += string.Format("[{0}]", "key" + j);
                }
                value1 += ".";

                string value2 = string.Format("{0}", "key" + (i + 1));

                valueStr = Regex.Replace(valueStr, "#VALUE1#", value1);
                valueStr = Regex.Replace(valueStr, "#VALUE2#", value2);
                valueStr = Regex.Replace(valueStr, "#VALUE3#", genValueStr(i, keyFields, cnfType));
                currStr += valueStr;
            }

            return currStr;
        }

        #endregion
        
        #region FUNC2

        private string GenFunc2(string resStr, List<BaseProperty> keyFields, string cnfType)
        {
            //函数参数
            string func2Param = "";
            for (int i = 0; i < keyFields.Count; i++)
            {
                BaseProperty fieldInfo = keyFields[i];
                func2Param = func2Param + string.Format("config.{0}", fieldInfo.name) + ", ";
            }
            func2Param = func2Param + "config";
            resStr = Regex.Replace(resStr, "#FUNC2PARAM#", func2Param);
            resStr = Regex.Replace(resStr, "#CLASSNAME#", cnfType);
            return resStr;
        }


        #endregion
    }
}