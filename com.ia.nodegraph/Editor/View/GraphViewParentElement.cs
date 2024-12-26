using UnityEngine.UIElements;

namespace IANodeGraph.View
{
    /// <summary>
    /// 视图父元素
    /// </summary>
    public class GraphViewParentElement : VisualElement
    {
        public ToolbarView Toolbar { get; private set; }
        public VisualElement GraphViewElement { get; private set; }

        public GraphViewParentElement()
        {
            name = "GraphViewParent";

            Toolbar = new ToolbarView();
            Toolbar.style.height = 20;
            Toolbar.style.flexGrow = 1;
            Toolbar.StretchToParentWidth();
            Add(Toolbar);

            GraphViewElement = new VisualElement();
            GraphViewElement.name = "GraphView";
            GraphViewElement.StretchToParentSize();
            GraphViewElement.style.top = Toolbar.style.height;
            Add(GraphViewElement);
        }
    }
}
