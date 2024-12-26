using IANodeGraph.Model;
using UnityEngine;

namespace IAECS.EntityGraph
{
    public class EntityGraphAsset : BaseGraphAsset<EntityGraph>
    {
        [Header("实体Id")]
        public int entityId;

        [Header("实体名")]
        public string entityName;
    }
}
