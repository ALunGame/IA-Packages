using System;
using UnityEngine;

namespace IAEngine
{
    /// <summary>
    /// 基础生命类型
    /// </summary>
    public enum BaseLifeType
    {
        OnEnable,
        OnDisable,
        OnDestroy,
    }

    /// <summary>
    /// 基础Gameobject的生命周期监听
    /// </summary>
    public class BaseLifeEvent : MonoBehaviour
    {
        private Action onEnableCallBack;
        private void OnEnable()
        {
            onEnableCallBack?.Invoke();
        }
        public void AddEnableCallBack(Action pCallBack)
        {
            onEnableCallBack += pCallBack;
        }
        public void RemoveEnableCallBack(Action pCallBack)
        {
            onEnableCallBack -= pCallBack;
        }

        private Action onDisableCallBack;
        private void OnDisable()
        {
            onDisableCallBack?.Invoke();
        }
        public void AddDisableCallBack(Action pCallBack)
        {
            onDisableCallBack += pCallBack;
        }
        public void RemoveDisableCallBack(Action pCallBack)
        {
            onDisableCallBack -= pCallBack;
        }

        private Action onDestroyCallBack;
        private void OnDestroy()
        {
            onEnableCallBack = null;
            onDisableCallBack = null;

            Action func = onDestroyCallBack;
            onDestroyCallBack = null;
            func?.Invoke();
        }
        public void AddDestroyCallBack(Action pCallBack)
        {
            onDestroyCallBack += pCallBack;
        }
        public void RemoveDestroyCallBack(Action pCallBack)
        {
            onDestroyCallBack -= pCallBack;
        }
    }
}
