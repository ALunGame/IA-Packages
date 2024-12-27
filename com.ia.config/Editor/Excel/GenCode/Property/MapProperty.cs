using System.Text.RegularExpressions;

namespace IAConfig.Excel.GenCode.Property
{
    /// <summary>
    /// 字典属性
    /// </summary>
    internal class MapProperty : BaseProperty
    {
        public string KeyTypeName;
        public BaseProperty KeyProp;
        
        public string ValueTypeName;
        public BaseProperty ValueProp;
        
        public override string TypeName  { get  =>$"Dictionary<{KeyTypeName},{ValueTypeName}>"; }
        public override string NameSpace { get =>"using System.Collections.Generic;\n"+KeyProp.NameSpace+"\n"+ValueProp.NameSpace; }
        
        public override bool CanCatch(string pValue)
        {
            if (string.IsNullOrEmpty(pValue))
            {
                return true;
            }
            if (!pValue.Contains("|"))
            {
                return false;
            }
            
            string[] values = pValue.Split("|");
            string keyValue = values[0];
            string valueValue = values[1];
            if (KeyProp == null)
            {
                KeyProp = ExcelPropertyMap.GetPropertyByValue(keyValue);
                KeyTypeName = KeyProp.TypeName;
                ValueProp = ExcelPropertyMap.GetPropertyByValue(valueValue);
                ValueTypeName = KeyProp.TypeName;
            }

            if (KeyProp == null || ValueProp == null)
            {
                return false; 
            }
            return KeyProp.CanCatch(keyValue) && ValueProp.CanCatch(valueValue);
        }

        public override object Parse(string pValue)
        {
            return null;
        }

        public object GetKey(string pValue)
        {
            return KeyProp.Parse(pValue);
        }

        public object GetValue(string pValue)
        {
            return ValueProp.Parse(pValue);
        }
        
        public override string CreateExportStr(string pExportName, string pRowValueName)
        {
            string codeStr = @"
                #NAME#.#PRONAME# = new #TYPE#();
                for (int i = 0; i < propDict[""#PRONAME#""].Count; i++)
                {
                    string value = propDict[""#PRONAME#""][i];
                    if (string.IsNullOrEmpty(value))
                    {
                        break;
                    }
                    MapProperty prop = (MapProperty)GetProp(pProps,""#PRONAME#"");
                    string[] values = value.Split(""|"");
                    string key = values[0];
                    string va  = values[1];
                    #NAME#.#PRONAME#.Add((#KEYTYPE#)prop.GetKey(key),(#VALUETYPE#)prop.GetValue(va));
                }; 
            ";
            codeStr = Regex.Replace(codeStr, "#NAME#", pExportName);
            codeStr = Regex.Replace(codeStr, "#PRONAME#", name);
            codeStr = Regex.Replace(codeStr, "#TYPE#", TypeName);
            codeStr = Regex.Replace(codeStr, "#KEYTYPE#", KeyTypeName);
            codeStr = Regex.Replace(codeStr, "#VALUETYPE#", ValueTypeName);
            return codeStr;
        }
    }
}