using System;

namespace IANodeGraph
{
    /// <summary>
    /// 自定义组显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class CustomGroupViewAttribute : Attribute
    {
        public Type targetNodeType;

        public CustomGroupViewAttribute(Type targetNodeType)
        {
            this.targetNodeType = targetNodeType;
        }
    }
}