﻿using System;
using System.Collections.Generic;

namespace IANodeGraph.Model
{
    public partial class BaseGraphProcessor
    {
        #region Fields
        private Groups groups;

        public event Action<BaseGroupProcessor> OnGroupAdded;
        public event Action<BaseGroupProcessor> OnGroupRemoved;

        #endregion

        #region Properties

        public Groups Groups => groups;

        #endregion

        private void InitGroups()
        {
            this.groups = new Groups();

            for (int i = 0; i < Model.groups.Count; i++)
            {
                var group = Model.groups[i];
                if (group == null)
                {
                    Model.groups.RemoveAt(i--);
                    continue;
                }

                for (int j = group.nodes.Count - 1; j >= 0; j--)
                {
                    if (!nodes.ContainsKey(group.nodes[j]))
                        group.nodes.RemoveAt(j);
                }

                var groupVM = (BaseGroupProcessor)ViewModelFactory.ProduceViewModel(group);
                groupVM.Owner = this;
                groups.AddGroup(groupVM);
            }
        }

        #region API

        public void AddGroup(BaseGroupProcessor group)
        {
            groups.AddGroup(group);
            Model.groups.Add(group.Model);
            group.Owner = this;
            OnGroupAdded?.Invoke(group);
        }

        public void RemoveGroup(BaseGroupProcessor group)
        {
            groups.RemoveGroup(group);
            Model.groups.Remove(group.Model);
            OnGroupRemoved?.Invoke(group);
        }

        public virtual BaseGroupProcessor NewGroup(string groupName)
        {
            var group = new BaseGroup()
            {
                id = NewID(),
                groupName = groupName
            };
            return ViewModelFactory.ProduceViewModel(group) as BaseGroupProcessor;
        }

        #endregion
    }

    public class Groups
    {
        private Dictionary<int, BaseGroupProcessor> groupMap = new Dictionary<int, BaseGroupProcessor>();
        private Dictionary<int, BaseGroupProcessor> nodeGroupMap = new Dictionary<int, BaseGroupProcessor>();

        public IReadOnlyDictionary<int, BaseGroupProcessor> GroupMap
        {
            get { return groupMap; }
        }

        public IReadOnlyDictionary<int, BaseGroupProcessor> NodeGroupMap
        {
            get { return nodeGroupMap; }
        }

        public void AddNodeToGroup(BaseGroupProcessor group, BaseNodeProcessor node)
        {
            var nodes = new BaseNodeProcessor[] { node };
            if (nodeGroupMap.TryGetValue(node.ID, out var _group))
            {
                if (_group == group)
                {
                    return;
                }
                else
                {
                    _group.Model.nodes.Remove(node.ID);
                    _group.NotifyNodeRemoved(nodes);
                }
            }

            nodeGroupMap[node.ID] = group;
            group.Model.nodes.Add(node.ID);
            group.NotifyNodeAdded(nodes);
        }

        public void RemoveNodeFromGroup(BaseNodeProcessor node)
        {
            if (!nodeGroupMap.TryGetValue(node.ID, out var group))
                return;

            var nodes = new BaseNodeProcessor[] { node };
            nodeGroupMap.Remove(node.ID);
            group.Model.nodes.Remove(node.ID);
            group.NotifyNodeRemoved(nodes);
        }

        public void AddGroup(BaseGroupProcessor group)
        {
            this.groupMap.Add(group.ID, group);
            foreach (var pair in groupMap)
            {
                foreach (var nodeID in pair.Value.Nodes)
                {
                    this.nodeGroupMap[nodeID] = pair.Value;
                }
            }
        }

        public void RemoveGroup(BaseGroupProcessor group)
        {
            foreach (var nodeID in group.Nodes)
            {
                nodeGroupMap.Remove(nodeID);
            }

            groupMap.Remove(group.ID);
        }
    }
}
