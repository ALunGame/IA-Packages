using IAECS.Core;
using IAEngine;
using IANodeGraph;
using IANodeGraph.Model;
using IAToolkit;
using System;
using System.Collections.Generic;

namespace IAECS.EntityGraph
{
    public class EntityGraph : BaseGraph
    {
        
    }
    
    [NodeViewModel(typeof(EntityGraph))]
    public class EntityGraphVM : BaseGraphVM
    {
        /// <summary>
        /// 运行时实体
        /// </summary>
        [NonSerialized]
        public Entity RunningTimeEntity;

        public override List<string> NodeNamespace => new List<string>() { "IAECS.EntityGraph" };
        
        public EntityGraphVM(BaseGraph model) : base(model)
        {
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();
            foreach (var item in Model.nodes)
            {
                if (item.Value is Entity_Node)
                {
                    return;
                }
            }
            AddNode<Entity_Node>(UnityEngine.Vector2.zero);
        }

        private bool CheckHasCom(Type comType)
        {
            foreach (var item in Model.nodes)
            {
                if (item.Value.GetType() == comType)
                {
                    return true;
                }
            }
            return false;
        }

        public override IEnumerable<Type> GetNodeTypes()
        {
            foreach (var type in ReflectionHelper.GetChildTypes<Entity_ComNode>())
            {
                if (type.IsAbstract)
                    continue;
                if (!AttributeHelper.TryGetTypeAttribute(type, out NodeMenuItemAttribute attr))
                    continue;
                if (CheckHasCom(type))
                    continue;
                yield return type;
            }
        }
    }
}