using System.Collections.Generic;

namespace IAConfig.Excel.Export
{
    public class ExcelExportSystem
    {
        private ExcelExportModule _exportModule = new ExcelExportModule();
        
        public void ExportAll(List<GenConfigInfo> pConfigs)
        {
            _exportModule.ExportAll(pConfigs);
        }

        public List<T> Export<T>(GenConfigInfo pInfo)
        {
            return _exportModule.Export<T>(pInfo);
        }
    }
}