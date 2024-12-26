using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace IAToolkit
{
    public static partial class GUILayoutExtension
    {
        /// <summary>
        /// 创建一个显示隐藏组
        /// </summary>
        /// <param name="key">用于存取上下文数据</param>
        /// <param name="visible">是否可见</param>
        /// <param name="drawFunc">组内绘制函数</param>
        /// <param name="speed">动画速度</param>
        /// <returns></returns>
        public static void FadeGroup(string key, bool visible, Action drawFunc, float speed = 1)
        {
            if (!GUIHelper.TryGetContextData<AnimFloat>(key, out var contextData))
                contextData.value = new AnimFloat(visible ? 1 : 0);
            contextData.value.speed = speed * (visible ? 1 : 2);
            contextData.value.target = visible ? 1 : 0;
            float t = contextData.value.value;
            if (visible)
            {
                t--;
                t = -(t * t * t * t - 1);
            }
            else
            {
                t = t * t;
            }

            GUIExtension.SetAlpha(t, () =>
            {
                if (EditorGUILayout.BeginFadeGroup(t))
                {
                    drawFunc?.Invoke();
                }
                EditorGUILayout.EndFadeGroup();
            });
        }

        /// <summary>
        /// 创建一个垂直的布局
        /// </summary>
        /// <param name="drawFunc">组内绘制函数</param>
        /// <param name="options">布局属性</param>
        /// <returns></returns>
        public static void VerticalGroup(Action drawFunc,params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(EditorStylesExtension.RoundedBoxStyle, options);
            drawFunc?.Invoke();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 创建一个垂直的布局
        /// </summary>
        /// <param name="drawFunc">组内绘制函数</param>
        /// <param name="options">布局属性</param>
        /// <returns></returns>
        public static void VerticalGroup(Action drawFunc,GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style, options);
            drawFunc?.Invoke();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 创建一个水平的布局
        /// </summary>
        /// <param name="drawFunc">组内绘制函数</param>
        /// <param name="options">布局属性</param>
        /// <returns></returns>
        public static Rect HorizontalGroup(Action drawFunc, params GUILayoutOption[] options)
        {
            Rect rect = EditorGUILayout.BeginHorizontal(EditorStylesExtension.RoundedBoxStyle, options);
            drawFunc?.Invoke();
            EditorGUILayout.EndHorizontal();
            return rect;
        }

        /// <summary>
        /// 创建一个水平的布局
        /// </summary>
        /// <param name="drawFunc">组内绘制函数</param>
        /// <param name="options">布局属性</param>
        /// <returns></returns>
        public static Rect HorizontalGroup(Action drawFunc, GUIStyle style, params GUILayoutOption[] options)
        {
            Rect rect = EditorGUILayout.BeginHorizontal(style, options);
            drawFunc?.Invoke();
            EditorGUILayout.EndHorizontal();
            return rect;
        }

        /// <summary>
        /// 创建一个滚动视图
        /// </summary>
        public static void ScrollView(ref Vector2 pos, Action drawFunc, params GUILayoutOption[] options)
        {
            pos = GUILayout.BeginScrollView(pos, EditorStylesExtension.RoundedBoxStyle, options);
            {
                drawFunc?.Invoke();
            }
            GUILayout.EndScrollView();
        }


        /// <summary>
        /// 创建一个滚动视图
        /// </summary>
        public static void ScrollView(ref Vector2 pos, Action drawFunc, GUIStyle style, params GUILayoutOption[] options)
        {
            pos = GUILayout.BeginScrollView(pos, EditorStylesExtension.RoundedBoxStyle, options);
            {
                drawFunc?.Invoke();
            }
            GUILayout.EndScrollView();
        }

        public static float ScrollView(float scroll, int totalCnt, Action<int> drawFunc, int count = 10)
        {
            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal();
            Rect r = EditorGUILayout.BeginVertical();

            if (totalCnt > count)
            {
                int startIndex = Mathf.CeilToInt(totalCnt * scroll);
                startIndex = Mathf.Max(0, startIndex);
                for (int i = startIndex; i < startIndex + count; i++)
                {
                    drawFunc?.Invoke(i);
                }
            }
            else
            {
                for (int i = 0; i < totalCnt; i++)
                {
                    drawFunc?.Invoke(i);
                }
            }

            EditorGUILayout.EndVertical();
            if (totalCnt > count)
            {
                GUILayout.Space(20);
                if (totalCnt > count)
                {
                    if (Event.current.type == EventType.ScrollWheel && r.Contains(Event.current.mousePosition))
                    {
                        scroll += Event.current.delta.y * 0.01f;
                        Event.current.Use();
                    }

                    r.xMin += r.width + 5;
                    r.width = 20;
                    scroll = GUI.VerticalScrollbar(r, scroll, (float)count / totalCnt, 0, 1);
                }
            }
            GUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            return scroll;
        }

        /// <summary>
        /// 创建一个开关布局
        /// </summary>
        /// <param name="label">开关名</param>
        /// <param name="foldout">是否展开</param>
        /// <param name="enable">是否开启</param>
        /// <param name="drawFunc">布局内绘制函数</param> 
        /// <returns></returns>
        public static (bool foldout, bool enable) ToggleGroup(string label, bool foldout, bool enable, Action drawFunc)
        {
            VerticalGroup(() => {
                Rect rect = GUILayoutUtility.GetRect(50, 25);
                rect = EditorGUI.IndentedRect(rect);
                Rect toggleRect = new Rect(rect.x + 20, rect.y, rect.height, rect.height);

                Event current = Event.current;
                if (current.type == EventType.MouseDown && current.button == 0)
                {
                    if (toggleRect.Contains(current.mousePosition))
                    {
                        enable = !enable;
                    }
                    else if (rect.Contains(current.mousePosition))
                    {
                        foldout = !foldout;
                    }
                }

                switch (current.type)
                {
                    case EventType.MouseDown:
                    case EventType.MouseUp:
                    case EventType.Repaint:
                        GUI.Box(rect, string.Empty, GUI.skin.button);

                        Rect t = rect;
                        t.xMin += 5;
                        t.xMax = t.xMin + t.height;
                        EditorGUI.Foldout(t, foldout, string.Empty);

                        toggleRect.width = rect.width - t.width;
                        EditorGUI.ToggleLeft(toggleRect, label, enable);
                        break;
                    default:
                        break;
                }

                EditorGUI.BeginDisabledGroup(!enable);
                EditorGUI.indentLevel++;

                if (enable)
                    drawFunc?.Invoke();

                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup();
            });
            return (foldout, enable);
        }

        /// <summary>
        /// 创建一个折叠区域
        /// </summary>
        /// <param name="label">折叠名</param>
        /// <param name="foldout"><是否折叠/param>
        /// <returns></returns>
        public static void FoldoutGroup(string label, bool foldout,Action drawFunc,Action hideDrawFunc)
        {
            VerticalGroup(() =>
            {
                Rect rect = GUILayoutUtility.GetRect(50, 25);
                rect = EditorGUI.IndentedRect(rect);

                Event current = Event.current;
                if (current.type == EventType.MouseDown && current.button == 0)
                {
                    if (rect.Contains(current.mousePosition))
                    {
                        foldout = !foldout;
                    }
                }
                
                GUI.Box(rect, string.Empty, GUI.skin.button);
                        
                Rect t = rect;
                t.xMin += 5;
                t.xMax -= 5;
                EditorGUI.Foldout(t, foldout, label);
                
                if (foldout)
                    drawFunc?.Invoke();
                else
                    hideDrawFunc?.Invoke();
            });
        }

    }
}
