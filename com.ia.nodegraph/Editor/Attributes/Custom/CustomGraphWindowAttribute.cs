using System;

namespace IANodeGraph
{
    /// <summary>
    /// 自定义视图显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomGraphWindowAttribute : Attribute
    {
        public Type targetGraphType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetGraphType">视图类型</param>
        public CustomGraphWindowAttribute(Type targetGraphType)
        {
            this.targetGraphType = targetGraphType;
        }
    }
}
