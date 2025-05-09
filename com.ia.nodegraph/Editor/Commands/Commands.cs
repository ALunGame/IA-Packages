﻿using IAToolkit.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using IAToolkit;

namespace IANodeGraph.Model
{
    public class MoveElementsCommand : ICommand
    {
        private Dictionary<IGraphElementProcessor_Scope, Rect> oldPos;
        private Dictionary<IGraphElementProcessor_Scope, Rect> newPos;

        public MoveElementsCommand(Dictionary<IGraphElementProcessor_Scope, Rect> newPos)
        {
            this.newPos = newPos;
        }

        public void Do()
        {
            if (oldPos == null)
                oldPos = new Dictionary<IGraphElementProcessor_Scope, Rect>();
            else
                oldPos.Clear();

            foreach (var pair in newPos)
            {
                switch (pair.Key)
                {
                    case StickyNoteProcessor note:
                        {
                            var rect = new Rect(note.Position, note.Size.Value);
                            oldPos[pair.Key] = rect;
                            note.Position = pair.Value.position.ToVector2Int();
                            note.Size.Value = pair.Value.size.ToVector2Int();
                            break;
                        }
                    default:
                        {
                            var rect = new Rect(pair.Key.Position.ToVector2(), Vector2.zero);
                            oldPos[pair.Key] = rect;
                            pair.Key.Position = pair.Value.position.ToVector2Int();
                            break;
                        }
                }
            }
        }

        public void Redo()
        {
            Do();
        }

        public void Undo()
        {
            foreach (var pair in oldPos)
            {
                switch (pair.Key)
                {
                    case StickyNoteProcessor note:
                        {
                            note.Position = pair.Value.position.ToVector2Int();
                            note.Size.Value = pair.Value.size.ToVector2Int();
                            break;
                        }
                    default:
                        {
                            pair.Key.Position = pair.Value.position.ToVector2Int();
                            break;
                        }
                }
            }
        }
    }

    public class RemoveElementsCommand : ICommand
    {
        private BaseGraphProcessor graph;
        private List<IGraphElementProcessor> graphElements;
        private HashSet<IGraphElementProcessor> graphElementsSet = new HashSet<IGraphElementProcessor>();
        private Dictionary<BaseNodeProcessor, BaseGroupProcessor> nodeGroups = new Dictionary<BaseNodeProcessor, BaseGroupProcessor>();

        public RemoveElementsCommand(BaseGraphProcessor graph, IGraphElementProcessor[] graphElements)
        {
            this.graph = graph;
            this.graphElements = new List<IGraphElementProcessor>(graphElements);
            foreach (var graphElement in this.graphElements)
            {
                graphElementsSet.Add(graphElement);
            }

            for (int i = 0; i < graphElements.Length; i++)
            {
                var graphElement = graphElements[i];
                switch (graphElement)
                {
                    case BaseNodeProcessor node:
                        {
                            if (graph.Groups.NodeGroupMap.TryGetValue(node.ID, out var groupProcessor))
                            {
                                nodeGroups[node] = groupProcessor;
                            }

                            foreach (var connection in node.Ports.Values.SelectMany(port => port.connections))
                            {
                                if (this.graphElementsSet.Add(connection))
                                {
                                    this.graphElements.Add(connection);
                                }
                            }

                            break;
                        }
                }
            }

            this.graphElements.QuickSort((a, b) => { return GetPriority(a).CompareTo(GetPriority(b)); });
        }

        public void Do()
        {
            // 正向移除
            for (int i = 0; i < graphElements.Count; i++)
            {
                var graphElement = graphElements[i];
                switch (graphElement)
                {
                    case BaseConnectionProcessor connection:
                        {
                            graph.Disconnect(connection);
                            break;
                        }
                    case BaseGroupProcessor group:
                        {
                            graph.RemoveGroup(group);
                            break;
                        }
                    case BaseNodeProcessor node:
                        {
                            graph.RemoveNode(node);
                            break;
                        }
                    case StickyNoteProcessor stickNote:
                        {
                            graph.RemoveNote(stickNote.ID);
                            break;
                        }
                }
            }
        }

        public void Undo()
        {
            // 反向添加
            for (int i = graphElements.Count - 1; i >= 0; i--)
            {
                var graphElement = graphElements[i];
                switch (graphElement)
                {
                    case BaseNodeProcessor node:
                        {
                            graph.AddNode(node);
                            break;
                        }
                    case StickyNoteProcessor stickNote:
                        {
                            graph.AddNote(stickNote);
                            break;
                        }
                    case BaseConnectionProcessor connection:
                        {
                            graph.RevertDisconnect(connection);
                            break;
                        }
                    case BaseGroupProcessor group:
                        {
                            graph.AddGroup(group);
                            break;
                        }
                }
            }

            foreach (var pair in nodeGroups)
            {
                graph.Groups.AddNodeToGroup(pair.Value, pair.Key);
            }
        }

