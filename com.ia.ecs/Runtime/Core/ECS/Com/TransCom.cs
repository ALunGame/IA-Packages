using System;
using IAEngine;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace IAECS.Core
{
    public class TransCom : BaseCom
    {
        public Vector3 InitPos { get; private set; }
        public Vector3 InitRoate { get; private set; }
        public Vector3 InitScale { get; private set; }
        
        [NonSerialized]
        public BindableValue<Vector3> PosValue = new BindableValue<Vector3>();
        public Vector3 Pos { get => PosValue.Value;}
        
        [NonSerialized]
        public BindableValue<Vector3> RoateValue = new BindableValue<Vector3>();
        public Vector3 Roate { get => RoateValue.Value;}
        
        [NonSerialized]
        public BindableValue<Vector3> ScaleValue = new BindableValue<Vector3>();
        public Vector3 Scale { get => ScaleValue.Value;}

        [NonSerialized]
        private Transform trans;

        protected override void OnAwake(Entity pEntity)
        {
            BindGoCom bindGoCom = pEntity.GetCom<BindGoCom>();
            if (bindGoCom != null)
            {
                bindGoCom.RegGoChange(OnBindGoChange);
            }
        }

        protected override void OnDestroy()
        {
            PosValue.ClearChangedEvent();
            RoateValue.ClearChangedEvent();
            ScaleValue.ClearChangedEvent();
        }

        private void OnBindGoChange(GameObject pGo)
        {
            trans = pGo.transform;
            
            pGo.transform.position = Pos;
            pGo.transform.localEulerAngles = Roate;
            pGo.transform.localScale = Scale;

            InitPos = Pos;
            InitRoate = Roate;
            InitScale = Scale;
        }

        public void SetPos(Vector3 pPos,bool pJustData = false)
        {
            if (trans != null && !pJustData)
                trans.position = Pos;
            PosValue.Value = pPos;
        }

        public void SetRoate(Vector3 pRoate,bool pJustData = false)
        {
            if (trans != null && !pJustData)
                trans.localEulerAngles = Roate;
            RoateValue.Value = pRoate;
        }

        public void SetScale(Vector3 pScale,bool pJustData = false)
        {
            if (trans != null && !pJustData)
                trans.localScale = Scale;
            ScaleValue.Value = pScale;
        }

        public void UpdateTrans()
        {
            PosValue.Value = trans.position;
            RoateValue.Value = trans.localEulerAngles;
            ScaleValue.Value = trans.localScale;
        }
    }
}
