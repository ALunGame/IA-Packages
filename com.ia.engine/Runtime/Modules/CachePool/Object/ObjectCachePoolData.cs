using System;
using System.Collections.Generic;

namespace IAEngine
{
    internal class ObjectCachePoolData
    {
        // 对象容器
        private Queue<object> poolQueue;
        // 容量限制 -1代表无限
        private int maxCapacity = -1;
        private Func<object> createGoFunc;

        public void Init(string pGoName, Func<object> pCreateGoFunc, int pCapacity = -1)
        {
            createGoFunc = pCreateGoFunc;
            maxCapacity = pCapacity;

            if (pCapacity == -1)
            {
                poolQueue = new Queue<object>();
            }
            else
            {
                poolQueue = new Queue<object>(pCapacity);
            }
        }

        /// <summary>
        /// 创建Obj
        /// </summary>
        /// <returns></returns>
        internal object InternalCreateGo()
        {
            return createGoFunc.Invoke();
        }

        /// <summary>
        /// 将对象放进对象池
        /// </summary>
        public bool PushObj(object pObj)
        {
            // 检测是不是超过容量
            if (maxCapacity != -1 && poolQueue.Count >= maxCapacity)
            {
                return false;
            }

            // 对象进容器
            poolQueue.Enqueue(pObj);
            return true;
        }

        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <returns></returns>
        public object GetObj()
        {
            object obj = null;
            if (poolQueue.Count > 0)
            {
                obj = poolQueue.Dequeue();
            }
            else
            {
                obj = InternalCreateGo();
            }
            return obj;
        }

        /// <summary>
        /// 销毁层数据
        /// </summary>
        public void Desotry()
        {
            maxCapacity = -1;
            // 队列清理
            poolQueue.Clear();
        }
    }
}
