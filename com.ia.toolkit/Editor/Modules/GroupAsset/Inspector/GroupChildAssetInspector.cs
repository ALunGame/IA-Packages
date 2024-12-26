using UnityEditor;
using UnityEngine;

namespace IAToolkit.Inspector
{
    [CustomEditor(typeof(GroupChildAsset), true)]
    public class GroupChildAssetInspector : Editor
    {
        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();

        public override void OnInspectorGUI()
        {
            DrawFileName(target);
            
            base.OnInspectorGUI();
        }

        public static void DrawFileName(Object obj)
        {
            if (!ContextDataCache.TryGetContextData<GUIStyle>("BigLabel", out var bigLabel))
            {
                bigLabel.value = new GUIStyle(GUI.skin.label);
                bigLabel.value.fontSize = 18;
                bigLabel.value.fontStyle = FontStyle.Bold;
                bigLabel.value.alignment = TextAnchor.MiddleLeft;
                bigLabel.value.stretchWidth = true;
            }
            
            GroupChildAsset asset = obj as GroupChildAsset;
            GUILayoutExtension.VerticalGroup(() =>
            {
                EditorGUILayout.LabelField("资源名:",asset.FileName(),bigLabel.value);

                EditorGUILayout.Space(5);
                
                if (GUILayout.Button("复制资源名",GUILayout.Height(30)))
                {
                    UnityEngine.GUIUtility.systemCopyBuffer = asset.FileName();
                }
            });
        }
    }
}