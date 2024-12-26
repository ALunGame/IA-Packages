using IAToolkit;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IAToolkit
{
    internal class GroupAssetListWindow : BaseEditorWindow
    {
        public static void Open()
        {
            var window = GetWindow<GroupAssetListWindow>();
            window.titleContent = new GUIContent("分组列表");
            window.Init();
        }

        private Dictionary<Type, InternalGroupAsset> groupDict = new Dictionary<Type, InternalGroupAsset>();
        private InternalGroupAsset selGroup;
        private List<GroupChildAsset> childAssets = new List<GroupChildAsset>();

        public void Init()
        {
            groupDict = GroupAssetSetting.GetGroups();
        }

        private void OnGUI()
        {
            GUILayoutExtension.HorizontalGroup(() =>
            {
                GUILayoutExtension.VerticalGroup(() =>
                {
                    MiscHelper.Btn("刷新", 200, 35, () =>
                    {
                        Refresh();
                    });

                    foreach (var item in groupDict)
                    {
                        GUIExtension.SetColor(item.Value == selGroup ? Color.green : Color.white, () =>
                        {
                            MiscHelper.Btn(item.Value.name, 200, 35, () =>
                            {
                                OnSelGroup(item.Value);
                            });
                        });
                    }
                });

                GUILayoutExtension.VerticalGroup(() =>
                {
                    for (int i = 0; i < childAssets.Count; i++)
                    {
                        MiscHelper.Btn(childAssets[i].name, 200, 35, () =>
                        {
                            childAssets[i].Open(selGroup);
                        });
                    }
                });
            });
        }

        private void OnSelGroup(InternalGroupAsset group)
        {
            selGroup = group;
            childAssets = selGroup.GetAllAssets();
        }

        private void Refresh()
        {
            Init();
            selGroup = null;
        }
    }
}