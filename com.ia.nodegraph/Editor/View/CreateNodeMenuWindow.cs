using IANodeGraph.Model;
using IAToolkit;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using IAEngine;

namespace IANodeGraph.View
{
    public class CreateNodeMenuWindow : ScriptableObject, ISearchWindowProvider
    {
        private BaseGraphView graphView;
        private IEnumerable<Type> nodeTypes;

        class PortInfo { 
            public List<BasePortVM> InPorts = new List<BasePortVM>();
            public List<BasePortVM> OutPorts = new List<BasePortVM>();
        }
        private Dictionary<Type, PortInfo> nodePortMap = new Dictionary<Type, PortInfo>();

        //连接剔除
        public BaseConnectionView ConnectionFilter;

        public void Initialize(BaseGraphView _graphView, IEnumerable<Type> nodeTypes)
        {
            //排序
            Dictionary<string, Type> titlelist = new Dictionary<string, Type>();
            foreach (var item in nodeTypes)
            {
                Type nodeType = item;
                NodeMenuItemAttribute attr;
                AttributeHelper.TryGetTypeAttribute(nodeType, out attr);
                if (!titlelist.ContainsKey(attr.title))
                    titlelist.Add(attr.title, nodeType);
            }
            titlelist = titlelist.OrderBy(value => value.Key).ToDictionary(r => r.Key, r => r.Value);
            nodeTypes = titlelist.Values.ToList();

            graphView = _graphView;
            this.nodeTypes = nodeTypes;
            this.nodePortMap.Clear();
            foreach (var nodeType in nodeTypes)
            {
                if (nodePortMap.ContainsKey(nodeType))
                {
                    continue;
                }
                PortInfo portInfo = new PortInfo(); 
                foreach (FieldInfo item in ReflectionHelper.GetFieldInfos(nodeType))
                {
                    if (AttributeHelper.TryGetFieldAttribute(item, out NodeValueAttribute nodeValueAttribute))
                        continue;
                    BasePortVM port = null;
                    if (AttributeHelper.TryGetFieldAttribute(item, out InputPortAttribute inputAttr))
                    {
                        port = new BasePortVM(inputAttr.name, inputAttr.orientation, inputAttr.direction, inputAttr.capacity, item.FieldType, inputAttr.setIndex);
                        portInfo.InPorts.Add(port);
                    }
                    if (AttributeHelper.TryGetFieldAttribute(item, out OutputPortAttribute outputAttr))
                    {
                        port = new BasePortVM(outputAttr.name, outputAttr.orientation, outputAttr.direction, outputAttr.capacity, item.FieldType, outputAttr.setIndex);
                        portInfo.OutPorts.Add(port);
                    }
                }
                nodePortMap.Add(nodeType, portInfo);
            }
        }

        //创建默认菜单
        private void CreateStandardNodeMenu(List<SearchTreeEntry> tree)
        {
            var titlePaths = new HashSet<string>();
            foreach (Type type in nodeTypes)
            {
                if (!AttributeHelper.TryGetTypeAttribute(type, out NodeMenuItemAttribute attr))
                    continue;
                var nodePath = attr.title;
                var nodeName = nodePath;
                var level = 0;
                var parts = nodePath.Split('/');

                if (parts.Length > 1)
                {
                    level++;
                    nodeName = parts[parts.Length - 1];
                    var fullTitleAsPath = "";

                    for (var i = 0; i < parts.Length - 1; i++)
                    {
                        var title = parts[i];
                        fullTitleAsPath += title;
                        level = i + 1;

                        // 如果节点在子类别中，则添加节标题
                        if (!titlePaths.Contains(fullTitleAsPath))
                        {
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(title))
                            {
                                level = level
                            });
                            titlePaths.Add(fullTitleAsPath);
                        }
                    }
                }

