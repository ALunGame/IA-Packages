using System;
using UnityEngine;

namespace IAECS.Core
{
    /// <summary>
    /// 决策组件
    /// </summary>
    public class DecisionCom : BaseCom
    {
        public int DecisionId;
        public DecisionThread DecisionThread;

        protected override void OnAwake(Entity pEntity)
        {
            SetDecisionId(DecisionId);
        }

        public void SetDecisionId(int pDecisionId)
        {
            ECSLocate.DecCenter.RemoveEntityDecision(DecisionId, EntityUid);
            DecisionId = pDecisionId;
            ECSLocate.DecCenter.AddEntityDecision(DecisionThread, DecisionId, EntityUid);
        }

        public void SetDecisionThread(DecisionThread pDecisionThread)
        {
            ECSLocate.DecCenter.RemoveEntityDecision(DecisionId, EntityUid);
            DecisionThread = pDecisionThread;
            ECSLocate.DecCenter.AddEntityDecision(DecisionThread, DecisionId, EntityUid);
        }
    }
}
