using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAECS.Config
{
    [Serializable]
    public class RequestSort
    {
        public string key = "";
        public string name = "";
        public int sort = 1;
        public bool isCover = false;
        public bool isCustom = false;
    }

    [CreateAssetMenu(fileName = "RequestSortAsset", menuName = "配置组/请求排序", order = 1)]
    public class RequestSortAsset : ScriptableObject
    {
        public const string RequestCodePath = "Assets/com.lc.ecs/Runtime/Config/RequestId.cs";

        public List<RequestSort> requests = new List<RequestSort>();

        public RequestSort GetSort(string key)
        {
            for (int i = 0; i < requests.Count; i++)
            {
                if (requests[i].key == key)
                {
                    return requests[i];
                }
            }
            return null;
        }
    }
}