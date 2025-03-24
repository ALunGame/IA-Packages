using System;
using UnityEditor;
using UnityEngine;
using IAToolkit;
using IAToolkit.UnityEditors;

namespace IAToolkit.Misc
{
    public class InputWindow : EditorWindow
    {
        public string InputStr = "";
        public Action<string> CallBack;

        private void OnGUI()
        {
            EditorGUILayoutExtension.BeginVerticalBoxGroup();

            InputStr = EditorGUILayout.TextField("请输入：", InputStr);

            EditorGUILayout.Space();

            MiscHelper.Btn("确定", position.width * 0.9f, position.height * 0.5f, () => {
                if (CallBack != null && InputStr != "")
                {
                    CallBack(InputStr);
                    Close();
                }
            });

            EditorGUILayoutExtension.EndVerticalBoxGroup();
        }

        public static void PopWindow(string strContent, Action<string> callBack)
        {
            Vector2 mousePos = Event.current.mousePosition;
            Vector2 screenPos = GUIUtility.GUIToScreenPoint(mousePos);
            Rect rect = new Rect(screenPos, new Vector2(250, 80));
            InputWindow window = GetWindowWithRect<InputWindow>(rect, true, strContent);
            //window.position = rect;
            window.CallBack = callBack;
            window.Focus();
        }

        public static void PopWindow(string strContent, Action<string> callBack, Vector2 mousePosition)
        {
            Rect rect = new Rect(mousePosition, new Vector2(250, 80));
            InputWindow window = GetWindowWithRect<InputWindow>(rect, true, strContent);
            //window.position = rect;
            window.CallBack = callBack;
            window.Focus();
        }
    }
}
