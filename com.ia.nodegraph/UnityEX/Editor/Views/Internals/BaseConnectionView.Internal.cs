using IANodeGraph.Editors;
using IANodeGraph.Model;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace IANodeGraph.View
{
    /// <summary>
    /// 连接显示
    /// </summary>
    public partial class BaseConnectionView : Edge, IGraphElementView<BaseConnectionProcessor>
    {
        public BaseConnectionProcessor ViewModel { get; private set; }
        public IGraphElementProcessor V => ViewModel;
        protected BaseGraphView Owner { get; private set; }

        public BaseConnectionView()
        {
            styleSheets.Add(GraphProcessorEditorStyles.DefaultStyles.BaseConnectionViewStyle);
            this.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        }

        public void SetUp(BaseConnectionProcessor connection, BaseGraphView graphView)
        {
            ViewModel = connection;
            Owner = graphView;
            OnInitialized();
        }

        public void OnCreate()
        {
            this.RegisterCallback<ClickEvent>(OnClick);

            BindProperties();
        }

        public void OnDestroy()
        {
            this.UnregisterCallback<ClickEvent>(OnClick);

            UnbindProperties();
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            this.BringToFront();
        }

        private void OnClick(ClickEvent evt)
        {
            if (evt.clickCount == 2)
            {

            }
        }
    }
}
