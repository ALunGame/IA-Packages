using IAECS.Core.Tree.Base;

namespace IAECS.Core.Tree.Nodes.Action
{
    /// <summary>
    /// 延时行为            （延时指定时间后，退出）
    /// </summary>
    public class NodeActionDelay : NodeAction
    {
        class NodeActionDelayContext : NodeContext
        {
            //计时器
            internal float Timer;

            public NodeActionDelayContext()
            {
                Timer = 0;
            }
        }

        public float WaitTime = 0;

        protected override void OnEnter(NodeData wData)
        {
            NodeActionDelayContext context = GetContext<NodeActionDelayContext>(wData);
            context.Timer = NodeTime.TotalTime;
        }

        protected override int OnRunning(NodeData wData)
        {
            NodeActionDelayContext context = GetContext<NodeActionDelayContext>(wData);
            if (NodeTime.TotalTime - context.Timer > WaitTime)
                return NodeState.FINISHED;
            return NodeState.EXECUTING;
        }

        protected override void OnExit(NodeData wData, int runningStatus)
        {
            NodeActionDelayContext context = GetContext<NodeActionDelayContext>(wData);
            context.Timer = 0;
        }
    }
}
