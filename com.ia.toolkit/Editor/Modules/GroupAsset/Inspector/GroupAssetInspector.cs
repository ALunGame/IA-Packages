using IAToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace IAToolkit.Inspector
{
    [CustomEditor(typeof(InternalGroupAsset), true)]
    internal class GroupAssetInspector : Editor
    {
        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();
        
        private SerializedProperty childPathlistProperty;
        private ReorderableList childPathlist;

        private void OnEnable()
        {
            childPathlistProperty = serializedObject.FindProperty("ChildAssetPaths");
            childPathlist = new ReorderableList(serializedObject, childPathlistProperty, true, true, true, true)
            {
                drawHeaderCallback  = DrawChildAssetPathListElementHeader,
                drawElementCallback   = DrawChildAssetPathListElement,
            };
        }

        public override void OnInspectorGUI()
        {
            if (!ContextDataCache.TryGetContextData<GUIStyle>("BigLabel", out var bigLabel))
            {
                bigLabel.value = new GUIStyle(GUI.skin.label);
                bigLabel.value.fontSize = 18;
                bigLabel.value.fontStyle = FontStyle.Bold;
                bigLabel.value.alignment = TextAnchor.MiddleLeft;
                bigLabel.value.stretchWidth = true;
            }

            InternalGroupAsset groupAsset = target as InternalGroupAsset;
            
            GUILayoutExtension.VerticalGroup(() =>
            {
                GUILayout.Label($"{groupAsset.DisplayName}:{groupAsset.name}", bigLabel.value);
            });
            
            serializedObject.Update();
            childPathlist.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            
            GUILayoutExtension.VerticalGroup(() =>
            {
                if (GUILayout.Button($"创建{groupAsset.DisplayName}", GUILayout.Height(50)))
                {
                    groupAsset.OnClickCreateBtn();
                }
            
                if (GUILayout.Button($"导出{groupAsset.DisplayName}配置", GUILayout.Height(50)))
                {
                    groupAsset.OnClickExport();
                }
            });
        }

        private void DrawChildAssetPathListElementHeader(Rect rect)
        {
            string name = "资源文件夹";
            EditorGUI.LabelField(rect, name);
        }

        private void DrawChildAssetPathListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var property = childPathlistProperty.GetArrayElementAtIndex(index);
            var controlID = EditorGUIUtility.GetControlID("PackableElement".GetHashCode(), FocusType.Passive);
            var previousObject = property.objectReferenceValue;

            var changedObject = EditorGUI.ObjectField(rect,previousObject, typeof(UnityEngine.Object), false);
            if (changedObject != previousObject)
            {
                bool isFolder = AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(changedObject));
                if (isFolder)
                    property.objectReferenceValue = changedObject;
            }

            if (GUIUtility.keyboardControl == controlID && !isActive)
                childPathlist.index = index;
        }
    }
}