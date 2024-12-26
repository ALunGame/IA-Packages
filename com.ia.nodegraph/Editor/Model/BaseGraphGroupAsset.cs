using IANodeGraph.Model.Internal;
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Object = UnityEngine.Object;
using IAToolkit;

namespace IANodeGraph.Model
{
    [Serializable]
    public abstract class BaseGraphGroupAsset<T> : InternalGraphGroupAsset where T : InternalBaseGraphAsset
    {
        public override List<InternalBaseGraphAsset> GetAllGraph()
        {
            List<InternalBaseGraphAsset> graphs = new List<InternalBaseGraphAsset>();
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
            for (int i = 0; i < objs.Length; i++)
            {
                if (objs[i] is T)
                {
                    graphs.Add((T)objs[i]);
                }
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
            AssetDatabase.AddObjectToAsset(graph, this);
            EditorUtility.SetDirty(graph);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return graph;
        }

        public override void RemoveGraph(InternalBaseGraphAsset graph)
        {
            DestroyImmediate(graph, true);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
