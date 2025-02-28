using IANodeGraph.Model;
using UnityEditor.Experimental.GraphView;
using NodeDirection = IANodeGraph.Model.PortDirection;
using NodeCapacity = IANodeGraph.Model.PortCapacity;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace IANodeGraph.View
{
    public abstract partial class BasePortView
    {
        public BasePortView(BasePortProcessor port, IEdgeConnectorListener connectorListener) : this(
            orientation: (port.Direction == NodeDirection.Left || port.Direction == NodeDirection.Right) ? Orientation.Horizontal : Orientation.Vertical,
            direction: (port.Direction == NodeDirection.Left || port.Direction == NodeDirection.Top) ? Direction.Input : Direction.Output,
            capacity: port.Capacity == NodeCapacity.Single ? Capacity.Single : Capacity.Multi,
            port.portType.Value, connectorListener)
        {
        }
    }
}
