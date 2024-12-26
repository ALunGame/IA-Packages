using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IAEngine
{
    /// <summary>
    /// 反射辅助
    /// </summary>
    public static partial class ReflectionHelper
    {
        /// <summary>
        /// 运行时程序集
        /// </summary>
        public const string RunningtimeAssemblyName = "Assembly-CSharp";
        public const string EditorAssemblyName = "Assembly-CSharp-Editor";
        
        //各种缓存池
        static readonly Dictionary<string, Assembly> AssemblyCache = new Dictionary<string, Assembly>();
        static readonly Dictionary<string, Type> FullNameTypeCache = new Dictionary<string, Type>();
        static readonly List<Type> AllTypeCache = new List<Type>();
        static readonly Dictionary<Type, IEnumerable<Type>> ChildrenTypeCache = new Dictionary<Type, IEnumerable<Type>>();

        static ReflectionHelper()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("Unity")) continue;
                if (!assembly.FullName.Contains("Version=0.0.0")) continue;
                AssemblyCache[assembly.FullName] = assembly;
                AllTypeCache.AddRange(assembly.GetTypes());
            }
        }

        public static IEnumerable<Type> GetChildTypes<T>()
        {
            return GetChildTypes(typeof(T));
        }

        public static IEnumerable<Type> GetChildTypes(Type _type)
        {
            if (!ChildrenTypeCache.TryGetValue(_type, out IEnumerable<Type> childrenTypes))
                ChildrenTypeCache[_type] = childrenTypes = BuildCache(_type);

            foreach (var type in childrenTypes)
            {
                yield return type;
            }

            IEnumerable<Type> BuildCache(Type _baseType)
            {
                foreach (var type in AllTypeCache)
                {
                    if (type != _baseType && _baseType.IsAssignableFrom(type))
                    {
                        yield return type;
                    }    
                }
            }
        }

        public static Assembly LoadAssembly(string _assemblyString)
        {
            Assembly assembly;
            if (!AssemblyCache.TryGetValue(_assemblyString, out assembly))
                AssemblyCache[_assemblyString] = assembly = Assembly.Load(_assemblyString);
            return assembly;
        }

        /// <summary>
        /// 获得类型
        /// </summary>
        /// <param name="_fullName">类型全名</param>
        /// <param name="_assemblyString">程序集名</param>
        /// <returns></returns>
        public static Type GetType(string _fullName, string _assemblyString = "Assembly-CSharp")
        {
            Type type;
            if (FullNameTypeCache.TryGetValue(_fullName, out type))
                return type;
            Assembly assembly = LoadAssembly(_assemblyString);
            if (assembly == null) return null;
            foreach (var tempType in assembly.GetTypes())
            {
                FullNameTypeCache[tempType.FullName] = tempType;
            }
            if (FullNameTypeCache.TryGetValue(_fullName, out type))
                return type;
            return null;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object CreateInstance(Type type, params object[] args)
        {
            if (type == typeof(int))
            {
                return args.Length > 0 ? (int)args[0] : 0;
            }
            else if (type == typeof(float))
            {
                return args.Length > 0 ? (float)args[0] : 0;
            }
            else if (type == typeof(double))
            {
                return args.Length > 0 ? (double)args[0] : 0;
            }
            else if (type == typeof(bool))
            {
                return args.Length > 0 ? (bool)args[0] : false;
            }
            else if (type == typeof(string))
            {
                return args.Length > 0 ? (string)args[0] : "";
            }
            else if (type == typeof(Enum))
            {
                if (args.Length <= 0)
                {
                    string[] names = Enum.GetNames(type);
                    foreach (var item in names)
                    {
                        return Enum.Parse(type, item);
                    }
                }
                else
                {
                    return Enum.Parse(type, (string)args[0]);
                }
            }
            else
            {
                return Activator.CreateInstance(type, args);
            }
            return null;
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        public static T CreateInstance<T>(params object[] args)
        {
            return (T)CreateInstance(typeof(T), args);
        }

        #region MemberInfo  --成员信息

        static Dictionary<Type, List<MemberInfo>> TypeMemberInfoCache = new Dictionary<Type, List<MemberInfo>>();

        /// <summary>
        /// 获得类型的成员信息
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static IEnumerable<MemberInfo> GetMemberInfos(Type _type)
        {
            Type baseType = _type.BaseType;
            if (baseType != null)
            {
                foreach (var m in GetMemberInfos(baseType))
                {
                    yield return m;
                }
            }

            if (!TypeMemberInfoCache.TryGetValue(_type, out List<MemberInfo> memberInfos))
            {
                TypeMemberInfoCache[_type] = memberInfos = new List<MemberInfo>(_type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly));
            }

            foreach (var m in memberInfos)
            {
                yield return m;
            }
        }

        /// <summary> 
        /// 通过名字获得成员信息
        /// </summary>
        public static MemberInfo GetMemberInfo(Type _type, string _memberName)
        {
            return GetMemberInfos(_type).FirstOrDefault(f => f.Name == _memberName);
        }

        /// <summary> 
        /// 获取字段
        /// </summary>
        public static IEnumerable<FieldInfo> GetFieldInfos(Type _type)
        {
            foreach (var member in GetMemberInfos(_type))
            {
                if (member is FieldInfo fieldInfo)
                    yield return fieldInfo;
            }
        }

        /// <summary> 
        /// 获取指定字段
        /// </summary>
        public static FieldInfo GetFieldInfo(Type _type, string _fieldName)
        {
            return GetFieldInfos(_type).FirstOrDefault(f => f.Name == _fieldName);
        }

        /// <summary> 
        /// 获取属性
        /// </summary>
        public static IEnumerable<PropertyInfo> GetPropertyInfos(Type _type)
        {
            foreach (var member in GetMemberInfos(_type))
            {
                if (member is PropertyInfo propertyInfo)
                    yield return propertyInfo;
            }
        }

        /// <summary> 
        /// 获取指定属性
        /// </summary>
        public static PropertyInfo GetPropertyInfo(Type _type, string _propertyName)
        {
            return GetPropertyInfos(_type).FirstOrDefault(f => f.Name == _propertyName);
        }

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static IEnumerable<MethodInfo> GetMethodInfos(Type _type)
        {
            foreach (var member in GetMemberInfos(_type))
            {
                if (member is MethodInfo methodInfo)
                    yield return methodInfo;
            }
        }

        /// <summary>
        /// 获取指定方法
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_methodName"></param>
        /// <returns></returns>
        public static MethodInfo GetMethodInfo(Type _type, string _methodName)
        {
            return GetMethodInfos(_type).FirstOrDefault(t => t.Name == _methodName);
        }

        #endregion
    }
}
