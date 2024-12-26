using IAToolkit;
using IAToolkit.ViewModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using IAEngine;

namespace IANodeGraph.Model
{
    [NodeViewModel(typeof(BaseNode))]
    public class BaseNodeVM : ViewModel, IGraphElement
    {
        #region Fields
        [NonSerialized] Dictionary<string, BasePortVM> ports;

        /// <summary>
        /// 端口添加事件
        /// </summary>
        public event Action<BasePortVM> onPortAdded;

        /// <summary>
        /// 端口移除事件
        /// </summary>
        public event Action<BasePortVM> onPortRemoved;
        #endregion

        #region Properties
        public BaseNode Model { get; }
        public Type ModelType { get; }
        private BaseGraphVM owner;
        public BaseGraphVM Owner
        {
            get { return owner; }
        }
        public string GUID
        {
            get { return Model.guid; }
        }
        public IReadOnlyDictionary<string, BasePortVM> Ports
        {
            get { return ports; }
        }
        public Vector2 Position
        {
            get { return GetPropertyValue<Vector2>(POSITION_NAME); }
            internal set { SetPropertyValue(POSITION_NAME, value); }
        }
        #endregion

        public BaseNodeVM(BaseNode model)
        {
            Model = model;
            ModelType = model.GetType();
        }

        internal void Enable(BaseGraphVM graph)
        {
            owner = graph;
            ports = new Dictionary<string, BasePortVM>();
            InitPort();
            //位置绑定
            this[POSITION_NAME] = new BindableProperty<Vector2>(() => Model.position, v => Model.position = v);

            OnEnabled();
        }
        
        internal void Disable()
        {
            OnDisabled();
        }

        private void InitPort()
        {
            //属性端口
            foreach (FieldInfo item in ReflectionHelper.GetFieldInfos(ModelType))
            {
                if (AttributeHelper.TryGetFieldAttribute(item, out NodeValueAttribute nodeValueAttribute))
                    continue;
                BasePortVM port = null;
                if (AttributeHelper.TryGetFieldAttribute(item, out InputPortAttribute inputAttr))
                    port = new BasePortVM(inputAttr.name, inputAttr.orientation, inputAttr.direction, inputAttr.capacity, item.FieldType, inputAttr.setIndex);
                if (AttributeHelper.TryGetFieldAttribute(item, out OutputPortAttribute outputAttr))
                    port = new BasePortVM(outputAttr.name, outputAttr.orientation, outputAttr.direction, outputAttr.capacity, item.FieldType, outputAttr.setIndex);
                if (port != null)
                {
                    AddPort(port);
                }
            }
        }

        #region Display

        public void RefreshTitle()
        {
            if (Model.OnTooltipChanged == null)
            {
                return;
            }
            Model.OnTooltipChanged?.Invoke(Model.Title);
        }

        #endregion

        #region API
        public IEnumerable<BaseNodeVM> GetConnections(string portName)
        {
            if (!Ports.TryGetValue(portName, out var port))
                yield break;
            if (port.Model.direction == BasePort.Direction.Input)
            {
                foreach (var connection in port.Connections)
                {
                    yield return connection.FromNode;
                }
            }
            else
            {
                foreach (var connection in port.Connections)
                {
                    yield return connection.ToNode;
                }
            }
        }

        public void AddPort(BasePortVM port)
        {
            if (ports.ContainsKey(port.Model.name))
            {
                throw new ArgumentException($"Already contains port:{port.Model.name}");
            }
            ports[port.Model.name] = port;
            port.Enable(this);
            onPortAdded?.Invoke(port);
        }

        public void RemovePort(string portName)
        {
            RemovePort(ports[portName]);
        }

        public void RemovePort(BasePortVM port)
        {
            if (port.Owner != this)
            {
                return;
            }
            if (!ports.ContainsKey(port.Model.name))
            {
                throw new ArgumentException($"Not contains port:{port.Model.name}");
            }
            Owner.Disconnect(port);
            ports.Remove(port.Model.name);
            onPortRemoved?.Invoke(port);
        }

        public BasePort.Direction GetPortDirection(string portName)
        {
            if (!Ports.TryGetValue(portName, out var port))
                return BasePort.Direction.Input;
            return port.Model.direction;
        }

        #endregion

        #region Overrides
        protected virtual void OnEnabled()
        {

        }

        protected virtual void OnDisabled()
        {
        }
        #endregion

        #region 常量
        public const string POSITION_NAME = nameof(Position);
        #endregion

        #region 静态
        
        /// <summary> 根据T创建一个节点，并设置位置 </summary>
        // public static T CreateNew<T>(BaseGraphVM graph, Vector2 position) where T : BaseNodeVM
        // {
        //     return CreateNew(graph, typeof(T), position) as T;
        // }

        /// <summary> 根据type创建一个节点，并设置位置 </summary>
        public static BaseNodeVM CreateNew(BaseGraphVM graph, Type type, Vector2 position)
        {
            if (!type.IsSubclassOf(typeof(BaseNode)))
                return null;
            var node = Activator.CreateInstance(type) as BaseNode;
            node.position = position;
            IDAllocation(node, graph);
            return ViewModelFactory.CreateViewModel(node) as BaseNodeVM;
        }

        /// <summary> 给节点分配一个GUID，这将会覆盖已有GUID </summary>
        public static void IDAllocation(BaseNode node, BaseGraphVM graph)
        {
            node.guid = graph.GenerateNodeGUID();
        }
        
        #endregion

    }
}
