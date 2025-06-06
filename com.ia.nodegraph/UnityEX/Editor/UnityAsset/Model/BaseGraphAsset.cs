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
        protected string serializedGraph = string.Empty;

        public override void SaveGraph(BaseGraph graph)
        {
            serializedGraph = JsonConvert.SerializeObject(graph, GraphProcessorEditorUtil.JsonSetting);
        }

        public override BaseGraph LoadGraph()
        {
            BaseGraph graph;
            try
            {
                graph = JsonConvert.DeserializeObject<BaseGraph>(serializedGraph, GraphProcessorEditorUtil.JsonSetting);
                if (graph == null)
                    graph = new GraphClass();
            }
            catch (Exception ex)
            {
                graph = new GraphClass();
                Debug.LogError($"Error deserializing graph: {ex}");
            }
            return graph;
        }

        public override string GetSerializedStr()
        {
            return serializedGraph;
        }

        public override void ClearSerializedStr()
        {
            serializedGraph = "";
        }

        #endregion
    }
}
