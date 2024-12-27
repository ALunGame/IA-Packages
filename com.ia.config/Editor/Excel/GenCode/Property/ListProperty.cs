using System.Text.RegularExpressions;

namespace IAConfig.Excel.GenCode.Property
{
    /// <summary>
    /// 列表属性
    /// </summary>
    internal class ListProperty : BaseProperty
    {
        public string ChildTypeName;
        public BaseProperty ChildProp;
        public override string TypeName { get => $"List<{ChildTypeName}>"; }

        public override string NameSpace
        {
            get => "using System.Collections.Generic;"+"\n"+ChildProp.NameSpace;
        }
        
        public override bool CanCatch(string pValue)
        {
            if (ChildProp == null)
            {
                ChildProp = ExcelPropertyMap.GetPropertyByValue(pValue);
                ChildTypeName = ChildProp.TypeName;
            }

            if (ChildProp == null)
            {
                return false;
            }

           
            return ChildProp.CanCatch(pValue);
        }

        public override object Parse(string pValue)
        {
            return ChildProp.Parse(pValue);
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
                    #NAME#.#PRONAME#.Add((#CHILDTYPE#)GetProp(pProps,""#PRONAME#"").Parse(value));
                }; 
            ";
            codeStr = Regex.Replace(codeStr, "#NAME#", pExportName);
            codeStr = Regex.Replace(codeStr, "#PRONAME#", name);
            codeStr = Regex.Replace(codeStr, "#TYPE#", TypeName);
            codeStr = Regex.Replace(codeStr, "#CHILDTYPE#", ChildTypeName);
            codeStr = Regex.Replace(codeStr, "#ROWVALUENAME#", pRowValueName);
            return codeStr;
        }
    }
}