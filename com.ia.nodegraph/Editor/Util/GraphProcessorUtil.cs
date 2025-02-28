using IAEngine;
using IANodeGraph.Model;
using IANodeGraph.View;
using IAToolkit.Misc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace IANodeGraph
{
    public struct ToggleValue<T>
    {
        public bool enable;
        public T value;
    }

    public class PortStaticInfo
    {
        public Type portType;
        public PortDirection direction;
    }

    public class NodeStaticInfo
    {
        public string path;
        public string[] menu;
        public bool hidden;
        public string title;
        public string tooltip;
        public ToggleValue<Color> customTitleColor;

        public List<PortStaticInfo> portInfos;
    }

    public static class GraphProcessorUtil
    {
        private static bool s_Initialized;
        private static Dictionary<Type, NodeStaticInfo> s_NodeStaticInfos = new Dictionary<Type, NodeStaticInfo>();

        public static Dictionary<Type, NodeStaticInfo> NodeStaticInfos
        {
            get { return s_NodeStaticInfos; }
        }

        static GraphProcessorUtil()
        {
            Init(true);
        }

        public static void Init(bool force)
        {
            if (!force && s_Initialized)
                return;

            if (s_NodeStaticInfos == null)
                s_NodeStaticInfos = new Dictionary<Type, NodeStaticInfo>();
            else
                s_NodeStaticInfos.Clear();

            foreach (var t in TypesCache.GetTypesDerivedFrom<BaseNode>())
            {
                if (t.IsAbstract)
                    continue;

                var nodeStaticInfo = new NodeStaticInfo();
                nodeStaticInfo.title = t.Name;
                nodeStaticInfo.tooltip = string.Empty;
                nodeStaticInfo.customTitleColor = new ToggleValue<Color>();
                nodeStaticInfo.portInfos = new List<PortStaticInfo>();

                NodeStaticInfos.Add(t, nodeStaticInfo);
                var nodeMenuAttribute = t.GetCustomAttribute(typeof(NodeMenuItemAttribute)) as NodeMenuItemAttribute;
                if (nodeMenuAttribute != null)
                {
                    if (!string.IsNullOrEmpty(nodeMenuAttribute.path))
                    {
                        nodeStaticInfo.path = nodeMenuAttribute.path;
                        nodeStaticInfo.menu = nodeMenuAttribute.path.Split('/');
                        nodeStaticInfo.title = nodeStaticInfo.menu[nodeStaticInfo.menu.Length - 1];
                    }
                    else
                    {
                        nodeStaticInfo.path = t.Name;
                        nodeStaticInfo.menu = new string[] { t.Name };
                        nodeStaticInfo.title = t.Name;
                    }

                    nodeStaticInfo.hidden = nodeMenuAttribute.hidden;
                }
                else
                {
                    nodeStaticInfo.path = t.Name;
                    nodeStaticInfo.menu = new string[] { t.Name };
                    nodeStaticInfo.title = t.Name;
                    nodeStaticInfo.hidden = false;
                }

                var titleAttribute = t.GetCustomAttribute(typeof(NodeTitleAttribute)) as NodeTitleAttribute;
                if (titleAttribute != null && !string.IsNullOrEmpty(titleAttribute.title))
                    nodeStaticInfo.title = titleAttribute.title;

                var tooltipAttribute = t.GetCustomAttribute(typeof(NodeTooltipAttribute)) as NodeTooltipAttribute;
                if (tooltipAttribute != null)
                    nodeStaticInfo.tooltip = tooltipAttribute.Tooltip;

                var titleColorAttribute = t.GetCustomAttribute(typeof(NodeTitleColorAttribute)) as NodeTitleColorAttribute;
                if (titleColorAttribute != null)
                {
                    nodeStaticInfo.customTitleColor.enable = true;
                    nodeStaticInfo.customTitleColor.value = titleColorAttribute.color;
                }

                //属性端口
                foreach (FieldInfo item in ReflectionHelper.GetFieldInfos(t))
                {
                    if (AttributeHelper.TryGetFieldAttribute(item, out NodeValueAttribute attr))
                    {
                        PortStaticInfo portInfo = new PortStaticInfo();
                        portInfo.direction = attr.direction;
                        portInfo.portType = item.FieldType;
                        nodeStaticInfo.portInfos.Add(portInfo);
                    }
                }
            }

            s_Initialized = true;
        }

        public static List<NodeStaticInfo> GetNodeStaticInfosByPort(BasePort pPort, BaseGraphView pGraphView)
        {
            List<NodeStaticInfo> infos = new List<NodeStaticInfo>();

            foreach (var item in NodeStaticInfos)
            {
                Type nodeType = item.Key;
                NodeStaticInfo nodeInfo = item.Value;

                foreach (PortStaticInfo portStaticInfo in nodeInfo.portInfos)
                {
                    if (portStaticInfo.direction != pPort.direction)
                    {
                        if (pGraphView.IsCompatible(portStaticInfo.portType, pPort.portType))
                        {
                            infos.Add(nodeInfo);
                        }
                    }
                }
            }

            return infos;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ModelAs<T>(this IGraphElementProcessor graphElement) where T : class
        {
            return graphElement.Model as T;
        }

        public static T Model<T>(this IGraphElementProcessor<T> graphElement) where T : class
        {
            return graphElement.Model as T;
        }
    }
}