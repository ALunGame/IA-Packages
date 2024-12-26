using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAEngine
{
    /// <summary>
    /// 缓存池子
    /// </summary>
    public static class CachePool
    {
        /// <summary>
        /// 根节点
        /// </summary>
        public static Transform RootTrans { get; private set; }

        public static Dictionary<string, Transform> GameObjectPoolTransDict { get; private set; } = new Dictionary<string, Transform>();

        private static GameObjectCachePoolModule GameObjectPoolModule;

        private static ObjectCachePoolModule ObjectPoolModule;

        public static void Init()
        {
            RootTrans = new GameObject("CachePoolRoot").transform;
            RootTrans.Reset();
            UnityEngine.Object.DontDestroyOnLoad(RootTrans);

            GameObjectPoolModule = new GameObjectCachePoolModule();
            ObjectPoolModule = new ObjectCachePoolModule();
        }

        public static void Clear()
        {
            RootTrans = null;

            GameObjectPoolModule.ClearAll();
            ObjectPoolModule.ClearAll();
        }

        public static bool HasGameObjectPool(string pPoolName)
        {
            return GameObjectPoolModule.HasPool(pPoolName);
        }

        /// <summary>
        /// 创建GameObject对象池
        /// </summary>
        /// <param name="pPoolName">缓存池名</param>
        /// <param name="pCreateFunc">创建GameObject函数</param>
        /// <param name="pDefaultNum"></param>
        /// <param name="pMaxCapacity"></param>
        public static void CreateGameObjectPool(string pPoolName, Func<GameObject> pCreateFunc, int pDefaultNum = 0, int pMaxCapacity = -1)
        {
            GameObjectPoolModule.CreateGameObjectPoolData(pPoolName, pCreateFunc, pDefaultNum, pMaxCapacity);
        }

        public static bool HasObjectPool(string pPoolName)
        {
            return ObjectPoolModule.HasPool(pPoolName);
        }

        /// <summary>
        /// 创建Object对象池
        /// </summary>
        /// <param name="pPoolName">缓存池名</param>
        /// <param name="pCreateFunc">创建Object函数</param>
        /// <param name="pDefaultNum"></param>
        /// <param name="pMaxCapacity"></param>
        public static void CreatObjectPool(string pPoolName, Func<GameObject> pCreateFunc, int pDefaultNum = 0, int pMaxCapacity = -1)
        {
            ObjectPoolModule.CreateGameObjectPoolData(pPoolName, pCreateFunc, pDefaultNum, pMaxCapacity);
        }

        #region GameObject

        public static GameObject GetGameObject(string pPoolName, Transform pParent = null)
        {
            return GameObjectPoolModule.GetObject(pPoolName);
        }

        public static void PushGameObject(GameObject pGo)
        {
            GameObjectPoolModule.PushObject(pGo);
        }

        public static void PushGameObject(string pPoolName, GameObject pGo)
        {
            GameObjectPoolModule.PushObject(pPoolName, pGo);
        }

        public static void ClearGameObjectPool(string pPoolName)
        {
            GameObjectPoolModule.Clear(pPoolName);
        }

        #endregion


        #region Object

        public static object GetObject(string pPoolName)
        {
            return ObjectPoolModule.GetObject(pPoolName);
        }

        public static object GetObject(System.Type pType)
        {
            return ObjectPoolModule.GetObject(pType);
        }

        public static T GetObject<T>() where T : class
        {
            return ObjectPoolModule.GetObject<T>();
        }

        public static T GetObject<T>(string pPoolName) where T : class
        {
            return ObjectPoolModule.GetObject<T>(pPoolName);
        }

        public static void PushObject(string pPoolName, object pObj)
        {
            ObjectPoolModule.PushObject(pPoolName, pObj);
        }

        public static void ClearObjectPool(string pPoolName)
        {
            ObjectPoolModule.Clear(pPoolName);
        }

        #endregion
    }
}
