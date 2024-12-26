using IAToolkit;
using System;
using System.Collections.Generic;
using IAEngine;
using UnityEditor;

namespace IANodeGraph.View.Utils
{
    /// <summary>
    /// 视图工具
    /// </summary>
    public static class GraphProcessorEditorUtility
    {
        #region GraphWindowTypeCache
        static Dictionary<Type, Type> WindowTypeCache;

        public static Type GetGraphWindowType(Type graphType)
        {
            if (WindowTypeCache == null)
            {
                WindowTypeCache = new Dictionary<Type, Type>();
                foreach (var type in TypeCache.GetTypesDerivedFrom<BaseGraphWindow>())
                {
                    if (type.IsAbstract) continue;

                    foreach (var att in AttributeHelper.GetTypeAttributes(type, true))
                    {
                        if (att is CustomGraphWindowAttribute sAtt)
                            WindowTypeCache[sAtt.targetGraphType] = type;
                    }
                }
            }
            if (WindowTypeCache.TryGetValue(graphType, out Type windowType))
                return windowType;
            if (graphType.BaseType != null)
                return GetGraphWindowType(graphType.BaseType);
            else
                return typeof(BaseGraphWindow);
        }

        #endregion

        #region NodeViewTypeCache
        static Dictionary<Type, Type> NodeViewTypeCache;

        public static Type GetNodeViewType(Type nodeType)
        {
            if (NodeViewTypeCache == null)
            {
                NodeViewTypeCache = new Dictionary<Type, Type>();
                foreach (var type in TypeCache.GetTypesDerivedFrom<BaseNodeView>())
                {
                    if (type.IsAbstract) continue;
                    foreach (var att in AttributeHelper.GetTypeAttributes(type, true))
                    {
                        if (att is CustomNodeViewAttribute sAtt)
                            NodeViewTypeCache[sAtt.targetNodeType] = type;
                    }
                }
            }
            if (NodeViewTypeCache.TryGetValue(nodeType, out Type nodeViewType))
                return nodeViewType;
            if (nodeType.BaseType != null)
                return GetNodeViewType(nodeType.BaseType);
            else
                return typeof(BaseNodeView);
        }
        #endregion

        #region GroupViewTypeCache

        static Dictionary<Type, Type> GroupViewTypeCache;

        public static Type GetGroupViewType(Type groupType)
        {
            if (GroupViewTypeCache == null)
            {
                GroupViewTypeCache = new Dictionary<Type, Type>();
                foreach (var type in TypeCache.GetTypesDerivedFrom<BaseGroupView>())
                {
                    if (type.IsAbstract) continue;
                    foreach (var att in AttributeHelper.GetTypeAttributes(type, true))
                    {
                        if (att is CustomGroupViewAttribute sAtt)
                            GroupViewTypeCache[sAtt.targetNodeType] = type;
                    }
                }
            }
            if (GroupViewTypeCache.TryGetValue(groupType, out Type groupViewType))
                return groupViewType;
            if (groupType.BaseType != null)
                return GetGroupViewType(groupType.BaseType);
            else
                return typeof(BaseGroupView);
        }

        #endregion

        #region NodeNames

        public static string GetDisplayName(string fieldName)
        {
            return ObjectNames.NicifyVariableName(fieldName);
        }
        #endregion
    }
}
