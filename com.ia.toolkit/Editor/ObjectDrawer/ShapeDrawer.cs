// using IAToolkit.Core;
// using System;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
//
// namespace IAToolkit
// {
//     [CustomObjectDrawer(typeof(Shape))]
//     public class ShapeDrawer : ObjectDrawer
//     {
//         private List<string> shapeTyps = new List<string>();
//         public ShapeDrawer()
//         {
//             shapeTyps.Clear();
//             foreach (var item in Enum.GetNames(typeof(ShapeType)))
//             {
//                 shapeTyps.Add(item);
//             }
//         }
//
//         public override object OnGUI(Rect _position, GUIContent _label)
//         {
//             Shape shape = (Shape)Target;
//
//             EditorGUILayout.LabelField(_label);
//
//             shape.ShapeType = (ShapeType)EditorGUILayout.EnumPopup("形状:", shape.ShapeType);
//             switch (shape.ShapeType)
//             {
//                 case ShapeType.AABB:
//                     shape.AABBMin = EditorGUILayout.Vector2Field(GUIHelper.TextContent("Min:"), shape.AABBMin);
//                     shape.AABBMax = EditorGUILayout.Vector2Field(GUIHelper.TextContent("Max:"), shape.AABBMax);
//                     break;
//                 case ShapeType.Circle:
//                     shape.Center = EditorGUILayout.Vector2Field(GUIHelper.TextContent("Center:"), shape.Center);
//                     shape.CircleRadius = EditorGUILayout.FloatField(GUIHelper.TextContent("CircleRadius:"), shape.CircleRadius);
//                     break;
//                 case ShapeType.Polygon:
//                     if (shape.PolygonVertices == null)
//                         shape.PolygonVertices = new List<Vector2>();
//                     int count = shape.PolygonVertices.Count;
//                     int newCount = EditorGUILayout.IntField(GUIHelper.TextContent("Size"), count);
//                     if (count > newCount)
//                     {
//                         for (int i = newCount - 1; i < count - 1; i++)
//                             shape.PolygonVertices.RemoveAt(i);
//                     }
//                     else if (count < newCount)
//                     {
//                         for (int i = count - 1; i < newCount - 1; i++)
//                             shape.PolygonVertices.Add(new Vector2());
//                     }
//                     if (count == newCount)
//                     {
//                         for (int i = 0; i < shape.PolygonVertices.Count; i++)
//                         {
//                             shape.PolygonVertices[i] = EditorGUILayout.Vector2Field(GUIHelper.TextContent($"Index:{i + 1}"), shape.PolygonVertices[i]);
//                         }
//                     }
//                     break;
//                 default:
//                     break;
//             }
//
//             EditorGUILayout.Space(2);
//             return shape;
//         }
//
//         public override float GetHeight()
//         {
//             return 0;
//         }
//     }
// }
