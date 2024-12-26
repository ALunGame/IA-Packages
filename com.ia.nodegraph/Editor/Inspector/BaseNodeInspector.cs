using IANodeGraph.Model;
using IANodeGraph.View;
using IANodeGraph.View.Utils;
using IAToolkit;
using IAToolkit.Core;
using System.Collections.Generic;
using UnityEngine;

namespace IANodeGraph.Inspector
{
    [CustomInspectorDrawer(typeof(BaseNodeView))]
    public class BaseNodeInspector : ObjectInspectorDrawer
    {
        public static HashSet<string> IgnoreProperties = new HashSet<string>() {
        };

        protected static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();

        public override void OnEnable()
        {
            base.OnEnable();
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

            if (Target is BaseNodeView view && view.Model != null)
            {
                
                GUILayoutExtension.VerticalGroup(() =>
                {
                    bigLabel.value.alignment = TextAnchor.MiddleLeft;
                    GUILayout.Label(string.Concat("Node：", view.Model.GUID), bigLabel.value);
                });

                GUILayoutExtension.VerticalGroup(() => {
                    foreach (var property in view.Model)
                    {
                        if (IgnoreProperties.Contains(property.Key)) continue;

                        object newValue = GUILayoutExtension.DrawField(property.Value.ValueType, property.Value.ValueBoxed, GraphProcessorEditorUtility.GetDisplayName(property.Key), property.Value.ValueTooltip);
                        if (newValue == null || !newValue.Equals(property.Value.ValueBoxed))
                        {
                            view.Owner.CommandDispacter.Do(new BindableChangeValueCommand(property.Value, newValue));
                            //property.Value.ValueBoxed = newValue;
                        }
                    }
                });
            }
        }
    }
}