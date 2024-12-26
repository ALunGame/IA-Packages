using IAECS.Core;
using IANodeGraph.Inspector;
using IAToolkit.Core;
using System.Reflection;
using System.Collections.Generic;
using IAToolkit;
using System;
using UnityEngine;
using IANodeGraph.View.Utils;
using UnityEditor;
using IAEngine;

namespace IAECS.EntityGraph
{
    [CustomInspectorDrawer(typeof(EntityComNodeView))]
    public class EntityComNodeInspector : BaseNodeInspector
    {
        private List<string> IgnoreFieldName = new List<string>()
        {
            "IsActive",
            "entityId",
            "entityCnfId",
            "EntityCnfId",
            "EntityId",
        };


        private bool _serFoldout = false;
        private bool _noSerFoldout = false;

        public override void OnEnable()
        {
            base.OnEnable();
            _serFoldout = true;
        }

        public override void OnInspectorGUI()
        {
            EntityComNodeView comNodeView = Target as EntityComNodeView;
            EntityGraphVM graph = comNodeView.Owner.Model as EntityGraphVM;
            if (graph.RunningTimeEntity == null)
            {
                base.OnInspectorGUI();
            }
            else
            {
                DrawRunningTimeEntity(graph.RunningTimeEntity);
            }
        }

        private void DrawRunningTimeEntity(Entity entity)
        {
            EntityComNodeView comNodeView = Target as EntityComNodeView;
            Entity_ComNode comNode = comNodeView.Model.Model as Entity_ComNode;
            foreach (var item in entity.GetComs())
            {
                if (item.GetType() == comNode.RuntimeNode)
                {
                    DrawCom(item);
                    return;
                }
            }
        }

        private void DrawCom(BaseCom baseCom)
        {
            if (!ContextDataCache.TryGetContextData<GUIStyle>("BigLabel", out var bigLabel))
            {
                bigLabel.value = new GUIStyle(GUI.skin.label);
                bigLabel.value.fontSize = 18;
                bigLabel.value.fontStyle = FontStyle.Bold;
                bigLabel.value.alignment = TextAnchor.MiddleLeft;
                bigLabel.value.stretchWidth = true;
            }

            List<FieldInfo> serFields = new List<FieldInfo>();
            List<FieldInfo> noSerFields = new List<FieldInfo>();
            foreach (var item in ReflectionHelper.GetFieldInfos(baseCom.GetType()))
            {
                bool ignore = false;
                foreach (var ignoreName in IgnoreFieldName)
                {
                    if (item.Name.Contains(ignoreName))
                    {
                        ignore = true;
                        continue;
                    }
                }
                if (ignore)
                    continue;

                if (AttributeHelper.TryGetFieldAttribute(item,out NonSerializedAttribute attr))
                {
                    noSerFields.Add(item);
                }
                else
                {
                    serFields.Add(item);
                }
            }

            _serFoldout = EditorGUILayout.Foldout(_serFoldout, "Serialized：");
            if (_serFoldout)
            {
                foreach (var item in serFields)
                {
                    object newValue = GUILayoutExtension.DrawField(item.FieldType, item.GetValue(baseCom), GraphProcessorEditorUtility.GetDisplayName(item.Name), "");
                    if (newValue == null || !newValue.Equals(item.GetValue(baseCom)))
                    {
                        item.SetValue(baseCom, newValue);
                    }
                }
            }

            _noSerFoldout = EditorGUILayout.Foldout(_noSerFoldout, "NoSerialized：");
            if (_noSerFoldout)
            {
                EditorGUI.BeginDisabledGroup(true);
                foreach (var item in noSerFields)
                {
                    object value = item.GetValue(baseCom);
                    if (value == null)
                    {
                        GUILayoutExtension.DrawField(item.FieldType, item.GetValue(baseCom), GraphProcessorEditorUtility.GetDisplayName(item.Name), "");
                    }
                    else
                    {
                        GUILayoutExtension.DrawField(value.GetType(), item.GetValue(baseCom), GraphProcessorEditorUtility.GetDisplayName(item.Name), "");
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
        }
    }
}