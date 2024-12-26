using System;
using System.Collections.Generic;
using UnityEngine;

namespace IANodeGraph.Model
{
    public partial class BaseGraph
    {
        //位置
        public Vector3 pos = Vector3.zero;
        //缩放
        public Vector3 zoom = Vector3.one;

        //节点列表
        public Dictionary<string, BaseNode> nodes = new Dictionary<string, BaseNode>();
        
        //组
        public List<BaseGroup> groups = new List<BaseGroup>();

        //连接
        public List<BaseConnection> connections = new List<BaseConnection>();
    }
}
