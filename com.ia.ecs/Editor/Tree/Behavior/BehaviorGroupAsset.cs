using IAECS.Layer.Behavior;
using IAToolkit;
using IANodeGraph.Model;
using IANodeGraph.Model.Internal;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using IAECS.Core.Tree.Base;
using IAEngine;

namespace IAECS.Tree
{
    [CreateAssetMenu(fileName = "行为组", menuName = "配置组/行为组", order = 3)]
    public class BehaviorGroupAsset : BaseGraphGroupAsset<BehaviorAsset>
    {
        public override string DisplayName => "行为树";

        public override void OnClickCreateBtn()
        {
            List<string> requests = new List<string>();
            foreach (var item in Enum.GetNames(typeof(RequestId)))
            {
                requests.Add(item);
            }
            MiscHelper.Menu(requests, (int x) =>
            {
                RequestId requestId = (RequestId)x;
                string assetName = "behavior_" + requestId.ToString();
                BehaviorAsset asset = CreateGraph(assetName) as BehaviorAsset;
                asset.ReqId = (RequestId)x;
            });
        }

        public override void ExportGraph(List<InternalBaseGraphAsset> assets)
        {
            for (int i = 0; i < assets.Count; i++)
            {
                BehaviorAsset behaviorAsset = assets[i] as BehaviorAsset;
                BaseGraph graphData = behaviorAsset.DeserializeGraph();

                //运行时数据结构
                List<Node> trees = SerializeHelp.SerializeToTrees(graphData);
                List<BehaviorTree> models = new List<BehaviorTree>();
                for (int j = 0; j < trees.Count; j++)
                {
                    models.Add(new BehaviorTree(behaviorAsset.ReqId,trees[j]));
                }

                string filePath = ECSDefPath.GetBevTreePath(behaviorAsset.ReqId);
                IOHelper.WriteText(JsonMapper.ToJson(models), filePath);
                Debug.Log($"行为树生成成功>>>>{filePath}");
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
