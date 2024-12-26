using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using IAToolkit.Command;
using System.Reflection;
using IAToolkit.ViewModel;

namespace IANodeGraph.Model
{
    /// <summary>
    /// 添加节点命令
    /// </summary>
    public class AddNodeCommand : ICommand
    {
        BaseGraphVM graph;
        BaseNodeVM node;
        
        public AddNodeCommand(BaseGraphVM graph, BaseNodeVM node)
        {
            this.graph = graph;
            this.node = node;
        }
        
        public AddNodeCommand(BaseGraphVM graph, BaseNode node)
        {
            this.graph = graph;
            this.node  = ViewModelFactory.CreateViewModel(node) as BaseNodeVM;
        }

        public void Do()
        {
            graph.AddNode(node);
        }

        public void Undo()
        {
            graph.RemoveNode(node);
        }
    }

    /// <summary>
    /// 删除节点命令
    /// </summary>
    public class RemoveNodeCommand : ICommand
    {
        BaseGraphVM graph;
        BaseNodeVM node;

        List<BaseConnectionVM> connections = new List<BaseConnectionVM>();
        public RemoveNodeCommand(BaseGraphVM graph, BaseNodeVM node)
        {
            this.graph = graph;
            this.node = node;
        }

        public void Do()
        {
            foreach (var edge in graph.Connections.ToArray())
            {
                if (edge.FromNode == node || edge.ToNode == node)
                {
                    connections.Add(edge);
                }
            }
            graph.RemoveNode(node);
        }

        public void Undo()
        {
            graph.AddNode(node);
            foreach (var edge in connections)
            {
                graph.Connect(edge);
            }
            connections.Clear();
        }
    }

    #region Group

    public class AddGroupCommand : ICommand
    {
        public BaseGraphVM graph;
        public BaseGroupVM group;

        public AddGroupCommand(BaseGraphVM graph, BaseGroupVM group)
        {
            this.graph = graph;
            this.group = group;
        }

        public void Do()
        {
            graph.AddGroup(group);
        }

        public void Undo()
        {
            graph.RemoveGroup(group);
        }
    }

    public class RemoveGroupCommand : ICommand
    {
        public BaseGraphVM graph;
        public BaseGroupVM group;

        public RemoveGroupCommand(BaseGraphVM graph, BaseGroupVM group)
        {
            this.graph = graph;
            this.group = group;
        }

        public void Do()
        {
            graph.RemoveGroup(group);
        }

        public void Undo()
        {
            graph.AddGroup(group);
        }
    }

    public class MoveGroupsCommand : ICommand
    {
        Dictionary<BaseGroupVM, Vector2> oldPos = new Dictionary<BaseGroupVM, Vector2>();
        Dictionary<BaseGroupVM, Vector2> newPos = new Dictionary<BaseGroupVM, Vector2>();

        public MoveGroupsCommand(Dictionary<BaseGroupVM, Vector2> groups)
        {
            this.newPos = groups;
            foreach (var pair in groups)
            {
                oldPos[pair.Key] = pair.Key.Position;
            }
        }

        public void Do()
        {
            foreach (var pair in newPos)
            {
                pair.Key.Position = pair.Value;
            }
        }

        public void Undo()
        {
            foreach (var pair in oldPos)
            {
                pair.Key.Position = pair.Value;
            }
        }
    }

    public class RenameGroupCommand : ICommand
    {
        public BaseGroupVM group;
        public string oldName;
        public string newName;

        public RenameGroupCommand(BaseGroupVM group, string newName)
        {
            this.group = group;
            this.oldName = group.GroupName;
            this.newName = newName;
        }

        public void Do()
        {
            group.GroupName = newName;
        }

        public void Undo()
        {
            group.GroupName = oldName;
        }
    }

    #endregion

    public class AddPortCommand : ICommand
    {
        BaseNodeVM node;
        BasePortVM port;
        bool successed = false;

        public AddPortCommand(BaseNodeVM node, string name, BasePort.Orientation orientation, BasePort.Direction direction, BasePort.Capacity capacity, Type type = null)
        {
            this.node = node;
            port = new BasePortVM(name, orientation, direction, capacity, type);
        }

        public void Do()
        {
            successed = false;
            if (!node.Ports.ContainsKey(port.Model.name))
            {
                node.AddPort(port);
                successed = true;
            }
        }

