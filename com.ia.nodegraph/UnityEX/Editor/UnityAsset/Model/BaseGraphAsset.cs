using IANodeGraph.Model.Internal;
using System;
using UnityEngine;
using Newtonsoft.Json;
using IANodeGraph.Editors;

namespace IANodeGraph.Model
{
    [Serializable]
    public abstract class BaseGraphAsset<GraphClass> : InternalBaseGraphAsset where GraphClass : BaseGraph, new()
    {
        public override Type GraphType => typeof(GraphClass);

        #region Serialize

        [HideInInspector]
        [SerializeField]
        [TextArea(20, 20)]
        string serializedGraph = String.Empty;

        public override void SaveGraph(BaseGraph graph)
        {
            serializedGraph = JsonConvert.SerializeObject(graph, GraphProcessorEditorUtil.JsonSetting);
        }

        public override BaseGraph LoadGraph()
        {
            var graph = JsonConvert.DeserializeObject<BaseGraph>(serializedGraph, GraphProcessorEditorUtil.JsonSetting);
            if (graph == null)
                graph = new GraphClass();
            return graph;
        }

        public override string GetSerializedStr()
        {
            return serializedGraph;
        }

        #endregion
    }
}