        public void Redo()
        {
            Do();
        }

        public int GetPriority(IGraphElementProcessor graphElement)
        {
            switch (graphElement)
            {
                case BaseConnectionProcessor:
                case BaseGroupProcessor:
                    {
                        return 1;
                    }
                case BaseNodeProcessor:
                case StickyNoteProcessor:
                    {
                        return 2;
                    }
            }

            return int.MaxValue;
        }
    }

    public class AddNodeCommand : ICommand
    {
        BaseGraphProcessor graph;
        public BaseNodeProcessor nodeVM;

        public AddNodeCommand(BaseGraphProcessor graph, Type nodeType, Vector2Int position)
        {
            this.graph = graph;
            this.nodeVM = graph.NewNode(nodeType, position);
        }

        public AddNodeCommand(BaseGraphProcessor graph, BaseNode node)
        {
            this.graph = graph;
            this.nodeVM = ViewModelFactory.ProduceViewModel(node) as BaseNodeProcessor;
        }

        public AddNodeCommand(BaseGraphProcessor graph, BaseNodeProcessor node)
        {
            this.graph = graph;
            this.nodeVM = node;
        }

        public void Do()
        {
            graph.AddNode(nodeVM);
        }

        public void Redo()
        {
            Do();
        }

        public void Undo()
        {
            graph.RemoveNode(nodeVM);
        }
    }

    public class AddGroupCommand : ICommand
    {
        public BaseGraphProcessor graph;
        public BaseGroupProcessor group;

        public AddGroupCommand(BaseGraphProcessor graph, BaseGroupProcessor group)
        {
            this.graph = graph;
            this.group = group;
        }

        public AddGroupCommand(BaseGraphProcessor graph, BaseGroup group)
        {
            this.graph = graph;
            this.group = ViewModelFactory.ProduceViewModel(group) as BaseGroupProcessor;
        }

        public void Do()
        {
            graph.AddGroup(group);
        }

        public void Redo()
        {
            Do();
        }

        public void Undo()
        {
            graph.RemoveGroup(group);
        }
    }

    public class AddToGroupCommand : ICommand
    {
        private BaseGraphProcessor graph;
        private BaseGroupProcessor group;
        private BaseNodeProcessor[] nodes;

        public AddToGroupCommand(BaseGraphProcessor graph, BaseGroupProcessor group, BaseNodeProcessor[] nodes)
        {
            this.graph = graph;
            this.group = group;
            this.nodes = nodes;
        }

        public void Do()
        {
            foreach (var node in nodes)
            {
                graph.Groups.AddNodeToGroup(group, node);
            }
        }

        public void Redo()
        {
            Do();
        }

        public void Undo()
        {
            foreach (var node in nodes)
            {
                graph.Groups.RemoveNodeFromGroup(node);
            }
        }
    }

    public class RemoveFromGroupCommand : ICommand
    {
        private BaseGraphProcessor graph;
        private BaseGroupProcessor group;
        private BaseNodeProcessor[] nodes;

        public RemoveFromGroupCommand(BaseGraphProcessor graph, BaseGroupProcessor group, BaseNodeProcessor[] nodes)
        {
            this.graph = graph;
            this.group = group;
            this.nodes = nodes;
        }

        public void Do()
        {
            foreach (var node in nodes)
            {
                graph.Groups.RemoveNodeFromGroup(node);
            }
        }

        public void Redo()
        {
            Do();
        }

        public void Undo()
        {
            foreach (var node in nodes)
            {
                graph.Groups.AddNodeToGroup(group, node);
            }
        }
    }

    public class RenameGroupCommand : ICommand
    {
        public BaseGroupProcessor group;
        public string oldName;
        public string newName;

        public RenameGroupCommand(BaseGroupProcessor group, string newName)
        {
            this.group = group;
            this.oldName = group.GroupName.Value;
            this.newName = newName;
        }

        public void Do()
        {
            group.GroupName.Value = newName;
        }

        public void Redo()
        {
            Do();
        }

        public void Undo()
        {
            group.GroupName.Value = oldName;
        }
    }

    public class CopyCommand : ICommand
    {
        private BaseGraphProcessor graph;
        private List<BaseNodeProcessor> nodes = new List<BaseNodeProcessor>();
        private List<BaseConnectionProcessor> connections = new List<BaseConnectionProcessor>();

        public CopyCommand(BaseGraphProcessor graph, List<BaseNode> nodes, List<BaseConnection> connections)
        {
            this.graph = graph;
            for (int i = 0; i < nodes.Count; i++) 
            {
                var nodeProcessor = ViewModelFactory.ProduceViewModel(nodes[i]);
                this.nodes.Add(nodeProcessor as BaseNodeProcessor);
            }
            for (int i = 0; i < connections.Count; i++)
            {
                this.connections.Add(ViewModelFactory.ProduceViewModel(connections[i]) as BaseConnectionProcessor);
            }
        }