        public void Undo()
        {
            if (!successed)
            {
                return;
            }
            node.RemovePort(port);
        }
    }

    /// <summary>
    /// 删除端口命令
    /// </summary>
    public class RemovePortCommand : ICommand
    {
        BaseNodeVM node;
        BasePortVM port;
        bool successed = false;

        public RemovePortCommand(BaseNodeVM node, BasePortVM port)
        {
            this.node = node;
            this.port = port;
        }

        public RemovePortCommand(BaseNodeVM node, string name)
        {
            this.node = node;
            node.Ports.TryGetValue(name, out port);
        }

        public void Do()
        {
            successed = false;
            if (node.Ports.ContainsKey(port.Model.name))
            {
                node.AddPort(port);
                successed = true;
            }
        }

        public void Undo()
        {
            if (!successed)
            {
                return;
            }
            node.RemovePort(port);
        }
    }

    /// <summary>
    /// 连接命令
    /// </summary>
    public class ConnectCommand : ICommand
    {
        private readonly BaseGraphVM graph;
        private readonly BaseNodeVM from;
        private readonly string fromPortName;
        private readonly BaseNodeVM to;
        private readonly string toPortName;

        BaseConnectionVM connection;
        HashSet<BaseConnectionVM> replacedConnections = new HashSet<BaseConnectionVM>();

        public ConnectCommand(BaseGraphVM graph, BaseNodeVM from, string fromPortName, BaseNodeVM to, string toPortName)
        {
            this.graph = graph;
            this.from = from;
            this.fromPortName = fromPortName;
            this.to = to;
            this.toPortName = toPortName;
        }

        public ConnectCommand(BaseGraphVM graph, BaseConnectionVM connection)
        {
            this.graph = graph;
            this.connection = connection;
            this.from = connection.FromNode;
            this.fromPortName = connection.FromPortName;
            this.to = connection.ToNode;
            this.toPortName = connection.ToPortName;
        }

        public void Do()
        {
            replacedConnections.Clear();
            if (from.Ports[fromPortName].Model.capacity == BasePort.Capacity.Single)
            {
                foreach (var connection in from.Ports[fromPortName].Connections)
                {
                    replacedConnections.Add(connection);
                }
            }
            if (to.Ports[toPortName].Model.capacity == BasePort.Capacity.Single)
            {
                foreach (var connection in to.Ports[toPortName].Connections)
                {
                    replacedConnections.Add(connection);
                }
            }

            if (connection == null)
            {
                connection = graph.Connect(from, fromPortName, to, toPortName);
            }
            else
            {
                graph.Connect(connection);
            }
        }

        public void Undo()
        {
            graph.Disconnect(connection);

            // 还原
            foreach (var connection in replacedConnections)
            {
                graph.Connect(connection);
            }
        }
    }

    /// <summary>
    /// 连接转向命令
    /// </summary>
    public class ConnectionRedirectCommand : ICommand
    {
        BaseGraphVM graph;
        BaseConnectionVM connection;

        BaseNodeVM oldFrom, oldTo;
        string oldFromPortName, oldToPortName;

        BaseNodeVM newFrom, newTo;
        string newFromPortName, newToPortName;

        List<BaseConnectionVM> replacedConnections = new List<BaseConnectionVM>();

        public ConnectionRedirectCommand(BaseGraphVM graph, BaseConnectionVM connection, BaseNodeVM from, string fromPortName, BaseNodeVM to, string toPortName)
        {
            this.graph = graph;
            this.connection = connection;

            newFrom = from;
            newFromPortName = fromPortName;
            newTo = to;
            newToPortName = toPortName;
        }

        public void Do()
        {
            oldFrom = connection.FromNode;
            oldFromPortName = connection.FromPortName;
            oldTo = connection.ToNode;
            oldToPortName = connection.ToPortName;

            replacedConnections.Clear();
            if (connection.FromNodeGUID == newFrom.GUID && connection.FromPortName == newFromPortName)
            {
                if (newTo.Ports[newToPortName].Model.capacity == BasePort.Capacity.Single)
                    replacedConnections.AddRange(newTo.Ports[newToPortName].Connections);
            }
            else
            {
                if (newFrom.Ports[newFromPortName].Model.capacity == BasePort.Capacity.Single)
                    replacedConnections.AddRange(newFrom.Ports[newFromPortName].Connections);
            }

            connection.Redirect(newFrom, newFromPortName, newTo, newToPortName);
            graph.Connect(connection);
        }

