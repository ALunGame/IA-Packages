using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IANodeGraph.Model
{
    public interface IGraphAsset
    {
        Type GraphType { get; }
        void SaveGraph(BaseGraph graph);
        BaseGraph DeserializeGraph();
    }
}
