using IAECS.Core;
using IAToolkit;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace IAECS.Config
{
    [CustomEditor(typeof(RequestSortAsset), true)]
    public class RequestSortAssetInspector : Editor
    {
        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();

        private RequestSortAsset sortAsset;

        private void OnEnable()
        {
            sortAsset = target as RequestSortAsset;
            foreach (var item in Enum.GetNames(typeof(RequestId)))
            {
                if (sortAsset.GetSort(item) == null)
                {
                    RequestSort sort = new RequestSort();
                    sort.key = item;
                    sort.name = item;
                    sort.sort = sortAsset.requests.Count;
                    sortAsset.requests.Add(sort);
                }
            }
            UpdateSystemSort();
        }

        private List<RequestSort> coverReqests = new List<RequestSort>();
        private List<RequestSort> customReqests = new List<RequestSort>();
        private List<RequestSort> sortReqests = new List<RequestSort>();

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

            MiscHelper.Btn("清除废弃请求", 250, 35, () =>
            {
                for (int i = 0; i < sortAsset.requests.Count; i++)
                {
                    RequestSort request = sortAsset.requests[i];
                    if (!Enum.TryParse<RequestId>(request.key,out RequestId id))
                    {
                        sortAsset.requests.RemoveAt(i);
                    }
                }
                UpdateSystemSort();
            });

            GUILayoutExtension.VerticalGroup(() =>
            {
                GUILayout.Label($"请求排序", bigLabel.value);
            });

            GUILayoutExtension.VerticalGroup(() =>
            {
                GUILayout.Label(" ----------- 覆盖请求 ----------- ", bigLabel.value);
                for (int i = 0; i < coverReqests.Count; i++)
                {
                    DrawRequestSort(coverReqests[i]);
                }

                GUILayout.Label(" ----------- 自定义规则请求 ----------- ", bigLabel.value);
                for (int i = 0; i < customReqests.Count; i++)
                {
                    DrawRequestSort(customReqests[i]);
                }

                GUILayout.Label(" ----------- 排序请求 ----------- ", bigLabel.value);
                for (int i = 0; i < sortReqests.Count; i++)
                {
                    DrawRequestSort(sortReqests[i]);
                }

                if (GUILayout.Button("创建请求",GUILayout.Height(50)))
                {
                    MiscHelper.Input("输入请求名", (x) =>
                    {
                        if (sortAsset.GetSort(x) != null)
                        {
                            Debug.LogError($"请求重复>>>>{x}");
                            return;
                        }
                        RequestSort sort = new RequestSort();
                        sort.key = x;
                        sort.name = x;
                        sort.sort = sortAsset.requests.Count;
                        sortAsset.requests.Add(sort);
                        GenCode();
                    });
                }

                if (GUILayout.Button("生成请求代码", GUILayout.Height(50)))
                {
                    GenCode();
                }
            });
        }

        private void DrawRequestSort(RequestSort sort)
        {
            GUILayoutExtension.VerticalGroup(() =>
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField(sort.key);
                sort.name = EditorGUILayout.TextField(sort.name);

                if (!sort.isCover && !sort.isCustom)
                {
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
                        sortAsset.requests.Remove(sort);
                        UpdateSystemSort();
                    });
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("覆盖:");
                bool isCover = EditorGUILayout.Toggle(sort.isCover);
                if (isCover != sort.isCover)
                {
                    sort.isCover = isCover;
                    if (sort.isCover)
                        sort.isCustom = false;
                    UpdateSystemSort();
                }

                GUILayout.Label("自定义:");
                bool isCustom = EditorGUILayout.Toggle(sort.isCustom);
                if (isCustom != sort.isCustom)
                {
                    sort.isCustom = isCustom;
                    if (sort.isCustom)
                        sort.isCover = false;
                    UpdateSystemSort();
                }
                EditorGUILayout.EndHorizontal();
            });
            
        }

        private void UpdateSystemSort()
        {
            coverReqests.Clear();
            customReqests.Clear();
            sortReqests.Clear();
            for (int i = 0; i < sortAsset.requests.Count; i++)
            {
                RequestSort sort = sortAsset.requests[i];
                if (sort.isCover)
                {
                    sort.sort = ECSDefinition.REForceSwithWeight;
                    coverReqests.Add(sort);
                }
                else if (sort.isCustom)
                {
                    sort.sort = ECSDefinition.RESwithRuleSelf;
                    customReqests.Add(sort);
                }
                else
                {
                    sortReqests.Add(sort);
                }
            }
            int SystemSortFunc(RequestSort a, RequestSort b)
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
            sortReqests.Sort(SystemSortFunc);
            EditorUtility.SetDirty(sortAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void GenCode()
        {
            string codeStr = @"
namespace IAECS
{
    /// <summary>
    /// 请求Id
    /// </summary>
    public enum RequestId
    {
#KEY#
    }
}
";
            string keyStr = @"
        /// <summary>
        /// #NAME#
        /// </summary>
		#KEY#,
";
            string keyValue = "";
            for (int i = 0; i < sortAsset.requests.Count; i++)
            {
                string tmp = keyStr;
                tmp = Regex.Replace(tmp, "#KEY#", sortAsset.requests[i].key);
                tmp = Regex.Replace(tmp, "#NAME#", sortAsset.requests[i].name);
                keyValue += tmp;
            }
            string resStr = codeStr;
            resStr = Regex.Replace(resStr, "#KEY#", keyValue);
            IOHelper.WriteText(resStr, RequestSortAsset.RequestCodePath);
            AssetDatabase.Refresh();
        }
    }
}