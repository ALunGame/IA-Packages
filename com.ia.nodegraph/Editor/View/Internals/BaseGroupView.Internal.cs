using System.Collections.Generic;
using System.Linq;
using IANodeGraph.Model;
using IANodeGraph.View.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

using GroupView = UnityEditor.Experimental.GraphView.Group;

namespace IANodeGraph.View
{
    public partial class BaseGroupView : GroupView, IBindableView<BaseGroupVM>
    {
        bool WithoutNotify { get; set; }
        public TextField TitleField { get; private set; }
        public ColorField BackgroudColorField { get; private set; }
        public Label TitleLabel { get; private set; }
        public BaseGroupVM Model { get; protected set; }
        public BaseGraphView Owner { get; private set; }
        
        
        public BaseGroupView()
        {
            this.styleSheets.Add(GraphProcessorStyles.BaseGroupViewStyle);

            TitleLabel = headerContainer.Q<Label>();
            TitleField = headerContainer.Q<TextField>();

            BackgroudColorField = new ColorField();
            BackgroudColorField.name = "backgroundColorField";
            headerContainer.Add(BackgroudColorField);

            TitleField.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            TitleField.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });
        }

        public void SetUp(BaseGroupVM group, BaseGraphView graphView)
        {
            this.Model = group;
            this.Owner = graphView;
            this.title = Model.GroupName;
            this.style.backgroundColor = Model.BackgroundColor;
            this.BackgroudColorField.SetValueWithoutNotify(Model.BackgroundColor);
            base.SetPosition(new Rect(Model.Position, GetPosition().size));
            WithoutNotify = true;
            base.AddElements(Model.Nodes.Select(nodeID => Owner.NodeViews[nodeID]).ToArray());
            WithoutNotify = false;
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            BackgroudColorField.RegisterValueChangedCallback(OnGroupColorChanged);
        }

        public void BindingProperties()
        {
            Model.BindingProperty<string>(nameof(BaseGroup.groupName),OnTitleChanged);
            Model.BindingProperty<Vector2>(nameof(BaseGroup.position),OnPositionChanged);
            Model.BindingProperty<Color>(nameof(BaseGroup.backgroundColor),OnBackgroundColorChanged);
            
            Model.onNodesAdded += OnNodesAdded;
            Model.onNodesRemoved += OnNodesRemoved;
        }

        public void UnBindingProperties()
        {
            Model.ClearBindingEvent();
            Model.onNodesAdded -= OnNodesAdded;
            Model.onNodesRemoved -= OnNodesRemoved;
        }

        #region Callbacks
        private void OnTitleChanged(string title)
        {
            if (string.IsNullOrEmpty(title))
                return;
            this.title = Model.GroupName;
            Owner.SetDirty();
        }

        private void OnPositionChanged(Vector2 newPos)
        {
            base.SetPosition(new Rect(newPos, GetPosition().size));
        }

        private void OnBackgroundColorChanged(Color newColor)
        {
            this.BackgroudColorField.SetValueWithoutNotify(newColor);
            this.style.backgroundColor = newColor;
            Owner.SetDirty();
        }

        private void OnNodesAdded(IEnumerable<BaseNodeVM> nodes)
        {
            WithoutNotify = true;
            base.AddElements(nodes.Select(node => Owner.NodeViews[node.Model.guid]));
            WithoutNotify = false;
        }

        private void OnNodesRemoved(IEnumerable<BaseNodeVM> nodes)
        {
            WithoutNotify = true;
            base.RemoveElements(nodes.Select(node => Owner.NodeViews[node.Model.guid]));
            WithoutNotify = false;
        }
        #endregion

        protected override void OnGroupRenamed(string oldName, string newName)
        {
            if (string.IsNullOrEmpty(newName))
                return;
            Owner.CommandDispacter.Do(new RenameGroupCommand(Model, newName));
        }

        private void OnGroupColorChanged(ChangeEvent<Color> evt)
        {
            Model.BackgroundColor = evt.newValue;
        }

        public override bool AcceptsElement(GraphElement element, ref string reasonWhyNotAccepted)
        {
            if (!base.AcceptsElement(element, ref reasonWhyNotAccepted))
                return false;
            if (element is BaseNodeView)
                return true;
            if (element is BaseConnectionView)
                return true;
            return false;
        }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            base.OnElementsAdded(elements);
            if (WithoutNotify)
                return;
            var nodes = elements.Where(element => element is BaseNodeView).Select(element => (element as BaseNodeView).Model);
            Model.AddNodesWithoutNotify(nodes);
            Owner.SetDirty();
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            base.OnElementsRemoved(elements);
            if (WithoutNotify)
                return;
            var nodes = elements.Where(element => element is BaseNodeView).Select(element => (element as BaseNodeView).Model).ToArray();
            Model.RemoveNodesWithoutNotify(nodes);
            Owner.SetDirty();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            this.BringToFront();
        }


    }
}