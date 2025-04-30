using System;

namespace IANodeGraph
{
    /// <summary>
    /// 自定义节点显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CustomViewAttribute : Attribute
    {
        public Type targetType;

        public CustomViewAttribute(Type targetType)
        {
            this.targetType = targetType;
        }
    }
    
    /// <summary>
    /// 自定义绑定端口
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CustomPortAttribute : Attribute
    {
        public Type targetType;

        public CustomPortAttribute(Type targetType)
        {
            this.targetType = targetType;
        }
    }
}
