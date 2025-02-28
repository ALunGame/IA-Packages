using IANodeGraph.Model.Internal;
using IANodeGraph.View;
using IAToolkit;
using IAToolkit.UnityEditors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IANodeGraph.Window
{
    public class GraphListWindow : BaseEditorWindow
    {
        public static void Open()
        {
            var window = GetWindow<GraphListWindow>();
            window.titleContent = new GUIContent("视图列表");
            window.Init();
        }

        private Dictionary<Type, InternalGraphGroupAsset> groupDict = new Dictionary<Type, InternalGraphGroupAsset>();
        private InternalGraphGroupAsset selGroup;
        private List<InternalBaseGraphAsset> graphs = new List<InternalBaseGraphAsset>();

        public void Init()
        {
            groupDict = GraphSetting.GetGroups();
        }

        private void OnGUI()
        {
            EditorGUILayoutExtension.BeginHorizontalBoxGroup();

            EditorGUILayoutExtension.BeginVerticalBoxGroup();
            MiscHelper.Btn("刷新", 200, 35, () =>
            {
                Refresh();
            });

            foreach (var item in groupDict)
            {
                MiscHelper.Btn(item.Value.name, 200, 35, () =>
                {
                    OnSelGroup(item.Value);
                });
            }
            EditorGUILayoutExtension.EndVerticalBoxGroup();

            EditorGUILayoutExtension.BeginVerticalBoxGroup();
            for (int i = 0; i < graphs.Count; i++)
            {
                MiscHelper.Btn(graphs[i].name, 200, 35, () =>
                {
                    BaseGraphWindow.Open(graphs[i]);
                });
            }
            EditorGUILayoutExtension.EndVerticalBoxGroup();

            EditorGUILayoutExtension.EndHorizontalBoxGroup();
        }

        private void OnSelGroup(InternalGraphGroupAsset group)
        {
            selGroup = group;
            graphs = selGroup.GetAllGraph();
        }

        private void Refresh()
        {
            Init();
            selGroup = null;
        }
    }
}
