using IANodeGraph.Model.Internal;
using IANodeGraph.View;
using IAToolkit;
using IAToolkit.UnityEditors;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IANodeGraph.Inspector
{
    [CustomEditor(typeof(InternalGraphGroupAsset), true)]
    public class BaseGraphGroupAssetInspector : Editor
    {
        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!ContextDataCache.TryGetContextData<GUIStyle>("BigLabel", out var bigLabel))
            {
                bigLabel.value = new GUIStyle(GUI.skin.label);
                bigLabel.value.fontSize = 18;
                bigLabel.value.fontStyle = FontStyle.Bold;
                bigLabel.value.alignment = TextAnchor.MiddleLeft;
                bigLabel.value.stretchWidth = true;
            }

            InternalGraphGroupAsset groupAsset = target as InternalGraphGroupAsset;

            EditorGUILayoutExtension.VerticalGroup(() =>
            {
                GUILayout.Label($"{groupAsset.DisplayName}:{groupAsset.name}", bigLabel.value);
            });

            EditorGUILayoutExtension.VerticalGroup(() => {
                List<InternalBaseGraphAsset> graphs = groupAsset.GetAllGraph();
                for (int i = 0; i < graphs.Count; i++)
                {
                    DrawGraph(groupAsset, graphs[i]);
                }
            });

            EditorGUILayoutExtension.VerticalGroup(() =>
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

        public void DrawGraph(InternalGraphGroupAsset groupAsset,InternalBaseGraphAsset graphAsset)
        {
            EditorGUILayoutExtension.HorizontalGroup(() => {
                EditorGUILayout.LabelField(graphAsset.name, GUILayout.Width(150));

                if (GUILayout.Button("打开", GUILayout.Width(50)))
                    BaseGraphWindow.Open(graphAsset);

                if (GUILayout.Button("删除", GUILayout.Width(50)))
                    groupAsset.RemoveGraph(graphAsset);

                if (GUILayout.Button("重命名", GUILayout.Width(50)))
                {
                    MiscHelper.Input($"输入{groupAsset.DisplayName}名：", (string name) =>
                    {
                        graphAsset.name = name;
                        EditorUtility.SetDirty(graphAsset);
                        EditorUtility.SetDirty(target);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    });
                }
            });
        }
    }
}
