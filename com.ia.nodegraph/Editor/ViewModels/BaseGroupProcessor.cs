using IAEngine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IANodeGraph.Model
{
    [ViewModel(typeof(BaseGroup))]
    public class BaseGroupProcessor : ViewModel, IGraphElementProcessor, IGraphElementProcessor_Scope
    {
        #region Fileds

        private BaseGroup model;
        private Type modelType;
        public event Action<BaseNodeProcessor[]> onNodeAdded;
        public event Action<BaseNodeProcessor[]> onNodeRemoved;

        public BindableValue<Vector2Int> PositionBindable;

        public BindableValue<string> GroupName;

        public BindableValue<Color> BackgroundColor;

        #endregion

        #region Property

        public BaseGroup Model => model;
        public Type ModelType => modelType;

        object IGraphElementProcessor.Model => model;

        Type IGraphElementProcessor.ModelType => modelType;

        public int ID => Model.id;

        public IReadOnlyList<int> Nodes => Model.nodes;

        public BaseGraphProcessor Owner { get; internal set; }

        public Vector2Int Position
        {
            get => Model.position;
            set => PositionBindable.Value = value;
        }

        #endregion

        public BaseGroupProcessor(BaseGroup model)
        {
            this.model = model;
            this.modelType = model.GetType();
            this.model.position = model.position == default ? Vector2Int.zero : model.position;

            this.PositionBindable = new BindableValue<Vector2Int>(this, this.model.position);
            this.PositionBindable.RegisterChanged((newValue, oldValue) =>
            {
                this.model.position = newValue;
            });

            this.GroupName = new BindableValue<string>(this, model.groupName);
            this.BackgroundColor = new BindableValue<Color>(this, model.backgroundColor);
        }

        internal void NotifyNodeAdded(BaseNodeProcessor[] node)
        {
            onNodeAdded?.Invoke(node);
        }

        internal void NotifyNodeRemoved(BaseNodeProcessor[] node)
        {
            onNodeRemoved?.Invoke(node);
        }
    }
}