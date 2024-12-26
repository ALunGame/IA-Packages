using IANodeGraph.Model;
using UnityEditor.Experimental.GraphView;

namespace IANodeGraph.View
{
    public partial class BasePortView
    {
        public BasePortView(BasePortVM port, IEdgeConnectorListener connectorListener) : this(
            orientation: port.Model.orientation == BasePort.Orientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical,
            direction: port.Model.direction == BasePort.Direction.Input ? Direction.Input : Direction.Output,
            capacity: port.Model.capacity == BasePort.Capacity.Single ? Capacity.Single : Capacity.Multi,
            port.Model.type, connectorListener)
        {

        }
    }
}
