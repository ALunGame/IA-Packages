using System;
using UnityEngine;

namespace IAEngine
{
    public static class BaseLifeEventEx
    {
        public static void OnEnable(this GameObject pGo, Action pCallBack)
        {
            BaseLifeEvent pCom = pGo.transform.GetOrAddCom<BaseLifeEvent>();
            pCom.AddEnableCallBack(pCallBack);
        }

        public static void OnDisable(this GameObject pGo, Action pCallBack)
        {
            BaseLifeEvent pCom = pGo.transform.GetOrAddCom<BaseLifeEvent>();
            pCom.AddDisableCallBack(pCallBack);
        }

        public static void OnDestroy(this GameObject pGo, Action pCallBack)
        {
            BaseLifeEvent pCom = pGo.transform.GetOrAddCom<BaseLifeEvent>();
            pCom.AddDestroyCallBack(pCallBack);
        }
    }
}
