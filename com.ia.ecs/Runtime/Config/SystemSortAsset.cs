using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;

namespace IAECS.Config
{
    public enum SystemType
    {
        Update,
        FixedUpdate,
        //Thread,
    }

    [Serializable]
    public class SystemSort
    {
        public string typeName = "";
        public string typeFullName = "";
        public int sort = 1;
    }

    [CreateAssetMenu(fileName = "SystemSortAsset", menuName = "配置组/系统排序", order = 1)]
    public class SystemSortAsset : ScriptableObject
    {
        public List<SystemSort> updateSystems       = new List<SystemSort>();
        public List<SystemSort> fixedUpdateSystems  = new List<SystemSort>();
        public List<SystemSort> threadSystems       = new List<SystemSort>();

        private SystemSort GetSystemSort(List<SystemSort> systems, string fullName)
        {
            for (int i = 0; i < systems.Count; i++)
            {
                if (systems[i].typeFullName == fullName)
                {
                    return systems[i];
                }
            }
            return null;
        }

        public SystemSort GetSystemSort(string fullName)
        {
            SystemSort sort = GetSystemSort(updateSystems, fullName);
            if (sort != null)
                return sort;
            sort = GetSystemSort(fixedUpdateSystems, fullName);
            if (sort != null)
                return sort;
            sort = GetSystemSort(threadSystems, fullName);
            if (sort != null)
                return sort;
            return null;
        }

        public List<SystemSort> GetSystemSorts(SystemType systemType)
        {
            switch (systemType)
            {
                case SystemType.Update:
                    return updateSystems;
                case SystemType.FixedUpdate:
                    return fixedUpdateSystems;
                //case SystemType.Thread:
                //    return threadSystems;
                default:
                    return updateSystems;
            }
        }
    }
}