using IANodeGraph.Model;
using UnityEditor;
using UnityEngine;
using IAToolkit;

namespace IAECS.Tree
{
    public class BehaviorAsset : BaseGraphAsset<Behavior>
    {
        [Header("行为请求")]
        public RequestId ReqId;
    }
}
