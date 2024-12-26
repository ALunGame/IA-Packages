using IANodeGraph.Model;
using IANodeGraph.View.Utils;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace IANodeGraph.View
{
    /// <summary>
    /// 端口显示
    /// </summary>
    public partial class BasePortView : Port
    {
        public Image Icon { get; }
        public BaseGraphView GraphView { get; private set; }
        public BasePortVM Model { get; private set; }
        public Dictionary<BaseConnectionVM, BaseConnectionView> ConnectionViews { get; private set; }

        protected BasePortView(Orientation orientation, Direction direction, Capacity capacity, Type type, IEdgeConnectorListener connectorListener) : base(orientation, direction, capacity, type)
        {
            styleSheets.Add(GraphProcessorStyles.PortViewStyle);

            Icon = new Image();
            Icon.AddToClassList("port-icon");
            Insert(1, Icon);

            visualClass = "Port_" + portType.Name;

            var portLabel = this.Q("type");
            portLabel.pickingMode = PickingMode.Position;
            portLabel.style.flexGrow = 1;
            bool vertical = orientation == Orientation.Vertical;
            if (vertical)
            {
                portLabel.style.display = DisplayStyle.None;
                this.Q("connector").pickingMode = PickingMode.Position;
                AddToClassList("vertical");
            }

            m_EdgeConnector = new EdgeConnector<BaseConnectionView>(connectorListener);
            ConnectionViews = new Dictionary<BaseConnectionVM, BaseConnectionView>();
            this.AddManipulator(m_EdgeConnector);
        }

        public void SetUp(BasePortVM port, BaseGraphView graphView)
        {
            Model = port;
            GraphView = graphView;

            portName = port.Model.name;
            tooltip = port.Model.name;
        }

        public virtual void Connect(BaseConnectionView connection)
        {
            base.Connect(connection);
            if (connection is BaseConnectionView connectionView)
            {
                ConnectionViews[connectionView.Model] = connectionView;
            }
        }

        public virtual void Disconnect(BaseConnectionView connection)
        {
            base.Disconnect(connection);
            if (connection is BaseConnectionView connectionView)
            {
                ConnectionViews.Remove(connectionView.Model);
            }
        }

        #region 不建议使用
        /// <summary>
        /// 不建议使用
        /// </summary>
        /// <param name="edge"></param>
        public sealed override void Connect(Edge edge)
        {
            base.Connect(edge);
        }

        /// <summary>
        /// 不建议使用
        /// </summary>
        /// <param name="edge"></param>
        public sealed override void Disconnect(Edge edge)
        {
            base.Connect(edge);
        }
        #endregion
    }
}
