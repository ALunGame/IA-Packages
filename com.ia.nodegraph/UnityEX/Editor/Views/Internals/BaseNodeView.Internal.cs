using IANodeGraph.Editors;
using IANodeGraph.Model;
using IAToolkit;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using PortDirection = IANodeGraph.Model.PortDirection;
using NodeView = UnityEditor.Experimental.GraphView.Node;

namespace IANodeGraph.View
{
    /// <summary>
    /// 节点显示
    /// </summary>
    public abstract partial class BaseNodeView : NodeView, IGraphElementView<BaseNodeProcessor>
    {
        #region 字段

        public readonly Label nodeLabel;
        public readonly Image nodeIcon;
        public readonly VisualElement controls;
        public readonly VisualElement nodeBorder;
        public readonly VisualElement topPortContainer;
        public readonly VisualElement bottomPortContainer;
        public readonly VisualElement titleInputPortContainer;
        public readonly VisualElement titleOutputPortContainer;
        public readonly VisualElement horizontalDivider;
        public readonly VisualElement verticalDivider;

        private VisualElement inputContainerElement;

        private List<IconBadge> badges = new List<IconBadge>();
        private Dictionary<string, BasePortView> portViews = new Dictionary<string, BasePortView>();

        private Dictionary<FieldInfo, NodeValueAttribute> nodeValues = new Dictionary<FieldInfo, NodeValueAttribute>();
        private Dictionary<VisualElement, FieldInfo> nodeValueElements = new Dictionary<VisualElement, FieldInfo>();

        #endregion

        #region 属性

        public bool Selectable
        {
            get => (capabilities & Capabilities.Selectable) == Capabilities.Selectable;
            set => capabilities = value ? (capabilities | Capabilities.Selectable) : (capabilities & ~Capabilities.Selectable);
        }

        public bool Deletable
        {
            get => (capabilities & Capabilities.Deletable) == Capabilities.Deletable;
            set => capabilities = value ? (capabilities | Capabilities.Deletable) : (capabilities & ~Capabilities.Deletable);
        }

        public bool Movable
        {
            get => (capabilities & Capabilities.Movable) == Capabilities.Movable;
            set => capabilities = value ? (capabilities | Capabilities.Movable) : (capabilities & ~Capabilities.Movable);
        }

        public Label NodeLabel => nodeLabel;

        public Image NodeIcon => nodeIcon;

        public BaseGraphView Owner { get; private set; }
        public BaseNodeProcessor ViewModel { get; protected set; }
        public IGraphElementProcessor V => ViewModel;

        public IReadOnlyDictionary<string, BasePortView> PortViews => portViews;

        #endregion

        /// <summary>
        /// 创建水平分割线
        /// </summary>
        /// <returns></returns>
        public VisualElement CreateHorDividerElement()
        {
            VisualElement dividerElement = new VisualElement();
            dividerElement.name = "divider";
            dividerElement.AddToClassList("horizontal");
            dividerElement.style.display = DisplayStyle.Flex;
            return dividerElement;
        }

        /// <summary>
        /// 创建垂直分割线
        /// </summary>
        /// <returns></returns>
        public VisualElement CreateVerDividerElement()
        {
            VisualElement dividerElement = new VisualElement();
            dividerElement.name = "divider";
            dividerElement.AddToClassList("vertical");
            dividerElement.style.display = DisplayStyle.Flex;
            return dividerElement;
        }

        public BaseNodeView()
        {
            styleSheets.Add(GraphProcessorEditorStyles.DefaultStyles.BaseNodeViewStyle);
            
            var contents = mainContainer.Q("contents");

            nodeBorder = this.Q(name: "node-border");
            nodeLabel = titleContainer.Q<Label>("title-label");
            horizontalDivider = this.Q(name: "divider", className: "horizontal");
            verticalDivider = topContainer.Q(name: "divider", className: "vertical");

            nodeIcon = new Image() { name = "title-icon" };
            titleContainer.Insert(0, nodeIcon);

            controls = new BaseVisualElement() { name = "controls" };
            contents.Add(controls);

            topPortContainer = new VisualElement { name = "top-input" };
            nodeBorder.Insert(0, topPortContainer);

            bottomPortContainer = new VisualElement { name = "bottom-input" };
            nodeBorder.Add(bottomPortContainer);

            outputContainer.style.alignItems = Align.FlexEnd;

            titleInputPortContainer = new VisualElement { name = "title-input" };
            titleContainer.Add(titleInputPortContainer);
            titleInputPortContainer.SendToBack();

            titleOutputPortContainer = new VisualElement { name = "title-output" };
            titleContainer.Add(titleOutputPortContainer);
            titleOutputPortContainer.BringToFront();

            controls.RegisterCallback<BaseVisualElement.ChildChangedEvent>(OnChildChanged);
            this.RegisterCallback<PointerDownEvent>(OnPointerDown);

            nodeBorder.style.overflow = Overflow.Visible;
        }

