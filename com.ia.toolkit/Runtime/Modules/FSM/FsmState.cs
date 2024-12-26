namespace IAToolkit
{
    /// <summary>
    /// 状态上下文数据
    /// </summary>
    public class FsmStateContext
    {
    }
    
    /// <summary>
    /// 有限状态机状态基类。
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型。</typeparam>
    public abstract class FsmState<T> where T : class
    {
        /// <summary>
        /// 状态拥有者
        /// </summary>
        public T Owner { get; private set; }

        /// <summary>
        /// 状态机
        /// </summary>
        protected Fsm<T> Fsm;

        /// <summary>
        /// 上下文数据
        /// </summary>
        private FsmStateContext context;
        
        protected TContext GetContext<TContext>() where TContext : FsmStateContext
        {
            if (context == null)
            {
                return null;
            }

            return context as TContext;
        }

        public void SetContext(FsmStateContext pContext)
        {
            context = pContext;
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pOwner"></param>
        /// <param name="pFsm"></param>
        public void Init(T pOwner, Fsm<T> pFsm)
        {
            Owner = pOwner;
            Fsm = pFsm;
            OnInit();
        }

        public bool Evaluate()
        {
            return OnEvaluate();
        }

        public void Enter()
        {
            OnEnter();
        }

        public void Update(float pDeltaTime, float pRealElapseSeconds)
        {
            OnUpdate(pDeltaTime,pRealElapseSeconds);
        }

        public void Leave()
        {
            context = null;
            OnLeave();
        }

        public void Destroy()
        {
            context = null;
            OnDestroy();
        }

        #region 子类重写

        /// <summary>
        /// 初始化
        /// </summary>
        protected internal virtual void OnInit()
        {
        }
        
        /// <summary>
        /// 评估状态是否可以进入，当其他状态离开时，检测
        /// </summary>
        /// <returns></returns>
        public virtual bool OnEvaluate()
        {
            return true;
        }

        /// <summary>
        /// 状态进入
        /// </summary>
        protected internal virtual void OnEnter()
        {
        }
        
        /// <summary>
        /// 状态更新
        /// </summary>
        protected internal virtual void OnUpdate(float pDeltaTime, float pRealElapseSeconds)
        {
        }

        /// <summary>
        /// 状态离开
        /// </summary>
        protected internal virtual void OnLeave()
        {
        }
        
        /// <summary>
        /// 状态销毁
        /// </summary>
        protected internal virtual void OnDestroy()
        {
        }

        /// <summary>
        /// 自动切换状态
        /// </summary>
        protected void AutoChangeState()
        {
            Fsm.OnStateLeave(this);
        }

        #endregion
    }
}