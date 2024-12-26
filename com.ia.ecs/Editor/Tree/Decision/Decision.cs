using IANodeGraph;
using IANodeGraph.Model;
using IAToolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using IAEngine;
using UnityEngine;

namespace IAECS.Tree
{
    public class Decision : BaseGraph
    {
        
    }
    
    [NodeViewModel(typeof(Decision))]
    public class DecisionVM : BaseGraphVM
    {
        [NonSerialized]
        private List<Type> NodeTypes = new List<Type>();

        public DecisionVM(BaseGraph model) : base(model)
        {
        }
        
        protected override void OnEnabled()
        {
            base.OnEnabled();
            CollectNodeTypes();
            foreach (var item in Model.nodes)
            {
                if (item.Value is Tree_RootNode)
                {
                    return;
                }
            }
            AddNode<Tree_RootNode>(UnityEngine.Vector2.zero);
        }

        private void CollectNodeTypes()
        {
            NodeTypes.Clear();
            foreach (var type in ReflectionHelper.GetChildTypes<Tree_PremiseNode>())
            {
                if (type.IsAbstract)
                    continue;
                if (!AttributeHelper.TryGetTypeAttribute(type, out NodeMenuItemAttribute attr))
                    continue;
                if (type == typeof(Tree_PremiseNode))
                    continue;
                if (type.BaseType == typeof(Base_BEV_PRE_Node))
                    continue;
                if (!NodeTypes.Contains(type))
                    NodeTypes.Add(type);
            }
            foreach (var type in ReflectionHelper.GetChildTypes<Tree_BaseNode>())
            {
                if (type.IsAbstract)
                    continue;
                if (!AttributeHelper.TryGetTypeAttribute(type, out NodeMenuItemAttribute attr))
                    continue;
                if (type == typeof(Tree_RootNode))
                    continue;
                if (type.BaseType == typeof(Base_BEV_ACT_Node))
                    continue;
                NodeTypes.Add(type);
            }
            foreach (var type in ReflectionHelper.GetChildTypes<Base_DEC_PRE_Node>())
            {
                if (type.IsAbstract)
                    continue;
                if (!AttributeHelper.TryGetTypeAttribute(type, out NodeMenuItemAttribute attr))
                    continue;
                if (type == typeof(Tree_RootNode))
                    continue;
                NodeTypes.Add(type);
            }
            foreach (var type in ReflectionHelper.GetChildTypes<Base_DEC_Act_Node>())
            {
                if (type.IsAbstract)
                    continue;
                if (!AttributeHelper.TryGetTypeAttribute(type, out NodeMenuItemAttribute attr))
                    continue;
                if (type == typeof(Tree_RootNode))
                    continue;
                NodeTypes.Add(type);
            }
        }

        public override IEnumerable<Type> GetNodeTypes()
        {
            foreach (var type in NodeTypes)
            {
                yield return type;
            }
        }
    }
}
