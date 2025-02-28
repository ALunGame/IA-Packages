using UnityEngine;
using UnityEditor.Experimental.GraphView;
using IANodeGraph.View;
using IANodeGraph.Model;
using UnityEditor;

namespace IANodeGraph.Editors
{
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        private BaseGraphView graphView;

        public EdgeConnectorListener(BaseGraphView graphView)
        {
            this.graphView = graphView;
        }

        /// <summary> 拖拽到符合条件的接口上松开时触发 </summary>
        public virtual void OnDrop(GraphView graphView, Edge edge)
        {
            BaseGraphView tempGraphView = graphView as BaseGraphView;

            BasePortProcessor from = (edge.output as BasePortView).ViewModel;
            BasePortProcessor to = (edge.input as BasePortView).ViewModel;
            // 如果连线不是一个新建的连线就重定向
            if (edge.userData is BaseConnectionProcessor)
                tempGraphView.CommandDispatcher.Do(new ConnectCommand(tempGraphView.ViewModel, from, to));
            else
                tempGraphView.CommandDispatcher.Do(new ConnectCommand(tempGraphView.ViewModel, from, to));
        }

        /// <summary> 拖到空白松开时触发 </summary>
        public void OnDropOutsidePort(Edge edge, Vector2 position) 
        {
            BaseConnectionView connectionView = edge as BaseConnectionView;
            if (!edge.isGhostEdge)
            {
                if (connectionView.ViewModel != null)
                {
                    graphView.ViewModel.Disconnect(connectionView.ViewModel);
                }
            }

            ShowNodeCreationMenuFromEdge(connectionView, position);
        }

        private void ShowNodeCreationMenuFromEdge(BaseConnectionView connectionView, Vector2 position)
        {
            graphView.NodeMenuWindow.ConnectionFilter = connectionView;
            graphView.ShowNodeMenuWindow(position + EditorWindow.focusedWindow.position.position);
        }
    }
}