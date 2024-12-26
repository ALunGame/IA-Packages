using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace IAToolkit
{
    public static partial class GUILayoutExtension
    {
        /// <summary>
        /// 绘制一个可接收多个拖拽资源的区域
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static UnityObject[] DragDropAreaMulti(params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return GUIExtension.DragDropAreaMulti(rect);
        }

        /// <summary>
        /// 绘制一个可接收多个拖拽资源的区域
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static UnityObject[] DragDropAreaMulti(DragAndDropVisualMode dropVisualMode, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return GUIExtension.DragDropAreaMulti(rect, dropVisualMode);
        }

        /// <summary>
        /// 绘制一个可接收多个拖拽资源的区域
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static UnityObject[] DragDropAreaMulti(Color hightlightColor, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return GUIExtension.DragDropAreaMulti(rect, hightlightColor);
        }

        /// <summary>
        /// 绘制一个可接收多个拖拽资源的区域
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static UnityObject[] DragDropAreaMulti(DragAndDropVisualMode dropVisualMode, Color hightlightColor, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return GUIExtension.DragDropAreaMulti(rect, dropVisualMode, hightlightColor);
        }

        /// <summary>
        /// 绘制一个可接收一个拖拽资源的区域
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static UnityObject DragDropAreaSingle(params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return GUIExtension.DragDropAreaSingle(rect);
        }

        /// <summary>
        /// 绘制一个可接收一个拖拽资源的区域
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static UnityObject DragDropAreaSingle(DragAndDropVisualMode dropVisualMode, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return GUIExtension.DragDropAreaSingle(rect, dropVisualMode);
        }

        /// <summary>
        /// 绘制一个可接收一个拖拽资源的区域
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static UnityObject DragDropAreaSingle(Color hightlightColor, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return GUIExtension.DragDropAreaSingle(rect, hightlightColor);
        }

        /// <summary>
        /// 绘制一个可接收一个拖拽资源的区域
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static UnityObject DragDropAreaSingle(DragAndDropVisualMode dropVisualMode, Color hightlightColor, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.label, options);
            return GUIExtension.DragDropAreaSingle(rect, dropVisualMode, hightlightColor);
        }
    }
}
