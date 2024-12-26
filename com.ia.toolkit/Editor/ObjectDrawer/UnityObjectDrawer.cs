// using System;
// using UnityEditor;
// using UnityEngine;
// using UnityObject = UnityEngine.Object;
//
// namespace IAToolkit.Core
// {
//     [CustomObjectDrawer(typeof(UnityObjectAsset))]
//     public class UnityObjectDrawer : ObjectDrawer
//     {
//         private GameObject GetRootGo(GameObject gameObject)
//         {
//             if (gameObject == null || gameObject.transform.parent == null)
//                 return gameObject;
//             return GetRootGo(gameObject.transform.parent.gameObject);
//         }
//
//         private string GetPathParentToChild(Transform child)
//         {
//             if (child == null)
//             {
//                 return "";
//             }
//
//             Transform selectChild = child.transform;
//             string result = "";
//             if (selectChild != null)
//             {
//                 result = selectChild.name;
//                 while (selectChild.parent != null)
//                 {
//                     selectChild = selectChild.parent;
//                     result = string.Format("{0}/{1}", selectChild.name, result);
//                 }
//             }
//             return result;
//         }
//
//         private void GetAsset(UnityObjectAsset unityObject)
//         {
//             if (unityObject == null || string.IsNullOrEmpty(unityObject.ObjPath))
//                 return;
//             unityObject.Obj = unityObject.GetObj();
//         }
//
//         private void UpdateAssetPath(UnityObjectAsset unityObject)
//         {
//             if (unityObject == null || unityObject.Obj == null)
//             {
//                 unityObject.ObjPath = "";
//                 return;
//             }
//             unityObject.ObjPath = AssetDatabase.GetAssetPath(unityObject.Obj);
//             unityObject.ObjName = unityObject.Obj.name;
//         }
//
//         public override object OnGUI(Rect _position, GUIContent _label)
//         {
//             if (Target == null)
//             {
//                 Target = new UnityObjectAsset();
//             }
//
//             UnityObjectAsset unityObject = Target as UnityObjectAsset;
//             Type objType = unityObject.GetObjType();
//
//             GetAsset(unityObject);
//             UnityObject tmpObj = unityObject.Obj;
//             tmpObj = EditorGUI.ObjectField(_position, _label, tmpObj, objType, false);
//             if (tmpObj != null && !tmpObj.Equals(unityObject.Obj))
//             {
//                 unityObject.Obj = tmpObj;
//                 UpdateAssetPath(unityObject);
//             }
//
//             return Target;
//         }
//     }
// }