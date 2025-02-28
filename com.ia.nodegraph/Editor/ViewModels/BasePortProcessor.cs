using IAEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IANodeGraph.Model
{
    #region Define

    /// <summary>
    /// 端口方向
    /// </summary>
    public enum PortDirection
    {
        Left,
        Right,
        Top,
        Bottom,
    }

    /// <summary>
    /// 端口接受数量
    /// </summary>
    public enum PortCapacity
    {
        /// <summary>
        /// 单个
        /// </summary>
        Single,
        
        /// <summary>
        /// 多个
        /// </summary>
        Multi
    }

    #endregion

    public class BasePort
    {
        public string name;
        public string toolTip;
        public PortDirection direction;
        public PortCapacity capacity;
        public bool showDrawer;

        public string fieldName;
        public Type portType;

        public BasePort(string name, string toolTip, bool showDrawer, PortDirection direction, PortCapacity capacity, Type type = null)
        {
            this.name = name;
            this.direction = direction;
            this.capacity = capacity;
            this.portType = type;
            this.showDrawer = showDrawer;
            this.toolTip = toolTip;
        }
    }
    
    [ViewModel(typeof(BasePort))]
    public class BasePortProcessor : ViewModel, IGraphElementProcessor
    {
        #region Fields

        private BasePort model;
        private Type modelType;
        internal List<BaseConnectionProcessor> connections = new List<BaseConnectionProcessor>();

        public event Action<BaseConnectionProcessor> onBeforeConnected;
        public event Action<BaseConnectionProcessor> onAfterConnected;
        public event Action<BaseConnectionProcessor> onBeforeDisconnected;
        public event Action<BaseConnectionProcessor> onAfterDisconnected;
        public event Action onConnectionChanged;

        public BindableValue<Type> portType;
        public BindableValue<bool> HideLabel;

        #endregion

        #region Properties

        public BasePort Model => model;
        public Type ModelType => modelType;

        object IGraphElementProcessor.Model => model;

        Type IGraphElementProcessor.ModelType => modelType;

        public string Name => model.name;

        public string ToolTip => model.toolTip;

        public PortDirection Direction => model.direction;

        public PortCapacity Capacity => model.capacity;

        public IReadOnlyList<BaseConnectionProcessor> Connections => connections;

        public BaseNodeProcessor Owner { get; internal set; }

        #endregion

        public BasePortProcessor(BasePort model)
        {
            this.model = model;
            this.modelType = typeof(BasePort);

            InitBindableValues();
        }

        public BasePortProcessor(string name, PortDirection direction, PortCapacity capacity, Type type = null, string toolTip = "", bool showDrawer = false)
        {
            this.model = new BasePort(name, toolTip, showDrawer, direction, capacity, type)
            {
                name = name,
                direction = direction,
                capacity = capacity
            };
            this.modelType = typeof(BasePort);

            InitBindableValues();
        }

        private void InitBindableValues()
        {
            portType = new BindableValue<Type>(this, model.portType == null ? typeof(object) : model.portType);
            portType.RegisterChanged((newValue, oldValue) =>
            {
                model.portType = newValue;
            });
            HideLabel = new BindableValue<bool>(this, false);
        }

        #region API
        public void ConnectTo(BaseConnectionProcessor connection)
        {
            onBeforeConnected?.Invoke(connection);
            connections.Add(connection);

            switch (this.Direction)
            {
                case PortDirection.Left:
                    {
                        connections.QuickSort(ConnectionProcessorHorizontalComparer.ToPortSortDefault);
                        break;
                    }
                case PortDirection.Right:
                    {
                        connections.QuickSort(ConnectionProcessorHorizontalComparer.FromPortSortDefault);
                        break;
                    }
                case PortDirection.Top:
                    {
                        connections.QuickSort(ConnectionProcessorVerticalComparer.InPortSortDefault);
                        break;
                    }
                case PortDirection.Bottom:
                    {
                        connections.QuickSort(ConnectionProcessorVerticalComparer.OutPortSortDefault);
                        break;
                    }
            }

            onAfterConnected?.Invoke(connection);
            onConnectionChanged?.Invoke();
        }

        public void DisconnectTo(BaseConnectionProcessor connection)
        {
            onBeforeDisconnected?.Invoke(connection);
            connections.Remove(connection);
            onAfterDisconnected?.Invoke(connection);
            onConnectionChanged?.Invoke();
        }

        /// <summary> 整理 </summary>
        public bool Trim()
        {
            var removeNum = connections.RemoveAll(ConnectionProcessorComparer.EmptyComparer);

            switch (Direction)
            {
                case PortDirection.Left:
                    return removeNum != 0 && connections.QuickSort(ConnectionProcessorHorizontalComparer.ToPortSortDefault);
                case PortDirection.Right:
                    return removeNum != 0 && connections.QuickSort(ConnectionProcessorHorizontalComparer.FromPortSortDefault);
                case PortDirection.Top:
                    return removeNum != 0 && connections.QuickSort(ConnectionProcessorVerticalComparer.InPortSortDefault);
                case PortDirection.Bottom:
                    return removeNum != 0 && connections.QuickSort(ConnectionProcessorVerticalComparer.OutPortSortDefault);
            }

            return removeNum != 0;
        }

        /// <summary> 获取连接的第一个接口的值 </summary>
        public object GetConnectionValue()
        {
            return GetConnectionValues().FirstOrDefault();
        }

        /// <summary> 获取连接的接口的值 </summary>
        public IEnumerable<object> GetConnectionValues()
        {
            if (Model.direction == PortDirection.Left)
            {
                foreach (var connection in Connections)
                {
                    if (connection.FromNode is IGetPortValue fromPort)
                        yield return fromPort.GetValue(connection.FromPortName);
                }
            }
            else
            {
                foreach (var connection in Connections)
                {
                    if (connection.ToNode is IGetPortValue toPort)
                        yield return toPort.GetValue(connection.ToPortName);
                }
            }
        }

        /// <summary> 获取连接的第一个接口的值 </summary>
        public T GetConnectionValue<T>()
        {
            return GetConnectionValues<T>().FirstOrDefault();
        }

        /// <summary> 获取连接的接口的值 </summary>
        public IEnumerable<T> GetConnectionValues<T>()
        {
            if (Model.direction == PortDirection.Left)
            {
                foreach (var connection in Connections)
                {
                    if (connection.FromNode is IGetPortValue<T> fromPort)
                        yield return fromPort.GetValue(connection.FromPortName);
                }
            }
            else
            {
                foreach (var connection in Connections)
                {
                    if (connection.ToNode is IGetPortValue<T> toPort)
                        yield return toPort.GetValue(connection.ToPortName);
                }
            }
        }

        #endregion
    }
}
