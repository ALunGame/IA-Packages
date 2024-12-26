// using UnityEditor;
// using UnityEngine;
//
// namespace IAToolkit.Core
// {
//     [CustomObjectDrawer(typeof(UnityGoRelativePath))]
//     public class UnityGoRelativePathDrawer : ObjectDrawer
//     {
//         private string GetPathParentToChild(Transform child,Transform rootTrans)
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
//                 while (selectChild.parent != null && !selectChild.parent.Equals(rootTrans))
//                 {
//                     selectChild = selectChild.parent;
//                     result = string.Format("{0}/{1}", selectChild.name, result);
//                 }
//             }
//             return result;
//         }
//
//         public override object OnGUI(Rect _position, GUIContent _label)
//         {
//             if (Target == null)
//                 return null;
//
//             var target = Target as UnityGoRelativePath;
//             GameObject tmpGo = target.GetObj();
//             tmpGo = (GameObject)EditorGUILayout.ObjectField(_label, tmpGo, typeof(GameObject), true);
//             if (tmpGo != null && !tmpGo.Equals(target.Go))
//             {
//                 string tempPath = GetPathParentToChild(tmpGo.transform, target.RootGo.transform);
//                 if (!string.IsNullOrEmpty(tempPath))
//                 {
//                     target.RelativePath = tempPath;
//                     target.Go = tmpGo;
//                     target.Dispatch();
//                 }
//             }
//
//             return Target;
//         }
//     }
// }