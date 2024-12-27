using IAConfig.Excel.GenCode.Property;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IAConfig.Excel.GenCode
{
    internal class CommonExcelGenClassCode
    {
        private const string namespaceStr = @"
namespace #KEY#
{
    #STR#
}
";

        private const string classCodeStr = @"
    /// <summary>
    /// #Comment#
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
        
        private string nameSpace;
        private string className;
        
        public CommonExcelGenClassCode(string pNameSpace,string pClassName)
        {
            nameSpace = pNameSpace;
            className = pClassName;
        }

        public string GenClassCode(List<BaseProperty> pProps,GenConfigInfo pInfo)
        {
            string usingNameStr = "using MemoryPack;\n";
            string propStr = "";
            
            List<string> usingNamsList = new List<string>();
            foreach (BaseProperty prop in pProps)
            {
                string tStr = Regex.Replace(propCodeStr, "#TYPE#", prop.TypeName);
                tStr = Regex.Replace(tStr, "#PRONAME#", prop.name);
                tStr = Regex.Replace(tStr, "#Comment#", prop.comment);
                propStr += tStr;
                
                if (!usingNamsList.Contains(prop.NameSpace))
                {
                    usingNamsList.Add(prop.NameSpace);
                    usingNameStr += prop.NameSpace + "\n";
                }
            }

            string classStr = classCodeStr;
            classStr = Regex.Replace(classStr, "#KEY#", className);
            classStr = Regex.Replace(classStr, "#STR#", propStr);
            classStr = Regex.Replace(classStr, "#Comment#", pInfo.comment == null ? "":pInfo.comment);
            
            string resStr = usingNameStr + namespaceStr;
            resStr = Regex.Replace(resStr, "#KEY#", nameSpace);
            resStr = Regex.Replace(resStr, "#STR#", classStr);
            return resStr;
        }
    }
}