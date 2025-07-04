using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IANodeGraph.Model.Internal
{
    public class GraphExportInfo
    {
        public InternalBaseGraphAsset Asset;
        public BaseGraph Graph;
    }
    
    public abstract class InternalGraphGroupAsset : ScriptableObject
    {
        public abstract string DisplayName { get; }

        /// <summary>
        /// 获得所有视图
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetAllGraphFileName();

        /// <summary>
        /// 获得所有视图
        /// </summary>
        /// <returns></returns>
        public abstract List<InternalBaseGraphAsset> GetAllGraph();

        /// <summary>
        /// 加载指定视图
        /// </summary>
        /// <typeparam name="TGraphAsset"></typeparam>
        /// <param name="pAssetName"></param>
        /// <returns></returns>
        public abstract InternalBaseGraphAsset LoadGraphAsset(string pAssetName);

        /// <summary>
        /// 检测存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract bool CheckHasGraph(string name);

        /// <summary>
        /// 当点击创建按钮
        /// </summary>
        public abstract void OnClickCreateBtn();

        /// <summary>
        /// 创建视图
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public abstract InternalBaseGraphAsset CreateGraph(string name);  

        /// <summary>
        /// 移除视图
        /// </summary>
        /// <param name="name"></param>
        public abstract void RemoveGraph(InternalBaseGraphAsset graph);

        /// <summary>
        /// 导出
        /// </summary>
        public virtual void OnClickExport()
        {
            GraphGroupPath path                  = GraphSetting.Setting.GetSearchPath(this.GetType().FullName);
            List<InternalGraphGroupAsset> groups = GraphSetting.Setting.GetGroups(path.searchPath);

            List<GraphExportInfo> allInfos = new List<GraphExportInfo>();
            
            foreach (InternalGraphGroupAsset group in groups)
            {
                if (group.GetType() == GetType())
                {
                    List<GraphExportInfo> groupInfos = new List<GraphExportInfo>();
                    List<InternalBaseGraphAsset> tAssets = group.GetAllGraph();
                    foreach (var asset in tAssets)
                    {
                        groupInfos.Add(new GraphExportInfo(){Asset = asset, Graph = asset.LoadGraph()});
                    }

                    if (group.Equals(this))
                    {
                        ExportGroupGraphs(groupInfos);
                    }
                    
                    allInfos.AddRange(groupInfos);
                }
            }

            this.ExportAllGroupGraphs(allInfos);
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 导出此分组的视图
        /// </summary>
        /// <param name="pInfos"></param>
        public abstract void ExportGroupGraphs(List<GraphExportInfo> pInfos);

        /// <summary>
        /// 导出所有分组视图
        /// </summary>
        /// <param name="pAllInfos"></param>
        public abstract void ExportAllGroupGraphs(List<GraphExportInfo> pAllInfos);
    }
}
