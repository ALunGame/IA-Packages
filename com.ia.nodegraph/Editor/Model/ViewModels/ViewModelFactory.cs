using System;
using System.Collections.Generic;
using System.Reflection;

namespace IANodeGraph.Model
{
    public class ViewModelFactory
    {
        static Dictionary<Type, Type> ViewModelTypeCache;

        static ViewModelFactory()
        {
            ViewModelTypeCache = new Dictionary<Type, Type>();
            
            foreach (var type in Util_TypeCache.GetTypesWithAttribute<NodeViewModelAttribute>())
            {
                if (type.IsAbstract) continue;
                var attribute = type.GetCustomAttribute<NodeViewModelAttribute>(true);
                ViewModelTypeCache[attribute.targetType] = type;
            }
        }
        
        public static Type GetViewModelType(Type modelType)
        {
            var viewModelType = (Type)null;
            while (viewModelType == null)
            {
                ViewModelTypeCache.TryGetValue(modelType, out viewModelType);
                if (modelType.BaseType == null)
                    break;
                modelType = modelType.BaseType;
            }
            return viewModelType;
        }

        public static object CreateViewModel(object model)
        {
            return Activator.CreateInstance(GetViewModelType(model.GetType()), model);
        }
    }
}