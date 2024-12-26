using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace IAECS.Core
{
    public abstract class BaseSystem
    {
        //警告执行时间毫秒
        public static int ExcuteWarnTime = 10;
        public static bool OpenTest = true;

        //检测运行时长
        private Stopwatch stopwatch;

        //包含的组件
        private List<Type> ContainComs = new List<Type>();

        //不包含的组件
        private List<Type> NoContainComs = new List<Type>();

        //需要处理的组件列表
        private Dictionary<int, List<BaseCom>> IdHandleComsDict = new Dictionary<int, List<BaseCom>>();

        public void Init()
        {
            ContainComs = RegContainListenComs();
            NoContainComs = RegNoContainComs();
            if (OpenTest)
            {
                stopwatch = new Stopwatch();
            }
        }

        //检测实体是否处理符合条件
        public bool CheckEntity(Entity entity)
        {
            //是否需要检测
            bool check = true;

            //需要监听的组件列表
            List<BaseCom> listenComs = new List<BaseCom>();

            //实体关闭
            if (entity.IsActive == false)
                check = false;
            else
            {
                for (int i = 0; i < ContainComs.Count; i++)
                {
                    string typeName = ContainComs[i].Name;
                    BaseCom com = entity.GetCom(typeName);

                    //没有这个组件活着组件处于禁用状态
                    if (com == null || !com.IsActive)
                    {
                        check = false;
                        break;
                    }
                    else
                    {
                        listenComs.Add(com);
                    }
                }

                if (check)
                {
                    for (int i = 0; i < NoContainComs.Count; i++)
                    {
                        string typeName = NoContainComs[i].Name;
                        BaseCom com = entity.GetCom(typeName);

                        //有这个组件并且处于激活状态
                        if (com != null && com.IsActive)
                        {
                            check = false;
                            break;
                        }
                    }
                }
            }

            int entityId = entity.GetHashCode();
            //需要检测
            if (check)
            {
                if (!IdHandleComsDict.ContainsKey(entityId))
                {
                    IdHandleComsDict.Add(entityId, listenComs);
                    OnAddCheckComs(IdHandleComsDict[entityId]);
                }
            }
            else
            {
                if (IdHandleComsDict.ContainsKey(entityId))
                {
                    OnRemoveCheckComs(IdHandleComsDict[entityId]);
                    IdHandleComsDict.Remove(entityId);
                }
            }
            return check;
        }

        public void Excute()
        {
            if (IdHandleComsDict.Count == 0)
                return;

            if (OpenTest)
                stopwatch.Restart();
            foreach (List<BaseCom> coms in IdHandleComsDict.Values)
            {
                HandleComs(coms);
            }
            if (OpenTest)
            {
                stopwatch.Stop();
                if (stopwatch.Elapsed.TotalMilliseconds >= ExcuteWarnTime)
                {
                    //ECSLocate.Log.LogWarning("系统运行超时>>>>", this.GetType(), stopwatch.Elapsed.TotalMilliseconds);
                }
            }
        }

        public void FixedUpdateExecute()
        {
            if (IdHandleComsDict.Count == 0)
                return;

            if (OpenTest)
                stopwatch.Restart();
            foreach (List<BaseCom> coms in IdHandleComsDict.Values)
            {
                FixedUpdateHandleComs(coms);
            }
            if (OpenTest)
            {
                stopwatch.Stop();
                if (stopwatch.Elapsed.TotalMilliseconds >= ExcuteWarnTime)
                {
                    //ECSLocate.Log.LogWarning("系统运行超时>>>>", this.GetType(), stopwatch.Elapsed.TotalMilliseconds);
                }
            }
        }

        public void Clear()
        {
            ContainComs.Clear();
            IdHandleComsDict.Clear();
        }

        protected static T GetCom<T>(BaseCom baseCom) where T : BaseCom
        {
            return baseCom as T;
        }

        //注册需要监听的组件列表
        protected abstract List<Type> RegContainListenComs();
        protected virtual List<Type> RegNoContainComs()
        {
            return new List<Type>();
        }

        //处理组件
        protected abstract void HandleComs(List<BaseCom> comList);

        //物理帧处理
        protected virtual void FixedUpdateHandleComs(List<BaseCom> comList)
        {
            
        }

        //当组件添加检测的时候
        protected virtual void OnAddCheckComs(List<BaseCom> comList)
        {

        }

        //当组件移除检测的时候
        protected virtual void OnRemoveCheckComs(List<BaseCom> comList)
        {

        }
    }
}
