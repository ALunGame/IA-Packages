using IAEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAConfig.Excel.GenCode.Property
{
    internal static class ExcelPropertyMap
    {
        private static List<BaseProperty> singleProps = new List<BaseProperty>();
        private static List<EnumInfo> enumInfos = new List<EnumInfo>();
        private static List<ClassInfo> classInfos = new List<ClassInfo>();

        static ExcelPropertyMap()
        {
            singleProps.Add(new IntProperty());
            singleProps.Add(new FloatProperty());
            singleProps.Add(new BoolProperty());
            singleProps.Add(new Vector2Property());
            singleProps.Add(new Vector3Property());

            singleProps.Add(new StringProperty());
        }

        public static void SetEnumInfos(List<EnumInfo> pEnumInfos)
        {
            enumInfos.Clear();
            enumInfos = pEnumInfos;
        }
        
        public static void SetClassInfos(List<ClassInfo> pClassInfos)
        {
            classInfos.Clear();
            classInfos = pClassInfos;
        }


        public static BaseProperty GetPropertyByValue(string pValue)
        {
            for (int i = 0; i < singleProps.Count; i++)
            {
                BaseProperty prop = singleProps[i];
                if (prop.CanCatch(pValue))
                {
                    return (BaseProperty)Activator.CreateInstance(prop.GetType());
                }
            }
            
            Debug.LogError($"该值 {pValue} 没有可以转换的属性声明！！！");
            return null;
        }

        public static BaseProperty GetPropertyByTypeName(string pTypeName)
        {
            for (int i = 0; i < singleProps.Count; i++)
            {
                BaseProperty prop = singleProps[i];
                if (prop.TypeName == pTypeName)
                {
                    return (BaseProperty)Activator.CreateInstance(prop.GetType());
                }
            }

            for (int i = 0; i < enumInfos.Count; i++)
            {
                if (enumInfos[i].enumName == pTypeName || enumInfos[i].exName == pTypeName)
                {
                    EnumProperty enumProperty = new EnumProperty();
                    enumProperty.enumInfo = enumInfos[i];
                    return enumProperty;
                }
            }
            
            for (int i = 0; i < classInfos.Count; i++)
            {
                if (classInfos[i].className == pTypeName || classInfos[i].exName == pTypeName)
                {
                    CustomClassProperty customProperty = new CustomClassProperty();
                    customProperty.classInfo = classInfos[i];
                    return customProperty;
                }
            }

            Type type = ReflectionHelper.GetType(pTypeName);
            if (type != null)
            {
                if (type.IsEnum)
                {
                    DefaultEnumProperty enumProperty = new DefaultEnumProperty(type);
                    return enumProperty;
                }
            }

            Debug.LogError($"该类型 {pTypeName} 没有可以转换的属性声明！！！");
            return null;
        }

        public static bool CheckHasProperty(string pTypeName)
        {
            for (int i = 0; i < singleProps.Count; i++)
            {
                BaseProperty prop = singleProps[i];
                if (prop.TypeName == pTypeName)
                {
                    return true;
                }
            }
            
            for (int i = 0; i < enumInfos.Count; i++)
            {
                if (enumInfos[i].enumName == pTypeName || enumInfos[i].exName == pTypeName)
                {
                    return true;
                }
            }
            
            for (int i = 0; i < classInfos.Count; i++)
            {
                if (classInfos[i].className == pTypeName || classInfos[i].exName == pTypeName)
                {
                    return true;
                }
            }
            
            Type type = ReflectionHelper.GetType(pTypeName);
            if (type != null)
            {
                if (type.IsEnum)
                {
                    DefaultEnumProperty enumProperty = new DefaultEnumProperty(type);
                    return true;
                }
            }

            Debug.LogError($"该类型 {pTypeName} 没有可以转换的属性声明！！！");
            return false;
        }
    }
}