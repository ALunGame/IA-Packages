using IANodeGraph.Editors;
using IANodeGraph.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace IANodeGraph.View
{
    public sealed partial class GroupView : Group, IGraphElementView<BaseGroupProcessor>
    {
        public VisualElement GroupBorder { get; private set; }
        public TextField TitleField { get; private set; }
        public ColorField BackgroudColorField { get; private set; }
        public Label TitleLabel { get; private set; }
        public BaseGroupProcessor ViewModel { get; protected set; }
        public IGraphElementProcessor V => ViewModel;
        public BaseGraphView Owner { get; private set; }

        bool WithoutNotify { get; set; }

        public GroupView()
        {
            this.styleSheets.Add(GraphProcessorEditorStyles.DefaultStyles.GroupViewStyle);

            this.GroupBorder = new VisualElement() { name = "group-border", pickingMode = PickingMode.Ignore };
            this.Add(GroupBorder);

            this.TitleLabel = headerContainer.Q<Label>();
            this.TitleField = headerContainer.Q<TextField>();
            this.BackgroudColorField = new ColorField();
            this.BackgroudColorField.name = "backgroundColorField";
            this.headerContainer.Add(BackgroudColorField);

            this.TitleField.RegisterCallback<FocusInEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.On; });
            this.TitleField.RegisterCallback<FocusOutEvent>(evt => { Input.imeCompositionMode = IMECompositionMode.Auto; });
        }

        public void SetUp(BaseGroupProcessor group, BaseGraphView graphView)
        {
            this.ViewModel = group;
            this.Owner = graphView;
            this.title = ViewModel.GroupName.Value;
            this.style.backgroundColor = ViewModel.BackgroundColor.Value;
            this.style.unityBackgroundImageTintColor = ViewModel.BackgroundColor.Value;
            this.BackgroudColorField.SetValueWithoutNotify(ViewModel.BackgroundColor.Value);
            base.SetPosition(new Rect(ViewModel.Position, GetPosition().size));
            WithoutNotify = true;
            base.AddElements(ViewModel.Nodes.Where(nodeID => Owner.NodeViews.ContainsKey(nodeID)).Select(nodeID => Owner.NodeViews[nodeID]).ToArray());
            WithoutNotify = false;
            BackgroudColorField.RegisterValueChangedCallback(OnGroupColorChanged);
        }

        public void OnCreate()
        {
            RegisterValueChangedEvents();
            ViewModel.onNodeAdded += OnNodesAdded;
            ViewModel.onNodeRemoved += OnNodesRemoved;
        }

        private void RegisterValueChangedEvents()
        {
            ViewModel.PositionBindable.RegisterChanged(OnPositionChange);
            ViewModel.GroupName.RegisterChanged(OnGroupNameChange);
            ViewModel.BackgroundColor.RegisterChanged(OnBackgroundColorChange);
        }

        public void OnDestroy()
        {
            UnRegisterValueChangedEvents();
            ViewModel.onNodeAdded -= OnNodesAdded;
            ViewModel.onNodeRemoved -= OnNodesRemoved;
        }

        private void UnRegisterValueChangedEvents()
        {
            ViewModel.PositionBindable.RegisterChanged(OnPositionChange);
            ViewModel.GroupName.RegisterChanged(OnGroupNameChange);
            ViewModel.BackgroundColor.RegisterChanged(OnBackgroundColorChange);
        }

        #region Callbacks

        private void OnPositionChange(Vector2Int pNewPos, Vector2Int pOldPos)
        {
            base.SetPosition(new Rect(pNewPos, GetPosition().size));
        }

        private void OnGroupNameChange(string pNewGroupName, string pOldGroupName)
        {
            if (string.IsNullOrEmpty(pNewGroupName))
                return;
            this.title = pNewGroupName;
            Owner.SetDirty();
        }

        private void OnBackgroundColorChange(Color pNewBackgroundColor, Color pOldBackgroundColor)
        {
            this.BackgroudColorField.SetValueWithoutNotify(pNewBackgroundColor);
            this.style.backgroundColor = pNewBackgroundColor;
            this.style.unityBackgroundImageTintColor = pNewBackgroundColor;
            Owner.SetDirty();
        }

        private void OnNodesAdded(BaseNodeProcessor[] nodes)
        {
            if (WithoutNotify)
            {
                return;
            }

            var tmp = WithoutNotify;
            try
            {
                WithoutNotify = false;
                base.AddElements(nodes.Select(node => Owner.NodeViews[node.ID]));
            }
            finally
            {
                WithoutNotify = tmp;
            }
        }

        private void OnNodesRemoved(BaseNodeProcessor[] nodes)
        {
            if (WithoutNotify)
            {
                return;
            }

            var tmp = WithoutNotify;
            try
            {
                WithoutNotify = false;
                base.RemoveElementsWithoutNotification(nodes.Select(node => Owner.NodeViews[node.ID]));
            }
            finally
            {
                WithoutNotify = tmp;
            }
        }

        #endregion

        protected override void OnGroupRenamed(string oldName, string newName)
        {
            Owner.CommandDispatcher.Do(new RenameGroupCommand(ViewModel, newName));
        }

        private void OnGroupColorChanged(ChangeEvent<Color> evt)
        {
            ViewModel.BackgroundColor.Value = evt.newValue;
        }

        public override bool AcceptsElement(GraphElement element, ref string reasonWhyNotAccepted)
        {
            if (!base.AcceptsElement(element, ref reasonWhyNotAccepted))
                return false;
            switch (element)
            {
                case BaseNodeView:
                    return true;
                case StickyNoteView:
                    return true;
            }

            return false;
        }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            if (WithoutNotify)
                return;

            var tmp = WithoutNotify;
            WithoutNotify = true;
            var nodes = elements.Where(item => item is BaseNodeView).Select(item => (item as BaseNodeView).ViewModel).ToArray();
            Owner.CommandDispatcher.Do(new AddToGroupCommand(Owner.ViewModel, this.ViewModel, nodes));

            Owner.SetDirty();
            WithoutNotify = tmp;
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            if (WithoutNotify)
                return;
            var tmp = WithoutNotify;
            WithoutNotify = true;
            var nodes = elements.Where(item => item is BaseNodeView).Select(item => (item as BaseNodeView).ViewModel).ToArray();
            Owner.CommandDispatcher.Do(new RemoveFromGroupCommand(Owner.ViewModel, this.ViewModel, nodes));

            Owner.SetDirty();
            WithoutNotify = tmp;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            this.BringToFront();
        }
    }
}