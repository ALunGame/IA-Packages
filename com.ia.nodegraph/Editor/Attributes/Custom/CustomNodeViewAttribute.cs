using System;

namespace IANodeGraph
{
    /// <summary>
    /// 自定义节点显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CustomNodeViewAttribute : Attribute
    {
        public Type targetNodeType;

        public CustomNodeViewAttribute(Type targetNodeType)
        {
            this.targetNodeType = targetNodeType;
        }
    }
}
