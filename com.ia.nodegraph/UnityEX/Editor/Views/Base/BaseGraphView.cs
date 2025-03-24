using IANodeGraph.Editors;
using IANodeGraph.Model;
using IANodeGraph.Window;
using IAToolkit.Command;
using IAToolkit.UnityEditors;
using IAToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace IANodeGraph.View
{
    public class GraphViewContext
    {
        public BaseGraphWindow window;

        public CommandDispatcher commandDispatcher;
    }

    public abstract partial class BaseGraphView
    {
        List<Port> compatiblePorts = new List<Port>();

        protected virtual void OnInitialized()
        {
        }

        protected virtual void OnDestroyed()
        {
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var mousePosition = evt.mousePosition;
            var localMousePosition = contentViewContainer.WorldToLocal(mousePosition);

            base.BuildContextualMenu(evt);

            evt.menu.MenuItems().RemoveAll(item =>
            {
                if (item is DropdownMenuSeparator)
                {
                    return true;
                }

                if (!(item is DropdownMenuAction actionItem))
                {
                    return false;
                }

                switch (actionItem.name)
                {
                    case "Cut":
                    case "Copy":
                    case "Paste":
                    case "Duplicate":

                    case "Light Theme":
                    case "Dark Theme":
                    case "Small Text Size":
                    case "Medium Text Size":
                    case "Large Text Size":
                    case "Huge Text Size":
                        return true;
                    default:
                        return false;
                }
            });

            evt.menu.AppendAction("创建分组", delegate
            {
                var group = ViewModel.NewGroup("New Group");
                group.Model.nodes.AddRange(selection.Where(select => select is BaseNodeView).Select(select => (select as BaseNodeView).ViewModel.ID));
                CommandDispatcher.Do(new AddGroupCommand(ViewModel, group));
            }, (DropdownMenuAction a) => canDeleteSelection && selection.Find(s => s is BaseNodeView) != null ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Hidden);

            evt.menu.AppendAction("创建节点", delegate
            {
                var data = new StickyNote();
                data.id = ViewModel.NewID();
                data.position = localMousePosition.ToVector2Int();
                data.size = new Vector2Int(300, 200);
                data.title = "title";
                data.content = "contents";
                var note = ViewModelFactory.ProduceViewModel(data) as StickyNoteProcessor;
                CommandDispatcher.Do(() => { ViewModel.AddNote(note); }, () => { ViewModel.RemoveNote(note.ID); });
            });

            //删除
            switch (evt.target)
            {
                case GraphView:
                case UnityEditor.Experimental.GraphView.Node:
                case UnityEditor.Experimental.GraphView.Group:
                case Edge:
                case UnityEditor.Experimental.GraphView.StickyNote:
                    {
                        evt.menu.AppendAction("删除", delegate { DeleteSelectionCallback(AskUser.DontAskUser); }, (DropdownMenuAction a) => canDeleteSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Hidden);
                        break;
                    }
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPortView, NodeAdapter nodeAdapter)
        {
            BasePortView portView = startPortView as BasePortView;

            compatiblePorts.Clear();
            foreach (var _nodeView in NodeViews.Values)
            {
                if (_nodeView.PortViews.Count == 0)
                {
                    continue;
                }

                foreach (var _portView in _nodeView.PortViews.Values)
                {
                    if (IsCompatible(_portView, portView, nodeAdapter))
                        compatiblePorts.Add(_portView);
                }
            }

            return compatiblePorts;
        }

        protected virtual void BuildNodeMenu(NodeMenuWindow nodeMenu)
        {
            foreach (var item in GraphProcessorUtil.NodeStaticInfos)
            {
                var nodeType = item.Key;
                var nodeStaticInfo = GraphProcessorUtil.NodeStaticInfos[nodeType];
                if (nodeStaticInfo.hidden)
                    continue;

                if (!ViewModel.Model.CheckNodeDisplay(nodeType))
                    continue;

                var path = nodeStaticInfo.path;
                var menu = nodeStaticInfo.menu;
                nodeMenu.entries.Add(new NodeMenuWindow.NodeEntry(path, menu, nodeType));
            }
        }

        protected virtual BaseNodeView NewNodeView(BaseNodeProcessor nodeVM)
        {
            return Activator.CreateInstance(GraphProcessorEditorUtil.GetViewType(nodeVM.ModelType)) as BaseNodeView;
        }

        protected virtual GroupView NewGroupView(BaseGroupProcessor groupVM)
        {
            return Activator.CreateInstance(GraphProcessorEditorUtil.GetViewType(groupVM.ModelType)) as GroupView;
        }

        protected virtual BaseConnectionView NewConnectionView(BaseConnectionProcessor connectionVM)
        {
            return Activator.CreateInstance(GraphProcessorEditorUtil.GetViewType(connectionVM.ModelType)) as BaseConnectionView;
        }

        protected virtual void UpdateInspector()
        {
            foreach (var element in selection)
            {
                if (!ObjectEditor.HasEditor(element.GetType()))
                    continue;

                ObjectInspector.Show(element);
                return;
            }

            ObjectInspector.Show(this);
        }

        protected virtual bool IsCompatible(BasePortView fromPortView, BasePortView toPortView, NodeAdapter nodeAdapter)
        {
            if (toPortView.direction == fromPortView.direction)
                return false;
            return IsCompatible(toPortView.ViewModel.portType.Value, fromPortView.ViewModel.portType.Value);
        }

        /// <summary>
        /// 判断端口是否可以连接
        /// </summary>
        /// <param name="pToPortType"></param>
        /// <param name="pFromPortType"></param>
        /// <returns></returns>
        public virtual bool IsCompatible(Type pToPortType, Type pFromPortType)
        {
            if ((pToPortType == null) || (pFromPortType == null))
                return false;

            pToPortType = GetArrayOrListItemType(pToPortType) ?? pToPortType;
            pFromPortType = GetArrayOrListItemType(pFromPortType) ?? pFromPortType;
            
            // 类型兼容查询
            if (!pToPortType.IsAssignableFrom(pFromPortType) && !pFromPortType.IsAssignableFrom(pToPortType))
                return false;

            return true;
        }

        /// <summary>
        /// 是数组或者
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static bool IsArrayOrList(Type pType)
        {
            if (pType.IsArray)
                return true;
            else if (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(List<>)) 
                return true;
            return false;
        }

        /// <summary>
        /// 获取数组或List的元素类型
        /// </summary>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static Type GetArrayOrListItemType(Type pType)
        {
            if (pType.IsArray)
            {
                Type elementType = pType.GetElementType();
                return elementType;
            }
            else if (pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(List<>)) 
            {
                Type elementType = pType.GetGenericArguments()[0];
                return elementType;
            }
            return null;
        }
    }
}
