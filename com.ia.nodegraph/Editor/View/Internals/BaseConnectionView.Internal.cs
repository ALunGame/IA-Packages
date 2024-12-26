using IANodeGraph.Model;
using IANodeGraph.View.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;


namespace IANodeGraph.View
{
    /// <summary>
    /// 连接显示
    /// </summary>
    public partial class BaseConnectionView : Edge, IBindableView<BaseConnectionVM>
    {
        public BaseConnectionVM Model { get; private set; }
        protected BaseGraphView Owner { get; private set; }

        public BaseConnectionView() : base()
        {
            styleSheets.Add(GraphProcessorStyles.EdgeViewStyle);
            this.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        }

        public void SetUp(BaseConnectionVM connection, BaseGraphView graphView)
        {
            Model = connection;
            Owner = graphView;
        }

        public virtual void UnBindingProperties()
        {

        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            this.BringToFront();
        }
    }
}
