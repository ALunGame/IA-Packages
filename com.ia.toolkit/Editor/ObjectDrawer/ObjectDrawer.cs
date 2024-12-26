using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using IAEngine;

namespace IAToolkit.Core
{
    /// <summary>
    /// 自定义类型绘制
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
    public sealed class CustomObjectDrawerAttribute : Attribute
    {
        public Type targetType;

        public CustomObjectDrawerAttribute(Type _targetType)
        {
            targetType = _targetType;
        }
    }

    /// <summary>
    /// 自定义字段绘制
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class CustomFieldDrawerAttribute : Attribute
    {
        private Type type;

        public Type Type { get { return type; } }

        public CustomFieldDrawerAttribute(Type _type)
        {
            type = _type;
        }
    }

    /// <summary>
    /// EditorGUI面板绘制器
    /// </summary>
    public class ObjectDrawer
    {
        #region Static

        private static Dictionary<Type, Type> ObjectEditorTypeCache;

        static ObjectDrawer()
        {
            BuildCache();
        }

        private static void BuildCache()
        {
            ObjectEditorTypeCache = new Dictionary<Type, Type>();

            foreach (var type in TypeCache.GetTypesDerivedFrom<ObjectDrawer>())
            {
                foreach (var att in AttributeHelper.GetTypeAttributes(type, false))
                {
                    if (att is CustomObjectDrawerAttribute sAtt)
                        ObjectEditorTypeCache[sAtt.targetType] = type;
                }
            }
            
            
        }

        public static bool HasCustomEditor()
        {
            return false;
        }

        private static ObjectDrawer InternalCreateEditor(object _targetObject)
        {
            if (_targetObject == null) return null;

            return Activator.CreateInstance(GetEditorType(_targetObject.GetType()), true) as ObjectDrawer;
        }

        public static Type GetEditorType(Type _objectType)
        {
            if (ObjectEditorTypeCache.TryGetValue(_objectType, out Type editorType))
                return editorType;
            if (_objectType.BaseType != null)
                return GetEditorType(_objectType.BaseType);
            else
                return typeof(ObjectDrawer);
        }

        public static ObjectDrawer CreateEditor(object _targetObject)
        {
            ObjectDrawer objectEditor = InternalCreateEditor(_targetObject);
            if (objectEditor == null) return null;

            objectEditor.Init(_targetObject);
            return objectEditor;
        }

        public static float GetHeight(Type _objectType)
        {
            ObjectDrawer objectEditor = Activator.CreateInstance(GetEditorType(_objectType), true) as ObjectDrawer;
            if (objectEditor == null) return 0;
            return objectEditor.GetHeight();
        }

        public static bool CheckHasCustomDrawer(Type _objectType)
        {
            Type customDrawer = ObjectDrawer.GetEditorType(_objectType);
            if (!customDrawer.Equals(typeof(ObjectDrawer)) && customDrawer.BaseType == typeof(ObjectDrawer))
                return true;
            return false;
        }

        #endregion Static

        protected ObjectDrawer()
        {
        }

        public FieldInfo FieldInfo { get; set; }
        public object Target { get; set; }

        private void Init(object _target)
        {
            Target = _target;
            OnInit();
        }

        private void Init(object _target, FieldInfo _field)
        {
            Target = _target;
            OnInit();
        }

        public virtual void OnInit()
        {
            
        }

        public virtual object OnGUI(Rect _position, GUIContent _label)
        {
            GUI.Label(_position, _label);
            return Target;
        }

        public virtual float GetHeight()
        {
            return 20;
        }
    }
}