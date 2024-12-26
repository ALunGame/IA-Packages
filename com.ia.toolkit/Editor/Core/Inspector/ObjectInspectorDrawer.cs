using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IAEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace IAToolkit.Core
{
    /// <summary>
    /// 自定义Inspector绘制
    /// </summary>
    public class CustomInspectorDrawer : Attribute
    {
        public Type targetType;

        public CustomInspectorDrawer(Type _targetType) { targetType = _targetType; }
    }

    /// <summary>
    /// Inspector面板绘制器
    /// </summary>
    public class ObjectInspectorDrawer
    {
        #region Static

        static Dictionary<Type, Type> ObjectEditorTypeCache;

        static ObjectInspectorDrawer()
        {
            BuildCache();
        }

        private static void BuildCache()
        {
            ObjectEditorTypeCache = new Dictionary<Type, Type>();

            foreach (var type in TypeCache.GetTypesDerivedFrom<ObjectInspectorDrawer>())
            {
                foreach (var att in AttributeHelper.GetTypeAttributes(type, true))
                {
                    if (att is CustomInspectorDrawer sAtt)
                        ObjectEditorTypeCache[sAtt.targetType] = type;
                }
            }
        }

        private static List<Type> GetBaseTypes(Type objectType)
        {
            //只找四层
            List<Type> baseTypes = new List<Type>();
            if (objectType.BaseType != null)
            {
                baseTypes.Add(objectType.BaseType);
                if (objectType.BaseType.BaseType != null)
                {
                    baseTypes.Add(objectType.BaseType.BaseType);
                    if (objectType.BaseType.BaseType.BaseType != null)
                    {
                        baseTypes.Add(objectType.BaseType.BaseType.BaseType);
                    }
                }
            }
            return baseTypes;
        }

        //获得对象绘制类
        private static Type GetEditorType(Type objectType)
        {
            if (ObjectEditorTypeCache.TryGetValue(objectType, out Type editorType))
                return editorType;

            //接口
            Type[] interfaces = objectType.GetInterfaces();
            if (interfaces != null && interfaces.Length > 0)
            {
                for (int i = 0; i < interfaces.Length; i++)
                {
                    if (ObjectEditorTypeCache.TryGetValue(interfaces[i], out Type interfacesEditorType))
                        return interfacesEditorType;
                }
            }

            //父类
            List<Type> baseTypes = GetBaseTypes(objectType);
            foreach (Type type in baseTypes)
            {
                if (ObjectEditorTypeCache.TryGetValue(type, out Type baseEditorType))
                    return baseEditorType;
            }
            return typeof(ObjectInspectorDrawer);
        }

        private static ObjectInspectorDrawer InternalCreateEditor(object _targetObject)
        {
            if (_targetObject == null) return null;
            return Activator.CreateInstance(GetEditorType(_targetObject.GetType()), true) as ObjectInspectorDrawer;
        }

        //创建对象绘制类
        public static ObjectInspectorDrawer CreateEditor(object _targetObject)
        {
            ObjectInspectorDrawer objectEditor = InternalCreateEditor(_targetObject);
            if (objectEditor == null) return null;

            objectEditor.Init(_targetObject);
            return objectEditor;
        }

        public static ObjectInspectorDrawer CreateEditor(object _targetObject, object _owner)
        {
            ObjectInspectorDrawer objectEditor = InternalCreateEditor(_targetObject);
            if (objectEditor == null) return null;

            objectEditor.Init(_targetObject, _owner);
            return objectEditor;
        }

        public static ObjectInspectorDrawer CreateEditor(object _targetObject, object _owner, Editor _editor,Action _clickBackFunc = null)
        {
            ObjectInspectorDrawer objectEditor = InternalCreateEditor(_targetObject);
            if (objectEditor == null) return null;

            objectEditor.Init(_targetObject, _owner, _editor,_clickBackFunc);
            return objectEditor;
        }

        #endregion

        protected IReadOnlyList<FieldInfo> Fields { get; private set; }

        public object Target { get; private set; }
        public object Owner { get; private set; }
        public Editor Editor { get; private set; }
        public MonoScript Script { get; private set; }
        
        public Action onClickBackFunc { get; set; }

        protected ObjectInspectorDrawer() { }

        #region 初始化

        public void Init(object _target,Action _clickBackFunc = null)
        {
            onClickBackFunc = _clickBackFunc;
            Target = _target;
            Script = EditorExtension.FindScriptFromType(Target.GetType());
            Fields = ReflectionHelper.GetFieldInfos(Target.GetType()).Where(field => GUILayoutExtension.CanDraw(field)).ToList();
        }

        public void Init(object _target, object _owner, Action _clickBackFunc = null)
        {
            Owner = _owner;
            Init(_target,_clickBackFunc);
        }

        public void Init(object _target, object _owner, Editor _editor,Action _clickBackFunc = null)
        {
            Owner = _owner;
            Editor = _editor;
            Init(_target,_clickBackFunc);
        }

        public void UpdateTarget(object _target)
        {
            Target = _target;
        }

        #endregion

        /// <summary>
        /// 标题
        /// </summary>
        public virtual string GetTitle() { return string.Empty; }

        public virtual void OnEnable() { }

        public virtual void OnHeaderGUI() { }

        public virtual void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", Script, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
            foreach (var field in Fields)
            {
                GUILayoutExtension.DrawField(field, Target);
            }
        }

        public virtual bool HasPreviewGUI() { return false; }

        public virtual GUIContent GetPreviewTitle() { return null; }

        public virtual void OnPreviewSettings() { }

        public virtual void DrawPreview(Rect previewArea) { }

        public virtual void OnPreviewGUI(Rect _r, GUIStyle _background) { }

        public virtual void OnInteractivePreviewGUI(Rect _r, GUIStyle _background) { }

        public virtual void OnValidate() { }

        public virtual void OnSceneGUI() { }

        public virtual void OnDisable() { }

        public VisualElement CreateInspectorGUI() { return null; }
    } 
}
