using IANodeGraph.Model;
using IANodeGraph.View.Utils;
using UnityEngine.UIElements;

namespace IANodeGraph.View
{
    public abstract class BaseSimpleNodeView<M> : BaseNodeView<M> where M : BaseNode
    {
        protected BaseSimpleNodeView() : base()
        {
            styleSheets.Add(GraphProcessorStyles.SimpleNodeViewStyle);
            m_CollapseButton.style.display = DisplayStyle.None;
        }
    }

    public class BaseSimpleNodeView : BaseSimpleNodeView<BaseNode> { }
}
