using IAEngine;
using System;
using UnityEngine;

namespace IAECS.Core
{
    /// <summary>
    /// 实体绑定节点
    /// </summary>
    public class BindGoCom : BaseCom
    {
        [NonSerialized]
        private BindableValue<GameObject> go = new BindableValue<GameObject>();

        public GameObject Go { get { return go.Value; } }

        public void SetBindGo(GameObject pGo)
        {
            go.Value = pGo;
// #if UNITY_EDITOR
//             EntityHelperCom helperCom = pGo.transform.GetOrAddCom<EntityHelperCom>();
//             helperCom.SetEntity(ECSLocate.ECS.GetEntity(EntityUid));
// #endif
        }

        public void RegGoChange(Action<GameObject> callBack)
        {
            go.RegisterValueChangedEvent(callBack);
        }

        public void ClearGoChange()
        {
            go.ClearChangedEvent();
        }
    }
}
