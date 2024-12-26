using IAECS.Core.Tree.Base;

namespace IAECS.Core.Tree.Nodes
{
    /// <summary>
    /// 根节点             (只能放一个子节点)
    /// </summary>
    public class NodeRoot : Node
    {
        protected override bool OnEvaluate(NodeData wData)
        {
            if (IsIndexValid(0))
            {
                Node node = GetChild<Node>(0);
                return node.Evaluate(wData);
            }
            return false;
        }

        protected override int OnExcute(NodeData wData)
        {
            if (IsIndexValid(0))
            {
                Node node = GetChild<Node>(0);
                return node.Execute(wData);
            }
            return base.OnExcute(wData);
        }

        protected override void OnTransition(NodeData wData)
        {
            if (IsIndexValid(0))
            {
                Node node = GetChild<Node>(0);
                node.Transition(wData);
            }
        }
    }
}
