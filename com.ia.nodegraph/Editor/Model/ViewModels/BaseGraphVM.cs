using IAToolkit;
using IAToolkit.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using IAEngine;
using UnityEngine;

namespace IANodeGraph.Model
{
    [NodeViewModelAttribute(typeof(BaseGraph))]
    public class BaseGraphVM : ViewModel
    {
        /// <summary>
        /// 节点命名空间限制
        /// </summary>
        public virtual List<string> NodeNamespace => null;

        #region Fields
        public event Action<BaseNodeVM> onNodeAdded;
        public event Action<BaseNodeVM> onNodeRemoved;

        public event Action<BaseConnectionVM> onConnected;
        public event Action<BaseConnectionVM> onDisconnected;
        
        public event Action<BaseGroupVM> OnGroupAdded;
        public event Action<BaseGroupVM> OnGroupRemoved;
        
        private Dictionary<string, BaseNodeVM> nodes = new Dictionary<string, BaseNodeVM>();
        private List<BaseConnectionVM> connections = new List<BaseConnectionVM>();
        private List<BaseGroupVM> groups = new List<BaseGroupVM>();
        //private Blackboard<string> blackboard = new Blackboard<string>();
        
        #endregion

        #region Properties
        
        public BaseGraph Model { get; }
        public Type ModelType { get; }
        public Vector3 Pos
        {
            get { return GetPropertyValue<Vector3>(POS_NAME); }
            set { SetPropertyValue(POS_NAME, value); }
        }
        public Vector3 Zoom
        {
            get { return GetPropertyValue<Vector3>(ZOOM_NAME); }
            set { SetPropertyValue(ZOOM_NAME, value); }
        }
        public IReadOnlyDictionary<string, BaseNodeVM> Nodes
        {
            get { return nodes; }
        }
        public IReadOnlyList<BaseGroupVM> Groups
        {
            get { return groups; }
        }
        public IReadOnlyList<BaseConnectionVM> Connections
        {
            get { return connections; }
        }
        #endregion

        public BaseGraphVM(BaseGraph model)
        {
            Model = model;
            ModelType = model.GetType();
        }

        public void Enable()
        {
            this[POS_NAME] = new BindableProperty<Vector3>(() => Model.pos, v => Model.pos = v);
            this[ZOOM_NAME] = new BindableProperty<Vector3>(() => Model.zoom, v => Model.zoom = v);
            
            //节点
            foreach (var pair in Model.nodes)
            {
                if (pair.Value == null)
                    continue;
                var nodeVM = ViewModelFactory.CreateViewModel(pair.Value) as BaseNodeVM;
                nodes.Add(pair.Key, nodeVM);
                nodeVM.Enable(this);
            }
            
            //连接
            for (int i = 0; i < Model.connections.Count; i++)
            {
                var connection = Model.connections[i];

                if (!nodes.TryGetValue(connection.from, out var fromNode) || !fromNode.Ports.TryGetValue(connection.fromPortName, out var fromPort))
                {
                    Model.connections.RemoveAt(i--);
                    continue;
                }

                if (!nodes.TryGetValue(connection.to, out var toNode) || !toNode.Ports.TryGetValue(connection.toPortName, out var toPort))
                {
                    Model.connections.RemoveAt(i--);
                    continue;
                }

                var connectionVM = ViewModelFactory.CreateViewModel(connection) as BaseConnectionVM;
                connectionVM.Enable(this);
                connections.Add(connectionVM);
                
                fromPort.ConnectTo(connectionVM);
                toPort.ConnectTo(connectionVM);
            }
            
            //组
            for (int i = 0; i < Model.groups.Count; i++)
            {
                var group = Model.groups[i];
                if (group == null)
                {
                    Model.groups.RemoveAt(i--);
                    continue;
                }

                group.nodes.RemoveAll(nodeID => !nodes.ContainsKey(nodeID));
                var groupVM = ViewModelFactory.CreateViewModel(group) as BaseGroupVM;
                groups.Add(groupVM);
                groupVM.Enable(this);
            }
            
            OnEnabled();
        }

        #region API

        //生成唯一Id
        public string GenerateNodeGUID()
        {
            while (true)
            {
                string guid = Guid.NewGuid().ToString();
                if (!nodes.ContainsKey(guid)) return guid;
            }
        }

        //添加节点
        public void AddNode(BaseNodeVM node)
        {
            if (node.Owner != null && node.Owner != this)
                throw new Exception("节点存在其它Graph中");
            if (node.ContainsKey(node.GUID))
                throw new Exception("节点添加失败，GUID重复");

            nodes.Add(node.GUID, node);
            Model.nodes.Add(node.GUID, node.Model);
            node.Enable(this);
            onNodeAdded?.Invoke(node);
        }

        //添加节点
        public BaseNodeVM AddNode<T>(Vector2 position) where T : BaseNode, new()
        {
            var node = BaseNodeVM.CreateNew(this, typeof(T),position);
            AddNode(node);
            return node;
        }

