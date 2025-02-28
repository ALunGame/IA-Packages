using System;
using System.Collections.Generic;
using UnityEngine;

namespace IANodeGraph.Model.Internal
{
    public abstract class InternalGraphGroupAsset : ScriptableObject
    {
        public abstract string DisplayName { get; }

        /// <summary>
        /// 获得所有视图
        /// </summary>
        /// <returns></returns>
        public abstract List<InternalBaseGraphAsset> GetAllGraph();

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
            List<InternalBaseGraphAsset> assets = new List<InternalBaseGraphAsset>();

            foreach (InternalGraphGroupAsset group in groups)
            {
                if (group.GetType() == GetType())
                {
                    assets.AddRange(group.GetAllGraph());
                }
            }

            this.ExportGraph(assets);
        }

        /// <summary>
        /// 导出视图
        /// </summary>
        /// <param name="graph"></param>
        public abstract void ExportGraph(List<InternalBaseGraphAsset> assets);
    }
}
