using IANodeGraph;
using IANodeGraph.Model;
using IAToolkit;
using System;
using System.Collections.Generic;
using IAEngine;

namespace IAECS.Tree
{
    public class Behavior : BaseGraph
    {
        
    }
    
    [NodeViewModel(typeof(Behavior))]
    public class BehaviorVM : BaseGraphVM
    {
        [NonSerialized]
        private List<Type> NodeTypes = new List<Type>();

        
        public BehaviorVM(BaseGraph model) : base(model)
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
                if (type.BaseType == typeof(Base_DEC_PRE_Node))
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
                if (type.BaseType == typeof(Base_DEC_Act_Node))
                    continue;
                if (!NodeTypes.Contains(type))
                    NodeTypes.Add(type);
            }
            foreach (var type in ReflectionHelper.GetChildTypes<Base_BEV_PRE_Node>())
            {
                if (type.IsAbstract)
                    continue;
                if (!AttributeHelper.TryGetTypeAttribute(type, out NodeMenuItemAttribute attr))
                    continue;
                if (type == typeof(Tree_RootNode))
                    continue;
                if (!NodeTypes.Contains(type))
                    NodeTypes.Add(type);
            }
            foreach (var type in ReflectionHelper.GetChildTypes<Base_BEV_ACT_Node>())
            {
                if (type.IsAbstract)
                    continue;
                if (!AttributeHelper.TryGetTypeAttribute(type, out NodeMenuItemAttribute attr))
                    continue;
                if (type == typeof(Tree_RootNode))
                    continue;
                if (!NodeTypes.Contains(type))
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
