using IAECS.Core;
using IANodeGraph;
using IANodeGraph.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAECS.EntityGraph
{
    public class EntityComData { }

    public abstract class Entity_ComNode : BaseNode
    {
        public override Color TitleColor { get => Color.magenta; set => base.TitleColor = value; }

        [InputPort("父节点", BasePort.Capacity.Single, BasePort.Orientation.Vertical)]
        public EntityComData parentNode;

        [NodeValue("是否开启")]
        public bool isActive = true;

        public abstract Type RuntimeNode { get; }

        public BaseCom GetRuntimeNode()
        {
            BaseCom baseCom = CreateRuntimeNode();
            baseCom.isActive = isActive;
            return baseCom;
        }

        public abstract BaseCom CreateRuntimeNode();
    }

    [NodeMenuItem("实体配置")]
    public class Entity_Node : BaseNode
    {
        public override string Title { get => "实体配置"; set => base.Title = value; }
        public override string Tooltip { get => "实体配置"; set => base.Tooltip = value; }
        public override Color TitleColor { get => Color.white; set => base.TitleColor = value; }

        [OutputPort("组件列表", BasePort.Capacity.Multi, BasePort.Orientation.Vertical,setIndex = true)]
        public EntityComData coms;

        public List<BaseCom> GetModel()
        {
            List<BaseCom> coms = new List<BaseCom>();
            //组件节点
            List<Entity_ComNode> nodes = NodeHelper.GetNodeOutNodes<Entity_ComNode>(Owner, this);
            if (nodes.Count > 0)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    coms.Add(nodes[i].GetRuntimeNode());
                }
            }
            return coms;
        }
    }


    [NodeMenuItem("实体/基础/节点绑定组件")]
    public class Entity_Entity_BindGoCom : Entity_ComNode
    {
        public override string Title { get => "节点绑定组件"; set => base.Title = value; }
        public override string Tooltip { get => "节点绑定组件"; set => base.Tooltip = value; }
        public override Type RuntimeNode => typeof(BindGoCom);

        public override BaseCom CreateRuntimeNode()
        {
            BindGoCom com = new BindGoCom();
            return com;
        }
    }

    [NodeMenuItem("实体/基础/决策组件")]
    public class Entity_Entity_DecisionCom : Entity_ComNode
    {
        public override string Title { get => "决策组件"; set => base.Title = value; }
        public override string Tooltip { get => "决策组件"; set => base.Tooltip = value; }
        public override Type RuntimeNode => typeof(DecisionCom);

        [NodeValue("决策树Id")]
        public int decisionId = 0;

        [NodeValue("决策树执行线程")]
        public DecisionThread decisionThread = DecisionThread.MainThread;

        public override BaseCom CreateRuntimeNode()
        {
            DecisionCom com = new DecisionCom();
            com.DecisionId = decisionId;
            com.DecisionThread = decisionThread;
            return com;
        }
    }

    [NodeMenuItem("实体/基础/位置组件")]
    public class Entity_Entity_TransCom : Entity_ComNode
    {
        public override string Title { get => "位置组件"; set => base.Title = value; }
        public override string Tooltip { get => "位置组件"; set => base.Tooltip = value; }
        public override Type RuntimeNode => typeof(TransCom);

        public override BaseCom CreateRuntimeNode()
        {
            TransCom com = new TransCom();
            return com;
        }
    }
}
