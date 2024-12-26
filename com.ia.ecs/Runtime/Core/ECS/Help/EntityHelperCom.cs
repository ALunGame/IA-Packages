using IAECS.Core;
using IAToolkit;
using UnityEngine;

namespace IAECS
{
    /// <summary>
    /// 编辑器下实体辅助
    /// </summary>
    public class EntityHelperCom : MonoBehaviour
    {
        private Entity _entity;

        [ReadOnly]
        [SerializeField]
        private string uid;

        [ReadOnly]
        [SerializeField]
        private int entityCnfId;
        
        // [ReadOnly]
        // [SerializeField]
        // private int actorId;
        
        public void SetEntity(Entity pEntity)
        {
            _entity = pEntity;
            uid = _entity.Uid;

            entityCnfId = _entity.EntityId;

            // if (pEntity is Actor)
            // {
            //     actorId = ((Actor) pEntity).Id;
            // }
        }

        private void OnDrawGizmosSelected()
        {
            foreach (BaseCom baseCom in _entity.GetComs())
            {
                baseCom.OnDrawGizmosSelected();
            }
        }

        private void OnDrawGizmos()
        {
            foreach (BaseCom baseCom in _entity.GetComs())
            {
                baseCom.OnDrawGizmos();
            }
        }
    }
}