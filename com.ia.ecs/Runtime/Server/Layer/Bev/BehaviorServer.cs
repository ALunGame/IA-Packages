using IAECS.Data;
using IAECS.Layer.Behavior;
using IAEngine;
using IAServer;
using System;
using System.Collections.Generic;

namespace IAECS.Server.Layer
{
    public class BehaviorServer : BaseServer
    {
        private Dictionary<RequestId, List<BehaviorTree>> BevDict = new Dictionary<RequestId, List<BehaviorTree>>();
        
        public override void OnInit()
        {
            foreach (var item in Enum.GetValues(typeof(RequestId)))
            {
                //TODO: JSON
                List<BehaviorTree> trees = LoadBehavior((RequestId)item,"");
                if (trees != null)
                {
                    BevDict.Add((RequestId)item, trees);
                }
            }
        }

        public void ReqBev(EntityWorkData workData, RequestId clearReqId)
        {
            //删除
            if (BevDict.TryGetValue(clearReqId, out List<BehaviorTree> lastBehaviors))
            {
                foreach (var bev in lastBehaviors)
                {
                    bev.RemoveWorkData(workData);
                }
            }
            
            //执行
            BehaviorTree currBehavior = GetBehavior(workData.CurrReqId,workData);
            if (currBehavior != null)
                currBehavior.AddWorkData(workData);
        }

        public void Execute()
        {
            foreach (List<BehaviorTree> items in BevDict.Values)
            {
                foreach (var tree in items)
                {
                    tree.Execute();
                }
            }
        }

        /// <summary>
        /// 获得行为树
        /// </summary>
        /// <returns></returns>
        private BehaviorTree GetBehavior(RequestId reqId,EntityWorkData workData)
        {
            if (!BevDict.ContainsKey(reqId))
            {
                return null;
            }
            
            List<BehaviorTree> items = BevDict[reqId];
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Evaluate(workData))
                {
                    return items[i];
                }   
            }
            return null;
        }

        /// <summary>
        /// 加载行为树
        /// </summary>
        /// <returns></returns>
        private List<BehaviorTree> LoadBehavior(RequestId pTreeId, string pJsonStr)
        {
            string jsonStr = pJsonStr;
            try
            {
                List<BehaviorTree> behaviors = JsonMapper.ToObject<List<BehaviorTree>>(jsonStr);
                if (behaviors == null)
                    return null;
                return behaviors;
            }
            catch (Exception e)
            {
                ECSLocate.Log.LogError("行为树序列化失败",pTreeId);
                return null;
            }
        }
    }
}


