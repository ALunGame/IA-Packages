using IAECS.Core;
using IAToolkit;
using IANodeGraph;
using IANodeGraph.Model;
using IANodeGraph.Model.Internal;
using System.Collections.Generic;
using IAEngine;
using UnityEditor;
using UnityEngine;

namespace IAECS.EntityGraph
{
    [CreateAssetMenu(fileName = "实体组", menuName = "配置组/实体组", order = 1)]
    public class EntityGraphGroupAsset : BaseGraphGroupAsset<EntityGraphAsset>
    {
        public override string DisplayName => "实体配置";

        public void CreateTemplateGraph()
        {
            if (CheckHasGraph("实体模板"))
            {
                return;
            }
            EntityGraphAsset asset = CreateGraph("实体模板") as EntityGraphAsset;
            asset.name = "实体模板";
        }

        public EntityGraphAsset GetTemplateGraph()
        {
            List<InternalBaseGraphAsset> graphs = GetAllGraph();
            for (int i = 0; i < graphs.Count; i++)
            {
                if (graphs[i].name == "实体模板")
                {
                    return graphs[i] as EntityGraphAsset;
                }
            }
            return null;
        }

        public override void OnClickCreateBtn()
        {
            MiscHelper.Input($"输入实体Id：", (string x) =>
            {
                int entityId = int.Parse(x);
                string assetName = "entity_" + entityId;
                EntityGraphAsset asset = CreateGraph(assetName) as EntityGraphAsset;
                asset.entityId = entityId;
            });
        }

        public override InternalBaseGraphAsset CreateGraph(string name)
        {
            InternalBaseGraphAsset asset = base.CreateGraph(name);
            EntityGraphAsset tmpGraph = GetTemplateGraph();
            if (tmpGraph != null)
            {
                ((EntityGraphAsset)asset).SetSerializedStr(tmpGraph.GetSerializedStr());
            }
            return asset;
        }

        public override void ExportGraph(List<InternalBaseGraphAsset> assets)
        {
            for (int i = 0; i < assets.Count; i++)
            {
                EntityGraphAsset entityAsset = assets[i] as EntityGraphAsset;
                if (entityAsset.name == "实体模板")
                {
                    continue;
                }
                BaseGraph graphData = entityAsset.DeserializeGraph();

                //运行时数据结构
                List<BaseCom> coms = SerializeToEntityComs(graphData, entityAsset);

                string filePath = ECSDefPath.GetEntityPath(entityAsset.entityId);
                IOHelper.WriteText(JsonMapper.ToJson(coms), filePath);
                Debug.Log($"实体配置生成成功>>>>{filePath}");
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private List<BaseCom> SerializeToEntityComs(BaseGraph graph, EntityGraphAsset entityAsset)
        {
            List<Entity_Node> rootNodes = NodeHelper.GetNodes<Entity_Node>(graph);
            if (rootNodes.Count <= 0)
            {
                Debug.LogError($"试图序列化出错，没有根节点");
            }
            List<BaseCom> coms = rootNodes[0].GetModel();
            return coms;
        }
    }
}
