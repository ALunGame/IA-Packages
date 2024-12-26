using IAECS.Core.Tree.Base;
using IAECS.Core.Tree.Nodes;
using IANodeGraph;
using IANodeGraph.Model;
using System.Collections.Generic;
using UnityEngine;

namespace IAECS.Tree
{
    public class PremiseData { }
    public class NodeData { }

    public abstract class Tree_BaseNode : BaseNode
    {
        [InputPort("前提", BasePort.Capacity.Multi, setIndex = true)]
        public PremiseData premise;

        private NodePremise CreatePremise()
        {
            NodePremise genNodePremise(Tree_PremiseNode preNode, NodePremise lastPre)
            {
                NodePremise nodePremise = preNode.GetRuntimePremise(this);
                if (lastPre!=null)
                    lastPre.otherPremise = nodePremise;
                return nodePremise;
            }

            List<Tree_PremiseNode> nodes = NodeHelper.GetNodeInNodes<Tree_PremiseNode>(Owner, this);
            if (nodes.Count<=0)
                return null;
            NodePremise startPremise = null;
            NodePremise premise = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i == 0)
                {
                    startPremise = genNodePremise(nodes[i], null);
                    premise = startPremise;
                }
                else
                {
                    Tree_PremiseNode nextPreNode = nodes[i];
                    premise = genNodePremise(nextPreNode, premise);
                }
            }
            return startPremise;
        }

        public Node GetRuntimeNode()
        {
            Node node = CreateRuntimeNode();
            node.Uid = guid;
            node.nodePremise = CreatePremise();
            node.childNodes = CreateChildNodes();
            return node;
        }

        /// <summary>
        /// 创建运行时节点
        /// </summary>
        /// <returns></returns>
        public abstract Node CreateRuntimeNode();

        /// <summary>
        /// 创建子节点
        /// </summary>
        /// <returns></returns>
        public List<Node> CreateChildNodes()
        {
            List<Tree_BaseNode> nodes = NodeHelper.GetNodeOutNodes<Tree_BaseNode>(Owner, this);
            if (nodes.Count <= 0)
                return null;
            List<Node> childNodes = new List<Node>();
            for (int i = 0; i < nodes.Count; i++)
            {
                childNodes.Add(nodes[i].GetRuntimeNode());
            }
            return childNodes;
        }
    }

    [NodeMenuItem("入口")]
    public class Tree_RootNode : Tree_BaseNode
    {
        public override string Title { get => "入口"; set => base.Title = value; }
        public override string Tooltip { get => "入口"; set => base.Tooltip = value; }
        public override Color TitleColor { get => Color.white; set => base.TitleColor = value; }

        [OutputPort("子节点", BasePort.Capacity.Single, BasePort.Orientation.Vertical)]
        public NodeData childNode;

        public override Node CreateRuntimeNode()
        {
            NodeRoot nodeRoot = new NodeRoot();
            return nodeRoot;
        }
    }
}
