namespace IAConfig.Excel.GenCode.Property
{
    internal abstract class BaseProperty
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string name;

        /// <summary>
        /// 是键
        /// </summary>
        public bool isKey;

        /// <summary>
        /// 注释
        /// </summary>
        public string comment = "";


        public BaseProperty()
        {
            
        }

        /// <summary>
        /// 可以当作键
        /// </summary>
        public virtual bool CanBeKey
        {
            get => false;
        }

        /// <summary>
        /// 属性类型名
        /// </summary>
        public abstract string TypeName { get; }
        
        /// <summary>
        /// 所需要的命名空间
        /// </summary>
        public abstract string NameSpace { get; }

        /// <summary>
        /// 判断值是否是该属性
        /// </summary>
        /// <param name="pValue"></param>
        /// <returns></returns>
        public abstract bool CanCatch(string pValue);

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="pValue"></param>
        /// <returns></returns>
        public abstract object Parse(string pValue);

        /// <summary>
        /// 创建导出代码
        /// </summary>
        /// <param name="pExportName">导出字段</param>
        /// <param name="pRowValues">一行数据</param>
        /// <returns></returns>
        public abstract string CreateExportStr(string pExportName,string pRowValueName);
    }
}