using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAToolkit
{
    public sealed class Fsm<T> where  T : class
    {
        /// <summary>
        /// 状态机拥有者
        /// </summary>
        public T Owner { get; private set; }
        
        //状态列表
        private readonly Dictionary<Type, FsmState<T>> states = new Dictionary<Type, FsmState<T>>();

        //当前状态
        private FsmState<T> currState;
        //当前状态持续时间
        private float currentStateTime;
        private bool isDestroyed;

        /// <summary>
        /// 状态数量
        /// </summary>
        public int StateCount
        {
            get
            {
                return states.Count;
            }
        }
        
        /// <summary>
        /// 获取有限状态机是否正在运行。
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return currState != null;
            }
        }
        
        /// <summary>
        /// 获取有限状态机是否正在运行。
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                return isDestroyed;
            }
        }
        
        /// <summary>
        /// 获取当前有限状态机状态。
        /// </summary>
        public FsmState<T> CurrentState
        {
            get
            {
                return currState;
            }
        }
        
        /// <summary>
        /// 获取当前有限状态机状态名称。
        /// </summary>
        public string CurrentStateName
        {
            get
            {
                return currState != null ? currState.GetType().FullName : null;
            }
        }
        
        /// <summary>
        /// 获取当前有限状态机状态持续时间。
        /// </summary>
        public float CurrentStateTime
        {
            get
            {
                return currentStateTime;
            }
        }

        #region Create And Init

        /// <summary>
        /// 创建有限状态机。
        /// </summary>
        /// <param name="name">有限状态机名称。</param>
        /// <param name="owner">有限状态机持有者。</param>
        /// <param name="states">有限状态机状态集合。</param>
        /// <returns>创建的有限状态机。</returns>
        public static Fsm<T> Create(T pOwner, List<FsmState<T>> pStates)
        {
            if (pOwner == null)
            {
                return null;
            }

            if (pStates == null || pStates.Count < 1)
            {
                return null;
            }

            Fsm<T> fsmSpec = new Fsm<T>();
            fsmSpec.Owner = pOwner;
            foreach (FsmState<T> state in pStates)
            {
                if (state == null)
                {
                    continue;
                }

                if (fsmSpec.states.ContainsKey(state.GetType()))
                {
                    continue;
                }
                
                fsmSpec.states.Add(state.GetType(),state);
                state.Init(pOwner,fsmSpec);
            }

            fsmSpec.isDestroyed = false;
            return fsmSpec;
        }
        
        #endregion

        #region Clear

        public void Clear()
        {
            if (currState != null)
            {
                currState.Leave();
            }
            
            foreach (KeyValuePair<Type, FsmState<T>> state in states)
            {
                state.Value.Destroy();
            }

            Owner = null;
            states.Clear();
            currState = null;
            currentStateTime = 0;
            isDestroyed = true;
        }

        #endregion

        /// <summary>
        /// 开始
        /// </summary>
        /// <param name="pStateTypeName">开始的状态</param>
        public void Start(Type pStateType)
        {
            if (IsRunning)
            {
                return;
            }
            
            FsmState<T> stateSpec = GetState(pStateType);
            if (stateSpec == null)
            {
                return;
            }

            currState = stateSpec;
            currentStateTime = 0;
            currState.Enter();
        }

        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        /// <param name="pDeltaTime">更新间隔时间</param>
        /// <param name="pRealElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float pDeltaTime, float pRealElapseSeconds)
        {
            if (currState == null)
            {
                return;
            }

            currentStateTime += pDeltaTime;
            currState.Update(pDeltaTime, pRealElapseSeconds);
        }
        
        /// <summary>
        /// 主动切换到其他状态
        /// </summary>
        /// <param name="pStateType"></param>
        public void ChangeState(Type pStateType,FsmStateContext pContext = null)
        {
            if (currState == null || currState.GetType() == pStateType) 
            {
                return;
            }
            
            FsmState<T> changeStateSpec = GetState(pStateType);
            if (changeStateSpec == null)
            {
                return;
            }
            
            currState.Leave();
            currentStateTime = 0;

            currState = changeStateSpec;
            currState.SetContext(pContext);
            currState.Enter();
        }

        /// <summary>
        /// 当前状态离开，通过评估其他状态来自动切状态
        /// </summary>
        public void OnStateLeave(FsmState<T> pSpec)
        {
            pSpec.Leave();
            
            if (currState == null)
            {
                return;
            }

            if (currState != pSpec)
            {
                return;
            }
            
            currentStateTime = 0;
            
            foreach (KeyValuePair<Type, FsmState<T>> state in states)
            {
                if (state.Value != pSpec && state.Value.Evaluate())
                {
                    currState = state.Value;
                    currState.Enter();
                    return;
                }
            }
            
            Debug.LogError($"没有状态可以自动切换{pSpec.GetType()}");
        }

        #region Get

        /// <summary>
        /// 获取有限状态机状态。
        /// </summary>
        public FsmState<T> GetState(Type pStateType)
        {
            if (!states.ContainsKey(pStateType))
            {
                return null;
            }

            return states[pStateType];
        }

        #endregion
    }
}