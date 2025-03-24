using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace IAToolkit.Element
{

    public class SearchPopupTreeNode
    {
        public string Name { get; set; }
        public string Tooltip { get; set; }
        public object UserData { get; set; }
        public List<SearchPopupTreeNode> Children { get; } = new List<SearchPopupTreeNode>();

        public SearchPopupTreeNode(string pName, string pTooltip = null, object pUserData = null, List<SearchPopupTreeNode> pChildren = null)
        {
            Name = pName;
            Tooltip = pTooltip;
            UserData = pUserData;

            if (pChildren != null)
            {
                Children = pChildren;
            }
        }
    }

    /// <summary>
    /// 带有搜索框的菜单弹框
    /// </summary>
    public class SearchPopupWindow : EditorWindow
    {
        public static SearchPopupWindow ShowWindow(string pTitle, List<SearchPopupTreeNode> pOriData,
            Func<SearchPopupTreeNode, bool> pOnClickTreeNode, Func<List<SearchPopupTreeNode>> pOnGetOriData = null)
        {
            Vector2 mousePos = Event.current.mousePosition;
            Vector2 screenPos = GUIUtility.GUIToScreenPoint(mousePos);
            return HandleShowWindow(pTitle, screenPos, pOriData, pOnClickTreeNode, pOnGetOriData);
        }
        
        public static SearchPopupWindow ShowWindow(string pTitle, Vector2 pMousePos, List<SearchPopupTreeNode> pOriData,
            Func<SearchPopupTreeNode, bool> pOnClickTreeNode, Func<List<SearchPopupTreeNode>> pOnGetOriData = null)
        {
            Vector2 screenPos = GUIUtility.GUIToScreenPoint(pMousePos);
            return HandleShowWindow(pTitle, screenPos, pOriData, pOnClickTreeNode, pOnGetOriData);
        }

        private static SearchPopupWindow HandleShowWindow(string pTitle, Vector2 pScreenPos, List<SearchPopupTreeNode> pOriData,
            Func<SearchPopupTreeNode, bool> pOnClickTreeNode, Func<List<SearchPopupTreeNode>> pOnGetOriData = null)
        {
            SearchPopupWindow wnd = GetWindow<SearchPopupWindow>();
            wnd.titleContent = new GUIContent(pTitle);
            wnd.OriData = pOriData;
            wnd.OnClickTreeNode = pOnClickTreeNode;
            wnd.OnGetOriData = pOnGetOriData;
            wnd.ShowPopup();
            wnd.InitWindow();
            wnd.position = new Rect(pScreenPos + new Vector2(0, 20), new Vector2(320, 480));
            return wnd;
        }

        public List<SearchPopupTreeNode> OriData { get; private set; }

        public int TreeNodeId;
        public List<TreeViewItemData<SearchPopupTreeNode>> TreeSource { get; set; } = new();
        public Func<SearchPopupTreeNode, bool> OnClickTreeNode { get; private set; }
        public Func<List<SearchPopupTreeNode>> OnGetOriData { get; private set; }

        public VisualElement Content { get; private set; }
        public TextField SearchField {  get; private set; }
        private string searchInputStr;
        public TreeView TreeView { get; private set; }

        public void CreateGUI()
        {
            Content = new VisualElement();
            Content.name = "content";
            Content.style.flexGrow = 1;
            Content.style.flexDirection = FlexDirection.Column;

            var searchBox = new VisualElement();
            searchBox.style.flexDirection = FlexDirection.Row;
            searchBox.style.justifyContent = Justify.SpaceBetween;

            SearchField = new TextField();
            SearchField.name = "search_input";
            SearchField.RegisterValueChangedCallback((evt) =>
            {
                searchInputStr = evt.newValue;
            });
            SearchField.style.flexGrow = 1;
            searchBox.Add(SearchField);

            Button searchBtn = new Button(() =>
            {
                var filterData = FilterNodes(searchInputStr.Trim());
                CreateTreeSource(filterData);

                RefreshTreeItems();
            });
            searchBtn.text = "搜索";
            searchBox.Add(searchBtn);

            Content.Add(searchBox);

            TreeView = new TreeView();
            TreeView.makeItem = MakeItem;
            TreeView.bindItem = BindItem;
            TreeView.fixedItemHeight = 38;
            TreeView.BorderWidthColor(1.5f, Color.black);

            Content.Add(TreeView);

            rootVisualElement.Add(Content);
        }

        public void InitWindow()
        {
            RebuildTreeSource();
            RefreshTreeItems();
        }

        private void CreateSearchField()
        {
            SearchField = new TextField();
            SearchField.name = "search_input";
            SearchField.RegisterValueChangedCallback(OnSearchChanged);
        }

        private void OnSearchChanged(ChangeEvent<string> evt)
        {
            var filterData = FilterNodes(evt.newValue.Trim());
            CreateTreeSource(filterData);

            RefreshTreeItems();
        }

        #region TreeView

        private VisualElement MakeItem()
        {
            var label = new Label();
            label.style.flexGrow = 1;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            return label;
        }

        private void BindItem(VisualElement pElement, int pIndex)
        {
            SearchPopupTreeNode nodeInfo = (SearchPopupTreeNode)TreeView.viewController.GetItemForIndex(pIndex);

            var label = pElement.Q<Label>();
            label.text = nodeInfo.Name;
            label.RegisterCallback<ClickEvent>((evt) =>
            {
                if (OnClickTreeNode.Invoke(nodeInfo))
                {
                    Close();
                }
            });
        }

        private void RefreshTreeItems()
        {
            TreeView.SetRootItems(TreeSource);
            TreeView.RefreshItems();
        }

        #endregion

        #region TreeViewData

        private List<SearchPopupTreeNode> FilterNodes(string pKeyword)
        {
            if (string.IsNullOrEmpty(pKeyword))
            {
                return OriData;
            }

            string tKeyStr = pKeyword.ToLower();

            var result = new List<SearchPopupTreeNode>();
            foreach (var node in OriData)
            {
                if (node.Name.ToLower().Contains(tKeyStr))
                {
                    result.Add(node);
                    continue;
                }
                else
                {
                    SearchPopupTreeNode newTree = null;

                    List<SearchPopupTreeNode> childs = node.Children;
                    if (childs != null)
                    {
                        foreach (var item in childs)
                        {
                            if (item.Name.ToLower().Contains(tKeyStr))
                            {
                                if (newTree == null)
                                {
                                    newTree = new SearchPopupTreeNode(node.Name, node.Tooltip, node.UserData, new List<SearchPopupTreeNode>());
                                }
                                newTree.Children.Add(item);
                            }
                        }
                    }

                    if (newTree != null)
                    {
                        result.Add(newTree);
                    }
                }
            }
            return result;
        }

        private void RebuildTreeSource()
        {
            if (OnGetOriData != null)
            {
                OriData = OnGetOriData.Invoke();
            }
            CreateTreeSource(OriData);
        }

        private void CreateTreeSource(List<SearchPopupTreeNode> pNodes)
        {
            TreeNodeId = 0;
            TreeSource.Clear();

            var orderData = pNodes
                .OrderBy(x =>
                {
                    return x.Name;
                })
                .ToArray();

            foreach (var nodeInfo in orderData)
            {
                int tId = TreeNodeId++;
                TreeViewItemData<SearchPopupTreeNode> treeData = new TreeViewItemData<SearchPopupTreeNode>(tId, nodeInfo, AddTreeItemData(nodeInfo));
                TreeSource.Add(treeData);
            }
        }

        private List<TreeViewItemData<SearchPopupTreeNode>> AddTreeItemData(SearchPopupTreeNode pRootNode)
        {
            List<SearchPopupTreeNode> childNodeInfos = pRootNode.Children;
            if (childNodeInfos == null)
            {
                return null;
            }

            List<TreeViewItemData<SearchPopupTreeNode>> childTreeItems = new List<TreeViewItemData<SearchPopupTreeNode>>();
            //先检测子
            foreach (var child in childNodeInfos)
            {
                int tId = TreeNodeId++;
                TreeViewItemData<SearchPopupTreeNode> childItem = new TreeViewItemData<SearchPopupTreeNode>(tId, child, AddTreeItemData(child));
                childTreeItems.Add(childItem);
            }

            return childTreeItems;
        } 

        #endregion
    }
}
