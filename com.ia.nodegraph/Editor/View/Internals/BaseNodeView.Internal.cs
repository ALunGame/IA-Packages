using IANodeGraph.Model;
using IANodeGraph.View.Utils;
using IAToolkit;
using IAToolkit.ViewModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using IAEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using NodeView = UnityEditor.Experimental.GraphView.Node;

namespace IANodeGraph.View
{
    /// <summary>
    /// 节点显示
    /// </summary>
    public partial class BaseNodeView : NodeView, IBindableView<BaseNodeVM>
    {
        #region 字段
        Label titleLabel;
        public readonly VisualElement nodeBorder;
        public readonly VisualElement topPortContainer;
        public readonly VisualElement bottomPortContainer;
        public readonly VisualElement controlsContainer;
        public readonly VisualElement contentsHorizontalDivider;
        public readonly VisualElement portsVerticalDivider;
        public readonly VisualElement controlsHorizontalDivider;
        public readonly Dictionary<string, BasePortView> portViews = new Dictionary<string, BasePortView>();
        [NonSerialized]
        private List<IconBadge> badges = new List<IconBadge>();
        [NonSerialized]
        private Dictionary<FieldInfo, NodeValueAttribute> nodeValues = new Dictionary<FieldInfo, NodeValueAttribute>();
        [NonSerialized]
        private Dictionary<VisualElement, FieldInfo> nodeValueElements = new Dictionary<VisualElement, FieldInfo>();
        #endregion

        #region 属性
        public Label TitleLabel
        {
            get
            {
                if (titleLabel == null)
                    titleLabel = titleContainer.Q<Label>("title-label");
                return titleLabel;
            }
        }
        public BaseGraphView Owner { get; private set; }
        public BaseNodeVM Model { get; protected set; }
        #endregion

        public BaseNodeView()
        {
            styleSheets.Add(GraphProcessorStyles.BaseNodeViewStyle);
            styleSheets.Add(GraphProcessorStyles.PortViewTypesStyle);

            nodeBorder = this.Q(name: "node-border");

            contentsHorizontalDivider = contentContainer.Q(name: "divider", className: "horizontal");
            contentsHorizontalDivider.AddToClassList("contents-horizontal-divider");

            portsVerticalDivider = topContainer.Q(name: "divider", className: "vertical");
            portsVerticalDivider.AddToClassList("ports-vertical-divider");

            controlsContainer = new VisualElement { name = "controls" };
            controlsContainer.AddToClassList("node-controls");
            mainContainer.Add(controlsContainer);

            topPortContainer = new VisualElement { name = "top-port-container" };
            topPortContainer.style.justifyContent = Justify.Center;
            topPortContainer.style.alignItems = Align.Center;
            topPortContainer.style.flexDirection = FlexDirection.Row;
            Insert(0, topPortContainer);

            bottomPortContainer = new VisualElement { name = "bottom-port-container" };
            bottomPortContainer.style.justifyContent = Justify.Center;
            bottomPortContainer.style.alignItems = Align.Center;
            bottomPortContainer.style.flexDirection = FlexDirection.Row;
            Add(bottomPortContainer);

            TitleLabel.style.flexWrap = Wrap.Wrap;
        }

        #region Initialization

        public void SetUp(BaseNodeVM node, BaseGraphView graphView)
        {
            Model = node;
            Owner = graphView;

            // 初始化
            base.SetPosition(new Rect(Model.Position == default ? Vector2.zero : Model.Position, GetPosition().size));
            OnTitleChanged(Model.Model.Title);
            tooltip = Model.Model.Tooltip;
            titleContainer.style.backgroundColor = Model.Model.TitleColor;
            TitleLabel.style.color = Model.Model.TitleColor.GetLuminance() > 0.5f && Model.Model.TitleColor.a > 0.5f ? Color.black : Color.white * 0.9f;

            //创建节点内部值
            CreateDrawerValues();

            //刷新端口
            InitPorts();
            RefreshPorts();

            //绑定事件
            BindingProperties();
            //子类初始化
            OnInitialized();
        }

        #endregion

        #region 点击菜单

        public virtual void CreateSelectMenu(DropdownMenu dropdownMenu)
        {

        }

        #endregion

        #region 端口初始化

        private void InitPorts()
        {
            void AddPortView(BasePortVM port)
            {
                if (port == null)
                    return;
                BasePortView portView = NewPortView(port);
                portView.SetUp(port, Owner);
                portViews[port.Model.name] = portView;

                if (portView.orientation == Orientation.Horizontal)
                {
                    if (portView.direction == Direction.Input)
                        inputContainer.Add(portView);
                    else
                        outputContainer.Add(portView);
                }
                else
                {
                    if (portView.direction == Direction.Input)
                        topPortContainer.Add(portView);
                    else
                        bottomPortContainer.Add(portView);
                }
            }

            //自定义端口
            foreach (var port in Model.Ports.Values)
            {
                AddPortView(port);
            }
        }

        #endregion

        #region 抽屉显示字段

