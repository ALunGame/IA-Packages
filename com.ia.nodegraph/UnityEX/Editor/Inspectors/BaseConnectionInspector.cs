using IANodeGraph.View;
using IAToolkit;
using IAToolkit.UnityEditors;
using UnityEngine;

namespace IANodeGraph.Inspector
{
    /// <summary>
    /// 连接Inspector展示
    /// </summary>
    [CustomObjectEditor(typeof(BaseConnectionView))]
    public class BaseConnectionInspector : ObjectEditor
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
                GUILayout.Label("Connection", bigLabel.value);
            });

            if (Target is BaseConnectionView view && view.ViewModel != null)
            {
                EditorGUILayoutExtension.VerticalGroup(() => {
                    GUILayout.Label(string.Concat(view.output?.node.title, "：", view.ViewModel.FromPortName, "  >>  ", view.input?.node.title, "：", view.ViewModel.ToPortName), bigLabel.value);
                });
            }
        }
    }
}
