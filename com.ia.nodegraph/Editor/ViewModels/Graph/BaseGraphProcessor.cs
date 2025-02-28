using IAEngine;
using IAToolkit.Misc;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IANodeGraph.Model
{
    [ViewModel(typeof(BaseGraph))]
    public partial class BaseGraphProcessor : ViewModel
    {
        /// <summary>
        /// 节点命名空间限制
        /// </summary>
        public virtual List<string> NodeNamespace => null;

        #region Fields

        private BaseGraph model;
        private Type modelType;

        public BindableValue<Vector2Int> Pan;
        public BindableValue<float> Zoom;

        #endregion

        #region Properties

        public BaseGraph Model => model;

        public Type ModelType => modelType;

        #endregion

        public BaseGraphProcessor(BaseGraph model)
        {
            this.model = model;
            this.modelType = model.GetType();
            this.model.pan = Model.pan == default ? Vector2Int.zero : Model.pan;
            this.model.zoom = Model.zoom == 0 ? 1 : Model.zoom;
            this.model.notes = Model.notes == null ? new List<StickyNote>() : Model.notes;

            Pan = new BindableValue<Vector2Int>(this, Model.pan);
            Pan.RegisterChanged((newValue, oldValue) =>
            {
                Model.pan = newValue;
            });

            Zoom = new BindableValue<float>(this, Model.zoom);
            Zoom.RegisterChanged((newValue, oldValue) =>
            {
                Model.zoom = newValue;
            });

            BeginInitNodes();
            BeginInitConnections();
            EndInitConnections();
            EndInitNodes();
            InitGroups();
            InitNotes();
        }

        #region API

        public int NewID()
        {
            var id = 0;
            do
            {
                id++;
            } while (nodes.ContainsKey(id) || groups.GroupMap.ContainsKey(id) || notes.ContainsKey(id) || id == 0);

            return id;
        }

        #endregion
    }
}
