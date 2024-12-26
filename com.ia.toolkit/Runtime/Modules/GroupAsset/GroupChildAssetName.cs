using System;

namespace IAToolkit
{
    
#if UNITY_EDITOR
    /// <summary>
    /// 声明所属分组
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GroupAssetTypeNameAttribute : Attribute
    {
        public string fullName;

        public GroupAssetTypeNameAttribute(string fullName)
        {
            this.fullName = fullName;
        }
    }
#endif
    
    /// <summary>
    /// 组资源名
    /// </summary>
    public abstract class GroupChildAssetName
    {
        /// <summary>
        /// 资源名
        /// </summary>
        public string Name = "";

        public static string GetName(string pGroupName,string pAssetName)
        {
            return $"{pGroupName}_{pAssetName}";
        }
    }
}