using IAToolkit;
using UnityEditor;
using UnityEngine;

namespace IAConfig.Excel.Inspector
{
    [CustomEditor(typeof(ExcelReadSetting), true)]
    public class ExcelReadSettingInspector : Editor
    {
        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();
        static GUIHelper.ContextData<GUIStyle> BigLabel;

        private ExcelReadSetting settingAsset;

        private void OnEnable()
        {
            settingAsset = (ExcelReadSetting)target;
        }

        public override void OnInspectorGUI()
        {
            if (!ContextDataCache.TryGetContextData<GUIStyle>("BigLabel", out BigLabel))
            {
                BigLabel.value = new GUIStyle(GUI.skin.label);
                BigLabel.value.fontSize = 18;
                BigLabel.value.fontStyle = FontStyle.Bold;
                BigLabel.value.alignment = TextAnchor.MiddleLeft;
                BigLabel.value.stretchWidth = true;
            }

            GUILayoutExtension.VerticalGroup(() =>
            {
                GUILayout.Label($"配置路径设置", BigLabel.value);
            });

            GUILayoutExtension.VerticalGroup(() =>
            {
                string newPath = MiscHelper.FolderPath("表格读取目录:", settingAsset.ConfigRootPath);
                if (newPath != settingAsset.ConfigRootPath)
                {
                    settingAsset.ConfigRootPath = newPath;
                    EditorUtility.SetDirty(settingAsset);
                }
            });
            
            GUILayoutExtension.VerticalGroup(() =>
            {
                string newPath = MiscHelper.FolderPath("代码生成目录:", settingAsset.GenCodeRootPath);
                if (newPath != settingAsset.GenCodeRootPath)
                {
                    settingAsset.GenCodeRootPath = newPath;
                    EditorUtility.SetDirty(settingAsset);
                }
            });
            
            GUILayoutExtension.VerticalGroup(() =>
            {
                string newPath = MiscHelper.FolderPath("Json生成目录:", settingAsset.GenJsonRootPath);
                if (newPath != settingAsset.GenJsonRootPath)
                {
                    settingAsset.GenJsonRootPath = newPath;
                    EditorUtility.SetDirty(settingAsset);
                }
            });
            
            GUILayoutExtension.VerticalGroup(() =>
            {
                string newPath = EditorGUILayout.TextField("Json生成后缀名:", settingAsset.GenJsonExName);
                if (newPath != settingAsset.GenJsonExName)
                {
                    settingAsset.GenJsonExName = newPath;
                    EditorUtility.SetDirty(settingAsset);
                }
            });
        }
    }
}