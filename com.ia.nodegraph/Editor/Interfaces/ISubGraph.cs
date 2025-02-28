using IANodeGraph.Model;

namespace IANodeGraph
{
    public interface ISubGraph
    {
        BaseGraphProcessor Parent { get; set; }
    }
}
