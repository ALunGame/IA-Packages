using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IANodeGraph
{
    public abstract class BaseGraph
    {
        public float zoom = 1;
        public Vector2Int pan = new Vector2Int(0, 0);

        public List<BaseNode> nodes = new List<BaseNode>();
        public List<BaseConnection> connections = new List<BaseConnection>();
        public List<BaseGroup> groups = new List<BaseGroup>();
        public List<StickyNote> notes = new List<StickyNote>();

        /// <summary>
        /// 返回节点是否显示
        /// </summary>
        /// <param name="pNodeType"></param>
        /// <returns></returns>
        public abstract bool CheckNodeDisplay(Type pNodeType);

        /// <summary>
        /// 获得所有类型的节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetNodes<T>() where T : BaseNode
        {
            List<T> resNodes = new List<T>();
            foreach (var node in nodes)
            {
                if (node is T baseNode)
                {
                    resNodes.Add(baseNode);
                }
            }
            return resNodes;
        }

        /// <summary>
        /// 获取连接指定端口的节点
        /// </summary>
        /// <param name="pNode">检测的节点</param>
        /// <param name="pPortName">检测的端口名</param>
        /// <returns></returns>
        public List<BaseNode> GetConnectionNodes(BaseNode pNode, string pPortName)
        {
            List<int> nodeIds = new();
            foreach (var connect in connections)
            {
                if (connect.fromNode == pNode.id && connect.fromPort == pPortName)
                {
                    if (!nodeIds.Contains(connect.toNode))
                    {
                        nodeIds.Add(connect.toNode);
                    }
                }
                if (connect.toNode == pNode.id && connect.toPort == pPortName)
                {
                    if (!nodeIds.Contains(connect.fromNode))
                    {
                        nodeIds.Add(connect.fromNode);
                    }
                }
            }

            return nodes.Where(x => nodeIds.Contains(x.id)).ToList();
        }


        /// <summary>
        /// 获取连接指定端口的节点
        /// </summary>
        /// <param name="pNode">检测的节点</param>
        /// <param name="pPortName">检测的端口名</param>
        /// <returns></returns>
        public List<T> GetConnectionNodes<T>(BaseNode pNode, string pPortName) where T : BaseNode
        {
            List<BaseNode> tNodes = GetConnectionNodes(pNode, pPortName);
            return tNodes.Where(x => x is T).Cast<T>().ToList();
        }
        
        /// <summary>
        /// 获取连接指定端口的节点
        /// </summary>
        /// <param name="pNode">检测的节点</param>
        /// <param name="pPortName">检测的端口名</param>
        /// <returns></returns>
        public T GetConnectionNode<T>(BaseNode pNode, string pPortName) where T : BaseNode
        {
            List<T> tNodes = GetConnectionNodes<T>(pNode, pPortName);
            return tNodes.Count <= 0 ? null : tNodes[0];
        }

        /// <summary>
        /// 获取连接指定端口的节点
        /// </summary>
        /// <param name="pNode">检测的节点</param>
        /// <param name="pPortName">检测的端口名</param>
        /// <param name="outNode"></param>
        /// <returns></returns>
        public bool GetConnectionNode<T>(BaseNode pNode, string pPortName, out T outNode) where T : BaseNode
        {
            List<T> tNodes = GetConnectionNodes<T>(pNode, pPortName);
            if (tNodes.Count <= 0)
            {
                outNode = null;
                return false;
            }
            else
            {
                outNode = tNodes[0];
                return true;
            }
        }
    }
}
