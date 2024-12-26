using System.Collections;
using UnityEngine;

namespace IAEngine
{
    /// <summary>
    /// 锁定位置组件
    /// </summary>
    [ExecuteAlways]
    public class LockTransCom : MonoBehaviour
    {
        public bool lockPos = true;
        public Vector3 lockPosValue = Vector3.zero;

        public bool lockRoate;
        public Vector3 lockRoateValue = Vector3.zero;

        public bool lockScale;
        public Vector3 lockScaleValue = Vector3.one;


        private void Update()
        {
            UpdateLockValue();
        }

        private void UpdateLockValue()
        {
            if (lockPos)
                transform.localPosition = lockPosValue;
            if (lockRoate)
                transform.localEulerAngles = lockRoateValue;
            if (lockScale)
                transform.localScale = lockScaleValue;
        }
    }
}