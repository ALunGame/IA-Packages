using System;
using System.Collections.Generic;
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
        /// 返回视图中可以出现的节点
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<Type> GetNodeTypes();
    }
}
