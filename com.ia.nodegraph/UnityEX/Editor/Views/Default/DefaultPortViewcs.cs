using IANodeGraph.Model;
using IANodeGraph.View;
using System;
using UnityEditor.Experimental.GraphView;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace IANodeGraph.Editors
{
    [CustomView(typeof(BasePort))]
    public class DefaultPortView : BasePortView
    {
        protected DefaultPortView(Orientation orientation, Direction direction, Capacity capacity, Type type, IEdgeConnectorListener connectorListener) : base(orientation, direction, capacity, type, connectorListener)
        {
        }

        public DefaultPortView(BasePortProcessor port, IEdgeConnectorListener connectorListener) : base(port, connectorListener)
        {
        }
    }
}