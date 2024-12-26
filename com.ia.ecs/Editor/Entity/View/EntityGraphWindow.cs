using IAECS.Core;
using IANodeGraph;
using IANodeGraph.View;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using IAToolkit;
using UnityEditor.UIElements;

namespace IAECS.EntityGraph
{
    [CustomGraphWindow(typeof(EntityGraph))]
    public class EntityGraphWindow : BaseGraphWindow
    {
        protected override void BuildToolbar(ToolbarView toolbar)
        {
            base.BuildToolbar(toolbar);
            if (Application.isPlaying)
            {
                CreateRunningTimeToolbar(toolbar);
            }
        }

        private void CreateRunningTimeToolbar(ToolbarView toolbar)
        {
            EntityGraphVM graph = Graph as EntityGraphVM;
            //查看所有节点
            ToolbarButton btnSelEntity = new ToolbarButton()
            {
                text = graph.RunningTimeEntity == null ? "选择实体" : graph.RunningTimeEntity.Uid,
                tooltip = "选择实体"
            };
            btnSelEntity.clicked += () =>
            {
                SelRunningTimeEntity(btnSelEntity);
            };
            toolbar.AddButtonToLeft(btnSelEntity);

        }

        private void SelRunningTimeEntity(ToolbarButton btnSelEntity)
        {
            EntityGraphAsset graphAsset = GraphAsset as EntityGraphAsset;
            int entityId = graphAsset.entityId;

            List<string> selStrs = new List<string>();
            List<Entity> entities = new List<Entity>();
            
            // foreach (var item in ECSLocate.ECS.GetAllEntitys())
            // {
            //     if (item.Value.EntityId == entityId)
            //     {
            //         entities.Add(item.Value);
            //         if (item.Value.GetCom(out BindGoCom bindGoCom))
            //         {
            //             selStrs.Add(bindGoCom.Go.name);
            //         }
            //         else
            //         {
            //             selStrs.Add(item.Value.Uid);
            //         }
            //     }
            // }

            MiscHelper.Menu(selStrs, (int x) =>
            {
                EntityGraphVM graph = Graph as EntityGraphVM;
                graph.RunningTimeEntity = entities[x];
                btnSelEntity.text = selStrs[x];
            });
        }
    }
}