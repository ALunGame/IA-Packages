using System;
using UnityEngine;

namespace IANodeGraph.Model.Internal
{
    public abstract class InternalBaseGraphAsset : ScriptableObject, IGraphAsset
    {
        public abstract Type GraphType { get; }
        public abstract void SaveGraph(BaseGraph graph);
        public abstract BaseGraph LoadGraph();
        public abstract string GetSerializedStr();
    }
}
