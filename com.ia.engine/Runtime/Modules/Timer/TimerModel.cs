using System;

namespace IAEngine
{
    /// <summary>
    /// 一个计时器实例
    /// </summary>
    public class TimerModel
    {
        //使用者
        private string userName;
        //间隔时间
        private float intervalTime;
        //回调方法
        private Action action;
        //限制执行次数 -1 不限制
        private int limitExecuteCnt;

        //计时器
        private float timer;
        //已经执行的次数
        private int executedCnt;
        //暂停中
        private bool isPaused;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pUserName">使用者名</param>
        /// <param name="pIntervalTime">间隔时间</param>
        /// <param name="pAction">回调函数</param>
        /// <param name="pLimitExecuteCnt">限制调用次数-1不限制</param>
        public TimerModel(string pUserName, float pIntervalTime, Action pAction, int pLimitExecuteCnt = -1)
        {
            userName = pUserName;
            intervalTime = pIntervalTime;
            action = pAction;
            limitExecuteCnt = pLimitExecuteCnt;
            timer = 0;
            isPaused = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pUserType">使用者类型</param>
        /// <param name="pIntervalTime">间隔时间</param>
        /// <param name="pAction">回调函数</param>
        /// <param name="pLimitExecuteCnt">限制调用次数-1不限制</param>
        public TimerModel(Type pUserType, float pIntervalTime, Action pAction, int pLimitExecuteCnt = -1)
            :this(pUserType.FullName, pIntervalTime, pAction, pLimitExecuteCnt)
        {
        }

        public void Start() 
        {
            ReSet();

            isPaused = false;
        }

        public void Pause()
        {
            isPaused = true;
        }

        public void Update(float pTimeDelta)
        {
            if (isPaused)
                return;

            timer += pTimeDelta;
            if (timer >= intervalTime)
            {
                executedCnt++;
                timer = 0;
                action?.Invoke();
            }

            //检测执行次数
            if (limitExecuteCnt != -1 && executedCnt >= limitExecuteCnt)
            {
                isPaused = true;
                return;
            }
        }

        public void ReSet()
        {
            timer = 0;
            executedCnt = 0;
            isPaused = true;
        }

        /// <summary>
        /// 获得Cd时间
        /// </summary>
        /// <returns></returns>
        public float GetCdTime()
        {
            return intervalTime - timer;
        }

        /// <summary>
        /// 在CD中
        /// </summary>
        /// <returns></returns>
        public bool IsInCd()
        {
            return GetCdTime() > 0;
        }

        public bool IsPaused()
        {
            return isPaused;
        }
    }
}
