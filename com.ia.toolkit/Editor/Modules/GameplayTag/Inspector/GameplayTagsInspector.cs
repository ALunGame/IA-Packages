using System;
using System.Collections.Generic;
using IAToolkit;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace IAToolkit
{
    public enum GameplayTagsDisplayType
    {
        Tree,
        Row,
    }
    
    [CustomEditor(typeof(GameplayTagsSetting), true)]
    public class GameplayTagsInspector : Editor
    {
        private const string UxmlPath = "Assets/com.lc.toolkit/Editor/Modules/GameplayTag/GameplayTagsWindow.uxml";
        private static GUIHelper.ContextData<GUIStyle> BigLabel;
        private static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();
        
        public GameplayTags Model { get; private set; }
        public GameplayTagsDisplayType DisplayType = GameplayTagsDisplayType.Tree;
        public bool ExpandState { get; private set; }
            
        public ScrollView ScrollView;
        public GameplayTagsTop_Element TopElement { get; private set; }
        
        public VisualElement rootVisualElement { get; private set; }

        private void OnEnable()
        {
            Model = GameplayTagsSetting.LoadTags();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (!ContextDataCache.TryGetContextData<GUIStyle>("BigLabel", out BigLabel))
            {
                BigLabel.value = new GUIStyle(GUI.skin.label);
                BigLabel.value.fontSize = 18;
                BigLabel.value.fontStyle = FontStyle.Bold;
                BigLabel.value.alignment = TextAnchor.MiddleLeft;
                BigLabel.value.stretchWidth = true;
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            rootVisualElement = new VisualElement();
            
            CreateGUI();
            
            return rootVisualElement;
        }

        public void CreateGUI()
        {
            IMGUIContainer folderPathIMGUI = new IMGUIContainer(() =>
            {
                GUILayoutExtension.VerticalGroup(() =>
                {
                    GameplayTagsSetting setting = target as GameplayTagsSetting;
                    string newPath = MiscHelper.FolderPath("标签目录:", setting.tagSaveRootPath);
                    if (newPath != setting.tagSaveRootPath)
                    {
                        setting.tagSaveRootPath = newPath;
                        EditorUtility.SetDirty(target);
                    }
                });
            });
            
            rootVisualElement.Add(folderPathIMGUI);
            
            
            VisualElement root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            visualTree.CloneTree(root);
    
            CreateUIElement();
        }
    
        private void CreateUIElement()
        {
            TopElement = new GameplayTagsTop_Element();
            TopElement.Init(this);

            ScrollView = rootVisualElement.Q<ScrollView>("TagsArea");
            ChangeDisplayType(DisplayType);
            SetExpandState(ExpandState);
        }
        
        public void ChangeDisplayType(GameplayTagsDisplayType pNewType)
        {
            DisplayType = pNewType;
            if (pNewType == GameplayTagsDisplayType.Row)
            {
                RefreshRow();
            }
            else if (pNewType == GameplayTagsDisplayType.Tree)
            {
                RefreshTree();
            }
        }
        
        #region Node

        public List<GameplayTagNode_Element> RootNodeElements = new List<GameplayTagNode_Element>();
        private void RefreshTree()
        {
            ScrollView.Clear();
            RootNodeElements.Clear();
            
            List<GameplayTagNode> nodes = GameplayTagNode.GameplayTagsToRootNodes(Model);
            for (int i = 0; i < nodes.Count; i++)
            {
                Debug.Log(nodes[i]);
            }
            
            for (int i = 0; i < nodes.Count; i++)
            {
                GameplayTagNode rootModel = nodes[i];
                GameplayTagNode_Element rootNodeElement = new GameplayTagNode_Element();
                rootNodeElement.SetUp(rootModel,null);
                rootNodeElement.Inspector = this;
                
                ScrollView.Add(rootNodeElement.RootElement);
                RootNodeElements.Add(rootNodeElement);
            }
        }
        
        public void RemoveRootTag(GameplayTagNode_Element pRootNode)
        {
            ScrollView scrollView = rootVisualElement.Q<ScrollView>("TagsArea");
            if (RootNodeElements.Contains(pRootNode))
            {
                scrollView.Remove(pRootNode.RootElement);
                RootNodeElements.Remove(pRootNode);
            }
        }

        #endregion

        #region Row

        public List<GameplayTagRow_Element> RowElements = new List<GameplayTagRow_Element>();
        
        private void RefreshRow()
        {
            ScrollView.Clear();
            RowElements.Clear();

            Model.tags.Sort();
            for (int i = 0; i < Model.tags.Count; i++)
            {
                string tag = Model.tags[i];
                GameplayTagRow_Element rowElement = new GameplayTagRow_Element();
                rowElement.SetUp(tag,this);
                
                ScrollView.Add(rowElement.RootElement);
                RowElements.Add(rowElement);
            }
        }
        
        public void RemoveRowTag(string pRowTag)
        {
            Model.tags.Remove(pRowTag);
            RefreshRow();
        }

        public void UpdateRowTag(string pRowTag,string pNewRowTag)
        {
            for (int i = 0; i < Model.tags.Count; i++)
            {
                string tag = Model.tags[i];
                if (tag == pRowTag)
                {
                    Model.tags[i] = pNewRowTag;
                }
            }
        }
        
        #endregion

        public void SetExpandState(bool pExpandState)
        {
            ExpandState = pExpandState;
            foreach (GameplayTagNode_Element rootNode in RootNodeElements)
            {
                rootNode.SetExpandState(pExpandState);
            }
        }

        public void CreateRootTag(string pRootTag)
        {
            if (DisplayType == GameplayTagsDisplayType.Tree)
            {
                List<GameplayTagNode> rootNodes = new List<GameplayTagNode>();
                foreach (GameplayTagNode_Element node in RootNodeElements)
                {
                    if (node.Model.ShortName() == pRootTag)
                    {
                        Debug.LogError($"根标签重复:{pRootTag}");
                        return;
                    }
                }
                
                
            }

            for (int i = 0; i < Model.tags.Count; i++)
            {
                string tTag = Model.tags[i];
                string[] tags = tTag.Split(".");
                if (tags[0] == pRootTag)
                {
                    Debug.LogError($"根标签重复:{pRootTag}");
                    return;
                }
            }
            
            Model.tags.Add(pRootTag);
            ChangeDisplayType(DisplayType);
        }

        public void Save()
        {
            if (DisplayType == GameplayTagsDisplayType.Tree)
            {
                List<GameplayTagNode> rootNodes = new List<GameplayTagNode>();
                foreach (GameplayTagNode_Element node in RootNodeElements)
                {
                    rootNodes.Add(node.Model);
                }
                Model = GameplayTagNode.GameplayNodesToTags(rootNodes);
            }
            GameplayTagsSetting.SaveTags(Model);
        }
    }
}


