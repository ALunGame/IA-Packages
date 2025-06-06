using IANodeGraph.Editors;
using IANodeGraph.Model;
using IANodeGraph.Model.Internal;
using IAToolkit.Command;
using IAToolkit.Element;
using IAToolkit.UnityEditors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using IAGAS.Editors;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace IANodeGraph.View
{
    public class BaseGraphDisplayInfo
    {
        public IGraphAsset graphAsset;
        public BaseGraphProcessor graphProcessor;
        public CommandDispatcher commandDispatcher;

        public BaseGraphView graphView;
        public IVisualElementScheduledItem graphViewScheduledItem;

        public BaseGraphDisplayInfo(IGraphAsset pGraphAsset)
        {
            graphAsset = pGraphAsset;
            graphProcessor = ViewModelFactory.ProduceViewModel(graphAsset.LoadGraph()) as BaseGraphProcessor;
            commandDispatcher = new CommandDispatcher();
        }

        public void Show()
        {
            graphViewScheduledItem?.Resume();
        }

        public void Hide()
        {
            graphViewScheduledItem?.Pause();
        }
    }

    public abstract class BaseGraphWindow : BaseEditorWindow
    {
        #region Fields

        private Toolbar toolbarLeft;
        private Toolbar toolbarCenter;
        private Toolbar toolbarRight;
        private VisualElement graphViewContainer;
        private VisualElement inspectorView;

        private BaseGraphDisplayInfo currGraphDisplayInfo;
        private Stack<BaseGraphDisplayInfo> graphDisplayInfoStack = new Stack<BaseGraphDisplayInfo>();

        #endregion

        #region Properties

        private VisualElement GraphViewContainer => graphViewContainer;
        private VisualElement InspectorView => inspectorView;

        public Toolbar ToolbarLeft => toolbarLeft;

        public Toolbar ToolbarCenter => toolbarCenter;

        public Toolbar ToolbarRight => toolbarRight;

        public IGraphAsset GraphAsset => currGraphDisplayInfo.graphAsset;

        public BaseGraphProcessor GraphProcessor => currGraphDisplayInfo.graphProcessor;

        public CommandDispatcher CommandDispatcher => currGraphDisplayInfo.commandDispatcher;

        public BaseGraphView GraphView => currGraphDisplayInfo.graphView;

        #endregion

        #region Unity

        protected virtual void OnEnable()
        {
            titleContent = new GUIContent("Graph Processor");
            InitRootVisualElement();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            Reload();
        }

        protected virtual void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            if (Selection.activeObject is ObjectInspector objectInspector && objectInspector.target is GraphElement)
                Selection.activeObject = null;
            Clear();
        }

        public override void SaveChanges()
        {
            this.OnBtnSaveClick();
        }

        #endregion

        #region Private Methods

        protected virtual void InitRootVisualElement()
        {
            GraphProcessorEditorStyles.DefaultStyles.GraphWindowTree.CloneTree(rootVisualElement);
            rootVisualElement.name = "rootVisualContainer";
            rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDownCallback, TrickleDown.TrickleDown);
            rootVisualElement.styleSheets.Add(GraphProcessorEditorStyles.DefaultStyles.BasicStyle);

            toolbarLeft = rootVisualElement.Q<Toolbar>("ToolbarLeft", "unity-toolbar");
            toolbarCenter = rootVisualElement.Q<Toolbar>("ToolbarCenter", "unity-toolbar");
            toolbarRight = rootVisualElement.Q<Toolbar>("ToolbarRight", "unity-toolbar");
            graphViewContainer = rootVisualElement.Q("GraphViewContainer");
            inspectorView = rootVisualElement.Q("InspectorView");
        }

        protected virtual void BeforeLoad(BaseGraphProcessor graph, IGraphAsset graphAsset)
        {
        }

        protected void Load(BaseGraphDisplayInfo pGraphDisplayInfo)
        {
            Clear();

            currGraphDisplayInfo = pGraphDisplayInfo;

            BeforeLoad(pGraphDisplayInfo.graphProcessor, pGraphDisplayInfo.graphAsset);

            //复用视图
            if (currGraphDisplayInfo.graphView != null)
            {
                this.GraphViewContainer.Add(currGraphDisplayInfo.graphView);
                this.currGraphDisplayInfo.Show();
            }
            else
            {
                BaseGraphView newGraphView = NewGraphView();
                currGraphDisplayInfo.graphView = newGraphView;

                newGraphView.SetUp(GraphProcessor, new GraphViewContext() { window = this, commandDispatcher = currGraphDisplayInfo.commandDispatcher });
                newGraphView.Init();
                this.GraphViewContainer.Add(newGraphView);

                currGraphDisplayInfo.graphViewScheduledItem = newGraphView.schedule.Execute(() =>
                {
                    foreach (var pair in newGraphView.NodeViews)
                    {
                        if (!newGraphView.worldBound.Overlaps(pair.Value.worldBound))
                        {
                            pair.Value.controls.visible = false;
                        }
                        else
                        {
                            pair.Value.controls.visible = true;
                        }
                    }
                }).Every(50);

                AfterLoad();
            }

            BuildToolBar();

            GraphProcessorEditorSettings.MiniMapActive.onValueChanged += OnMiniMapActiveChanged;
            OnMiniMapActiveChanged(GraphProcessorEditorSettings.MiniMapActive.Value);
        }

        protected virtual void AfterLoad()
        {
        }

        protected void OnMiniMapActiveChanged(bool newValue)
        {
            currGraphDisplayInfo.graphView.MiniMapActive = newValue;
        }

        #endregion

        #region Public Methods

        public virtual void Clear()
        {
            ToolbarLeft.Clear();
            ToolbarCenter.Clear();
            ToolbarRight.Clear();
            GraphViewContainer.Clear();

            //graphProcessor = null;
            //graphView = null;
            //graphAsset = null;
            //commandDispatcher = null;
            currGraphDisplayInfo = null;

            GraphProcessorEditorSettings.MiniMapActive.onValueChanged -= OnMiniMapActiveChanged;

            this.SetHasUnsavedChanges(false);
        }

        // 重新加载Graph
        public virtual void Reload()
        {
            if (currGraphDisplayInfo != null)
            {
                Load(currGraphDisplayInfo);
            }
            else
            {
                //Debug.LogError($"Reload失败，没有GraphAsset");
            }
        }

        // 从Graph资源加载
        public void LoadFromGraphAsset(IGraphAsset graphAsset)
        {
            BaseGraphDisplayInfo graphDisplayInfo = new BaseGraphDisplayInfo(graphAsset);
            graphDisplayInfoStack.Push(graphDisplayInfo);
            Load(graphDisplayInfo);
        }

        public void SetHasUnsavedChanges(bool value)
        {
            this.hasUnsavedChanges = value;
        }

        #endregion

        #region Callbacks

        void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            switch (obj)
            {
                case PlayModeStateChange.EnteredEditMode:
                    Reload();
                    break;
            }
        }

        #endregion

        #region Overrides

        protected virtual BaseGraphView NewGraphView()
        {
            Type graphViewType = GraphProcessorEditorUtil.GetViewType(currGraphDisplayInfo.graphProcessor.ModelType);
            if (graphViewType == null)
            {
                return new DefaultGraphView();
            }
            return Activator.CreateInstance(GraphProcessorEditorUtil.GetViewType(currGraphDisplayInfo.graphProcessor.ModelType)) as BaseGraphView;
        }

        protected virtual void OnKeyDownCallback(KeyDownEvent evt)
        {
            if (evt.commandKey || evt.ctrlKey)
            {
                switch (evt.keyCode)
                {
                    case KeyCode.Z:
                        CommandDispatcher.Undo();
                        evt.StopPropagation();
                        break;
                    case KeyCode.Y:
                        CommandDispatcher.Redo();
                        evt.StopPropagation();
                        break;
                    case KeyCode.S:
                        OnBtnSaveClick();
                        evt.StopImmediatePropagation();
                        break;
                    case KeyCode.D:
                        OnBtnCopyClick();
                        evt.StopImmediatePropagation();
                        break;
                }
            }
        }

        protected virtual void OnBtnCopyClick()
        {
            BaseGraphView currView = currGraphDisplayInfo.graphView;
            List<BaseNode> nodes = new List<BaseNode>();

            for (int i = 0; i < currView.selection.Count; i++)
            {
                var selItem = currView.selection[i];
                if (selItem is BaseNodeView)
                {
                    nodes.Add(((BaseNodeView)selItem).ViewModel.Model);
                }
            }

            int startId = currView.ViewModel.NewID();
            //计算连接
            Dictionary<int, BaseNode> nodeDict = new Dictionary<int, BaseNode>();
            Vector2Int offsetPos = new Vector2Int(20, 100);
            for (int i = 0; i < nodes.Count; i++)
            {
                var oldNode = nodes[i];
                var jsonStr = JsonConvert.SerializeObject(oldNode, GraphProcessorEditorUtil.JsonSetting);
                var newNode = JsonConvert.DeserializeObject(jsonStr, oldNode.GetType(), GraphProcessorEditorUtil.JsonSetting);
                ((BaseNode)newNode).position += offsetPos;
                ((BaseNode)newNode).id = startId;
                startId++;
                nodeDict.Add(oldNode.id, ((BaseNode)newNode));
            }

            List<BaseConnection> oldConnections = currView.ViewModel.Model.connections;

            List<BaseConnection> newConnections = new List<BaseConnection>();
            foreach (var connection in oldConnections)
            {
                if (nodeDict.ContainsKey(connection.fromNode) && nodeDict.ContainsKey(connection.toNode))
                {
                    BaseConnection newConnect = new BaseConnection();
                    newConnect.fromNode = connection.fromNode;
                    newConnect.fromPort = connection.fromPort;

                    newConnect.toNode = connection.toNode;
                    newConnect.toPort = connection.toPort;

                    newConnections.Add(newConnect); 
                }
            }

            foreach (var connection in newConnections)
            {
                connection.fromNode = nodeDict[connection.fromNode].id;
                connection.toNode = nodeDict[connection.toNode].id;
            }

            //最后执行添加操作
            currView.CommandDispatcher.Do(new CopyCommand(GraphProcessor, nodeDict.Values.ToList(), newConnections));
        }

        protected virtual void OnBtnSaveClick()
        {
            if (GraphAsset is IGraphAsset graphSerialization)
                graphSerialization.SaveGraph(GraphProcessor.Model);

            if (GraphAsset is UnityObject uo)
                EditorUtility.SetDirty(uo);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            this.hasUnsavedChanges = false;
        }

        protected virtual void BuildToolBar()
        {
            var btnOverview = new ToolbarButton()
            {
                name = "btnOverview",
                text = "Overview",
                tooltip = "查看所有节点",
            };
            btnOverview.clicked += () => { GraphView.FrameAll(); };
            ToolbarLeft.Add(btnOverview);

            var togMiniMap = new ToolbarButton()
            {
                name = "togMiniMap",
                text = "MiniMap",
                tooltip = "小地图",
            };
            togMiniMap.clicked += () => { GraphProcessorEditorSettings.MiniMapActive.Value = !GraphProcessorEditorSettings.MiniMapActive.Value; };
            ToolbarLeft.Add(togMiniMap);
            
            if (currGraphDisplayInfo != null)
            {
                CreateGraphStackItemViews();
            }

            var btnReload = new ToolbarButton()
            {
                name = "btnReload",
                text = "Reload",
                tooltip = "重新加载",
                style =
                {
                    width = 80,
                    // backgroundImage = EditorGUIUtility.FindTexture("Refresh"),
                }
            };
            btnReload.clicked += () => { Reload(); };
            ToolbarRight.Add(btnReload);

            var btnSave = new ToolbarButton()
            {
                name = "btnSave",
                text = "Save",
                tooltip = "保存",
                style =
                {
                    width = 80,
                    // backgroundImage = EditorGUIUtility.FindTexture("SaveActive"),
                }
            };
            btnSave.clicked += OnBtnSaveClick;
            ToolbarRight.Add(btnSave);
        }
        
        /// <summary>
        /// 通过根节点加载视图，提供跳转的方法
        /// </summary>
        /// <param name="pAssetType"></param>
        /// <param name="outAsset"></param>
        /// <returns></returns>
        public virtual bool LoadGraphAsset(AssetType pAssetType, GraphRootNodeInfo pRootNodeInfo, out InternalBaseGraphAsset outAsset)
        {
            outAsset = null;
            return outAsset != null;
        }

        private  GUIStyle BreadCrumbLeft = "GUIEditor.BreadcrumbLeft";
        private  GUIStyle BreadCrumbMid = "GUIEditor.BreadcrumbMid";
        private  GUIStyle BreadCrumbLeftBg = "GUIEditor.BreadcrumbLeftBackground";
        private  GUIStyle BreadCrumbMidBg = "GUIEditor.BreadcrumbMidBackground";
        private void CreateGraphStackItemViews()
        {
            ToolbarCenter.Add(new IMGUIContainer(() =>
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);

                BaseGraphDisplayInfo[] displayInfos = graphDisplayInfoStack.ToArray();
                for (int i = displayInfos.Length - 1; i >= 0; i--)
                {
                    var displayInfo = displayInfos[i];
                    var asset = displayInfo.graphAsset as InternalBaseGraphAsset;

                    GUIStyle style1 = i == displayInfos.Length - 1 ? BreadCrumbLeft : BreadCrumbMid;
                    GUIStyle style2 = i == displayInfos.Length - 1 ? BreadCrumbLeftBg : BreadCrumbMidBg;

                    GUIContent guiContent = new GUIContent($"{asset.name}");
                    Rect rect = GetLayoutRect(guiContent, style1);
                    rect.position += new Vector2(1, 0);
                    rect.size += new Vector2(10, 5);

                    if (Event.current.type == EventType.Repaint)
                        style2.Draw(rect, GUIContent.none, 0);

                    if (GUI.Button(rect, guiContent, style1))
                    {
                        if (displayInfo.graphAsset != GraphAsset)
                            OnBtnGraphStackClick(displayInfo);
                    }
                }

                GUILayout.EndHorizontal();
            }));
        }

        private Rect GetLayoutRect(GUIContent content, GUIStyle style)
        {
            Texture image = content.image;
            content.image = null;

            Vector2 vector = style.CalcSize(content);
            content.image = image;

            if (image != null) 
                vector.x += vector.y;
            GUILayoutOption[] options = { GUILayout.MaxWidth(vector.x), GUILayout.MaxHeight(vector.y) };
            return GUILayoutUtility.GetRect(content, style, options);
        }

        protected virtual void OnBtnGraphStackClick(BaseGraphDisplayInfo pDisplayInfo)
        {
            int clickIndex = 0;

            BaseGraphDisplayInfo[] displayInfos = graphDisplayInfoStack.ToArray();

            for (int i = 0; i < displayInfos.Length; i++)
            {
                var tInfo = displayInfos[i];
                if (tInfo.Equals(pDisplayInfo))
                {
                    clickIndex = i;
                    break;
                }
            }

            graphDisplayInfoStack.Clear();
            for (int i = displayInfos.Length - 1; i >= clickIndex; i--)
            {
                graphDisplayInfoStack.Push(displayInfos[i]);
            }

            Load(pDisplayInfo);
        }

        #endregion

        #region Static

        /// <summary> 从Graph类型获取对应的GraphWindow </summary>
        public static BaseGraphWindow GetGraphWindow(Type graphType)
        {
            var windowType = GraphProcessorEditorUtil.GetWindowType(graphType);
            var windows = Resources.FindObjectsOfTypeAll(windowType);
            BaseGraphWindow window = null;
            foreach (var _window in windows)
            {
                if (_window.GetType() == windowType)
                {
                    window = _window as BaseGraphWindow;
                    break;
                }
            }

            if (window == null)
            {
                window = GetWindow(windowType) as BaseGraphWindow;
            }

            window.Focus();
            return window;
        }

        /// <summary> 从GraphAsset打开Graph </summary>
        public static BaseGraphWindow Open(IGraphAsset graphAsset)
        {
            var window = GetGraphWindow(graphAsset.GraphType);
            window.LoadFromGraphAsset(graphAsset);
            return window;
        }

        /// <summary> 双击资源 </summary>
        [OnOpenAsset(0)]
        public static bool OnOpen(int instanceID, int line)
        {
            UnityObject go = EditorUtility.InstanceIDToObject(instanceID);
            if (go == null)
                return false;
            IGraphAsset graphAsset = go as IGraphAsset;
            if (graphAsset == null)
                return false;
            Open(graphAsset);

            return true;
        }

        #endregion
    }
}