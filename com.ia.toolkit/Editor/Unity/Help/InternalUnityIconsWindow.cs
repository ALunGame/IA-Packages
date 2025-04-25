using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace IAToolkit.UnityEditors
{
    public class InternalUnityIconsWindow : EditorWindow
    {
        [MenuItem("Tools/Unity/内部图标")]
        private static void GetEditorStyles()
        {
            InternalUnityIconsWindow window = GetWindow<InternalUnityIconsWindow>("显示Unity内部图标");
            window.Show();
        }
        
        private List<GUIContent> icons = new List<GUIContent>();                  
        
        private void OnEnable()
        {
            icons.Clear();
            
            Texture2D[] textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            foreach (Texture2D texture in textures)
            {
                GUIContent icon = EditorGUIUtility.IconContent(texture.name, $"|{texture.name}");
                if (icon != null && icon.image != null) 
                    icons.Add(icon);
            }
        }

        private void OnGUI()
        {
            for (int i = 0; i < icons.Count; i += 35)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < 35; j++)
                {
                    if (i + j < icons.Count)
                    {
                        if (GUILayout.Button(icons[i + j], GUILayout.Width(25), GUILayout.Height(25)))
                        {
                            GUIUtility.systemCopyBuffer = icons[i + j].image.name;
                            Debug.Log($"复制图标成功：{icons[i + j].image.name}");
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}