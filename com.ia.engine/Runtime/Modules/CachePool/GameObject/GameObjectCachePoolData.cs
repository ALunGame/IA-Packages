using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAEngine
{
    internal class GameObjectCachePoolData
    {
        // 这一层物体的 父节点
        private Transform rootTransform;
        // 对象容器
        private Queue<GameObject> poolQueue;
        // 容量限制 -1代表无限
        private int maxCapacity = -1;
        private Func<GameObject> createGoFunc;

        public void Init(string pGoName, Func<GameObject> pCreateGoFunc, Transform pLayerTrans, int pCapacity = -1)
        {
            createGoFunc = pCreateGoFunc;
            rootTransform = pLayerTrans;
            maxCapacity = pCapacity;

            if (pCapacity == -1)
            {
                poolQueue = new Queue<GameObject>();
            }
            else
            {
                poolQueue = new Queue<GameObject>(pCapacity);
            }
        }

        /// <summary>
        /// 创建Go
        /// </summary>
        /// <returns></returns>
        internal GameObject InternalCreateGo()
        {
            return createGoFunc.Invoke();
        }

        /// <summary>
        /// 将对象放进对象池
        /// </summary>
        public bool PushObj(GameObject pObj)
        {
            // 检测是不是超过容量
            if (maxCapacity != -1 && poolQueue.Count >= maxCapacity)
            {
                GameObject.Destroy(pObj);
                return false;
            }
            // 对象进容器
            poolQueue.Enqueue(pObj);
            // 设置父物体
            pObj.transform.SetParent(rootTransform);
            // 设置隐藏
            pObj.SetActive(false);
            return true;
        }

        /// <summary>
        /// 从对象池中获取对象
        /// </summary>
        /// <returns></returns>
        public GameObject GetObj(Transform pParent = null)
        {
            GameObject obj = null;
            if (poolQueue.Count > 0)
            {
                obj = poolQueue.Dequeue();
            }
            else
            {
                obj = InternalCreateGo();
            }

            // 显示对象
            obj.SetActive(true);
            // 设置父物体
            obj.transform.SetParent(pParent);
            if (pParent == null)
            {
                // 回归默认场景
                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
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
            rootTransform = null;
        }
    }
}
