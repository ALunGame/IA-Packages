using IAEngine;
using System;
using UnityEngine;

namespace IANodeGraph.Model
{
    [ViewModel(typeof(StickyNote))]
    public class StickyNoteProcessor : ViewModel, IGraphElementProcessor, IGraphElementProcessor_Scope
    {
        private StickyNote model;
        private Type modelType;

        public BindableValue<Vector2Int> PositionBindable;
        public BindableValue<Vector2Int> Size;
        public BindableValue<string> Title;
        public BindableValue<string> Content;

        public StickyNote Model => model;
        public Type ModelType => modelType;

        object IGraphElementProcessor.Model => model;

        Type IGraphElementProcessor.ModelType => modelType;

        /// <summary> 唯一标识 </summary>
        public int ID => Model.id;

        public Vector2Int Position
        {
            get => Model.position;
            set => PositionBindable.Value = value;
        }

        public StickyNoteProcessor(StickyNote model)
        {
            this.model = model;
            this.modelType = model.GetType();

            this.PositionBindable = new BindableValue<Vector2Int>(this, this.model.position);
            this.PositionBindable.RegisterChanged((newValue, oldValue) =>
            {
                this.model.position = newValue;
            });

            this.Size = new BindableValue<Vector2Int>(this, model.size);
            this.Title = new BindableValue<string>(this, model.title);
            this.Content = new BindableValue<string>(this, model.content);
        }
    }
}
