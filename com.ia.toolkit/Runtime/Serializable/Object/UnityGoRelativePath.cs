using System;
using IAEngine;
using UnityEngine;

namespace IAToolkit
{
    /// <summary>
    /// 资源相对坐标
    /// </summary>
    public class UnityGoRelativePath
    {
        //根节点
        [JsonIgnore]
        public GameObject RootGo;

        [JsonIgnore]
        public GameObject Go;
        
        public event Action OnObjectAssetChange;

        //相对路径
        public string RelativePath;

        public UnityGoRelativePath(GameObject rootGo)
        {
            RootGo = rootGo;
        }

        public UnityGoRelativePath()
        {
            
        }
        
#if UNITY_EDITOR
        
        public void SetRootGo(GameObject rootGo)
        {
            RootGo = rootGo;
        }

        public GameObject GetObj()
        {
            if (string.IsNullOrEmpty(RelativePath) || RootGo == null)
                return null;

            Transform tmpObj = RootGo.transform.Find(RelativePath);
            return tmpObj.gameObject;
        }
        
        public void UpdateObj()
        {
            GameObject tGo = GetObj();
            Go = tGo;
            Dispatch();
        }

        public void Dispatch()
        {
            OnObjectAssetChange?.Invoke();
        }
#endif
    }
}