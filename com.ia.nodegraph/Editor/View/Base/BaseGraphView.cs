using IANodeGraph.Model;
using IANodeGraph.View.Utils;
using IAToolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using IAEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace IANodeGraph.View
{
    public partial class BaseGraphView
    {
        protected virtual void OnInitialized() { }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Create Group", delegate
            {
                var group = new BaseGroup() { groupName = "New Group" };
                group.nodes.AddRange(selection.Where(select => select is BaseNodeView).Select(select => (select as BaseNodeView).Model.Model.guid));
                
                var vm = ViewModelFactory.CreateViewModel(group) as BaseGroupVM;

                CommandDispacter.Do(new AddGroupCommand(Model, vm));
            }, (DropdownMenuAction a) => selection.Find(s => s is BaseNodeView) != null ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Hidden);
            
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
                        return true;
                    default:
                        return false;
                }
            });

            if (evt.target is GraphView || evt.target is Node || evt.target is Group || evt.target is Edge)
            {
                evt.menu.AppendAction("Delete", delegate
                {
                    DeleteSelectionCallback(AskUser.DontAskUser);
                }, (DropdownMenuAction a) => canDeleteSelection ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Hidden);
                evt.menu.AppendSeparator();
            }

            if (evt.target is Node)
            {
                evt.menu.AppendAction("复制", delegate
                {
                    DuplicateSelection();
                });

                BaseNodeView nodeView = (BaseNodeView)evt.target;
                nodeView.CreateSelectMenu(evt.menu);

                evt.menu.AppendSeparator();
            }
            if (evt.target is Edge)
            {
                evt.menu.AppendAction("复制", delegate
                {
                    DuplicateSelection();
                });

                evt.menu.AppendSeparator();
            }
        }

        /// <summary>
        /// 获得匹配的端口
        /// </summary>
        /// <param name="startPortView"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPortView, NodeAdapter nodeAdapter)
        {
            BasePortView portView = startPortView as BasePortView;
            List<Port> compatiblePorts = new List<Port>();
            ports.ForEach(_portView =>
            {
                var toPortView = _portView as BasePortView;
                if (IsCompatible(portView, toPortView, nodeAdapter))
                    compatiblePorts.Add(_portView);
            });
            return compatiblePorts;
        }

        public IEnumerable<Type> GetNodeTypes()
        {
            return Model.GetNodeTypes();
        }

        protected virtual Type GetNodeViewType(BaseNodeVM node)
        {
            return GraphProcessorEditorUtility.GetNodeViewType(node.ModelType);
        }
        
        protected virtual Type GetGroupViewType(BaseGroupVM group)
        {
            return GraphProcessorEditorUtility.GetGroupViewType(group.ModelType);
        }

        protected virtual Type GetConnectionViewType(BaseConnectionVM connection)
        {
            return typeof(BaseConnectionView);
        }

        protected virtual void UpdateInspector()
        {
            foreach (var element in selection)
            {
                switch (element)
                {
                    case BaseNodeView nodeView:
                        InspectorExtension.DrawObjectInInspector("Node",nodeView);
                        return;
                    case BaseConnectionView edgeView:
                        InspectorExtension.DrawObjectInInspector("Connection", edgeView, GraphAsset);
                        return;
                    default:
                        break;
                }
            }
            InspectorExtension.DrawObjectInInspector("Graph",this, GraphAsset);
        }

        /// <summary>
        /// 复制选择
        /// </summary>
        public virtual void DuplicateSelection()
        {
            List<BaseNodeView> nodes = new List<BaseNodeView>();
            List<BaseConnectionView> edges = new List<BaseConnectionView>();
            foreach (var element in selection)
            {
                switch (element)
                {
                    case BaseNodeView nodeView:
                        nodes.Add(element as BaseNodeView);
                        break;
                    case BaseConnectionView edgeView:
                        edges.Add(element as BaseConnectionView);
                        break;
                    default:
                        break;
                }
            }

            if (nodes.Count == 0 && edges.Count == 0)
            {
                return;
            }

            //序列化节点
            Dictionary<string, string> saveUidMap = new Dictionary<string, string>();
            Dictionary<string, BaseNodeVM> newNodeMap = new Dictionary<string, BaseNodeVM>();
            for (int i = 0; i < nodes.Count; i++)
            {
                BaseNodeVM oldNode = nodes[i].Model;
                string dataStr = JsonMapper.ToJson(oldNode.Model);

                string oldUid = oldNode.GUID;
                string newUid = Model.GenerateNodeGUID();

                BaseNode newNode = JsonMapper.ToObject<BaseNode>(dataStr);
                newNode.guid = newUid;
                newNode.position += new UnityEngine.Vector2(nodes[i].contentRect.width + 100, 0);
                saveUidMap.Add(oldUid, newUid);
                
                BaseNodeVM newNodeVM = ViewModelFactory.CreateViewModel(newNode) as BaseNodeVM;
                
                CommandDispacter.Do(new AddNodeCommand(Model, newNodeVM));
                newNodeMap.Add(newUid, newNodeVM);
            }

            //序列化链接
            for (int i = 0; i < edges.Count; i++)
            {
                BaseConnectionVM connectionView = edges[i].Model;
                
                string dataStr = JsonMapper.ToJson(connectionView.Model);
                BaseConnection newConnection = JsonMapper.ToObject<BaseConnection>(dataStr);

                //链接替换
                BaseNodeVM from = null;
                string fromPortName = connectionView.Model.fromPortName;
                if (saveUidMap.ContainsKey(newConnection.from))
                {
                    from = newNodeMap[saveUidMap[newConnection.from]];
                }

                BaseNodeVM to = null;
                string toPortName = connectionView.Model.toPortName;
                if (saveUidMap.ContainsKey(newConnection.to))
                {
                    to = newNodeMap[saveUidMap[newConnection.to]];
                }

                if (from != null && to != null)
                {
                    CommandDispacter.Do(new ConnectCommand(Model, from, fromPortName, to, toPortName));
                }
            }
        }

        protected virtual bool IsCompatible(BasePortView portView, BasePortView toPortView, NodeAdapter nodeAdapter)
        {
            if (toPortView.direction == portView.direction)
                return false;
            // 类型兼容查询
            if (!TypesAreConnectable(portView.Model.Model.type, toPortView.Model.Model.type))
                return false;
            return true;
        }

        /// <summary>
        /// 判断俩个类型是否可以相连
        /// </summary>
        /// <param name="fromType"></param>
        /// <param name="toType"></param>
        /// <returns></returns>
        public bool TypesAreConnectable(Type fromType, Type toType)
        {
            if (fromType == null || toType == null)
                return false;

            //是子类
            if (fromType.IsSubclassOf(toType))
            {
                return true;
            }

            //是否可以强转
            if (toType.IsAssignableFrom(fromType))
                return true;

            return false;
        }
    }
}
