using System;

namespace MemoryPack
{
    /// <summary>
    /// 需要序列化的基类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class MemoryPackableBaseClass : Attribute
    {
        
    }
}