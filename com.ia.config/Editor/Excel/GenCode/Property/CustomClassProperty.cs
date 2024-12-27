using IAEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IAConfig.Excel.GenCode.Property
{
    /// <summary>
    /// 自定义类型属性
    /// </summary>
    internal class CustomClassProperty : BaseProperty
    {
        public ClassInfo classInfo;
        
        public override string TypeName { get=>classInfo.className; }
        public override string NameSpace
        {
            get
            {
                if (string.IsNullOrEmpty(classInfo.nameSpace))
                {
                    return "";
                }
                else
                {
                    return $"using {classInfo.nameSpace};";
                }
            }
        }
        
        public override bool CanCatch(string pValue)
        {
            if (string.IsNullOrEmpty(pValue))
            {
                return true;
            }
            
            string[] values = pValue.Split(",");
            for (int i = 0; i < classInfo.fields.Count; i++)
            {
                ClassFieldInfo fieldInfo = classInfo.fields[i];
                BaseProperty fieldProp = ExcelPropertyMap.GetPropertyByTypeName(fieldInfo.typeName);
                if (!fieldProp.CanCatch(values[0]))
                {
                    return false;
                }
            }
            return true;
        }

        public override object Parse(string pValue)
        {
            if (string.IsNullOrEmpty(pValue))
            {
                return null;
            }
            string fullName = classInfo.nameSpace == ""
                ? classInfo.className
                : $"{classInfo.nameSpace}.{classInfo.className}";
            string[] values = pValue.Split(",");
            List<object> objlist = new List<object>();
            for (int i = 0; i < classInfo.fields.Count; i++)
            {
                ClassFieldInfo fieldInfo = classInfo.fields[i];
                BaseProperty fieldProp = ExcelPropertyMap.GetPropertyByTypeName(fieldInfo.typeName);
                objlist.Add(fieldProp.Parse(values[i]));
            }
            
            //return Activator.CreateInstance("Assembly-CSharp",classInfo.className, objlist.ToArray());

            return ReflectionHelper.CreateInstance(ReflectionHelper.GetType(fullName), objlist.ToArray());
        }

        public override string CreateExportStr(string pExportName, string pRowValueName)
        {
            string codeStr = "\t\t\t\t#NAME#.#PRONAME# = (#ValueTypeName#)GetProp(pProps,\"#PRONAME#\").Parse(propDict[\"#PRONAME#\"][0]);";
            codeStr = Regex.Replace(codeStr, "#NAME#", pExportName);
            codeStr = Regex.Replace(codeStr, "#PRONAME#", name);
            codeStr = Regex.Replace(codeStr, "#ValueTypeName#", TypeName);
            return codeStr;
        }
    }
}