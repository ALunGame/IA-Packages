using IANodeGraph.Editors;
using IANodeGraph.Model;
using IAToolkit;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace IANodeGraph.View
{
    /// <summary>
    /// 端口显示
    /// </summary>
    public abstract partial class BasePortView : Port
    {
        public Image Icon { get; }
        public VisualElement CapIconBG { get; }
        public VisualElement CapIcon { get; }
        public Label PortLabel { get; }
        public VisualElement Connector { get; }
        public VisualElement ConnectorCap { get; }
        public VisualElement InputContainer { get; private set; }
        public BaseGraphView GraphView { get; private set; }
        public BasePortProcessor ViewModel { get; private set; }
        public Dictionary<BaseConnectionProcessor, BaseConnectionView> ConnectionViews { get; private set; }

        protected BasePortView(Orientation orientation, Direction direction, Capacity capacity, Type type, IEdgeConnectorListener connectorListener) : base(orientation, direction, capacity, type)
        {
            styleSheets.Add(GraphProcessorEditorStyles.DefaultStyles.BasePortViewStyle);

            portType = type;

            visualClass = "port-" + portType.Name;
            this.AddToClassList("capacity-" + capacity.ToString());

            PortLabel = this.Q("type") as Label;
            Connector = this.Q("connector");
            ConnectorCap = Connector.Q("connectorCap");

            Icon = new Image();
            Icon.AddToClassList("port-icon");
            Insert(1, Icon);

            CapIconBG = new VisualElement();
            CapIconBG.name = "cap-icon-bg";
            Connector.Add(CapIconBG);

            CapIcon = new VisualElement();
            CapIcon.name = "cap-icon";
            CapIcon.pickingMode = PickingMode.Ignore;
            Connector.Add(CapIcon);

            PortLabel.pickingMode = PickingMode.Position;
            Connector.pickingMode = PickingMode.Position;
            bool vertical = orientation == Orientation.Vertical;
            if (vertical)
            {
                PortLabel.style.display = DisplayStyle.None;
                AddToClassList("vertical");
            }

            m_EdgeConnector = new EdgeConnector<BaseConnectionView>(connectorListener);
            ConnectionViews = new Dictionary<BaseConnectionProcessor, BaseConnectionView>();
            this.AddManipulator(m_EdgeConnector);
        }

        public void SetUp(BasePortProcessor port, BaseGraphView graphView)
        {
            ViewModel = port;
            GraphView = graphView;

            portName = ViewModel.Name;
            tooltip = ViewModel.ToolTip;

            if (ViewModel.HideLabel.Value)
                PortLabel.AddToClassList("hidden");

            CreateDrawerNodeValue();

            OnInitialized();
        }

        public void OnCreate()
        {
            RegisterValueChangedEvents();

            OnBindingProperties();
        }

        private void RegisterValueChangedEvents()
        {
            ViewModel.portType.RegisterChanged(OnPortTypeChange);
            ViewModel.HideLabel.RegisterChanged(OnHideLabelChange);
        }

        public void OnDestroy()
        {
            UnRegisterValueChangedEvents();

            OnUnBindingProperties();
        }

        private void UnRegisterValueChangedEvents()
        {
            ViewModel.portType.UnregisterChanged(OnPortTypeChange);
            ViewModel.HideLabel.UnregisterChanged(OnHideLabelChange);
        }

        private void CreateDrawerNodeValue()
        {
            if (ViewModel.Model.portType == null)
            {
                return;
            }
            if (!ViewModel.Model.showDrawer)
            {
                return;
            }
            if (ViewModel.Model.direction == Model.PortDirection.Top 
                || ViewModel.Model.direction == Model.PortDirection.Bottom)
            {
                Debug.LogError($"不支持上下方向的抽屉布局:{ViewModel.Owner.Model.GetType()}");
                return;
            }

            //创建元素
            InputContainer = new VisualElement { name = "input-container" };
            Connector.parent.Add(InputContainer);
            InputContainer.SendToBack();
            InputContainer.pickingMode = PickingMode.Ignore;

            if (ViewModel.Model.direction == Model.PortDirection.Right)
            {
                styleSheets.Add(GraphProcessorEditorStyles.DefaultStyles.BasePortViewStyle_RightInputContainer);
            }
            else if (ViewModel.Model.direction == Model.PortDirection.Left)
            {
                styleSheets.Add(GraphProcessorEditorStyles.DefaultStyles.BasePortViewStyle_LeftInputContainer);
            }

            FieldInfo fieldInfo = ViewModel.Owner.Model.GetType().GetField(ViewModel.Model.fieldName);
            VisualElement element = ElementExtension.DrawField("", ViewModel.Model.portType, fieldInfo.GetValue(ViewModel.Owner.Model), (object var) =>
            {
                GraphView.CommandDispatcher.Do(new ChangeNodeValueCommand(ViewModel.Owner.Model, fieldInfo, var, () =>
                {

                }, () =>
                {

                }));
            });
            if (element != null)
            {
                var box = new VisualElement { name = ViewModel.Model.fieldName };
                box.AddToClassList("port-input-element");
                box.Add(element);
                box.style.display = DisplayStyle.Flex;
                InputContainer.Add(box);
            }
        }

        #region Callback

        private void OnPortTypeChange(Type pNewType, Type pOldType)
        {
            this.portType = pNewType;
        }

        private void OnHideLabelChange(bool pNewHideLabel, bool pOldHideLabel)
        {
            if (pNewHideLabel)
                PortLabel.AddToClassList("hidden");
            else
                PortLabel.RemoveFromClassList("hidden");
        }

        #endregion

        public void Connect(BaseConnectionView connection)
        {
            base.Connect(connection);
            if (connection is BaseConnectionView connectionView)
            {
                ConnectionViews[connectionView.ViewModel] = connectionView;
            }
            if (InputContainer != null)
            {
                InputContainer.style.display = DisplayStyle.None;
            }
        }

        public void Disconnect(BaseConnectionView connection)
        {
            base.Disconnect(connection);
            if (connection is BaseConnectionView connectionView)
            {
                ConnectionViews.Remove(connectionView.ViewModel);
            }
            if (ConnectionViews.Count <= 0)
            {
                if (InputContainer != null)
                {
                    InputContainer.style.display = DisplayStyle.Flex;
                }
            }
        }

        protected virtual void OnInitialized()
        {
        }

        protected virtual void OnBindingProperties()
        {
        }

        protected virtual void OnUnBindingProperties()
        {
        }

        #region 不建议使用

        /// <summary>
        /// 不建议使用
        /// </summary>
        /// <param name="edge"></param>
        public sealed override void Connect(Edge edge)
        {
            base.Connect(edge);
        }

        /// <summary>
        /// 不建议使用
        /// </summary>
        /// <param name="edge"></param>
        public sealed override void Disconnect(Edge edge)
        {
            base.Connect(edge);
        }

        #endregion
    }
}