        //添加节点
        public BaseNodeVM AddNode(Type type, Vector2 position)
        {
            var node = BaseNodeVM.CreateNew(this, type,position);
            AddNode(node);
            return node;
        }

        //移除节点
        public void RemoveNode(BaseNodeVM node)
        {
            if (node == null)
                throw new NullReferenceException("节点不能为空");
            Disconnect(node);
            nodes.Remove(node.GUID);
            Model.nodes.Remove(node.GUID);
            node.Disable();
            onNodeRemoved?.Invoke(node);
        }

        //连接
        public bool Connect(BaseConnectionVM connection)
        {
            if (!Nodes.TryGetValue(connection.FromNodeGUID, out var fromNode))
                return false;
            if (!fromNode.Ports.TryGetValue(connection.FromPortName, out var fromPort))
                return false;

            if (!Nodes.TryGetValue(connection.ToNodeGUID, out var toNode))
                return false;
            if (!toNode.Ports.TryGetValue(connection.ToPortName, out var toPort))
                return false;

            var tmpConnection = fromPort.Connections.FirstOrDefault(tmp => tmp.ToNodeGUID == connection.ToNodeGUID && tmp.ToPortName == connection.ToPortName);
            if (tmpConnection != null)
                return false;

            if (fromPort.Model.capacity == BasePort.Capacity.Single)
                Disconnect(fromPort);
            if (toPort.Model.capacity == BasePort.Capacity.Single)
                Disconnect(toPort);

            connection.Enable(this);
            connections.Add(connection);
            Model.connections.Add(connection.Model);

            fromPort.ConnectTo(connection);
            toPort.ConnectTo(connection);

            fromPort.Resort();
            toPort.Resort();

            onConnected?.Invoke(connection);
            return true;
        }

        //连接
        public BaseConnectionVM Connect(BaseNodeVM from, string fromPortName, BaseNodeVM to, string toPortName)
        {
            var connection = NewConnection(from, fromPortName, to, toPortName);
            if (!Connect(connection))
                return null;
            return connection;
        }

        //断开连接
        public void Disconnect(BaseNodeVM node)
        {
            foreach (var connection in Connections.ToArray())
            {
                if (connection.FromNodeGUID == node.GUID || connection.ToNodeGUID == node.GUID)
                    Disconnect(connection);
            }
        }

        //断开连接
        public void Disconnect(BaseConnectionVM connection)
        {
            if (!connections.Contains(connection)) return;

            connection.FromNode.Ports.TryGetValue(connection.FromPortName, out BasePortVM fromPort);
            fromPort.DisconnectTo(connection);
            fromPort.Resort();

            connection.ToNode.Ports.TryGetValue(connection.ToPortName, out BasePortVM toPort);
            toPort.DisconnectTo(connection);
            toPort.Resort();

            connections.Remove(connection);
            Model.connections.Remove(connection.Model);
            onDisconnected?.Invoke(connection);
        }

        //断开连接
        public void Disconnect(BasePortVM port)
        {
            if (port.Owner == null || !Model.nodes.ContainsKey(port.Owner.GUID))
                return;
            foreach (var connection in port.Connections.ToArray())
            {
                Disconnect(connection);
            }
        }

        //断开连接
        public void Disconnect(BaseNodeVM node, string portName)
        {
            Disconnect(node.Ports[portName]);
        }
        
        public void AddGroup(BaseGroupVM group)
        {
            groups.Add(group);
            Model.groups.Add(group.Model);
            group.Enable(this);
            OnGroupAdded?.Invoke(group);
        }
        
        public void RemoveGroup(BaseGroupVM group)
        {
            bool removed = groups.Remove(group);
            groups.Remove(group);
            if (removed)
                OnGroupRemoved?.Invoke(group);
        }
        
        #endregion

        #region Overrides
        protected virtual void OnEnabled() { }

        protected virtual void OnInitialized() { }

        public T NewNode<T>(Vector2 position) where T : BaseNode
        {
            return NewNode(typeof(T), position) as T;
        }

        public virtual BaseNodeVM NewNode(Type type, Vector2 position)
        {
            return BaseNodeVM.CreateNew(this, type, position);
        }

        public virtual BaseConnectionVM NewConnection(BaseNodeVM from, string fromPortName, BaseNodeVM to, string toPortName)
        {
            return BaseConnectionVM.CreateNew(from, fromPortName, to, toPortName);
        }

        public virtual IEnumerable<Type> GetNodeTypes()
        {
            foreach (var type in ReflectionHelper.GetChildTypes<BaseNode>())
            {
                if (type.IsAbstract)
                    continue;
                if (!AttributeHelper.TryGetTypeAttribute(type, out NodeMenuItemAttribute attr))
                    continue;
                if (NodeNamespace != null && !NodeNamespace.Contains(type.Namespace))
                    continue;
                yield return type;
            }
        }

        #endregion

        #region Static

        public const string POS_NAME = "POS_NAME";
        public const string ZOOM_NAME = "ZOOM_NAME";
        #endregion
    }
}
