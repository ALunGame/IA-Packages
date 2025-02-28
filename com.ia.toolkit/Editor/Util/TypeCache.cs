using System;

namespace IAToolkit.Misc
{
    public static class TypeCache<T>
    {
        public static readonly Type TYPE = typeof(T);
        
        public static readonly int HASH = typeof(T).GetHashCode();
    }
}