using System;
using System.Collections.Generic;

namespace IAToolkit
{
    public class UnityEditorHelper
    {
        internal static Queue<Action> _mainThreadActions = new Queue<Action>();
        
        static UnityEditorHelper()
        {
            UnityEditorEventCatcher.OnEditorUpdateEvent -= OnEditorUpdate;
            UnityEditorEventCatcher.OnEditorUpdateEvent += OnEditorUpdate;
        }
        
        /// <summary>
        /// 捕获Unity Editor update事件
        /// </summary>
        private static void OnEditorUpdate()
        {
            // 主线程委托
            while (_mainThreadActions.Count > 0)
            {
                var action = _mainThreadActions.Dequeue();
                if (action != null) action();
            }
        }

        /// <summary>
        /// 异步线程回到主线程进行回调
        /// </summary>
        /// <param name="action"></param>
        public static void CallMainThread(Action action)
        {
            _mainThreadActions.Enqueue(action);
        }
    }
}