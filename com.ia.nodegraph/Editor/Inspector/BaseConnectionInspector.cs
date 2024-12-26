using IANodeGraph.View;
using IANodeGraph.View.Utils;
using IAToolkit;
using IAToolkit.Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IANodeGraph.Inspector
{
    /// <summary>
    /// 连接Inspector展示
    /// </summary>
    [CustomInspectorDrawer(typeof(BaseConnectionView))]
    public class BaseConnectionInspector : ObjectInspectorDrawer
    {

        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();

        protected HashSet<string> ignoreProperties;
        protected HashSet<string> IgnoreProperties
        {
            get
            {
                if (ignoreProperties == null)
                    ignoreProperties = new HashSet<string>(BuildIgnoreProperties());
                return ignoreProperties;
            }
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
            GUILayoutExtension.VerticalGroup(() =>
            {
                GUILayout.Label("Connection", bigLabel.value);
            });

            if (Target is BaseConnectionView view && view.Model != null)
            {
                GUILayoutExtension.VerticalGroup(() => {
                    GUILayout.Label(string.Concat(view.output?.node.title, "：", view.Model.FromPortName, "  >>  ", view.input?.node.title, "：", view.Model.ToPortName), bigLabel.value);
                });

                EditorGUI.BeginChangeCheck();
                GUILayoutExtension.VerticalGroup(() => {
                    foreach (var property in view.Model)
                    {
                        if (IgnoreProperties.Contains(property.Key)) continue;

                        object newValue = GUILayoutExtension.DrawField(property.Value.ValueType, property.Value.ValueBoxed, GraphProcessorEditorUtility.GetDisplayName(property.Key), property.Value.ValueTooltip);
                        if (!newValue.Equals(property.Value.ValueBoxed))
                            property.Value.ValueBoxed = newValue;

                    }
                });

                if (EditorGUI.EndChangeCheck())
                {

                }
            }
        }

        public virtual IEnumerable<string> BuildIgnoreProperties()
        {
            yield break;
        }
    }
}
