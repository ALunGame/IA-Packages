using IANodeGraph.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEditor;

namespace IANodeGraph.Editors
{
    public static class GraphProcessorEditorUtil
    {
        private static Dictionary<Type, Type> s_ViewTypesCache;
        private static Dictionary<Type, Type> s_WindowTypesCache;

        public static JsonSerializerSettings JsonSetting = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
        };

        static GraphProcessorEditorUtil()
        {
            Init();
        }

        private static void Init()
        {
            s_ViewTypesCache = new Dictionary<Type, Type>();
            s_WindowTypesCache = new Dictionary<Type, Type>();
            foreach (var type in TypeCache.GetTypesWithAttribute<CustomViewAttribute>())
            {
                if (type.IsAbstract) 
                    continue;
                if (typeof(BaseGraphWindow).IsAssignableFrom(type))
                {
                    foreach (var attribute in type.GetCustomAttributes(false))
                    {
                        if (!(attribute is CustomViewAttribute customViewAttribute))
                            continue;
                        s_WindowTypesCache[customViewAttribute.targetType] = type;
                    }
                }
                else
                {
                    foreach (var attribute in type.GetCustomAttributes(false))
                    {
                        if (!(attribute is CustomViewAttribute customViewAttribute))
                            continue;
                        s_ViewTypesCache[customViewAttribute.targetType] = type;
                    }
                }
            }
        }

        public static Type GetViewType(Type targetType)
        {
            var viewType = (Type)null;
            while (viewType == null)
            {
                s_ViewTypesCache.TryGetValue(targetType, out viewType);
                if (targetType.BaseType == null)
                    break;
                targetType = targetType.BaseType;
            }

            return viewType;
        }

        public static Type GetWindowType(Type targetType)
        {
            var windosType = (Type)null;
            while (windosType == null)
            {
                s_WindowTypesCache.TryGetValue(targetType, out windosType);
                if (targetType.BaseType == null)
                    break;
                targetType = targetType.BaseType;
            }

            return windosType;
        }
    }
}