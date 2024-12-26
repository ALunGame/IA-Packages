using IANodeGraph.Model.Internal;
using IAToolkit;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IANodeGraph.View
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
            GUILayoutExtension.HorizontalGroup(() =>
            {
                GUILayoutExtension.VerticalGroup(() =>
                {
                    MiscHelper.Btn("刷新", 200, 35, () =>
                    {
                        Refresh();
                    });

                    foreach (var item in groupDict)
                    {
                        GUIExtension.SetColor(item.Value == selGroup ? Color.green : Color.white, () =>
                        {
                            MiscHelper.Btn(item.Value.name, 200, 35, () =>
                            {
                                OnSelGroup(item.Value);
                            });
                        });
                    }
                });

                GUILayoutExtension.VerticalGroup(() =>
                {
                    for (int i = 0; i < graphs.Count; i++)
                    {
                        MiscHelper.Btn(graphs[i].name, 200, 35, () =>
                        {
                            BaseGraphWindow.Open(graphs[i]);
                        });
                    }
                });
            });
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
