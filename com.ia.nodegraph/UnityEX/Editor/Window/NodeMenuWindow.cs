using IANodeGraph.Model;
using IANodeGraph.View;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using IAToolkit;

namespace IANodeGraph.Window
{
    public class NodeMenuWindow : ScriptableObject, ISearchWindowProvider
    {
        #region Define

        public interface INodeEntry
        {
            string Path { get; }

            string[] Menu { get; }

            void CreateNode(BaseGraphView graphView, Vector2Int position);

            void CreateNode(BaseGraphView graphView, Vector2Int position, BasePortProcessor waitConnect, bool waitConnectIn);
        }

        public class NodeEntry : INodeEntry
        {
            private readonly string path;
            private readonly string[] menu;
            public readonly Type nodeType;

            public string Path
            {
                get { return path; }
            }

            public string[] Menu
            {
                get { return menu; }
            }

            public NodeEntry(string path, string[] menu, Type nodeType)
            {
                this.path = path;
                this.menu = menu;
                this.nodeType = nodeType;
            }

            public void CreateNode(BaseGraphView graphView, Vector2Int position)
            {
                graphView.CommandDispatcher.Do(new AddNodeCommand(graphView.ViewModel, nodeType, position));
            }

            public void CreateNode(BaseGraphView graphView, Vector2Int position, BasePortProcessor waitConnect, bool waitConnectIn)
            {
                AddNodeCommand addNodeCommand = new AddNodeCommand(graphView.ViewModel, nodeType, position);
                graphView.CommandDispatcher.Do(addNodeCommand);

                //自动连接
                BasePortProcessor connectPort = CalcCanConnectPort(graphView, addNodeCommand.nodeVM, waitConnect, waitConnectIn);
                if (connectPort != null)
                {
                    if (waitConnectIn)
                    {
                        graphView.CommandDispatcher.Do(new ConnectCommand(graphView.ViewModel, waitConnect, connectPort));
                    }
                    else
                    {
                        graphView.CommandDispatcher.Do(new ConnectCommand(graphView.ViewModel, connectPort, waitConnect));
                    }
                }
            }

            private (BasePortProcessor, bool) GetConnectPortInfo(BaseConnectionView connectionView)
            {
                //自动连接
                if (connectionView != null && connectionView.ViewModel != null)
                {
                    bool isIn = false;
                    BasePortProcessor port = null;
                    if (connectionView.input != null)
                    {
                        isIn = false;
                        port = ((BasePortView)connectionView.input).ViewModel;
                    }
                    if (connectionView.output != null)
                    {
                        isIn = true;
                        port = ((BasePortView)connectionView.output).ViewModel;
                    }
                    return (port, isIn);
                }
                return (null, false);
            }

            private BasePortProcessor CalcCanConnectPort(BaseGraphView graphView, BaseNodeProcessor nodeProcessor, BasePortProcessor portProcessor, bool isIn)
            {
                if (nodeProcessor == null)
                {
                    return null;
                }
                IReadOnlyList<BasePortProcessor> checkPorts = isIn ? nodeProcessor.InPorts : nodeProcessor.OutPorts;

                foreach (BasePortProcessor port in checkPorts)
                {
                    if (port.Direction != portProcessor.Direction)
                    {
                        if (graphView.IsCompatible(port.portType.Value, portProcessor.portType.Value))
                        {
                            return port;
                        }
                    }
                }

                return null;
            }
        }
        #endregion

        private string treeName;
        private BaseGraphView graphView;
        public List<INodeEntry> entries = new List<INodeEntry>(256);

        //连接剔除
        public BaseConnectionView ConnectionFilter;
        public BasePortProcessor WaitConnectPort;
        public bool WaitConnectIn;

        public void Initialize(string treeName, BaseGraphView graphView)
        {
            this.treeName = treeName;
            this.graphView = graphView;
        }

        public void QuickSortEntrys()
        {
            var multiLayereEntryCount = 0;
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Menu.Length > 1)
                    multiLayereEntryCount++;
            }

