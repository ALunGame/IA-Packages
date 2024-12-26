using IANodeGraph.Model;
using System.Collections.Generic;
using System.Linq;

namespace IANodeGraph
{
    /// <summary>
    /// 节点辅助方法
    /// </summary>
    public static class NodeHelper
    {
        /// <summary>
        /// 获得指定类型的节点
        /// </summary>
        /// <param name="baseGraph"></param>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        public static List<T> GetNodes<T>(BaseGraph baseGraph) where T:BaseNode
        {
            List<T> nodes = new List<T>();  
            foreach (var item in baseGraph.nodes)
            {
                if (item.Value is T)
                {
                    nodes.Add((T)item.Value);
                }
            }
            return nodes;
        }

        /// <summary>
        /// 获得节点的输出节点
        /// </summary>
        /// <param name="baseGraph"></param>
        /// <param name="checkNode"></param>
        /// <returns></returns>
        public static List<T> GetNodeOutNodes<T>(BaseGraph baseGraph, BaseNode checkNode, string portName = "") where T : BaseNode
        {
            List<T> nodes = new List<T>();
            List<BaseNode> childNodes = GetNodeOutNodes(baseGraph,checkNode, portName);
            for (int i = 0; i < childNodes.Count; i++)
            {
                if (childNodes[i] is T)
                {
                    nodes.Add((T)childNodes[i]);
                }
            }
            return nodes;
        }

        /// <summary>
        /// 获得节点的输出节点
        /// </summary>
        /// <param name="baseGraph"></param>
        /// <param name="checkNode"></param>
        /// <returns></returns>
        public static List<BaseNode> GetNodeOutNodes(BaseGraph baseGraph, BaseNode checkNode, string portName = "")
        {
            List<BaseNode> childNodes = new List<BaseNode>();
            for (int i = 0; i < baseGraph.connections.Count; i++)
            {
                BaseConnection connection = baseGraph.connections[i];
                if (connection.from == checkNode.guid && (string.IsNullOrEmpty(portName)|| connection.fromPortName == portName))
                {
                    BaseNode inputNode = baseGraph.nodes.First(v => v.Value.guid == connection.to).Value;
                    if (inputNode != null)
                    {
                        childNodes.Add(inputNode);
                    }
                }
            }
            childNodes.Sort((a, b) =>
            {
                if (a.inIndex == b.inIndex)
                    return 1;
                if (a.inIndex > b.inIndex)
                    return 1;
                return -1;
            });
            return childNodes;
        }


        /// <summary>
        /// 获得节点的输出节点
        /// </summary>
        /// <param name="baseGraph"></param>
        /// <param name="checkNode"></param>
        /// <returns></returns>
        public static List<T> GetNodeInNodes<T>(BaseGraph baseGraph, BaseNode checkNode, string portName = "") where T : BaseNode
        {
            List<T> nodes = new List<T>();
            List<BaseNode> childNodes = GetNodeInNodes(baseGraph, checkNode, portName);
            for (int i = 0; i < childNodes.Count; i++)
            {
                if (childNodes[i] is T)
                {
                    nodes.Add((T)childNodes[i]);
                }
            }
            return nodes;
        }

        /// <summary>
        /// 获得节点的输入节点
        /// </summary>
        /// <param name="baseGraph"></param>
        /// <param name="checkNode"></param>
        /// <returns></returns>
        public static List<BaseNode> GetNodeInNodes(BaseGraph baseGraph, BaseNode checkNode, string portName = "")
        {
            List<BaseNode> childNodes = new List<BaseNode>();
            for (int i = 0; i < baseGraph.connections.Count; i++)
            {
                BaseConnection connection = baseGraph.connections[i];
                if (connection.to == checkNode.guid && (string.IsNullOrEmpty(portName) || connection.toPortName == portName))
                {
                    BaseNode outputNode = baseGraph.nodes.First(v => v.Value.guid == connection.from).Value;
                    if (outputNode != null)
                    {
                        childNodes.Add(outputNode);
                    }
                }
            }
            childNodes.Sort((a, b) =>
            {
                if (a.outIndex == b.outIndex)
                    return 1;
                if (a.outIndex > b.outIndex)
                    return 1;
                return -1;
            });
            return childNodes;
        }
    }
}
