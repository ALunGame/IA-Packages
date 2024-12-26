using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace IAToolkit.Core
{
    public class ObjectInspector : ScriptableSingleton<ObjectInspector>
    {
        #region Static

        public static Editor objectEditor;

        public static void Repaint()
        {
            if (objectEditor == null)
            {
                return;
            }
            objectEditor.Repaint();
        }

        #endregion
        
        [SerializeField]
        object targetObject;

        public Action onTargetObjectChanged;

        public Action onClickBackFunc;

        public object TargetObject
        {
            get { return targetObject; }
            private set
            {
                if (targetObject == null || !targetObject.Equals(value))
                {
                    targetObject = value;
                    onTargetObjectChanged?.Invoke();
                }
            }
        }
        public object Owner { get; private set; }
        
        public void Init(object _targetObject, object _owner = null, Action _clickBackFunc = null)
        {
            Owner = _owner;
            TargetObject = _targetObject;
            onClickBackFunc = _clickBackFunc;
        }
    }

    [CustomEditor(typeof(ObjectInspector))]
    public class ObjectInspectorEditor : Editor
    {
        ObjectInspectorDrawer objectEditor;

        public object Owner { get; set; }

        public ObjectInspector T_Target { get { return target as ObjectInspector; } }

        private void OnEnable()
        {
            ObjectInspector.objectEditor = this;
            Owner = T_Target.Owner;

            OnEnable(T_Target.TargetObject);
            T_Target.onTargetObjectChanged = () =>
            {
                OnEnable(T_Target.TargetObject);
                Repaint();
            };

            void OnEnable(object _targetObject)
            {
                objectEditor = ObjectInspectorDrawer.CreateEditor(_targetObject, Owner, this, T_Target.onClickBackFunc);
                if (objectEditor != null)
                {
                    string title = objectEditor.GetTitle();
                    if (!string.IsNullOrEmpty(title))
                        target.name = title;
                    objectEditor.OnEnable();
                }
            }
        }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();
            if (objectEditor != null)
                objectEditor.OnHeaderGUI();
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement v = null;
            if (objectEditor != null)
                v = objectEditor.CreateInspectorGUI();
            if (v == null)
                v = base.CreateInspectorGUI();
            return v;
        }

        public override void OnInspectorGUI()
        {
            if (objectEditor != null)
            {
                if (objectEditor.onClickBackFunc != null)
                {
                    MiscHelper.Btn("返回",100,15, () =>
                    {
                        Action func = objectEditor.onClickBackFunc;
                        objectEditor.onClickBackFunc = null;

                        if (T_Target != null)
                        {
                            T_Target.onClickBackFunc = null;
                        }
                        func?.Invoke();
                    });
                }
                objectEditor.OnInspectorGUI();
            }
        }

        public override void DrawPreview(Rect previewArea)
        {
            base.DrawPreview(previewArea);
            if (objectEditor != null)
                objectEditor.DrawPreview(previewArea);
        }

        public override bool HasPreviewGUI()
        {
            if (objectEditor != null)
                return objectEditor.HasPreviewGUI();
            return base.HasPreviewGUI();
        }

        public override void OnPreviewSettings()
        {
            base.OnPreviewSettings();
            if (objectEditor != null)
                objectEditor.OnPreviewSettings();
        }

        public override GUIContent GetPreviewTitle()
        {
            GUIContent title = null;
            if (objectEditor != null)
                title = objectEditor.GetPreviewTitle();
            if (title == null)
                title = base.GetPreviewTitle();
            return title;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);
            if (objectEditor != null)
                objectEditor.OnPreviewGUI(r, background);
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            base.OnInteractivePreviewGUI(r, background);
            if (objectEditor != null)
                objectEditor.OnInteractivePreviewGUI(r, background);
        }

        private void OnSceneGUI()
        {
            if (objectEditor != null)
                objectEditor.OnSceneGUI();
        }

        private void OnValidate()
        {
            if (objectEditor != null)
                objectEditor.OnValidate();
        }

        private void OnDisable()
        {
            ObjectInspector.objectEditor = null;

            if (objectEditor != null)
                objectEditor.OnDisable();
        }
    }
}
