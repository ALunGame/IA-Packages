using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.UIElements.Cursor;

namespace IAToolkit.Element
{
    public static class Element_Help
    {
        /// <summary>
        /// 实现GUI绘制
        /// </summary>
        /// <param name="pElement"></param>
        /// <param name="pDrawFunc"></param>
        /// <returns></returns>
        public static IMGUIContainer CreateGUIFunc(this VisualElement pElement, System.Action pDrawFunc)
        {
            IMGUIContainer guiElement = new IMGUIContainer(pDrawFunc);
            guiElement.name = "gui";
            guiElement.pickingMode = PickingMode.Ignore;
            guiElement.style.position = Position.Absolute;
            guiElement.style.color = new StyleColor(Color.clear);
            guiElement.style.flexGrow = 1;
            guiElement.style.flexShrink = 0;
            guiElement.style.bottom = 0;
            guiElement.style.top = 0;
            guiElement.style.left = 0;
            guiElement.style.right = 0;
            pElement.Add(guiElement);
            return guiElement;
        }
        
        
        /// <summary>
        /// 设置边框
        /// </summary>
        /// <param name="pElement"></param>
        /// <param name="pWidth"></param>
        /// <param name="pColor"></param>
        public static void BorderWidthColor(this VisualElement pElement, float pWidth, Color pColor)
        {
            pElement.style.borderTopColor = new StyleColor(pColor);
            pElement.style.borderTopWidth = pWidth;
            pElement.style.borderBottomColor = new StyleColor(pColor);
            pElement.style.borderBottomWidth = pWidth;
            pElement.style.borderLeftColor = new StyleColor(pColor);
            pElement.style.borderLeftWidth = pWidth;
            pElement.style.borderRightColor = new StyleColor(pColor);
            pElement.style.borderRightWidth = pWidth;
        }
        
        
        /// <summary>
        /// 禁止ScrollView滚动
        /// </summary>
        public static void BanScrollViewScroll(this ScrollView pScrollView)
        {
            if (pScrollView == null)
            {
                return;
            }
            pScrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            pScrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            pScrollView.contentContainer.RegisterCallback<WheelEvent>(new EventCallback<WheelEvent>((evt) =>
            {
                evt.StopImmediatePropagation();
            }));
        }
        
        /// <summary>
        /// 禁止ScrollView鼠标滚动
        /// </summary>
        public static void BanScrollViewScrollWheel(this ScrollView pScrollView)
        {
            if (pScrollView == null)
            {
                return;
            }
            pScrollView.contentContainer.RegisterCallback<WheelEvent>(new EventCallback<WheelEvent>((evt) =>
            {
                evt.StopImmediatePropagation();
            }));
        } 
        
        /// <summary>
        /// 设置元素鼠标类型
        /// </summary>
        /// <param name="element"></param>
        /// <param name="cursor"></param>
        public static void SetCursor(this VisualElement element, MouseCursor cursor)
        {
            object objCursor = new Cursor();
            PropertyInfo fields = typeof(Cursor).GetProperty("defaultCursorId", BindingFlags.NonPublic | BindingFlags.Instance);
            fields.SetValue(objCursor, (int)cursor);
            element.style.cursor = new StyleCursor((Cursor)objCursor);
        }
    }
}