using UnityEngine;

namespace IAEngine
{
    public static class GameObjectEx 
    {
        /// <summary>
        /// 空检查
        /// </summary>
        public static bool IsNull(this GameObject pGo) => ReferenceEquals(pGo, null);

        /// <summary>
        /// 非空检查
        /// </summary>
        public static bool NotNull(this GameObject pGo) => !ReferenceEquals(pGo, null);

        public static void SetActive(this GameObject pGo, string pPath, bool pActive)
        {
            if (pGo == null)
                return;
            if (string.IsNullOrEmpty(pPath))
                pGo.SetActive(pActive);
            else
            {
                Transform findTrans = pGo.transform.Find(pPath);
                if (findTrans != null)
                    findTrans.gameObject.SetActive(pActive);
            }
        }

        public static void SetChildActive(this GameObject pGo, string pChildName, bool pActive)
        {
            if (pGo == null)
                return;
            if (string.IsNullOrEmpty(pChildName))
                return;
            for (int i = 0; i < pGo.transform.childCount; i++)
            {
                Transform childTrans = pGo.transform.GetChild(i);
                if (childTrans.name == pChildName)
                {
                    childTrans.gameObject.SetActive(pActive);
                }
                else
                {
                    childTrans.gameObject.SetActive(!pActive);
                }
            }
        }

        public static bool Find(this GameObject pGo, string pPath,out Transform pFindTrans)
        {
            pFindTrans = null;
            if (pGo == null)
                return false;
            return pGo.transform.Find(pPath,out pFindTrans);
        }
    }
}