using IAECS.Core.Tree;
using IAECS.Core.Tree.Base;
using IAECS.Core.Tree.Nodes.Control;
using IANodeGraph;
using IANodeGraph.Model;
using UnityEngine;

namespace IAECS.Tree
{
    public abstract class Tree_ControlNode : Tree_BaseNode
    {
        public override Color TitleColor { get => Color.magenta; set => base.TitleColor = value; }

        [InputPort("父节点", BasePort.Capacity.Single, BasePort.Orientation.Vertical)]
        public NodeData parentNode;
    }

    [NodeMenuItem("控制/循环")]
    public class Tree_LoopNode : Tree_ControlNode
    {
        public override string Title { get => "循环"; set => base.Title = value; }
        public override string Tooltip { get => "循环执行子节点"; set => base.Tooltip = value; }

        [OutputPort("子节点", BasePort.Capacity.Single, BasePort.Orientation.Vertical)]
        public NodeData childNode;

        [NodeValue("循环次数")]
        public int loopCnt;

        public override Node CreateRuntimeNode()
        {
            NodeControlLoop controlLoop = new NodeControlLoop();
            controlLoop.loopCount = loopCnt;    
            return controlLoop;
        }
    }

    [NodeMenuItem("控制/并行")]
    public class Tree_ParallelNode : Tree_ControlNode
    {
        public override string Title { get => "并行"; set => base.Title = value; }

        public override string Tooltip { get => "并行执行子节点"; set => base.Tooltip = value; }

        [OutputPort("子节点", BasePort.Capacity.Multi, BasePort.Orientation.Vertical, setIndex = true)]
        public NodeData childNodes;

        [NodeValue("检测类型", "AND:全部通过 OR:一个通过")]
        public NodeParallelType evaluateType = NodeParallelType.AND;

        [NodeValue("完成类型", "AND:全部结束 OR:一个结束")]
        public NodeParallelType excuteType = NodeParallelType.AND;

        public override Node CreateRuntimeNode()
        {
            NodeControlParallel controlParallel = new NodeControlParallel();
            controlParallel.evaluateType = evaluateType;
            controlParallel.excuteType = excuteType;
            return controlParallel;
        }
    }

    [NodeMenuItem("控制/随机选择")]
    public class Tree_RandomSelectorNode : Tree_ControlNode
    {
        public override string Title { get => "随机选择"; set => base.Title = value; }

        public override string Tooltip { get => "随机选择执行其中一个子节点"; set => base.Tooltip = value; }

        [OutputPort("子节点", BasePort.Capacity.Multi, BasePort.Orientation.Vertical, setIndex = true)]
        public NodeData childNodes;

        public override Node CreateRuntimeNode()
        {
            NodeControlRandomSelector randomSelector = new NodeControlRandomSelector();
            return randomSelector;
        }
    }

    [NodeMenuItem("控制/顺序选择")]
    public class Tree_SelectorNode : Tree_ControlNode
    {
        public override string Title { get => "顺序选择"; set => base.Title = value; }

        public override string Tooltip { get => "顺序选择执行其中一个子节点"; set => base.Tooltip = value; }

        [OutputPort("子节点", BasePort.Capacity.Multi, BasePort.Orientation.Vertical, setIndex = true)]
        public NodeData childNodes;

        public override Node CreateRuntimeNode()
        {
            NodeControlSelector controlSelector = new NodeControlSelector();    
            return controlSelector;
        }
    }


    [NodeMenuItem("控制/顺序执行")]
    public class Tree_SequenceNode : Tree_ControlNode
    {
        public override string Title { get => "顺序执行"; set => base.Title = value; }

        public override string Tooltip { get => "顺序执行子节点"; set => base.Tooltip = value; }

        [OutputPort("子节点", BasePort.Capacity.Multi, BasePort.Orientation.Vertical, setIndex = true)]
        public NodeData childNodes;

        public override Node CreateRuntimeNode()
        {
            NodeControlSequence controlSequence = new NodeControlSequence();
            return controlSequence;
        }
    }
}
