using IANodeGraph.Model;
using IANodeGraph.Model.Internal;
using IANodeGraph.Window;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IANodeGraph
{
    [Serializable]
    public class GraphGroupPath
    {
        public string typeName = "";
        public string typeFullName = "";

        public string searchPath = "";

        public GraphGroupPath(Type type)
        {
            this.typeName = type.Name;
            this.typeFullName = type.FullName;
        }
    }

    /// <summary>
    /// 视图设置
    /// </summary>
    public class GraphSetting : ScriptableObject
    {
        //地图设置文件目录
        public const string SettingPath = "Assets/Editor/Graph/Setting";

        public List<GraphGroupPath> groupPaths = new List<GraphGroupPath>();

        public GraphGroupPath GetSearchPath(string typeFullName)
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

        public GraphGroupPath GetSearchPath<T>() where T : InternalGraphGroupAsset
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

        public List<T> GetGroups<T>() where T : InternalGraphGroupAsset
        {
            GraphGroupPath graphGroupPath = GetSearchPath(typeof(T).FullName);
            if (graphGroupPath == null)
            {
                return new List<T>();
            }

            List<T> resList = new List<T>();
            List<InternalGraphGroupAsset> groups = GetGroups(graphGroupPath.searchPath);
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i] is T)
                {
                    resList.Add(groups[i] as T);
                }
            }
            return resList;
        }

        public List<InternalGraphGroupAsset> GetGroups(string groupPath)
        {
            List<InternalGraphGroupAsset> groups = new List<InternalGraphGroupAsset>();
            string[] tileAssetPath = new string[] { groupPath };
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", tileAssetPath);
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                InternalGraphGroupAsset assetGroup = AssetDatabase.LoadAssetAtPath<InternalGraphGroupAsset>(path);
                if (assetGroup != null)
                {
                    groups.Add(assetGroup);
                }
            }
            return groups;
        }

        public TGraphAsset LoadGraphAsset<TGroup,TGraphAsset>(string pGraphAssetName) where TGraphAsset : InternalBaseGraphAsset where TGroup : InternalGraphGroupAsset
        {
            List<TGroup> groups = GetGroups<TGroup>();
            if (groups.Count <= 0)
            {
                return null;
            }

            for (int i = 0; i < groups.Count; i++)
            {
                List<string> assetNames = groups[i].GetAllGraphFileName();
                foreach (var name in assetNames)
                {
                    if (name == pGraphAssetName)
                    {
                        return groups[i].LoadGraphAsset(pGraphAssetName) as TGraphAsset;
                    }
                }
            }

            return null;
        }

        public List<T> GetGroupAssets<T>(string groupPath) where T : InternalBaseGraphAsset
        {
            List<T> assets = new List<T>();
            string[] tileAssetPath = new string[] { groupPath };
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", tileAssetPath);
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                BaseGraphGroupAsset<T> assetGroup = AssetDatabase.LoadAssetAtPath<BaseGraphGroupAsset<T>>(path);
                if (assetGroup != null)
                {
                    foreach (var item in assetGroup.GetAllGraph())
                    {
                        assets.Add(item as T);
                    }
                }
            }
            return assets;
        }

        public T GetAsset<T>(string groupPath,string name) where T : InternalBaseGraphAsset
        {
            List<T> assets = GetGroupAssets<T>(groupPath);
            foreach (var item in assets)
            {
                if (item.name == name)
                {
                    return item;
                }
            }
            return null;
        }

        #region Static

        private static GraphSetting setting;
        public static GraphSetting Setting
        {
            get
            {
                if (setting == null)
                {
                    string[] tileAssetPath = new string[] { SettingPath };
                    string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", tileAssetPath);
                    foreach (var guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        GraphSetting asset = AssetDatabase.LoadAssetAtPath<GraphSetting>(path);
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

        [MenuItem("Tools/视图/设置", true)]
        public static bool CheckHasSetting()
        {
            return Setting == null;
        }

        [MenuItem("Tools/视图/设置")]
        public static void CreateSetting()
        {
            if (!Directory.Exists(SettingPath))
            {
                Directory.CreateDirectory(SettingPath);
            }
            GraphSetting setting = CreateInstance<GraphSetting>();
            setting.name = "视图设置";
            AssetDatabase.CreateAsset(setting, SettingPath + "/视图设置.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = setting;
        }

        [MenuItem("Tools/视图/列表")]
        public static void OpenList()
        {
            GraphListWindow.Open();
        }

        [MenuItem("Tools/视图/导出所有配置", true)]
        public static bool ExportAllCheck()
        {
            return Setting != null;
        }

        [MenuItem("Tools/视图/导出所有配置")]
        public static void ExportAll()
        {
            Dictionary<Type, InternalGraphGroupAsset> groupDict = GetGroups();
            foreach (var item in groupDict)
            {
                item.Value.OnClickExport();
            }
        }

        public static Dictionary<Type, InternalGraphGroupAsset> GetGroups()
        {
            var groupDict = new Dictionary<Type, InternalGraphGroupAsset>();
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