using IAECS.Data;
using IAECS.Server.Layer;
using IAEngine;
using System.Threading;
using System.Collections.Generic;
using IAECS.Layer.Decision;

namespace IAECS
{
    public enum DecisionThread
    {
        MainThread,     //主线程（Unity）
        HighThread,     //高刷新线程
        MidThread,      //中刷新线程
        LowThread,      //低刷新线程
    }

    public class DecisionCenter
    {
        class BaseDecisionServerObj
        {
            protected object lockObj = new object();
            protected DecisionServer server;

            public BaseDecisionServerObj()
            {
                this.server = new DecisionServer();
            }

            public virtual void Init()
            {
                server.Init();
            }

            public virtual void Execute()
            {
                server.Execute();
            }

            public virtual void Clear()
            {
                server.Clear();
            }

            public bool HasTree(int decId)
            {
                return server.HasTree(decId);
            }

            public void AddTree(DecisionTree tree)
            {
                lock (lockObj)
                {
                    server.AddTree(tree);
                }
            }

            public void AddEntity(int decId, EntityWorkData workData)
            {
                lock (lockObj)
                {
                    server.AddDecisionEntity(decId, workData);
                }
            }

            public void RemoveEntity(int decId, string uid)
            {
                lock (lockObj)
                {
                    server.RemoveDecisionEntity(decId, uid);
                }
            }
        }

        class ThreadDecisionServerObj : BaseDecisionServerObj
        {
            private int threadTime;
            private bool isActive = false;

            public ThreadDecisionServerObj(int threadTime)
            {
                this.threadTime = threadTime;
                this.server = new DecisionServer();
            }

            public override void Init()
            {
                base.Init();
                isActive = true;
            }

            public override void Execute()
            {
                TaskHelper.AddTask(() =>
                {
                    while (isActive)
                    {
                        Thread.Sleep(threadTime);
                        lock (lockObj)
                        {
                            //更新决策
                            server.Execute();
                        }
                    }

                }, () =>
                {
                    //ECSLocate.Log.LogWarning("决策更新结束");
                });
            }

            public override void Clear()
            {
                lock (lockObj)
                {
                    isActive = false;
                }
                base.Clear();
            }
        }

        private object lockObj = new object();
        private bool isThreadRunning = false;
        private BaseDecisionServerObj mainServer = new BaseDecisionServerObj();
        private ThreadDecisionServerObj highThreadServer;
        private ThreadDecisionServerObj midThreadServer;
        private ThreadDecisionServerObj lowThreadServer;
        private Dictionary<string, DecisionThread> decDict = new Dictionary<string, DecisionThread>();

        public void Init()
        {
            highThreadServer = new ThreadDecisionServerObj(200);
            highThreadServer.Init();
            midThreadServer = new ThreadDecisionServerObj(500);
            midThreadServer.Init();
            lowThreadServer = new ThreadDecisionServerObj(1000);
            lowThreadServer.Init();
            ECSLocate.InitDecCenter(this);
        }

        public void Execute_Update()
        {
            mainServer.Execute();
        }

        public void Start_ThreadUpdate()
        {
            if (isThreadRunning)
                return;
            isThreadRunning = true;
            highThreadServer.Execute();
            midThreadServer.Execute();
            lowThreadServer.Execute();
        }

        public void Clear()
        {
            lock (lockObj)
            {
                mainServer.Clear();
                highThreadServer.Clear();
                midThreadServer.Clear();
                lowThreadServer.Clear();
                decDict.Clear();
            }
        }

        public void AddEntityDecision(DecisionThread decisionGroup, int decId, string uid)
        {
            // EntityWorkData entityWorkData = ECSLayerLocate.Info.GetEntityWorkData(uid);
            // //清理旧的
            // RemoveEntityDecision(decId, entityWorkData.Uid);
            // //加新的
            // BaseDecisionServerObj newServer = GetServer(decisionGroup);
            // if (!newServer.HasTree(decId))
            // {
            //     DecisionTree tree = LoadDecision(decId);
            //     if (tree == null)
            //     {
            //         return;
            //     }
            //     newServer.AddTree(tree);
            // }
            // newServer.AddEntity(decId, entityWorkData);
            // decDict.Add(entityWorkData.Uid, decisionGroup);
        }

        public void RemoveEntityDecision(int decId, string uid)
        {
            //清理旧的
            if (decDict.ContainsKey(uid))
            {
                BaseDecisionServerObj server = GetServer(decDict[uid]);
                server.RemoveEntity(decId, uid);
                decDict.Remove(uid);
            }
        }

        private BaseDecisionServerObj GetServer(DecisionThread group)
        {
            switch (group)
            {
                case DecisionThread.MainThread:
                    return mainServer;
                case DecisionThread.HighThread:
                    return highThreadServer;
                case DecisionThread.MidThread:
                    return midThreadServer;
                case DecisionThread.LowThread:
                    return lowThreadServer;
                default:
                    break;
            }
            return mainServer;
        }

        #region 加载决策树

        /// <summary>
        /// 加载决策树
        /// </summary>
        /// <returns></returns>
        private DecisionTree LoadDecision(int treeId)
        {
            //TODO : 加载决策树
            // string jsonStr = IAFramework.GameContext.Asset.LoadString(ECSDefPath.GetDecTreeCnfName(treeId));
            // DecisionTree decision = JsonMapper.ToObject<DecisionTree>(jsonStr);
            // if (decision == null)
            //     return null;
            return null;
        }

        #endregion
    }
}
