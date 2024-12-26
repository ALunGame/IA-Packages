using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAToolkit.ViewModel;

namespace IANodeGraph.Model
{
    [NodeViewModel(typeof(BaseConnection))]
    public class BaseConnectionVM : ViewModel, IGraphElement
    {
        #region Fields
        [NonSerialized] BaseGraphVM owner;
        [NonSerialized] BaseNodeVM fromNode;
        [NonSerialized] BaseNodeVM toNode;
        #endregion

        #region Properties
        public BaseConnection Model { get; }
        public Type ModelType { get; }
        public BaseGraphVM Owner { get { return owner; } }
        public string FromNodeGUID { get { return Model.from; } }
        public string ToNodeGUID { get { return Model.to; } }
        public string FromPortName { get { return Model.fromPortName; } }
        public string ToPortName { get { return Model.toPortName; } }
        public BaseNodeVM FromNode { get { return fromNode; } }
        public BaseNodeVM ToNode { get { return toNode; } }
        #endregion
        
        public BaseConnectionVM(BaseConnection model)
        {
            Model = model;
            ModelType = model.GetType();
        }

        internal void Enable(BaseGraphVM graph)
        {
            owner = graph;
            owner.Nodes.TryGetValue(FromNodeGUID, out fromNode);
            owner.Nodes.TryGetValue(ToNodeGUID, out toNode);
            OnEnabled();
        }

        /// <summary>
        /// 重定向
        /// </summary>
        internal void Redirect(BaseNodeVM from, string fromPortName, BaseNodeVM to, string toPortName)
        {
            this.Model.from = from.GUID;
            this.Model.fromPortName = fromPortName;

            this.Model.to = to.GUID;
            this.Model.toPortName = toPortName;

            Enable(owner);
        }

        /// <summary>
        /// 获得From或者To端口
        /// </summary>
        /// <returns></returns>
        public (BasePortVM, BasePortVM) GetInOrOutPort()
        {
            (BasePortVM, BasePortVM) item;
            item.Item1 = FromNode != null ? FromNode.Ports[FromPortName] : null;
            item.Item2 = ToNode != null ? ToNode.Ports[ToPortName] : null;
            return item;
        }

        #region Overrides
        protected virtual void OnEnabled() { }
        #endregion

        #region Static
        
        public static BaseConnectionVM CreateNew(BaseNodeVM from, string fromPortName, BaseNodeVM to, string toPortName)
        {
            BaseConnection connectionModel = new BaseConnection();
            connectionModel.from = from.GUID;
            connectionModel.fromPortName = fromPortName;
            connectionModel.to = to.GUID;
            connectionModel.toPortName = toPortName;

            return ViewModelFactory.CreateViewModel(connectionModel) as BaseConnectionVM;
        }
        #endregion

    }
}
