using System;
using System.Collections.Generic;

namespace IAECS.Core.Tree.Base
{
    /// <summary>
    /// 节点环境
    /// </summary>
    public class NodeContext
    {

    }

    /// <summary>
    /// 节点基类
    /// </summary>
    public class Node
    {
        //节点唯一Id
        public string Uid;

        //子节点列表
        public List<Node> childNodes = new List<Node>();

        //节点前提
        public NodePremise nodePremise;

        public Node()
        {

        }

        /// <summary>
        /// 获取数据
        /// </summary>
        protected T GetContext<T>(NodeData wData) where T : NodeContext, new()
        {
            string uniqueKey = Uid + typeof(T).FullName;
            T thisContext;
            if (wData.Context.ContainsKey(uniqueKey) == false)
            {
                thisContext = new T();
                wData.Context.Add(uniqueKey, thisContext);
            }
            else
            {
                try
                {
                    thisContext = (T)wData.Context[uniqueKey];
                }
                catch (Exception e)
                {
                    thisContext = new T();
                    //ECSLocate.Log.LogError("节点转换失败》》》》》》", uniqueKey, typeof(T).FullName);
                }
            }
            return thisContext;
        }

        /// <summary>
        /// 设置前提
        /// </summary>
        public void SetPremise(NodePremise premise)
        {
            nodePremise = premise;
        }

        #region 添加删除获取子节点

        //添加
        public bool AddChild(Node node)
        {
            childNodes.Add(node);
            return true;
        }

        //删除
        public void RemoveChild(Node node)
        {
            childNodes.Remove(node);
        }

        //数量
        public int GetChildCount()
        {
            return childNodes.Count;
        }

        //检测子节点索引是否合法
        public bool IsIndexValid(int index)
        {
            if (childNodes == null)
                return false;
            return index >= 0 && index < childNodes.Count;
        }

        //获取子节点
        public T GetChild<T>(int index) where T : Node
        {
            if (index < 0 || index >= childNodes.Count)
            {
                return null;
            }
            return (T)childNodes[index];
        }

        #endregion

        #region 生命周期函数
        //评估 （评估是否可执行）
        public bool Evaluate(NodeData wData)
        {
            return (nodePremise == null || nodePremise.IsTrue(wData)) && OnEvaluate(wData);
        }

        //子类重写 （是一个节点评估成功就执行，还是啥）
        protected virtual bool OnEvaluate(NodeData wData)
        {
            return true;
        }

        //执行
        public int Execute(NodeData wData)
        {
            return OnExcute(wData);
        }

        //子类重写  （返回执行结果）
        protected virtual int OnExcute(NodeData wData)
        {
            return NodeState.FINISHED;
        }

        //节点转换 （执行下一个节点，这个时候做一些数据清理操作）
        public void Transition(NodeData wData)
        {
            OnTransition(wData);
        }

        //子类重写  （基本上执行数据清理）
        protected virtual void OnTransition(NodeData wData)
        {

        }
        #endregion
    }
}
