namespace IAECS
{
    /// <summary>
    /// ECS默认路径定义
    /// </summary>
    public static class ECSDefPath
    {
        /// <summary>
        /// 配置根目录
        /// </summary>
        public const string CnfRootPath = "Assets/Demo/Asset/Config/";

        /// <summary>
        /// 决策树根目录
        /// </summary>
        public const string DecTreeRootPath = CnfRootPath + "DecTree/";

        /// <summary>
        /// 行为树根目录
        /// </summary>
        public const string BevTreeRootPath = CnfRootPath + "BevTree/";

        /// <summary>
        /// 行为树根目录
        /// </summary>
        public const string EntityRootPath = CnfRootPath + "Entity/";

        public static string GetDecTreeCnfName(int entityId)
        {
            //return ConfigDef.GetCnfNoExName("Decision_" + entityId);
            return "";
        }

        /// <summary>
        /// 获得决策路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetDecTreePath(int treeId)
        {
            //return DecTreeRootPath + ConfigDef.GetCnfName("Decision_" + treeId);
            return "";
        }


        public static string GetBevTreeCnfName(RequestId requestId)
        {
            //return ConfigDef.GetCnfNoExName("Behavior_" + requestId.ToString());
            return "";
        }

        /// <summary>
        /// 获得行为路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetBevTreePath(RequestId requestId)
        {
            //return BevTreeRootPath + ConfigDef.GetCnfName("Behavior_" + requestId.ToString());
            return "";
        }

        public static string GetEntityCnfName(int entityId)
        {
            //return ConfigDef.GetCnfNoExName("Entity_" + entityId);
            return "";
        }

        /// <summary>
        /// 获得实体配置路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetEntityPath(int entityId)
        {
            //return EntityRootPath + ConfigDef.GetCnfName("Entity_" + entityId);
            return "";
        }
    }

    /// <summary>
    /// ECS各种定义
    /// </summary>
    public static class ECSDefinition
    {
        //只调用请求实例置换规则
        public const int RESwithRuleSelf = -9;
        //强制置换请求权重
        public const int REForceSwithWeight = -99;
    }
}
