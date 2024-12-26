using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityObject = UnityEngine.Object;
using IAEngine;

#if UNITY_EDITOR

using UnityEditor.SceneManagement;
using UnityEditor;

#endif

namespace IAToolkit
{
    /// <summary>
    /// 需要序列化的资源
    /// </summary>
    public class UnityObjectAsset
    {
        public enum AssetType
        {
            GameObj,
            AnimClip,
            Sprite,
        }

        [JsonIgnore]
        public UnityObject Obj;
        
        public AssetType assetType = AssetType.GameObj;
        public string ObjName = "";
        public string ObjPath = "";

        public UnityObjectAsset()
        {
            
        }

        public UnityObjectAsset(AssetType assetType)
        {
            this.assetType = assetType;
        }

#if UNITY_EDITOR

        public UnityObject GetObj()
        {
            if (string.IsNullOrEmpty(ObjPath))
                return null;
            return AssetDatabase.LoadAssetAtPath(ObjPath, GetObjType());
        }

        public Type GetObjType()
        {
            Type objType = typeof(GameObject);
            switch (assetType)
            {
                case AssetType.GameObj:
                    objType = typeof(GameObject);
                    break;
                case AssetType.AnimClip:
                    objType = typeof(AnimationClip);
                    break;
                case AssetType.Sprite:
                    objType = typeof(Sprite);
                    break;
                default:
                    break;
            }
            return objType;
        }

        public void SetObject(UnityObject pObject)
        {
            if (pObject == null)
            {
                Obj = null;
                ObjName = "";
                ObjPath = "";
                return;
            }
            Type objType = pObject.GetType();
            if (!objType.Equals(GetObjType()))
            {
                Obj = null;
                ObjName = "";
                ObjPath = "";
                return;
            }
            Obj = pObject;
            ObjName = AssetDatabase.GetAssetPath(pObject);
            ObjPath = pObject.name;
        }
#endif
    }
}