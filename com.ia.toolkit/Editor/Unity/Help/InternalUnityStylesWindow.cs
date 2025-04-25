using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace IAToolkit.UnityEditors
{
    internal class InternalUnityStylesWindow : EditorWindow
    {
        [MenuItem("Tools/Unity/内部样式")]
        private static void GetEditorStyles()
        {
            InternalUnityStylesWindow window = GetWindow<InternalUnityStylesWindow>("显示Unity内部样式");
            window.Show();
        }
        
        private List<GUIStyle> GUIStyles = new List<GUIStyle>();                       // 存储GUIStyle
        private Vector2 stylesScrollPosition = Vector2.zero;    // 滚动位置

        private void OnEnable()
        {
            GUIStyles.Clear();
            
            // 先反射获取 EditorStyles 下的所有样式，然后在窗口中依次绘制出来，BindingFlags 表示反射属性的类型
            // BindingFlags.Static 与 BindingFlags.Instance 二者必须有一项或者都有
            BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public;
            PropertyInfo[] propertyInfos = typeof(EditorStyles).GetProperties(bindingFlags);

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                GUIStyle gUIStyle = propertyInfo.GetValue(null, null) as GUIStyle;
                if (gUIStyle != null) GUIStyles.Add(gUIStyle);
            }
        }

        private void OnGUI()
        {
            stylesScrollPosition = GUILayout.BeginScrollView(stylesScrollPosition);
            for (int i = 0; i < GUIStyles.Count; i++)
            {
                if (GUILayout.Button(GUIStyles[i].name, GUIStyles[i]))
                {
                    GUIUtility.systemCopyBuffer = GUIStyles[i].name;
                    Debug.Log($"复制样式成功：{GUIStyles[i].name}");
                }
            }
            GUILayout.EndScrollView();
        }
    }
}