using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IAToolkit
{
    [Serializable]
    public class GroupPath
    {
        public string typeName = "";
        public string typeFullName = "";
        
        public string typeChildName = "";
        public string typeChildFullName = "";

        public string searchPath = "";
        public string exportPath = "";
        
        public string exportFileName = "{0}";
        public string exportExName = "txt";

        public GroupPath(Type type, Type childType)
        {
            this.typeName = type.Name;
            this.typeFullName = type.FullName;
            
            this.typeChildName = childType.Name;
            this.typeChildFullName = childType.FullName;
        }
    }

    /// <summary>
    /// 分组资源设置
    /// </summary>
    public class GroupAssetSetting : ScriptableObject
    {
        //地图设置文件目录
        public const string SetingPath = "Assets/Editor/Group/Setting";

        public List<GroupPath> groupPaths = new List<GroupPath>();

        public GroupPath GetSearchPath(string typeFullName)
        {
            foreach (var item in groupPaths)
            {
                if (item.typeFullName == typeFullName)
                {
                    return item;
                }
            }
            return null;
        }

        public GroupPath GetSearchPath<T>() where T : InternalGroupAsset
        {
            foreach (var item in groupPaths)
            {
                if (item.typeFullName == typeof(T).FullName)
                {
                    return item;
                }
            }
            return null;
        }

        public List<InternalGroupAsset> GetGroups(string groupPath)
        {
            List<InternalGroupAsset> groups = new List<InternalGroupAsset>();
            string[] tileAssetPath = new string[] { groupPath };
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", tileAssetPath);
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                InternalGroupAsset assetGroup = AssetDatabase.LoadAssetAtPath<InternalGroupAsset>(path);
                if (assetGroup != null)
                {
                    groups.Add(assetGroup);
                }
            }
            return groups;
        }

        public List<T> GetAlllChildAssets<T>(string groupPath) where T : GroupChildAsset
        {
            List<T> assets = new List<T>();
            string[] tileAssetPath = new string[] { groupPath };
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", tileAssetPath);

            List<InternalGroupAsset> groups = GetGroups(groupPath);
            foreach (InternalGroupAsset group in groups)
            {
                List<GroupChildAsset> childAssets = group.GetAllAssets();
                foreach (GroupChildAsset groupChildAsset in childAssets)
                {
                    if (groupChildAsset is T)
                    {
                        assets.Add(groupChildAsset as T);
                    }
                }
            }
            return assets;
        }

        public T GetAsset<T>(string groupPath,string name) where T : GroupChildAsset
        {
            List<T> assets = GetAlllChildAssets<T>(groupPath);
            foreach (var item in assets)
            {
                if (item.name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public List<GroupChildAsset> GetAlllChildAssets(string groupPath)
        {
            List<GroupChildAsset> assets = new List<GroupChildAsset>();
            string[] tileAssetPath = new string[] { groupPath };
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", tileAssetPath);

            List<InternalGroupAsset> groups = GetGroups(groupPath);
            foreach (InternalGroupAsset group in groups)
            {
                List<GroupChildAsset> childAssets = group.GetAllAssets();
                assets.AddRange(childAssets);
            }
            return assets;
        }

        #region Static

        private static GroupAssetSetting setting;
        public static GroupAssetSetting Setting
        {
            get
            {
                if (setting == null)
                {
                    string[] tileAssetPath = new string[] { SetingPath };
                    string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", tileAssetPath);
                    foreach (var guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        GroupAssetSetting asset = AssetDatabase.LoadAssetAtPath<GroupAssetSetting>(path);
                        if (asset != null)
                        {
                            setting = asset;
                            break;
                        }
                    }
                }
                return setting;
            }
        }
        
        [MenuItem("Tools/Group/分组设置")]
        public static void CreateSetting()
        {
            if (!Directory.Exists(SetingPath))
            {
                Directory.CreateDirectory(SetingPath);
            }
            
            if (Setting == null)
            {
                GroupAssetSetting setting = CreateInstance<GroupAssetSetting>();
                setting.name = "分组设置";
                AssetDatabase.CreateAsset(setting, SetingPath + "/分组设置.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            Selection.activeObject = setting;
        }

        [MenuItem("Tools/Group/列表")]
        public static void OpenList()
        {
            GroupAssetListWindow.Open();
        }

        [MenuItem("Tools/Group/导出所有配置", true)]
        public static bool ExportAllCheck()
        {
            return Setting != null;
        }

        [MenuItem("Tools/Group/导出所有配置")]
        public static void ExportAll()
        {
            Dictionary<Type, InternalGroupAsset> groupDict = GetGroups();
            foreach (var item in groupDict)
            {
                item.Value.OnClickExport();
            }
        }

        public static Dictionary<Type, InternalGroupAsset> GetGroups()
        {
            var groupDict = new Dictionary<Type, InternalGroupAsset>();
            foreach (var item in Setting.groupPaths)
            {
                var list = Setting.GetGroups(item.searchPath);
                for (int i = 0; i < list.Count; i++)
                {
                    if (!groupDict.ContainsKey(list[i].GetType()))
                    {
                        groupDict.Add(list[i].GetType(), list[i]);
                    }
                }
            }
            return groupDict;
        }

        #endregion
    }
}