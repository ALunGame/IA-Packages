using IANodeGraph.Editors;
using IANodeGraph.Model.Internal;
using IAToolkit;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static IANodeGraph.GraphGroupModel;

namespace IANodeGraph
{

    public class GraphGroupModel
    {
        public string DisplayName;
        public Type GraphGroupType;
        public int TreeId = 0;

        public class GropTreeData
        {
            public int Uid;
            public InternalGraphGroupAsset Group;
            public string GraphAssetName;

            public GropTreeData(int pUid, InternalGraphGroupAsset pGroup)
            {
                Uid = pUid;
                Group = pGroup;
            }

            public GropTreeData(int pUid, string pGraphAssetName)
            {
                Uid = pUid;
                GraphAssetName = pGraphAssetName;
            }

            public string GetDisplayName()
            {
                if (Group != null)
                {
                    return Group.name;
                }

                if (!string.IsNullOrEmpty(GraphAssetName))
                {
                    return GraphAssetName;
                }

                return string.Empty;
            }
        }

        public List<TreeViewItemData<GropTreeData>> TagTreeSource { get; set; } = new();

        public GraphGroupModel(string displayName, Type graphGroupType)
        {
            DisplayName = displayName;
            GraphGroupType = graphGroupType;

            RebuildTagTreeSource();
        }

        public void RebuildTagTreeSource()
        {
            TagTreeSource.Clear();

            GraphGroupPath groupPath = GraphSetting.Setting.GetSearchPath(GraphGroupType.FullName);

            List<InternalGraphGroupAsset> groupAssetList = GraphSetting.Setting.GetGroups(groupPath.searchPath);
            foreach (var group in groupAssetList)
            {
                int uid = TreeId++;
                TreeViewItemData<GropTreeData> treeData = new TreeViewItemData<GropTreeData>(uid, new GropTreeData(uid, group), AddChildGraphAssets(group));
                TagTreeSource.Add(treeData);
            }
        }

        private List<TreeViewItemData<GropTreeData>> AddChildGraphAssets(InternalGraphGroupAsset pGroup)
        {
            List<TreeViewItemData<GropTreeData>> childTreeItems = new List<TreeViewItemData<GropTreeData>>();

            List<string> childAssetNames = pGroup.GetAllGraphFileName();
            foreach (var childAssetName in childAssetNames)
            {
                int uid = TreeId++;
                TreeViewItemData<GropTreeData> childItem = new TreeViewItemData<GropTreeData>(uid, new GropTreeData(uid, childAssetName));
                childTreeItems.Add(childItem);
            }

            return childTreeItems;
        }
    }

    internal class GraphGroupView : VisualElement
    {
        public TreeView GroupTreeView { get; private set; }
        public GraphGroupModel Model { get; private set; }

        public GraphGroupView()
        {
            GraphProcessorEditorStyles.LoadUXML("GraphGroupView").CloneTree(this);
        }

        public void SetUp(string pGroupDisplayName, Type pGroupType)
        {
            Model = new GraphGroupModel(pGroupDisplayName, pGroupType);

            InitToolBarView();

            InitTagTreeView();
        }

        private void InitToolBarView()
        {
            this.Q<Button>("ExpanAllBtn").clicked += ClickExpanAllBtn;
            this.Q<Button>("FoldAllBtn").clicked += ClickFoldAllBtn;

            this.Q<Button>("SaveBtn").clicked += ClickSaveBtnBtn;
            this.Q<Button>("RemoveBtn").clicked += ClickRemoveTagBtn;
            this.Q<Button>("AddBtn").clickable.clickedWithEventInfo += ClickAddTagBtn;

            this.Q<Button>("searchButton").clicked += ClickSearchBtn;
        }

        private void InitTagTreeView()
        {
            GroupTreeView = this.Q<TreeView>("GraphTree");

            GroupTreeView.makeItem = MakeItem;
            GroupTreeView.bindItem = BindItem;
            GroupTreeView.fixedItemHeight = 38;
            GroupTreeView.SetRootItems(Model.TagTreeSource);
            GroupTreeView.RefreshItems();
        }

        #region ToolBar

        private void ClickExpanAllBtn()
        {
            GroupTreeView.ExpandAll();
        }

        private void ClickFoldAllBtn()
        {
            GroupTreeView.CollapseAll();
        }

        private void ClickSaveBtnBtn()
        {
        }

        private void ClickRemoveTagBtn()
        {
        }

        private void ClickAddTagBtn(EventBase pEvent)
        {
            MiscHelper.Input("输入视图分组名", (name) =>
            {
                GraphGroupPath groupPath = GraphSetting.Setting.GetSearchPath(Model.GraphGroupType.FullName);
                List<InternalGraphGroupAsset> groups = GraphSetting.Setting.GetGroups(groupPath.searchPath);
                foreach (InternalGraphGroupAsset group in groups)
                {
                    if (group.name == name)
                    {
                        Debug.LogError($"创建视图分组失败，重复的资源名: {name}");
                        return;
                    }
                }
                var graphGroup = ScriptableObject.CreateInstance(Model.GraphGroupType);
                graphGroup.name = name;
                AssetDatabase.CreateAsset(graphGroup, $"{groupPath.searchPath}/{name}.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                RefreshGroupTreeView();
            }, pEvent.originalMousePosition);
        }

        private void ClickSearchBtn()
        {
            //TextField searchText = this.Q<TextField>();

            //List<int> expandUids = new List<int>();

            //foreach (var tag in ViewModel.Model.TagInfos)
            //{
            //    if (tag.Name.Contains(searchText.text))
            //    {
            //        List<int> parentUids = ViewModel.Model.GetParentPath(tag.Uid);
            //        foreach (var parentUid in parentUids)
            //        {
            //            if (!expandUids.Contains(parentUid))
            //            {
            //                expandUids.Add(parentUid);
            //            }
            //        }
            //    }
            //}

            //foreach (var tagUid in expandUids)
            //{
            //    GroupTreeView.ExpandItem(tagUid, false, false);
            //}

            //GroupTreeView.RefreshItems();
        }

        #endregion

        #region GroupTreeView

        private VisualElement MakeItem()
        {
            VisualElement element = new VisualElement();
            element.style.flexGrow = 1;

            Label label = new Label();
            label.style.flexGrow = 1;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            element.Add(label);

            return element;
        }

        private void BindItem(VisualElement pElement, int pIndex)
        {
            var label = pElement.Q<Label>();

            GropTreeData treeData = (GropTreeData)GroupTreeView.viewController.GetItemForIndex(pIndex);
            label.text = treeData.GetDisplayName();
        }

        private void RefreshGroupTreeView()
        {
            Model.RebuildTagTreeSource();
            GroupTreeView.SetRootItems(Model.TagTreeSource);
            GroupTreeView.RefreshItems();
        }

        #endregion
    }
}
