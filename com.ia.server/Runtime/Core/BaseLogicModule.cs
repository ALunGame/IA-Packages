using System;
using System.Collections.Generic;

namespace IAServer
{
    /// <summary>
    /// 服务额外逻辑映射
    /// </summary>
    public abstract class ServerLogicModuleMapping
    {

        private Dictionary<Type,IServerLogicModule> beforeServerInitLogics = null;
        /// <summary>
        /// 服务初始化之前调用逻辑
        /// </summary>
        public Dictionary<Type,IServerLogicModule> BeforeServerInitLogics
        {
            get
            {
                if (beforeServerInitLogics == null)
                {
                    beforeServerInitLogics = new Dictionary<Type, IServerLogicModule>();
                    List<IServerLogicModule> modules = RegBeforeServerInitLogics();
                    foreach (IServerLogicModule module in modules)
                    {
                        beforeServerInitLogics.Add(module.GetType(),module);
                    }
                }

                return beforeServerInitLogics;
            }
        }
        
        /// <summary>
        /// 注册服务初始化之前调用逻辑
        /// </summary>
        /// <returns></returns>
        public abstract List<IServerLogicModule> RegBeforeServerInitLogics();

        
        private Dictionary<Type,IServerLogicModule> afterServerInitLogics = null;
        /// <summary>
        /// 服务初始化之后调用逻辑
        /// </summary>
        public Dictionary<Type,IServerLogicModule> AfterServerInitLogics {
            get
            {
                if (afterServerInitLogics == null)
                {
                    afterServerInitLogics = new Dictionary<Type, IServerLogicModule>();
                    List<IServerLogicModule> modules = RegAfterServerInitLogics();
                    foreach (IServerLogicModule module in modules)
                    {
                        afterServerInitLogics.Add(module.GetType(),module);
                    }
                }
                return afterServerInitLogics;
            }
        }

        /// <summary>
        /// 注册服务初始化之后调用逻辑
        /// </summary>
        /// <returns></returns>
        public abstract List<IServerLogicModule> RegAfterServerInitLogics();
        
        public void ExecuteInit(BaseServer pServer, bool pBefore)
        {
            Dictionary<Type,IServerLogicModule> logics = pBefore ? BeforeServerInitLogics : AfterServerInitLogics;
            foreach (IServerLogicModule module in logics.Values)
            {
                module.Init(pServer);
            }
        }
        
        public void ExecuteClear(bool pBefore)
        {
            Dictionary<Type,IServerLogicModule> logics = pBefore ? BeforeServerInitLogics : AfterServerInitLogics;
            foreach (IServerLogicModule module in logics.Values)
            {
                module.Clear();
            }
        }
    }


    public interface IServerLogicModule
    {
        void Init(BaseServer pServer);

        void Clear();
    }
    
    
    /// <summary>
    /// 服务逻辑模块
    /// 1，对服务逻辑的抽离
    /// 2，提供业务层对服务的扩展
    /// </summary>
    public abstract class BaseServerLogicModule<T> : IServerLogicModule where T : BaseServer
    {
        protected T server;
        
        public void Init(BaseServer pServer)
        {
            server = (T)pServer;
            OnInit();
        }

        public void Clear()
        {
            OnClear();
        }

        public virtual void OnInit()
        {
            
        }
        
        public virtual void OnClear()
        {
            
        }
    }
}