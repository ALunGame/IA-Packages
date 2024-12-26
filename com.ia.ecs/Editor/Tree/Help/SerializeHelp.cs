using IAECS.Core.Tree.Base;
using IANodeGraph;
using IANodeGraph.Model;
using System.Collections.Generic;
using UnityEngine;

namespace IAECS.Tree
{
    /// <summary>
    /// 序列化辅助
    /// </summary>
    public static class SerializeHelp
    {
        public static Node SerializeToTree(BaseGraph graph)
        {
            List<Tree_RootNode> rootNodes = NodeHelper.GetNodes<Tree_RootNode>(graph);
            if (rootNodes.Count <= 0)
            {
                Debug.LogError($"试图序列化出错，没有根节点");
            }
            Node rootNode = rootNodes[0].GetRuntimeNode();
            return rootNode;
        }
        
        
        public static List<Node> SerializeToTrees(BaseGraph graph)
        {
            List<Tree_RootNode> rootNodes = NodeHelper.GetNodes<Tree_RootNode>(graph);
            if (rootNodes.Count <= 0)
            {
                Debug.LogError($"试图序列化出错，没有根节点");
                return null;
            }

            List<Node> trees = new List<Node>();
            for (int i = 0; i < rootNodes.Count; i++)
            {
                trees.Add(rootNodes[i].GetRuntimeNode());
            }
            return trees;
        }
    }
}
