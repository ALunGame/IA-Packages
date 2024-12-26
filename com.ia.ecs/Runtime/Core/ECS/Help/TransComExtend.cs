using IAECS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace IAECS
{
    public static partial class TransComExtend
    {
        /// <summary>
        /// 移动到指定点
        /// </summary>
        /// <param name="pTransCom"></param>
        /// <param name="pTargetPos">目标位置</param>
        /// <param name="pSpeed">速度</param>
        public static void MoveTowards(this TransCom pTransCom,Vector3 pTargetPos,float pSpeed)
        {
            // Vector3 movePos = Vector3.MoveTowards(pTransCom.Pos, pTargetPos, pSpeed * Time.deltaTime);
            // pTransCom.WaitSetPos(movePos);
        }

        /// <summary>
        /// 朝指定方向移动
        /// </summary>
        /// <param name="pTransCom"></param>
        /// <param name="pDir"></param>
        /// <param name="pSpeed"></param>
        public static void MoveDir(this TransCom pTransCom, Vector3 pDir, float pSpeed)
        {
            // Vector3 delta = pDir.normalized *  pSpeed  * Time.deltaTime;
            // Vector3 movePos = pTransCom.Pos + delta;
            // pTransCom.WaitSetPos(movePos);
        }

        /// <summary>
        /// 旋转
        /// </summary>
        /// <param name="pTransCom"></param>
        /// <param name="pRoate"></param>
        public static void Roate(this TransCom pTransCom, Vector3 pRoate)
        {
            //pTransCom.WaitSetRoate(pRoate);
        }
    }
}
