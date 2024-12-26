using IAECS.Core.Tree;
using IAECS.Core.Tree.Base;
using IAECS.Data;
using System.Collections.Generic;
using UnityEngine;

namespace IAECS.Layer.Behavior
{
    /// <summary>
    /// 行为树
    /// </summary>
    public class BehaviorTree
    {
        private RequestId reqId;
        private Node tree;

        public Node Tree
        {
            get
            {
                return tree;
            }
        }

        //需要处理的工作数据
        private List<EntityWorkData> HandleList = new List<EntityWorkData>();

        public BehaviorTree()
        {

        }

        public BehaviorTree(RequestId reqId, Node tree)
        {
            this.tree = tree;
            this.reqId = reqId;
        }

        /// <summary>
        /// 添加工作（现在不允许重复添加）
        /// </summary>
        public void AddWorkData(EntityWorkData workData)
        {
            if (HandleList.Contains(workData))
            {
                return;
            }
            HandleList.Add(workData);
        }

        /// <summary>
        /// 删除工作
        /// </summary>
        public void RemoveWorkData(int index,EntityWorkData workData)
        {
            HandleList.RemoveAt(index);
            Tree.Transition(workData);
        }

        public void RemoveWorkData(EntityWorkData workData)
        {
            if (HandleList.Contains(workData))
            {
                HandleList.Remove(workData);
                Tree.Transition(workData);
            }
        }

        /// <summary>
        /// 判断实体是否可以进入该行为树
        /// </summary>
        /// <param name="workData"></param>
        /// <returns></returns>
        public bool Evaluate(EntityWorkData workData)
        {
            NodePremise nodePremise = tree.nodePremise;
            if (nodePremise == null)
            {
                return true;
            }

            return nodePremise.IsTrue(workData);
        }

        /// <summary>
        /// 执行行为
        /// </summary>
        public void Execute()
        {
            if (tree == null)
            {
                return;
            }
            for (int i = 0; i < HandleList.Count; i++)
            {
                EntityWorkData data = HandleList[i];

                //行为改变
                if (data.CurrReqId != reqId)
                {
                    //清理
                    RemoveWorkData(i, data);
                    continue;
                }

                if (tree.Evaluate(data))
                {
                    int treeState = tree.Execute(data);
                    //节点运行完成
                    if (treeState == NodeState.FINISHED)
                    {
                        data.RemoveCurrReqCnt();
                        if (data.CurrReqCnt <= 0)
                        {
                            OnBehaviorFinish(data);
                        }
                    }
                }
                else
                {
                    RemoveWorkData(i,data);
                }
            }
        }

        private void OnBehaviorFinish(EntityWorkData workData)
        {
            if (!HandleList.Contains(workData))
            {
                return;
            }
            workData.ChangeRequestId(RequestId.None);
            //清理
            Tree.Transition(workData);
            HandleList.Remove(workData);
        }
    }
}
