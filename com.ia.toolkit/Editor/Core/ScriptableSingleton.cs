using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IAToolkit.Core
{
    public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
    {
        /// <summary> 
        /// 线程锁 
        /// </summary>
        private static readonly object m_Lock = new object();

        private static T m_Instance;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (m_Lock)
                    {
                        if (m_Instance == null)
                            m_Instance = ScriptableObject.CreateInstance<T>();
                    }
                }
                return m_Instance;
            }
        }

        public static bool IsNull { get { return m_Instance == null; } }

        public static void Initialize()
        {
            _Get();
            T _Get() { return Instance; }
        }

        public static void Destroy()
        {
            if (m_Instance != null)
            {
                m_Instance.OnBeforeDestroy();
                m_Instance = null;
            }
        }

        protected virtual void OnBeforeDestroy() { }
    }
}
