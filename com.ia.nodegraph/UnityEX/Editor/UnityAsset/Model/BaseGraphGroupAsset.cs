using IANodeGraph.Model.Internal;
using IAToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace IANodeGraph.Model
{
    /// <summary>
    /// 视图分组显示属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class GraphGroupDisplayAttribute : Attribute
    {
        public string displayName;

        public GraphGroupDisplayAttribute(string displayName)
        {
            this.displayName = displayName;
        }
    }

    [Serializable]
    public abstract class BaseGraphGroupAsset<T> : InternalGraphGroupAsset where T : InternalBaseGraphAsset
    {
        public string GetChildAssetRootPath()
        {
            string assetPath = Path.GetFullPath(AssetDatabase.GetAssetPath(this));
            string dirPath = assetPath.Replace(".asset", "");
            return IOHelper.GetUnityRelativePath(dirPath);
        }

        public override List<string> GetAllGraphFileName()
        {
            List<string> fileNames = new List<string>();

            string rootAssetPath = GetChildAssetRootPath();
            rootAssetPath = Path.GetFullPath(rootAssetPath);
            if (!Directory.Exists(rootAssetPath))
            {
                return fileNames;
            }

            string[] filePaths = Directory.GetFiles(rootAssetPath);
            foreach (string filePath in filePaths) 
            {
                if (filePath.Contains(".meta"))
                {
                    continue;
                }
                fileNames.Add(Path.GetFileNameWithoutExtension(filePath));
            }

            return fileNames;
        }

        public override InternalBaseGraphAsset LoadGraphAsset(string pAssetName)
        {
            string rootAssetPath = GetChildAssetRootPath();
            string assetPath = $"{rootAssetPath}/{pAssetName}.asset";
            if (!AssetDatabase.AssetPathExists(assetPath))
            {
                return null;
            }
            return AssetDatabase.LoadAssetAtPath<T>(assetPath);
        }

        public override List<InternalBaseGraphAsset> GetAllGraph()
        {
            if (!Directory.Exists(Path.GetFullPath(GetChildAssetRootPath())))
            {
                return new List<InternalBaseGraphAsset>();
            }

            List<InternalBaseGraphAsset> graphs = new List<InternalBaseGraphAsset>();

            string[] searchPath = new string[] { GetChildAssetRootPath() };
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", searchPath);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                InternalBaseGraphAsset graph = AssetDatabase.LoadAssetAtPath<T>(path);
                graphs.Add(graph);
            }

            return graphs;
        }

        public override void OnClickCreateBtn()
        {
            MiscHelper.Input($"输入{DisplayName}名：", (string name) =>
            {
                CreateGraph(name);
            });
        }

        public override bool CheckHasGraph(string name)
        {
            List<InternalBaseGraphAsset> graphs = GetAllGraph();
            for (int i = 0; i < graphs.Count; i++)
            {
                if (graphs[i].name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public override InternalBaseGraphAsset CreateGraph(string name)
        {
            if (CheckHasGraph(name))
            {
                Debug.LogError($"创建视图失败，重复视图>>{name}");
                return null;
            }

            T graph = CreateInstance<T>();
            graph.name = name;

            string rootAssetPath = GetChildAssetRootPath();
            if (!Directory.Exists(Path.GetFullPath(rootAssetPath)))
            {
                Directory.CreateDirectory(Path.GetFullPath(rootAssetPath));
            }
            AssetDatabase.CreateAsset(graph, $"{rootAssetPath}/{graph.name}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return graph;
        }

        public override void RemoveGraph(InternalBaseGraphAsset graph)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(graph));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
