using IANodeGraph.Model;
using UnityEditor;
using UnityEngine;
using IAToolkit;

namespace IAECS.Tree
{
    public class DecisionAsset : BaseGraphAsset<Decision>
    {
        [Header("决策树Id")]
        public int TreeId;
    }
}