        public void Do()
        {
            for (int i = 0; i < nodes.Count;i++)
            {
                graph.AddNode(nodes[i]);
            }

            for (int i = 0; i < connections.Count; i++)
            {
                graph.Connect(connections[i]);
            }
        }

        public void Undo()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                graph.RemoveNode(nodes[i]);
            }

            for (int i = 0; i < connections.Count; i++)
            {
                graph.Disconnect(connections[i]);
            }
        }
    }

    public class AddPortCommand : ICommand
    {
        BaseNodeProcessor node;
        BasePortProcessor port;
        bool successed = false;

        public AddPortCommand(BaseNodeProcessor node, string name, string label, PortDirection direction, PortCapacity capacity, Type type = null)
        {
            this.node = node;
            port = new BasePortProcessor(name, label, direction, capacity, type);
        }

        public void Do()
        {
            successed = false;
            if (!node.Ports.ContainsKey(port.Name))
            {
                node.AddPort(port);
                successed = true;
            }
        }

        public void Redo()
        {
            Do();
        }

        public void Undo()
        {
            if (!successed)
            {
                return;
            }

            node.RemovePort(port);
        }
    }

    public class RemovePortCommand : ICommand
    {
        BaseNodeProcessor node;
        BasePortProcessor port;
        bool successed = false;

        public RemovePortCommand(BaseNodeProcessor node, BasePortProcessor port)
        {
            this.node = node;
            this.port = port;
        }

        public RemovePortCommand(BaseNodeProcessor node, string name)
        {
            this.node = node;
            node.Ports.TryGetValue(name, out port);
        }

        public void Do()
        {
            successed = false;
            if (node.Ports.ContainsKey(port.Name))
            {
                node.AddPort(port);
                successed = true;
            }
        }

        public void Redo()
        {
            Do();
        }

        public void Undo()
        {
            if (!successed)
            {
                return;
            }

            node.RemovePort(port);
        }
    }

    public class ConnectCommand : ICommand
    {
        private readonly BaseGraphProcessor graph;

        BasePortProcessor from;
        BasePortProcessor to;
        BaseConnectionProcessor connectionVM;
        HashSet<BaseConnectionProcessor> replacedConnections = new HashSet<BaseConnectionProcessor>();

        public ConnectCommand(BaseGraphProcessor graph, BasePortProcessor from, BasePortProcessor to)
        {
            this.graph = graph;
            this.connectionVM = graph.NewConnection(from, to);

            this.from = from;
            this.to = to;
        }

        public ConnectCommand(BaseGraphProcessor graph, BaseConnectionProcessor connection)
        {
            this.graph = graph;
            this.connectionVM = connection;
            this.from = graph.Nodes[connection.FromNodeID].Ports[connection.FromPortName];
            this.to = graph.Nodes[connection.ToNodeID].Ports[connection.ToPortName];
        }

        public void Do()
        {
            replacedConnections.Clear();
            if (from.Capacity == PortCapacity.Single)
            {
                foreach (var connection in from.Connections)
                {
                    replacedConnections.Add(connection);
                }
            }

            if (to.Capacity == PortCapacity.Single)
            {
                foreach (var connection in to.Connections)
                {
                    replacedConnections.Add(connection);
                }
            }

            foreach (var connection in replacedConnections)
            {
                graph.Disconnect(connection);
            }

            graph.Connect(connectionVM);
        }

        public void Redo()
        {
            Do();
        }

        public void Undo()
        {
            graph.Disconnect(connectionVM);

            // 还原
            foreach (var connection in replacedConnections)
            {
                graph.RevertDisconnect(connection);
            }
        }
    }

    /// <summary>
    /// 改变节点值命令
    /// </summary>
    public class ChangeNodeValueCommand : ICommand
    {
        object target;
        FieldInfo field;
        object oldValue, newValue;

        Action OnDoFunc;
        Action OnUndoFunc;

        public ChangeNodeValueCommand(object target, FieldInfo field, object newValue, Action OnDoFunc = null, Action OnUndoFunc = null)
        {
            this.target = target;
            this.field = field;
            this.newValue = newValue;
            this.OnDoFunc = OnDoFunc;
            this.OnUndoFunc = OnUndoFunc;
        }

        public void Do()
        {
            oldValue = field.GetValue(target);
            field.SetValue(target, newValue);
            OnDoFunc?.Invoke();
        }

        public void Undo()
        {
            field.SetValue(target, oldValue);
            OnUndoFunc?.Invoke();
        }
    }
}
