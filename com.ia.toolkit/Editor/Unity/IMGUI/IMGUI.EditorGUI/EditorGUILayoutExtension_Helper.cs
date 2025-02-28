using System;
using UnityEngine;

namespace IAToolkit.UnityEditors
{
    public static partial class EditorGUILayoutExtension
    {
        public static Rect VerticalGroup(Action pFunc, params GUILayoutOption[] options)
        {
            Rect rect = BeginVerticalBoxGroup(options);

            pFunc?.Invoke();

            EndVerticalBoxGroup();

            return rect;
        }

        public static Rect HorizontalGroup(Action pFunc, params GUILayoutOption[] options)
        {
            Rect rect = BeginHorizontalBoxGroup(options);

            pFunc?.Invoke();

            EndHorizontalBoxGroup();

            return rect;
        }
    }
}
