using IANodeGraph.Model;
using IANodeGraph.View;
using IANodeGraph.View.Utils;
using IAToolkit;
using IAToolkit.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IANodeGraph.Inspector
{
    [CustomInspectorDrawer(typeof(BaseGraphView))]
    public class BaseGraphInspector : ObjectInspectorDrawer
    {
        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();
        static HashSet<string> IgnoreProperties = new HashSet<string>()
        {
            BaseGraphVM.POS_NAME,
            BaseGraphVM.ZOOM_NAME
        };

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

            GUILayoutExtension.VerticalGroup(() =>
            {
                GUILayout.Label("Graph", bigLabel.value);
            });

            if (Target is BaseGraphView view && view.Model != null)
            {
                GUILayoutExtension.VerticalGroup(() =>
                {
                    GUILayout.Label(string.Concat("Nodes：", view.Model.Nodes.Count), bigLabel.value);
                    GUILayout.Label(string.Concat("Connections：", view.Model.Connections.Count), bigLabel.value);
                });

                EditorGUI.BeginChangeCheck();
                GUILayoutExtension.VerticalGroup(() => {
                    foreach (var property in view.Model)
                    {
                        if (IgnoreProperties.Contains(property.Key)) continue;

                        object newValue = GUILayoutExtension.DrawField(property.Value.ValueType, property.Value.ValueBoxed, GraphProcessorEditorUtility.GetDisplayName(property.Key), property.Value.ValueTooltip);
                        if (newValue == null || !newValue.Equals(property.Value.ValueBoxed))
                        {
                            view.CommandDispacter.Do(new BindableChangeValueCommand(property.Value, newValue));
                            //property.Value.ValueBoxed = newValue;
                        }
                    }
                });
                if (EditorGUI.EndChangeCheck())
                {

                }
            }
        }
    }
}