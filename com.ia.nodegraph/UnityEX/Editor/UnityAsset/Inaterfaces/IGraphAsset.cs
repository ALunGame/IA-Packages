using System;

namespace IANodeGraph
{
    public interface IGraphAsset
    {
        Type GraphType { get; }
        
        void SaveGraph(BaseGraph graph);
        
        BaseGraph LoadGraph();

        string GetSerializedStr();
    }
}