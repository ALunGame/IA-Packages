using IAEngine;
using IAToolkit.Misc;
using System;
using System.Collections.Generic;
using System.Reflection;
using IANodeGraph.View;
using UnityEngine;

namespace IANodeGraph.Model
{
    [ViewModel(typeof(BaseNode))]
    public class BaseNodeProcessor : ViewModel, IGraphElementProcessor, IGraphElementProcessor_Scope
    {
        #region Fields

        private BaseNode model;
        private Type modelType;

        private readonly List<BasePortProcessor> inPorts = new List<BasePortProcessor>();
        private readonly List<BasePortProcessor> outPorts = new List<BasePortProcessor>();
        private readonly Dictionary<string, BasePortProcessor> ports = new Dictionary<string, BasePortProcessor>();

        public event Action<BasePortProcessor> onPortAdded;
        public event Action<BasePortProcessor> onPortRemoved;

        public BindableValue<Vector2Int> PositionBindable;

        public BindableValue<string> Title;

        public BindableValue<Color> TitleColor;

        public BindableValue<string> Tooltip;

        #endregion

        #region Properties

        public BaseNode Model => model;
        public Type ModelType => modelType;

        object IGraphElementProcessor.Model => model;

        Type IGraphElementProcessor.ModelType => modelType;

        /// <summary> 唯一标识 </summary>
        public int ID => Model.id;

        public virtual Vector2Int Position
        {
            get => Model.position;
            set => PositionBindable.Value = value;
        }

        public IReadOnlyList<BasePortProcessor> InPorts => inPorts;

        public IReadOnlyList<BasePortProcessor> OutPorts => outPorts;

        public IReadOnlyDictionary<string, BasePortProcessor> Ports => ports;

        public BaseGraphProcessor Owner { get; internal set; }

        #endregion

        public BaseNodeProcessor(BaseNode model)
        {
            this.model = model;
            this.model.position = model.position == default ? Vector2Int.zero : model.position;
            this.modelType = model.GetType();

            var nodeStaticInfo = GraphProcessorUtil.NodeStaticInfos[this.modelType];

            this.PositionBindable = new BindableValue<Vector2Int>(this, this.model.position);
            this.PositionBindable.RegisterChanged((newValue, oldValue) =>
            {
                this.model.position = newValue;
            });

            this.Title = new BindableValue<string>(this, nodeStaticInfo.title);
            this.Tooltip = new BindableValue<string>(this, nodeStaticInfo.tooltip);
            this.TitleColor = new BindableValue<Color>(this, nodeStaticInfo.customTitleColor.value);

            InitNodeValuePorts();
        }

        internal void Enable()
        {
            foreach (var port in ports.Values)
            {
                if (port.connections.Count > 1)
                    port.Trim();
            }

            OnEnabled();
        }

        private void InitNodeValuePorts()
        {
            //属性端口
            foreach (FieldInfo item in ReflectionHelper.GetFieldInfos(ModelType))
            {
                if (AttributeHelper.TryGetFieldAttribute(item, out NodeValueAttribute attr))
                {
                    BasePort port = null;
                    PortCapacity capacity = BaseGraphView.IsArrayOrList(item.FieldType) ? PortCapacity.Multi : PortCapacity.Single;
                    
                    if (AttributeHelper.TryGetTypeAttribute(item.FieldType, out CustomPortAttribute attrPort))
                    {
                        port = Activator.CreateInstance(attrPort.targetType, item.Name, attr.Lable, attr.Tooltip, attr.showDrawer,
                            attr.direction, capacity, item.FieldType) as BasePort;
                    }
                    else
                    {
                        port = new BasePort(item.Name, attr.Lable, attr.Tooltip, attr.showDrawer, attr.direction, capacity, item.FieldType);
                    }
                    
                    port.fieldName = item.Name;
                    AddPort(port);
                }
            }
        }

        internal void Disable()
        {
            OnDisabled();
        }

        #region API

        public IEnumerable<BaseNodeProcessor> GetConnections(string portName)
        {
            if (!Ports.TryGetValue(portName, out var port))
                yield break;

            foreach (var connection in port.Connections)
            {
                yield return port.Direction == PortDirection.Left ? connection.FromNode : connection.ToNode;
            }
        }

        public BasePortProcessor AddPort(BasePort port)
        {
            var portVM = ViewModelFactory.ProduceViewModel(port) as BasePortProcessor;
            AddPort(portVM);
            return portVM;
        }

        public void AddPort(BasePortProcessor port)
        {
            ports.Add(port.Name, port);
            switch (port.Direction)
            {
                case PortDirection.Left:
                case PortDirection.Top:
                    {
                        inPorts.Add(port);
                        break;
                    }
                case PortDirection.Right:
                case PortDirection.Bottom:
                    {
                        outPorts.Add(port);
                        break;
                    }
            }

            port.Owner = this;
            onPortAdded?.Invoke(port);
        }

        public void RemovePort(string portName)
        {
            if (!ports.TryGetValue(portName, out var port))
                return;

            RemovePort(port);
        }

        public void RemovePort(BasePortProcessor port)
        {
            if (port.Owner != this)
                return;
            if (Owner != null)
                Owner.Disconnect(port);
            ports.Remove(port.Name);
            switch (port.Direction)
            {
                case PortDirection.Left:
                    {
                        inPorts.Remove(port);
                        break;
                    }
                case PortDirection.Right:
                    {
                        outPorts.Remove(port);
                        break;
                    }
            }

            onPortRemoved?.Invoke(port);
        }

        public void SortPort(Func<BasePortProcessor, BasePortProcessor, int> comparer)
        {
            inPorts.QuickSort(comparer);
            outPorts.QuickSort(comparer);
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
    }
}
