using UnityEngine;

namespace IAEngine
{
    public static class TransformEx
    {
        public static void SetActive(this Transform pTrans,string pPath, bool pActive)
        {
            if (pTrans == null)
                return;
            pTrans.gameObject.SetActive(pPath, pActive);
        }

        public static bool Find(this Transform pTrans, string pPath,out Transform pFindTrans)
        {
            pFindTrans = null;
            if (pTrans == null)
                return false;

            if (string.IsNullOrEmpty(pPath))
            {
                pFindTrans = pTrans;
                return true;
            }

            pFindTrans = pTrans.Find(pPath);
            if (pFindTrans == null)
                return false;
            return true;
        }
        
        public static void HideAllChild(this Transform pTrans)
        {
            if (pTrans == null)
                return;
            int childCnt = pTrans.childCount;
            if (childCnt <= 0)
            {
                return;;
            }

            for (int i = 0; i < childCnt; i++)
            {
                pTrans.GetChild(i).SetActive(null,false);
            }
        }

        /// <summary>
        /// 重置位置,旋转,缩放,
        /// </summary>
        /// <param name="trans"></param>
        public static void Reset(this Transform trans)
        {
            trans.transform.localPosition = Vector3.zero;
            trans.transform.localRotation = Quaternion.identity;
            trans.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 重置位置,旋转
        /// </summary>
        /// <param name="trans"></param>
        public static void ResetNoScale(this Transform trans)
        {
            trans.transform.localPosition = Vector3.zero;
            trans.transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// 重置位置,旋转
        /// </summary>
        /// <param name="trans"></param>
        public static T GetOrAddCom<T>(this Transform trans) where T : Component
        {
            if (trans.GetComponent<T>() != null)
            {
                return trans.GetComponent<T>();
            }
            else
            {
                return trans.gameObject.AddComponent<T>();
            }
        }
    }
}