using IAECS.Config;
using IAECS.Core;
using IAECS.Core.Tree;
using IAEngine;
using System;
using System.Threading;
using UnityEngine;

namespace IAECS
{
    /// <summary>
    /// ECS启动中心
    /// </summary>
    public class ECSCenter
    {
        private RequestSortAsset requestSortAsset;
        private SystemSortAsset systemSortAsset;
        private bool _Init = false;
        private bool DecThreadRun = false;

        #region 初始化

        public void Init(RequestSortAsset requestSort, SystemSortAsset systemSortAsset)
        {
            this.requestSortAsset = requestSort;
            this.systemSortAsset = systemSortAsset;
            _Init = true;
            DecThreadRun = true;
            InitServer();
            RegSystems();
        }

        //设置服务
        private void InitServer()
        {
            ECSLocate.InitECSCenter(this);
            ECSLocate.InitServer();
            //ECSLayerLocate.InitLayerServer();
        }

        //注册系统
        private void RegSystems()
        {
            foreach (var item in systemSortAsset.GetSystemSorts(SystemType.Update))
            {
                Type systemType = ReflectionHelper.GetType(item.typeFullName);
                if (systemType == null)
                {
                    IAECS.ECSLocate.Log.LogError("系统创建失败>>>", systemType, item.typeFullName);
                    continue;
                }
                object systemObj = ReflectionHelper.CreateInstance(systemType);
                BaseSystem system = systemObj as BaseSystem;
                system.Init();
                ECSLocate.ECS.RegSystem(system);
            }
        }

        public void Clear()
        {
            _Init = false;
            DecThreadRun = false;
        }

        #endregion

        #region Execute

        public void Execute_Update()
        {
            if (!_Init)
                return;

            //更新树时间
            NodeTime.UpdateTime(Time.deltaTime, 1);

            //执行行为树
            ECSLayerLocate.Behavior.Execute();

            //系统处理
            ECSLocate.ECS.ExcuteUpdateSystem();
        }

        public void Execute_FixedUpdate()
        {
            if (!_Init)
                return;

            //系统处理
            ECSLocate.ECS.ExcuteFixedUpdateSystem();
        }

        #endregion

        #region Get

        public RequestSort GetRequestSort(RequestId requestId)
        {
            for (int i = 0; i < requestSortAsset.requests.Count; i++)
            {
                RequestSort sort = requestSortAsset.requests[i];
                if (sort.key.Equals(requestId.ToString()))
                {
                    return sort;
                }
            }
            ECSLocate.Log.LogError("请求没有配置排序", requestId);
            return null;
        }

        #endregion
    }
}