                tree.Add(new SearchTreeEntry(new GUIContent(nodeName))
                {
                    level = level + 1,
                    userData = type
                });
            }
        }

        private BaseNodeVM waitConnectNode = null;
        private string waitConnectPortName = null;
        private bool checkInPort = false;
        private Dictionary<Type, string> nodePortNameMap = new Dictionary<Type, string>();

       
        //创建连接菜单
        private void CreateEdgeNodeMenu(List<SearchTreeEntry> tree)
        {
            nodePortNameMap.Clear();
            waitConnectNode = null;
            waitConnectPortName = null;

            void CollectNodePortName(BasePortVM checkPort, Type nodeType, PortInfo portInfo, bool isInPort)
            {
                if (isInPort)
                {
                    for (int i = 0; i < portInfo.InPorts.Count; i++)
                    {
                        if (graphView.TypesAreConnectable(checkPort.Model.type, portInfo.InPorts[i].Model.type))
                        {
                            nodePortNameMap.Add(nodeType, portInfo.InPorts[i].Model.name);
                            return;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < portInfo.OutPorts.Count; i++)
                    {
                        if (graphView.TypesAreConnectable(checkPort.Model.type, portInfo.OutPorts[i].Model.type))
                        {
                            nodePortNameMap.Add(nodeType, portInfo.OutPorts[i].Model.name);
                            return;
                        }
                    }
                }
            }

            Port port = ConnectionFilter.input == null ? ConnectionFilter.output : ConnectionFilter.input;
            if (port == null)
                return;
            BasePortView portView = port as BasePortView;
            waitConnectNode = portView.Model.Owner;
            waitConnectPortName = portView.Model.Model.name;
            checkInPort = waitConnectNode.GetPortDirection(waitConnectPortName) == BasePort.Direction.Input ? true : false;

            foreach (var item in nodePortMap)
            {
                Type nodeType = item.Key;
                PortInfo portInfo = item.Value;
                CollectNodePortName(portView.Model, nodeType, portInfo, !checkInPort);
            }

            //创建菜单
            var titlePaths = new HashSet<string>();
            foreach (var item in nodePortNameMap)
            {
                Type nodeType = item.Key;
                string portName = item.Value;
                if (!AttributeHelper.TryGetTypeAttribute(nodeType, out NodeMenuItemAttribute attr))
                    continue;
                var nodePath = attr.title;
                var nodeName = nodePath;
                var level = 0;
                var parts = nodePath.Split('/');

                if (parts.Length > 1)
                {
                    level++;
                    nodeName = parts[parts.Length - 1];
                    var fullTitleAsPath = "";

                    for (var i = 0; i < parts.Length - 1; i++)
                    {
                        var title = parts[i];
                        fullTitleAsPath += title;
                        level = i + 1;

                        // 如果节点在子类别中，则添加节标题
                        if (!titlePaths.Contains(fullTitleAsPath))
                        {
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(title))
                            {
                                level = level
                            });
                            titlePaths.Add(fullTitleAsPath);
                        }
                    }
                }

                tree.Add(new SearchTreeEntry(new GUIContent(nodeName))
                {
                    level = level + 1,
                    userData = nodeType
                });
            }
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var windowRoot = graphView.GraphWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - graphView.GraphWindow.position.position);
            var graphMousePosition = graphView.contentViewContainer.WorldToLocal(windowMousePosition);

            BaseNodeVM createNode = graphView.Model.NewNode(searchTreeEntry.userData as Type, graphMousePosition);

            //自动连接
            if (ConnectionFilter != null)
            {
                graphView.CommandDispacter.Do(new AddNodeCommand(graphView.Model, createNode));
                Port port = ConnectionFilter.input == null ? ConnectionFilter.output : ConnectionFilter.input;
                BasePortView portView1 = port as BasePortView;

                string portName = nodePortNameMap[searchTreeEntry.userData as Type];
                if (checkInPort)
                {
                    graphView.CommandDispacter.Do(new ConnectCommand(graphView.Model, createNode, portName, waitConnectNode, waitConnectPortName));
                }
                else
                {
                    graphView.CommandDispacter.Do(new ConnectCommand(graphView.Model, waitConnectNode, waitConnectPortName, createNode, portName));
                }
                waitConnectNode.Ports[waitConnectPortName].RefreshIndex();
            }
            else
            {
                graphView.CommandDispacter.Do(new AddNodeCommand(graphView.Model, createNode));
            }
            graphView.GraphWindow.Focus();
            return true;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("创建节点"), 0),
            };
            if (ConnectionFilter==null)
            {
                CreateStandardNodeMenu(tree);
            }
            else
            {
                CreateEdgeNodeMenu(tree);
            }
            return tree;
        }
    }
}
