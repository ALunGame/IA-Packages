using System;
using UnityEditor;
using UnityEngine;
using IAToolkit;

namespace IAToolkit.Misc
{
    public class InputWindow : EditorWindow
    {
        public string InputStr = "";
        public Action<string> CallBack;

        private void OnGUI()
        {
            GUILayoutExtension.VerticalGroup(() =>
            {
                InputStr = EditorGUILayout.TextField("请输入：", InputStr);

                EditorGUILayout.Space();

                MiscHelper.Btn("确定", position.width * 0.9f, position.height * 0.5f, () => {
                    if (CallBack != null && InputStr != "")
                    {
                        CallBack(InputStr);
                        Close();
                    }
                });
            });
        }

        public static void PopWindow(string strContent, Action<string> callBack)
        {
            Rect rect = new Rect(new Vector2(0, 0), new Vector2(250, 80));
            if (Event.current != null)
            {
                rect = new Rect(Event.current.mousePosition, new Vector2(250, 80));
            }
            InputWindow window = GetWindowWithRect<InputWindow>(rect, true, strContent);
            //window.position = rect;
            window.CallBack = callBack;
            window.Focus();
        }
    }
}
