using IAECS.Core;
using IAToolkit;
using System;
using System.Collections.Generic;
using IAEngine;
using UnityEditor;
using UnityEngine;

namespace IAECS.Config
{
    [CustomEditor(typeof(SystemSortAsset), true)]
    public class SystemSortAssetInspector : Editor
    {
        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();

        private SystemSortAsset sortAsset;
        private List<string> systemTyps = new List<string>();

        private void OnEnable()
        {
            systemTyps.Clear();
            foreach (var item in Enum.GetNames(typeof(SystemType)))
            {
                systemTyps.Add(item);
            }

            sortAsset = target as SystemSortAsset;
            foreach (var item in ReflectionHelper.GetChildTypes<BaseSystem>())
            {
                if (sortAsset.GetSystemSort(item.FullName) == null)
                {
                    SystemSort sort = new SystemSort();
                    sort.typeName = item.Name;
                    sort.typeFullName = item.FullName;
                    sort.sort = sortAsset.updateSystems.Count;
                    sortAsset.updateSystems.Add(sort);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            if (!ContextDataCache.TryGetContextData<GUIStyle>("BigLabel", out var bigLabel))
            {
                bigLabel.value = new GUIStyle(GUI.skin.label);
                bigLabel.value.fontSize = 18;
                bigLabel.value.fontStyle = FontStyle.Bold;
                bigLabel.value.alignment = TextAnchor.MiddleLeft;
                bigLabel.value.stretchWidth = true;
            }

            MiscHelper.Btn("清除废弃系统", 250, 35, () =>
            {
                for (int i = 0; i < sortAsset.updateSystems.Count; i++)
                {
                    SystemSort sort = sortAsset.updateSystems[i];
                    if (ReflectionHelper.GetType(sort.typeFullName) == null)
                        sortAsset.updateSystems.RemoveAt(i);
                }

                for (int i = 0; i < sortAsset.fixedUpdateSystems.Count; i++)
                {
                    SystemSort sort = sortAsset.fixedUpdateSystems[i];
                    if (ReflectionHelper.GetType(sort.typeFullName) == null)
                        sortAsset.fixedUpdateSystems.RemoveAt(i);
                }

                for (int i = 0; i < sortAsset.threadSystems.Count; i++)
                {
                    SystemSort sort = sortAsset.threadSystems[i];
                    if (ReflectionHelper.GetType(sort.typeFullName) == null)
                        sortAsset.threadSystems.RemoveAt(i);
                }

                UpdateSystemSort();
            });

            GUILayoutExtension.VerticalGroup(() =>
            {
                GUILayout.Label($"系统运行排序", bigLabel.value);
            });

            GUILayoutExtension.VerticalGroup(() =>
            {
                GUILayout.Label(" ----------- Update ----------- ", bigLabel.value);
                List<SystemSort> upSorts = sortAsset.GetSystemSorts(SystemType.Update);
                for (int i = 0; i < upSorts.Count; i++)
                {
                    DrawSystemSort(upSorts[i], SystemType.Update);
                }

                GUILayout.Label(" ----------- FixedUpdate ----------- ", bigLabel.value);
                List<SystemSort> fixedSorts = sortAsset.GetSystemSorts(SystemType.FixedUpdate);
                for (int i = 0; i < fixedSorts.Count; i++)
                {
                    DrawSystemSort(fixedSorts[i], SystemType.FixedUpdate);
                }

                //GUILayout.Label(" ----------- Thread ----------- ", bigLabel.value);
                //List<SystemSort> threadSorts = sortAsset.GetSystemSorts(SystemType.Thread);
                //for (int i = 0; i < threadSorts.Count; i++)
                //{
                //    DrawSystemSort(threadSorts[i], SystemType.Thread);
                //}
            });
        }

        private void DrawSystemSort(SystemSort sort,SystemType systemType)
        {
            GUILayoutExtension.HorizontalGroup(() =>
            {
                EditorGUILayout.LabelField(sort.typeName);
                MiscHelper.Btn("Up", 50, 35, () =>
                {
                    sort.sort--;
                    UpdateSystemSort();
                });
                MiscHelper.Btn("Down", 50, 35, () =>
                {
                    sort.sort++;
                    UpdateSystemSort();
                });
                MiscHelper.Btn("Del", 50, 35, () =>
                {
                    sortAsset.GetSystemSorts(systemType).Remove(sort);
                    UpdateSystemSort();
                });
                MiscHelper.Dropdown(systemType.ToString(), systemTyps, (int x) =>
                {
                    SystemType newType = (SystemType)x;
                    if (newType == systemType)
                    {
                        return;
                    }
                    sortAsset.GetSystemSorts(systemType).Remove(sort);
                    sortAsset.GetSystemSorts(newType).Add(sort);
                    UpdateSystemSort();
                });
            });
        }

        private void UpdateSystemSort()
        {
            int SystemSortFunc(SystemSort a, SystemSort b)
            {
                int sysSort01 = a.sort;
                int sysSort02 = b.sort;

                if (sysSort01 == sysSort02)
                    return 0;
                else if (sysSort01 < sysSort02)
                    return -1;
                else
                    return 1;
            }
            sortAsset.GetSystemSorts(SystemType.Update).Sort(SystemSortFunc);
            sortAsset.GetSystemSorts(SystemType.FixedUpdate).Sort(SystemSortFunc);
            //sortAsset.GetSystemSorts(SystemType.Thread).Sort(SystemSortFunc);
            EditorUtility.SetDirty(sortAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}