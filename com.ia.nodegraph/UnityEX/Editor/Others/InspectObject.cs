using IANodeGraph.Model;
using UnityEngine;

namespace IANodeGraph.Editors
{
    public class InspectObject : ScriptableObject
    {
        [SerializeReference]
        public BaseGraph graph;
    }
}