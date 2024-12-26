using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAEngine
{
    internal class ObjectCachePoolModule
    {
        private Dictionary<string, ObjectCachePoolData> poolDic = new Dictionary<string, ObjectCachePoolData>();

        public bool HasPool(string pPoolName)
        {
            return poolDic.ContainsKey(pPoolName);
        }

        public void CreateGameObjectPoolData(string pPoolName, Func<object> pCreateFunc, int pDefaultNum = 0, int pMaxCapacity = -1)
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

            ObjectCachePoolData poolData = null;
            poolData = new ObjectCachePoolData();
            poolData.Init(pPoolName, pCreateFunc, pMaxCapacity);
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
                        object go = poolData.InternalCreateGo();
                        if (go.IsNull())
                        {
                            Debug.LogError($"Go缓存池初始化失败，创建Go返回为空{pPoolName}");
                            return;
                        }
                        poolData.PushObj(go);
                    }
                }
            }
        }

        public bool PushObject(string pPoolName, object pObj)
        {
            // 现在有没有这一层
            if (poolDic.TryGetValue(pPoolName, out var poolData))
            {
                return poolData.PushObj(pObj);
            }
            else
            {
                Debug.LogError($"当前没有这个缓存池{pPoolName}");
                return false;
            }
        }

        public object GetObject(string pPoolName)
        {
            object obj = null;
            // 检查有没有这一层
            if (poolDic.TryGetValue(pPoolName, out var poolData))
            {
                obj = poolData.GetObj();
            }
            return obj;
        }

        public object GetObject(System.Type type)
        {
            return GetObject(type.FullName);
        }

        public T GetObject<T>() where T : class
        {
            return (T)GetObject(typeof(T));
        }

        public T GetObject<T>(string keyName) where T : class
        {
            return (T)GetObject(keyName);
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
