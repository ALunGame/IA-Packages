using System;
using System.Collections.Generic;
using System.Linq;
using IAToolkit.ViewModel;
using UnityEngine;
using IAEngine;
using IAToolkit;

namespace IANodeGraph.Model
{
    public class BasePort
    {
        #region Define

        /// <summary>
        /// 输入输出
        /// </summary>
        public enum Direction {
            /// <summary>
            /// 输入
            /// </summary>
            Input,
            /// <summary>
            /// 输出
            /// </summary>
            Output
        }

        /// <summary>
        /// 排列方式
        /// </summary>
        public enum Orientation { 
            /// <summary>
            /// 水平
            /// </summary>
            Horizontal,
            /// <summary>
            /// 竖直
            /// </summary>
            Vertical
        }

        /// <summary>
        /// 数量
        /// </summary>
        public enum Capacity { 
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
        
        public string name;
        public Orientation orientation;
        public Direction direction;
        public Capacity capacity;

        //端口值类型
        public Type type;

        //该端口设置索引
        public bool setIndex;
    }
    
    [NodeViewModel(typeof(BasePort))]
    public class BasePortVM : ViewModel, IGraphElement
    {
        #region Fields
        [NonSerialized] BaseNodeVM owner;
        [NonSerialized] List<BaseConnectionVM> connections;
        [NonSerialized] Func<BaseConnectionVM, BaseConnectionVM, int> comparer;

        public event Action<BaseConnectionVM> onConnected;
        public event Action<BaseConnectionVM> onDisconnected;
        public event Action onSorted;
        #endregion

        #region Properties
        
        public BasePort Model { get; }
        public Type ModelType { get; }
        public BaseNodeVM Owner { get { return owner; } }
        public IReadOnlyCollection<BaseConnectionVM> Connections { get { return connections; } }
        
        #endregion

        public BasePortVM(string name, BasePort.Orientation orientation, BasePort.Direction direction, BasePort.Capacity capacity, Type type = null, bool setIndex = false)
        {
            this.Model = new BasePort()
            {
                name = name,
                orientation = orientation,
                direction = direction,
                capacity = capacity,
                type = type == null ? typeof(object) : type,
                setIndex = setIndex,
            };
            this.ModelType = typeof(BasePort);
        }
        
        internal void Enable(BaseNodeVM node)
        {
            owner = node;
            switch (Model.orientation)
            {
                case BasePort.Orientation.Horizontal:
                    connections = new List<BaseConnectionVM>();
                    comparer = HorizontalComparer;
                    break;
                case BasePort.Orientation.Vertical:
                    connections = new List<BaseConnectionVM>();
                    comparer = VerticalComparer;
                    break;
            }
            OnEnabled();
            RefreshIndex();
        }

        #region API
        public void ConnectTo(BaseConnectionVM connection)
        {
            connections.Add(connection);
            Resort();
            onConnected?.Invoke(connection);
            RefreshIndex();
        }

        public void DisconnectTo(BaseConnectionVM connection)
        {
            connections.Remove(connection);
            Resort();
            onDisconnected?.Invoke(connection);
            RefreshIndex();
        }

        /// <summary> 强制重新排序 </summary>
        public void Resort()
        {
            connections.QuickSort(comparer);
            onSorted?.Invoke();
            RefreshIndex();
        }

        public void RefreshIndex()
        {
            if (!Model.setIndex)
                return;
            if (connections == null || connections.Count <= 0)
                return;
            if (Model.direction == BasePort.Direction.Input)
                RefreshInIndex();
            else if (Model.direction == BasePort.Direction.Output)
                RefreshOutIndex();
        }

        private void RefreshOutIndex()
        {
            for (int i = 0; i < connections.Count; i++)
            {
                BaseConnectionVM connect = connections[i];
                BaseNodeVM node = connect.ToNode;
                if (node != null)
                {
                    node.Model.inIndex = i;
                    Debug.LogWarning($"InIndex{node.GetType().Name}--{node.Model.outIndex}");

                    if (node.Model.OnTitleChanged != null)
                    {
                        node?.Model.OnTitleChanged(node.Model.Title);
                    }
                }
            }
        }

        private void RefreshInIndex()
        {
            for (int i = 0; i < connections.Count; i++)
            {
                BaseConnectionVM connect = connections[i];
                BaseNodeVM node = connect.FromNode;
                if (node != null)
                {
                    node.Model.outIndex = i;
                    Debug.LogWarning($"OutIndex{node.GetType().Name}--{node.Model.outIndex}");
                    if (node.Model.OnTitleChanged != null)
                    {
                        node?.Model.OnTitleChanged(node.Model.Title);
                    }
                }
            }
        }


        #endregion

        #region Overrides
        protected virtual void OnEnabled() { }
        #endregion

        #region Static
        private int VerticalComparer(BaseConnectionVM x, BaseConnectionVM y)
        {
            // 若需要重新排序的是input接口，则根据FromNode排序
            // 若需要重新排序的是output接口，则根据ToNode排序
            var nodeX = Model.direction == BasePort.Direction.Input ? x.FromNode : x.ToNode;
            var nodeY = Model.direction == BasePort.Direction.Input ? y.FromNode : y.ToNode;

            // 则使用x坐标比较排序
            // 遵循从左到右
            if (nodeX.Position.x < nodeY.Position.x)
                return -1;
            if (nodeX.Position.x > nodeY.Position.x)
                return 1;

            // 若节点的x坐标相同，则使用y坐标比较排序
            // 遵循从上到下
            if (nodeX.Position.y < nodeY.Position.y)
                return -1;
            if (nodeX.Position.y > nodeY.Position.y)
                return 1;

            return 0;
        }

        private int HorizontalComparer(BaseConnectionVM x, BaseConnectionVM y)
        {
            // 若需要重新排序的是input接口，则根据FromNode排序
            // 若需要重新排序的是output接口，则根据ToNode排序
            var nodeX = Model.direction == BasePort.Direction.Input ? x.FromNode : x.ToNode;
            var nodeY = Model.direction == BasePort.Direction.Input ? y.FromNode : y.ToNode;

            // 则使用y坐标比较排序
            // 遵循从上到下
            if (nodeX.Position.y < nodeY.Position.y)
                return -1;
            if (nodeX.Position.y > nodeY.Position.y)
                return 1;

            // 若节点的y坐标相同，则使用x坐标比较排序
            // 遵循从左到右
            if (nodeX.Position.x < nodeY.Position.x)
                return -1;
            if (nodeX.Position.x > nodeY.Position.x)
                return 1;

            return 0;
        }
        #endregion
    }
}
