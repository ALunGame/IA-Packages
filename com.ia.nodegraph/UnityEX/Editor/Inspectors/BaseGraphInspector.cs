using IANodeGraph.View;
using IAToolkit;
using IAToolkit.UnityEditors;
using UnityEditor;
using UnityEngine;

namespace IANodeGraph.Inspector
{
    [CustomObjectEditor(typeof(BaseGraphView))]
    public class BaseGraphInspector : ObjectEditor
    {
        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();

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

            EditorGUILayoutExtension.VerticalGroup(() =>
            {
                GUILayout.Label("Graph", bigLabel.value);
            });

            if (Target is BaseGraphView view && view.ViewModel != null)
            {
                EditorGUILayoutExtension.VerticalGroup(() =>
                {
                    GUILayout.Label(string.Concat("Nodes：", view.ViewModel.Nodes.Count), bigLabel.value);
                    GUILayout.Label(string.Concat("Connections：", view.ViewModel.Connections.Count), bigLabel.value);
                });
            }
        }
    }
}