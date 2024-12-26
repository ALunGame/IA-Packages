using IAECS.Core.Tree.Base;
using IAECS.Data;

namespace IAECS.Core.Tree.Nodes.Premise
{
    /// <summary>
    /// 检测实体Id
    /// </summary>
    public class BEV_PRE_CheckEntityId : NodePremise
    {
        public int entityId;
        public override bool OnMakeTrue(NodeData wData)
        {
            EntityWorkData workData = wData as EntityWorkData;
            return workData.MEntity.EntityId == entityId;
        }
    }
}