        private void CreateDrawerValues()
        {
            int index = 0;
            foreach (FieldInfo item in ReflectionHelper.GetFieldInfos(Model.ModelType))
            {
                if (AttributeHelper.TryGetFieldAttribute(item, out NodeValueAttribute nodeValueAttribute))
                {
                    VisualElement element = ElementExtension.DrawField(nodeValueAttribute.Lable,item.FieldType, item.GetValue(Model.Model), (object var) =>
                    {
                        Owner.CommandDispacter.Do(new ChangeNodeValueCommand(Model, item, var, () =>
                        {
                            OnDrawerValuesChange();
                        }, () => {
                            RefreshDrawerValues();
                        }));
                    });
                    if (element!=null)
                    {
                        element.tooltip = nodeValueAttribute.Tooltip;
                        index++;
                        if (index % 2 == 0)
                            outputContainer.Add(element);
                        else
                            inputContainer.Add(element);

                        //保存
                        nodeValues.Add(item, nodeValueAttribute);
                        nodeValueElements.Add(element, item);
                    }

                    //自动绑定
                    ViewModel model = Model;
                    model[nodeValueAttribute.Lable] = new BindableProperty(() => item.GetValue(Model.Model), (object value) => {
                        item.SetValue(Model.Model, value);
                        RefreshDrawerValues();
                    }, nodeValueAttribute.Tooltip);
                }
            }
        }

        public void RefreshDrawerValues()
        {
            foreach (var item in nodeValueElements)
            {
                ElementExtension.SetFieldValue(item.Key, item.Value.GetValue(Model.Model));
            }
            OnDrawerValuesChange();
        }

        #endregion

        #region 数据监听
        protected virtual void BindingProperties()
        {
            Model.Model.OnTitleChanged += OnTitleChanged;
            Model.Model.OnTitleColorChanged += OnTitleColorChanged;
            Model.Model.OnTooltipChanged += OnTooltipChanged;

            Model.BindingProperty<Vector2>(BaseNodeVM.POSITION_NAME, OnPositionChanged);

            Model.onPortAdded += OnPortAdded;
            Model.onPortRemoved += OnPortRemoved;
        }

        public virtual void UnBindingProperties()
        {
            Model.ClearBindingEvent();
            Model.onPortAdded -= OnPortAdded;
            Model.onPortRemoved -= OnPortRemoved;
        }

        /// <summary>
        /// 当抽屉值改变
        /// </summary>
        protected virtual void OnDrawerValuesChange()
        {

        }

        void OnPortAdded(BasePortVM port)
        {
            BasePortView portView = NewPortView(port);
            portView.SetUp(port, Owner);
            portViews[port.Model.name] = portView;

            if (portView.orientation == Orientation.Horizontal)
            {
                if (portView.direction == Direction.Input)
                    inputContainer.Add(portView);
                else
                    outputContainer.Add(portView);
            }
            else
            {
                if (portView.direction == Direction.Input)
                    topPortContainer.Add(portView);
                else
                    bottomPortContainer.Add(portView);
            }
            RefreshPorts();
        }

        void OnPortRemoved(BasePortVM port)
        {
            portViews[port.Model.name].RemoveFromHierarchy();
            portViews.Remove(port.Model.name);
            RefreshPorts();
        }

        void OnTitleChanged(string title)
        {
            if (Model.Model.inIndex != -1)
            {
                title = $"({Model.Model.inIndex}) {title}";
            }

            if (Model.Model.outIndex != -1)
            {
                title = $"{title} ({Model.Model.outIndex})";
            }

            base.title = title;
        }
        void OnTooltipChanged(string title)
        {
            base.tooltip = tooltip;
        }
        void OnPositionChanged(Vector2 position)
        {
            base.SetPosition(new Rect(position, GetPosition().size));
            Owner.SetDirty();
        }
        void OnTitleColorChanged(Color color)
        {
            titleContainer.style.backgroundColor = color;
            TitleLabel.style.color = color.GetLuminance() > 0.5f && color.a > 0.5f ? Color.black : Color.white * 0.9f;
        }
        #endregion

        #region 方法
        public void HighlightOn()
        {
            nodeBorder.AddToClassList("highlight");
        }

        public void HighlightOff()
        {
            nodeBorder.RemoveFromClassList("highlight");
        }

        public void Flash()
        {
            HighlightOn();
            schedule.Execute(_ => { HighlightOff(); }).ExecuteLater(2000);
        }

        public void SetDeletable(bool deletable)
        {
            if (deletable)
                capabilities |= Capabilities.Deletable;
            else
                capabilities &= ~Capabilities.Deletable;
        }

        public void SetMovable(bool movable)
        {
            if (movable)
                capabilities |= Capabilities.Movable;
            else
                capabilities &= ~Capabilities.Movable;
        }

        public void SetSelectable(bool selectable)
        {
            if (selectable)
                capabilities |= Capabilities.Selectable;
            else
                capabilities &= ~Capabilities.Selectable;
        }

        public void AddBadge(IconBadge badge)
        {
            Add(badge);
            badges.Add(badge);
            badge.AttachTo(topContainer, SpriteAlignment.RightCenter);
        }


        public void RemoveBadge(Func<IconBadge, bool> callback)
        {
            badges.RemoveAll(b =>
            {
                if (callback(b))
                {
                    b.Detach();
                    b.RemoveFromHierarchy();
                    return true;
                }
                return false;
            });
        }
        #endregion
    }
}
