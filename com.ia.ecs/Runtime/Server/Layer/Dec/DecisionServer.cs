using IAECS.Data;
using IAECS.Layer.Decision;
using System.Collections.Generic;

namespace IAECS.Server.Layer
{
    public class DecisionServer
    {
        private Dictionary<int, DecisionTree> desDict = new Dictionary<int, DecisionTree>();
       
        public void Init()
        {
        }

        public void Execute()
        {
            foreach (DecisionTree item in desDict.Values)
            {
                if (item != null)
                {
                    item.Execute();
                }
            }
        }

        public void Clear()
        {
        }

        public void AddDecisionEntity(int decId, EntityWorkData workData)
        {
            if (!desDict.ContainsKey(decId))
                return;
            DecisionTree decTree = desDict[decId];
            if (!desDict.ContainsKey(decTree.TreeId))
                desDict.Add(decTree.TreeId, decTree);

            //删除已经存在的
            List<DecisionTree> trees = DecisionHasEntity(workData.MEntity.Uid);
            for (int i = 0; i < trees.Count; i++)
            {
                trees[i].RemoveEntity(workData.MEntity.Uid);
            }

            //加入新的
            decTree.AddEntity(workData);
        }

        public void RemoveDecisionEntity(int decId, string uid)
        {
            if (!desDict.ContainsKey(decId))
                return;
            DecisionTree decision = desDict[decId];
            decision.RemoveEntity(uid);
        }

        public bool HasTree(int decId)
        {
            return desDict.ContainsKey(decId);
        }

        public void AddTree(DecisionTree tree)
        {
            if (!desDict.ContainsKey(tree.TreeId))
            {
                desDict.Add(tree.TreeId, tree);
            }
        }

        /// <summary>
        /// 获得其他包含此实体的决策树
        /// </summary>
        /// <returns></returns>
        private List<DecisionTree> DecisionHasEntity(string uid)
        {
            List<DecisionTree> trees = new List<DecisionTree>();
            foreach (DecisionTree item in desDict.Values)
            {
                if (item.GetEntity(uid) != null)
                {
                    trees.Add(item);
                }
            }
            return trees;
        }

        
    }
}