        #region Initialize

        public void SetUp(BaseNodeProcessor node, BaseGraphView graphView)
        {
            ViewModel = node;
            Owner = graphView;

            // 初始化
            base.SetPosition(new Rect(ViewModel.Position.ToVector2(), GetPosition().size));
            title = ViewModel.Title.Value;
            tooltip = ViewModel.Tooltip.Value;

            var color = ViewModel.TitleColor.Value;
            var lum = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
            NodeLabel.style.color = lum > 0.5f && ViewModel.TitleColor.Value.a > 0.5f ? Color.black : Color.white * 0.9f;
            titleContainer.style.backgroundColor = color;

            foreach (var port in ViewModel.InPorts)
            {
                var portView = NewPortView(port);
                portView.SetUp(port, Owner);
                portViews[port.Name] = portView;
                
                if (port.Label.Contains(ConstValues.FLOW_IN_PORT_NAME))
                {
                    portView.portName = port.ToolTip.Contains(ConstValues.FLOW_IN_PORT_NAME) ? "" : port.ToolTip;
                    
                    titleInputPortContainer.Add(portView);
                    titleInputPortContainer.Add(CreateHorDividerElement());
                }
                else
                {
                    switch (port.Direction)
                    {
                        case PortDirection.Left:
                            {
                                inputContainer.Add(portView);
                                break;
                            }
                        case PortDirection.Top:
                            {
                                topPortContainer.Add(portView);
                                break;
                            }
                    }
                }
            }

            foreach (var port in ViewModel.OutPorts)
            {
                var portView = NewPortView(port);
                portView.SetUp(port, Owner);
                portViews[port.Name] = portView;

                if (port.Label.Contains(ConstValues.FLOW_OUT_PORT_NAME))
                {
                    portView.portName = port.ToolTip.Contains(ConstValues.FLOW_OUT_PORT_NAME) ? "" : port.ToolTip;
                    
                    titleOutputPortContainer.Add(portView);
                    titleOutputPortContainer.Add(CreateHorDividerElement());
                }
                else
                {
                    switch (port.Direction)
                    {
                        case PortDirection.Right:
                            {
                                outputContainer.Add(portView);
                                break;
                            }
                        case PortDirection.Bottom:
                            {
                                bottomPortContainer.Add(portView);
                                break;
                            }
                    }
                }
            }
            
            RefreshPorts();
            RefreshPortContainer();
            RefreshControls();
            RefreshContentsHorizontalDivider();
            
            OnInitialized();
        }

        private void OnChildChanged(BaseVisualElement.ChildChangedEvent evt)
        {
            RefreshControls();
            RefreshContentsHorizontalDivider();
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.shiftKey)
            {
                var hashSet = new HashSet<BaseNodeView>();
                var queue = new Queue<BaseNodeView>();
                queue.Enqueue(this);
                while (queue.Count > 0)
                {
                    var n = queue.Dequeue();
                    if (hashSet.Contains(n))
                    {
                        continue;
                    }

                    hashSet.Add(n);
                    foreach (var p in n.ViewModel.OutPorts)
                    {
                        foreach (var c in p.Connections)
                        {
                            if (Owner.NodeViews.TryGetValue(c.ToNodeID, out var nv))
                            {
                                queue.Enqueue(nv);
                            }
                        }
                    }
                }

                Owner.AddToSelection(hashSet.Where(n => n.Selectable));
                evt.StopPropagation();
            }
        }

        public void OnCreate()
        {
            ViewModel.onPortAdded += OnPortAdded;
            ViewModel.onPortRemoved += OnPortRemoved;

            foreach (var portView in portViews.Values)
            {
                portView.OnCreate();
            }

            OnBindingProperties();

            RegisterValueChangedEvents();
        }

        private void RegisterValueChangedEvents()
        {
            ViewModel.PositionBindable.RegisterChanged(OnPositionChange);
            ViewModel.Title.RegisterChanged(OnTitleChange);
            ViewModel.TitleColor.RegisterChanged(OnTitleColorChange);
            ViewModel.Tooltip.RegisterChanged(OnTooltipChange);
        }