        public void Undo()
        {
            graph.Disconnect(connection);
            connection.Redirect(oldFrom, oldFromPortName, oldTo, oldToPortName);
            graph.Connect(connection);

            // 还原
            foreach (var connection in replacedConnections)
            {
                graph.Connect(connection);
            }
        }
    }

    /// <summary>
    /// 断开连接命令
    /// </summary>
    public class DisconnectCommand : ICommand
    {
        BaseGraphVM graph;

        BaseConnectionVM connection;

        public DisconnectCommand(BaseGraphVM graph, BaseConnectionVM connection)
        {
            this.graph = graph;
            this.connection = connection;
        }

        public void Do()
        {
            graph.Disconnect(connection);
        }

        public void Undo()
        {
            graph.Connect(connection);
        }
    }

    /// <summary>
    /// 移动节点命令
    /// </summary>
    public class MoveNodeCommand : ICommand
    {
        BaseNodeVM node;
        Vector2 currentPosition;
        Vector2 targetPosition;

        public MoveNodeCommand(BaseNodeVM node, Vector2 position)
        {
            this.node = node;
            currentPosition = node.Position;
            targetPosition = position;
        }

        public void Do()
        {
            node.Position = targetPosition;
        }

        public void Undo()
        {
            node.Position = currentPosition;
        }
    }

    /// <summary>
    /// 移动一片节点命令
    /// </summary>
    public class MoveNodesCommand : ICommand
    {
        Dictionary<BaseNodeVM, Vector2> oldPos = new Dictionary<BaseNodeVM, Vector2>();
        Dictionary<BaseNodeVM, Vector2> newPos = new Dictionary<BaseNodeVM, Vector2>();

        public MoveNodesCommand(Dictionary<BaseNodeVM, Vector2> newPos)
        {
            this.newPos = newPos;
        }

        public void Do()
        {
            foreach (var pair in newPos)
            {
                oldPos[pair.Key] = pair.Key.Position;
                pair.Key.Position = pair.Value;
            }
        }

        public void Undo()
        {
            foreach (var pair in oldPos)
            {
                pair.Key.Position = pair.Value;
            }
        }
    }

    /// <summary>
    /// 改变节点值命令
    /// </summary>
    public class ChangeNodeValueCommand : ICommand
    {
        object target;
        FieldInfo field;
        object oldValue, newValue;

        Action OnDoFunc;
        Action OnUndoFunc;

        public ChangeNodeValueCommand(object target, FieldInfo field, object newValue, Action OnDoFunc = null, Action OnUndoFunc = null)
        {
            this.target = target;
            this.field = field;
            this.newValue = newValue;
            this.OnDoFunc = OnDoFunc;   
            this.OnUndoFunc = OnUndoFunc;
        }

        public void Do()
        {
            oldValue = field.GetValue(target);
            field.SetValue(target, newValue);
            OnDoFunc?.Invoke();
        }

        public void Undo()
        {
            field.SetValue(target, oldValue);
            OnUndoFunc?.Invoke();
        }
    }

    /// <summary>
    /// 绑定值改变命令
    /// </summary>
    public class BindableChangeValueCommand : ICommand
    {
        IBindableProperty property;
        object oldValue, newValue;

        public BindableChangeValueCommand(IBindableProperty property, object newValue)
        {
            this.property = property;
            this.newValue = newValue;
            this.oldValue = property.ValueBoxed;
        }

        public void Do()
        {
            property.ValueBoxed = newValue;
        }

        public void Undo()
        {
            property.ValueBoxed = oldValue;
        }
    }

    /// <summary>
    /// 值改变命令
    /// </summary>
    public class ChangeValueCommand : ICommand
    {
        object oldValue, newValue;
        Action<object> OnDoFunc;
        Action<object> OnUndoFunc;

        public ChangeValueCommand(object oldValue, object newValue, Action<object> onDoFunc, Action<object> onUndoFunc)
        {
            this.newValue = newValue;
            this.oldValue = oldValue;
            this.OnDoFunc = onDoFunc;
            this.OnUndoFunc = onUndoFunc;
        }

        public void Do()
        {
            OnDoFunc?.Invoke(newValue);
        }

        public void Undo()
        {
            OnUndoFunc?.Invoke(oldValue);
        }
    }
    
    
}
