using System;
using UnityEngine;

namespace IAServer
{
    public class BaseServer
    {
        //额外逻辑
        private ServerLogicModuleMapping logicMapping = null;
        
        //初始化状态
        public bool IsInit { get; protected set; }

        public void Init()
        {
            IsInit = false;
            
            logicMapping?.ExecuteInit(this,true);
            OnInit();
            logicMapping?.ExecuteInit(this,false);
            
            IsInit = true;
        }

        public void Clear()
        {
            IsInit = false;
            
            logicMapping?.ExecuteClear(true);
            OnClear();
            logicMapping?.ExecuteClear(false);
        }
        
        /// <summary>
        /// 设置逻辑映射
        /// </summary>
        /// <param name="pMapping"></param>
        public void SetLogicMapping(ServerLogicModuleMapping pMapping)
        {
            logicMapping = pMapping;
        }

        /// <summary>
        /// 获得逻辑
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetLogicModule<T>() where T : IServerLogicModule
        {
            if (!IsInit)
            {
                Debug.LogError($"GetLogicModule非法，服务尚未初始化完毕，{GetType().Name}");
                return default;
            }

            if (logicMapping == null)
                return default;

            Type type = typeof(T);
            if (logicMapping.BeforeServerInitLogics.ContainsKey(type))
            {
                return (T)logicMapping.BeforeServerInitLogics[type];
            }
            if (logicMapping.AfterServerInitLogics.ContainsKey(type))
            {
                return (T)logicMapping.AfterServerInitLogics[type];
            }

            return default;
        }

        public virtual void OnInit()
        {
            
        }
        
        public virtual void OnClear()
        {
            
        } 
    }
}