using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAEngine
{
    internal class GameObjectCachePoolModule
    {
        private Dictionary<string, GameObjectCachePoolData> poolDic = new Dictionary<string, GameObjectCachePoolData>();

        public bool HasPool(string pPoolName)
        {
            return poolDic.ContainsKey(pPoolName);
        }

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="pPoolName">缓存池名</param>
        /// <param name="pCreateFunc">创建GameObject函数</param>
        /// <param name="pDefaultNum"></param>
        /// <param name="pMaxCapacity"></param>
        public void CreateGameObjectPoolData(string pPoolName, Func<GameObject> pCreateFunc, int pDefaultNum = 0, int pMaxCapacity = -1)
        {
            if (pDefaultNum > pMaxCapacity && pMaxCapacity != -1)
            {
                Debug.LogError($"默认容量{pDefaultNum}超出最大容量限制:{pMaxCapacity}");
                return;
            }

            if (poolDic.ContainsKey(pPoolName))
            {
                Debug.LogError($"重复的缓存池{pPoolName}");
                return;
            }

            Transform poolRoot = new GameObject(pPoolName).transform;
            poolRoot.SetParent(CachePool.RootTrans);
            poolRoot.Reset();

            GameObjectCachePoolData poolData = null;
            poolData = new GameObjectCachePoolData();
            poolData.Init(pPoolName, pCreateFunc, poolRoot, pMaxCapacity);
            poolDic.Add(pPoolName, poolData);

            //在指定默认容量和默认对象时才有意义
            if (pDefaultNum > 0)
            {
                //在指定默认容量和默认对象时才有意义
                if (pDefaultNum != 0)
                {
                    // 生成容量个数的物体放入对象池
                    for (int i = 0; i < pDefaultNum; i++)
                    {
                        GameObject go = poolData.InternalCreateGo();
                        if (go.IsNull())
                        {
                            Debug.LogError($"Go缓存池初始化失败，创建Go返回为空{pPoolName}");
                            return;
                        }
                        go.name = pPoolName;
                        poolData.PushObj(go);
                    }
                }
            }
        }

        public GameObject GetObject(string pPoolName, Transform pParent = null)
        {
            GameObject obj = null;
            // 检查有没有这一层
            if (poolDic.TryGetValue(pPoolName, out var poolData))
            {
                obj = poolData.GetObj(pParent);
            }
            return obj;
        }

        public void PushObject(GameObject go)
        {
            PushObject(go.name, go);
        }

        public bool PushObject(string pPoolName, GameObject pGo)
        {
            // 现在有没有这一层
            if (poolDic.TryGetValue(pPoolName, out var poolData))
            {
                return poolData.PushObj(pGo);
            }
            else
            {
                Debug.LogError($"当前没有这个缓存池{pPoolName}");
                return false;
            }
        }

        public void Clear(string pPoolName)
        {
            if (poolDic.TryGetValue(pPoolName, out var gameObjectPoolData))
            {
                gameObjectPoolData.Desotry();
                poolDic.Remove(pPoolName);
            }
        }

        public void ClearAll()
        {
            var enumerator = poolDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.Desotry();
            }
            poolDic.Clear();
        }
    }
}