            entries.QuickSort((a, b) => -(a.Menu.Length.CompareTo(b.Menu.Length)));
            entries.QuickSort(0, multiLayereEntryCount - 1, (a, b) => String.Compare(a.Path, b.Path, StringComparison.Ordinal));
            entries.QuickSort(multiLayereEntryCount, entries.Count - 1, (a, b) => String.Compare(a.Path, b.Path, StringComparison.Ordinal));
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var windowRoot = graphView.GraphWindow.rootVisualElement;
            var windowMousePosition = windowRoot.ChangeCoordinatesTo(windowRoot.parent, context.screenMousePosition - graphView.GraphWindow.position.position);
            var graphMousePosition = graphView.contentViewContainer.WorldToLocal(windowMousePosition);

            var nodeEntry = searchTreeEntry.userData as INodeEntry;
            
            //自动连接
            if (ConnectionFilter != null)
            {
                nodeEntry.CreateNode(graphView, graphMousePosition.ToVector2Int(), WaitConnectPort, WaitConnectIn);
                ConnectionFilter = null;
            }
            else
            {
                nodeEntry.CreateNode(graphView, graphMousePosition.ToVector2Int());
            }

            graphView.GraphWindow.Focus();
            return true;
        }

        private void CreateStandardNodeMenu(List<SearchTreeEntry> tree)
        {
            HashSet<string> groups = new HashSet<string>();
            foreach (var nodeEntry in entries)
            {
                var nodeName = nodeEntry.Menu[nodeEntry.Menu.Length - 1];

                if (nodeEntry.Menu.Length > 1)
                {
                    var groupPath = "";
                    for (int i = 0; i < nodeEntry.Menu.Length - 1; i++)
                    {
                        var title = nodeEntry.Menu[i];
                        groupPath += title;
                        if (!groups.Contains(groupPath))
                        {
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(title))
                            {
                                level = i + 1
                            });
                            groups.Add(groupPath);
                        }
                    }
                }

                tree.Add(new SearchTreeEntry(new GUIContent(nodeName))
                {
                    level = nodeEntry.Menu.Length,
                    userData = nodeEntry
                });
            }
        }

        private void CreateEdgeNodeMenu(List<SearchTreeEntry> tree)
        {
            GetConnectPortInfo(ConnectionFilter);

            Port port = ConnectionFilter.input == null ? ConnectionFilter.output : ConnectionFilter.input;
            if (port == null)
                return;

            BasePortView portView = port as BasePortView;
            List<NodeStaticInfo> nodeStaticInfos = GraphProcessorUtil.GetNodeStaticInfosByPort(portView.ViewModel.Model, graphView);

            HashSet<string> groups = new HashSet<string>();
            foreach (var nodeEntry in entries)
            {
                bool hasNode = false;
                foreach (var node in nodeStaticInfos)
                {
                    if (node.path == nodeEntry.Path)
                    {
                        hasNode = true;
                        break;
                    }
                }

                if (!hasNode)
                {
                    continue;
                }

                var nodeName = nodeEntry.Menu[nodeEntry.Menu.Length - 1];

                if (nodeEntry.Menu.Length > 1)
                {
                    var groupPath = "";
                    for (int i = 0; i < nodeEntry.Menu.Length - 1; i++)
                    {
                        var title = nodeEntry.Menu[i];
                        groupPath += title;
                        if (!groups.Contains(groupPath))
                        {
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(title))
                            {
                                level = i + 1
                            });
                            groups.Add(groupPath);
                        }
                    }
                }

                tree.Add(new SearchTreeEntry(new GUIContent(nodeName))
                {
                    level = nodeEntry.Menu.Length,
                    userData = nodeEntry
                });
            }
        }

        private void GetConnectPortInfo(BaseConnectionView connectionView)
        {
            WaitConnectPort = null;
            WaitConnectIn = false;
            //自动连接
            if (connectionView != null)
            {
                if (connectionView.input != null)
                {
                    WaitConnectIn = false;
                    WaitConnectPort = ((BasePortView)connectionView.input).ViewModel;
                }
                if (connectionView.output != null)
                {
                    WaitConnectIn = true;
                    WaitConnectPort = ((BasePortView)connectionView.output).ViewModel;
                }
            }
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>(entries.Count + 1);
            tree.Add(new SearchTreeGroupEntry(new GUIContent(treeName)));

            if (ConnectionFilter == null)
            {
                CreateStandardNodeMenu(tree);
            }
            else
            {
                CreateEdgeNodeMenu(tree);
            }
            
            return tree;
        }
        
        
        public void ClearFilter()
        {
            ConnectionFilter = null;
            WaitConnectPort = null;
            WaitConnectIn = false;
        }
    }
}
