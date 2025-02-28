using IANodeGraph.Editors;
using IANodeGraph.Model;
using IAToolkit.Command;
using IAToolkit.UnityEditors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityObject = UnityEngine.Object;

namespace IANodeGraph.View
{
    public abstract class BaseGraphWindow : BaseEditorWindow
    {
        #region Fields

        private Toolbar toolbarLeft;
        private Toolbar toolbarCenter;
        private Toolbar toolbarRight;
        private VisualElement graphViewContainer;
        private VisualElement inspectorView;

        [SerializeField] private UnityObject unityGraphAsset;
        private IGraphAsset graphAsset;
        private BaseGraphProcessor graphProcessor;
        private BaseGraphView graphView;
        private CommandDispatcher commandDispatcher;

        #endregion

        #region Properties

        private VisualElement GraphViewContainer => graphViewContainer;
        private VisualElement InspectorView => inspectorView;

        public Toolbar ToolbarLeft => toolbarLeft;

        public Toolbar ToolbarCenter => toolbarCenter;

        public Toolbar ToolbarRight => toolbarRight;

        public IGraphAsset GraphAsset
        {
            get { return graphAsset; }
            protected set
            {
                graphAsset = value;
                unityGraphAsset = graphAsset as UnityObject;
            }
        }

        public BaseGraphProcessor GraphProcessor => graphProcessor;

        public CommandDispatcher CommandDispatcher => commandDispatcher;

        public BaseGraphView GraphView => graphView;

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

        protected void Load(BaseGraphProcessor graph, IGraphAsset graphAsset)
        {
            Clear();

            BeforeLoad(graph, graphAsset);

            this.commandDispatcher = new CommandDispatcher();
            this.graphProcessor = graph;
            this.GraphAsset = graphAsset;

            this.graphView = NewGraphView();
            this.graphView.SetUp(GraphProcessor, new GraphViewContext() { window = this, commandDispatcher = commandDispatcher });
            this.graphView.Init();
            this.GraphViewContainer.Add(graphView);

            graphView.schedule.Execute(() =>
            {
                foreach (var pair in graphView.NodeViews)
                {
                    if (!graphView.worldBound.Overlaps(pair.Value.worldBound))
                    {
                        pair.Value.controls.visible = false;
                    }
                    else
                    {
                        pair.Value.controls.visible = true;
                    }
                }
            }).Every(50);
            BuildToolBar();

            GraphProcessorEditorSettings.MiniMapActive.onValueChanged += OnMiniMapActiveChanged;
            OnMiniMapActiveChanged(GraphProcessorEditorSettings.MiniMapActive.Value);

            AfterLoad();
        }

        protected virtual void AfterLoad()
        {
        }

        protected void OnMiniMapActiveChanged(bool newValue)
        {
            graphView.MiniMapActive = newValue;
        }

        #endregion

        #region Public Methods

        public virtual void Clear()
        {
            ToolbarLeft.Clear();
            ToolbarCenter.Clear();
            ToolbarRight.Clear();
            GraphViewContainer.Clear();

            graphProcessor = null;
            graphView = null;
            GraphAsset = null;
            commandDispatcher = null;

            GraphProcessorEditorSettings.MiniMapActive.onValueChanged -= OnMiniMapActiveChanged;

            this.SetHasUnsavedChanges(false);
        }

        // 重新加载Graph
        public virtual void Reload()
        {
            if (graphAsset != null)
            {
                LoadFromGraphAsset(graphAsset);
            }
            else if (GraphProcessor is BaseGraphProcessor graphVM)
            {
                LoadFromGraphVM(graphVM);
            }
            else if (this.unityGraphAsset != null)
            {
                AssetDatabase.OpenAsset(this.unityGraphAsset);
            }
        }

        // 从Graph资源加载
        public void LoadFromGraphAsset(IGraphAsset graphAsset)
        {
            Load(ViewModelFactory.ProduceViewModel(graphAsset.LoadGraph()) as BaseGraphProcessor, graphAsset);
        }

        // 直接加载GraphVM对象
        public void LoadFromGraphVM(BaseGraphProcessor graph)
        {
            Load(graph, null);
        }

        // 直接加载Graph对象
        public void LoadFromGraph(BaseGraph graph)
        {
            Load(ViewModelFactory.ProduceViewModel(graph) as BaseGraphProcessor, null);
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
            return new DefaultGraphView();
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
            List<BaseNode> nodes = new List<BaseNode>();

            for (int i = 0; i < graphView.selection.Count; i++)
            {
                var selItem = graphView.selection[i];
                if (selItem is BaseNodeView)
                {
                    nodes.Add(((BaseNodeView)selItem).ViewModel.Model);
                }
            }

            int startId = graphView.ViewModel.NewID();
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

            List<BaseConnection> oldConnections = graphView.ViewModel.Model.connections;

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
            graphView.CommandDispatcher.Do(new CopyCommand(GraphProcessor, nodeDict.Values.ToList(), newConnections));
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

            if (graphAsset != null && graphAsset is UnityObject)
            {
                IMGUIContainer drawName = new IMGUIContainer(() =>
                {
                    GUILayout.BeginHorizontal();

                    if (unityGraphAsset != null)
                    {
                        EditorGUILayout.ObjectField(unityGraphAsset, typeof(UnityObject), true, GUILayout.Height(25));
                    }

                    GUILayout.Space(2);
                    GUILayout.EndHorizontal();
                });
                drawName.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
                drawName.style.flexGrow = 1;
                ToolbarCenter.Add(drawName);
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

        /// <summary> 打开Graph </summary>
        public static BaseGraphWindow Open(BaseGraphProcessor graph)
        {
            var window = GetGraphWindow(graph.ModelType);
            window.LoadFromGraphVM(graph);
            return window;
        }

        /// <summary> 打开Graph </summary>
        public static BaseGraphWindow Open(BaseGraph graph)
        {
            var window = GetGraphWindow(graph.GetType());
            window.LoadFromGraph(graph);
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