        public void OnDestroy()
        {
            ViewModel.onPortAdded -= OnPortAdded;
            ViewModel.onPortRemoved -= OnPortRemoved;

            foreach (var portView in portViews.Values)
            {
                portView.OnDestroy();
            }

            OnUnBindingProperties();

            UnRegisterValueChangedEvents();
        }

        private void UnRegisterValueChangedEvents()
        {
            ViewModel.PositionBindable.UnregisterChanged(OnPositionChange);
            ViewModel.Title.UnregisterChanged(OnTitleChange);
            ViewModel.TitleColor.UnregisterChanged(OnTitleColorChange);
            ViewModel.Tooltip.UnregisterChanged(OnTooltipChange);
        }

        #endregion

        #region Callbacks

        void OnPortAdded(BasePortProcessor port)
        {
            AddPortView(port);
            RefreshPorts();
            RefreshContentsHorizontalDivider();
            RefreshPortContainer();
        }

        void OnPortRemoved(BasePortProcessor port)
        {
            RemovePortView(port);
            RefreshPorts();
            RefreshContentsHorizontalDivider();
            RefreshPortContainer();
        }

        private void OnPositionChange(Vector2Int pNewPos, Vector2Int pOldPos)
        {
            base.SetPosition(new Rect(pNewPos.ToVector2(), GetPosition().size));
            Owner.SetDirty();
        }

        private void OnTitleChange(string pNewTitle, string pOldTitle)
        {
            base.title = pNewTitle;
        }

        private void OnTitleColorChange(Color pNewColor, Color pOldColor)
        {
            titleContainer.style.backgroundColor = pNewColor;
            var lum = 0.299f * pNewColor.r + 0.587f * pNewColor.g + 0.114f * pNewColor.b;
            NodeLabel.style.color = lum > 0.5f && pNewColor.a > 0.5f ? Color.black : Color.white * 0.9f;
        }

        private void OnTooltipChange(string pNewTooltip, string pOldTooltip)
        {
            this.tooltip = pNewTooltip;
        }

        #endregion

        protected void PortChanged()
        {
            RefreshPorts();
            RefreshPortContainer();
            RefreshContentsHorizontalDivider();
        }

        private void AddPortView(BasePortProcessor port)
        {
            BasePortView portView = NewPortView(port);
            portView.SetUp(port, Owner);
            portView.OnCreate();
            portViews[port.Name] = portView;

            if (portView.orientation == Orientation.Horizontal)
            {
                if (portView.ViewModel.Direction == PortDirection.Left)
                    inputContainer.Add(portView);
                else
                    outputContainer.Add(portView);
            }
            else
            {
                if (portView.ViewModel.Direction == PortDirection.Top)
                    topPortContainer.Add(portView);
                else
                    bottomPortContainer.Add(portView);
            }
        }
        
        public BasePortView GetPortViewByFieldName(string pFieldName)
        {
            foreach (BasePortView view in PortViews.Values)
            {
                if (view.ViewModel.Model.fieldName.Equals(pFieldName))
                {
                    return view;
                }
            }
            return null;
        }

        private void RemovePortView(BasePortProcessor port)
        {
            portViews[port.Name].RemoveFromHierarchy();
            portViews[port.Name].OnDestroy();
            portViews.Remove(port.Name);
        }

        private void RefreshContentsHorizontalDivider()
        {
            if (inputContainer.childCount > 0 || outputContainer.childCount > 0 || DrawingControls())
                horizontalDivider.RemoveFromClassList("hidden");
            else
                horizontalDivider.AddToClassList("hidden");

            if (inputContainer.childCount > 0 || outputContainer.childCount > 0)
                verticalDivider.RemoveFromClassList("hidden");
            else
                verticalDivider.AddToClassList("hidden");
        }

        private void RefreshPortContainer()
        {
            if (topPortContainer.childCount > 0)
                topPortContainer.RemoveFromClassList("hidden");
            else
                topPortContainer.AddToClassList("hidden");

            if (bottomPortContainer.childCount > 0)
                bottomPortContainer.RemoveFromClassList("hidden");
            else
                bottomPortContainer.AddToClassList("hidden");

            if (titleInputPortContainer.childCount > 0)
                titleInputPortContainer.RemoveFromClassList("hidden");
            else
                titleInputPortContainer.AddToClassList("hidden");

            if (titleOutputPortContainer.childCount > 0)
                titleOutputPortContainer.RemoveFromClassList("hidden");
            else
                titleOutputPortContainer.AddToClassList("hidden");
        }

        private void RefreshControls()
        {
            if (DrawingControls())
                controls.RemoveFromClassList("hidden");
            else
                controls.AddToClassList("hidden");
        }
    }
}
