using IAToolkit;
using UnityEditor;
using UnityEngine;
using IAEngine;
using static IAToolkit.GUIHelper;

namespace IAToolkit
{
    [CustomEditor(typeof(GroupAssetSetting), true)]
    internal class GroupAssetSettingInspector : Editor
    {
        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();
        static ContextData<GUIStyle> BigLabel;

        private GroupAssetSetting settingAsset;

        private void OnEnable()
        {
            settingAsset = (GroupAssetSetting)target;
            foreach (var item in ReflectionHelper.GetChildTypes<InternalGroupAsset>())
            {
                if (item.IsAbstract)
                    continue;
                if (settingAsset.GetSearchPath(item.FullName) == null)
                {
                    var tempData = ReflectionHelper.CreateInstance(item) as InternalGroupAsset;
                    settingAsset.groupPaths.Add(new GroupPath(item,tempData.ChildType));
                }
            }

            for (int i = 0; i < settingAsset.groupPaths.Count; i++)
            {
                GroupPath path = settingAsset.groupPaths[i];
                System.Type type = ReflectionHelper.GetType(path.typeFullName,ReflectionHelper.EditorAssemblyName);
                if (type == null || !typeof(InternalGroupAsset).IsAssignableFrom(type))
                {
                    settingAsset.groupPaths.RemoveAt(i);
                    EditorUtility.SetDirty(settingAsset);
                }
            }
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
                GUILayout.Label($"分组路径设置", BigLabel.value);
            });

            foreach (var item in settingAsset.groupPaths)
            {
                DrawGraphPath(item);
            }
        }

        private void DrawGraphPath(GroupPath path)
        {
            GUILayoutExtension.VerticalGroup(() =>
            {
                GUILayout.Label($"{path.typeName}", BigLabel.value);
                string newPath = MiscHelper.FolderPath("分组目录:", path.searchPath);
                if (newPath != path.searchPath)
                {
                    path.searchPath = newPath;
                    EditorUtility.SetDirty(settingAsset);
                }
                
                string exportPath = MiscHelper.FolderPath("导出目录:", path.exportPath);
                if (exportPath != path.exportPath)
                {
                    path.exportPath = exportPath;
                    EditorUtility.SetDirty(settingAsset);
                }

                string exportFileName = EditorGUILayout.TextField("导出文件名:", path.exportFileName);
                if (exportFileName != path.exportFileName)
                {
                    path.exportFileName = exportFileName;
                    EditorUtility.SetDirty(settingAsset);
                }
                
                string exportExName = EditorGUILayout.TextField("导出文件扩展名:", path.exportExName);
                if (exportExName != path.exportExName)
                {
                    path.exportExName = exportExName;
                    EditorUtility.SetDirty(settingAsset);
                }
            });
        }
    }
}