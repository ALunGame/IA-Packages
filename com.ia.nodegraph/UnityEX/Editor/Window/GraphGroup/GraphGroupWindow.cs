using IANodeGraph.Editors;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace IANodeGraph
{
    internal class GraphGroupWindow : EditorWindow
    {
        [MenuItem("Tools/Graph/分组")]
        public static void ShowExample()
        {
            GraphGroupWindow wnd = GetWindow<GraphGroupWindow>();
            wnd.minSize = new Vector2(1080, 720);
            wnd.titleContent = new GUIContent("Graph分组");
        }

        private VisualElement LeftPanel;

        private VisualElement RightPanel;
        private GraphGroupView GraphGroupView;

        public void CreateGUI()
        {
            GraphProcessorEditorStyles.LoadUXML("GraphGroupWindowTree").CloneTree(rootVisualElement);
            rootVisualElement.styleSheets.Add(GraphProcessorEditorStyles.LoadStyle("GraphGroupWindowStyle"));

            LeftPanel = rootVisualElement.Q<VisualElement>("LeftPanel");
            LeftPanel.Add(new GraphGroupListView(this));

            RightPanel = rootVisualElement.Q<VisualElement>("RightPanel");
        }

        internal void SelGraphGroup(string groupName, Type type)
        {
            if (GraphGroupView == null)
            {
                GraphGroupView = new GraphGroupView();
                RightPanel.Add(GraphGroupView);
            }

            GraphGroupView.SetUp(groupName, type);
        }
    }
}
