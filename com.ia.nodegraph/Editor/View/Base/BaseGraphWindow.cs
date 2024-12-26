using IANodeGraph.Model;
using IANodeGraph.View.Utils;
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using IAToolkit.Command;
using IAToolkit;
using UnityObject = UnityEngine.Object;
using System.Collections.Generic;

namespace IANodeGraph.View
{
    public class BaseGraphWindow : BaseEditorWindow
    {
        #region 字段
        protected UnityObject graphAsset;
        protected bool locked = false;

        protected List<UnityObject> jumpAssets = new List<UnityObject> ();
        #endregion

        #region 属性
        public UnityObject GraphAsset
        {
            get { return graphAsset; }
            protected set { graphAsset = value; }
        }
        public BaseGraphVM Graph
        {
            get;
            private set;
        }
        public BaseGraphView GraphView
        {
            get; private set;
        }
        public ToolbarView Toolbar
        {
            get { return GraphViewParent?.Toolbar; }
        }
        public GraphViewParentElement GraphViewParent
        {
            get;
            private set;
        }
        public CommandDispatcher CommandDispatcher
        {
            get;
            protected set;
        }
        #endregion

        #region Unity
        protected virtual void OnEnable()
        {
            titleContent = new GUIContent("Graph Processor");
            rootVisualElement.styleSheets.Add(GraphProcessorStyles.BasicStyle);

            Reload();
        }

        protected virtual void OnDestroy()
        {
            Selection.activeObject = null;
        }
        #endregion

        #region 方法
        protected virtual void BuildToolbar(ToolbarView toolbar)
        {
            //查看所有节点
            ToolbarButton btnOverview = new ToolbarButton()
            {
                text = "Overview",
                tooltip = "查看所有节点"
            };
            btnOverview.clicked += () =>
            {
                GraphView.FrameAll();
            };
            toolbar.AddButtonToLeft(btnOverview);
            btnOverview.style.width = 80;

            //跳转视图
            if (jumpAssets.Count > 0)
            {
                IMGUIContainer jumpDrawName = new IMGUIContainer(() =>
                {
                    GUILayout.BeginHorizontal();
                    UnityObject jumpAsset = jumpAssets[0];
                    if (jumpAsset != null && GUILayout.Button(jumpAsset.name, EditorStyles.toolbarButton))
                    {
                        jumpAssets.Remove(jumpAsset);
                        Open(jumpAsset as IGraphAsset);
                    }
                    GUILayout.EndHorizontal();
                });
                jumpDrawName.style.flexGrow = 1;
                toolbar.AddToLeft(jumpDrawName);
            }
            

            //视图名
            IMGUIContainer drawName = new IMGUIContainer(() =>
            {
                GUILayout.BeginHorizontal();
                if (GraphAsset != null && GUILayout.Button(GraphAsset.name, EditorStyles.toolbarButton))
                {
                    EditorGUIUtility.PingObject(GraphAsset);
                }
                GUILayout.EndHorizontal();
            });
            drawName.style.flexGrow = 1;
            toolbar.AddToLeft(drawName);

            //重新加载
            ToolbarButton btnReload = new ToolbarButton()
            {
                text = "Reload",
                tooltip = "重新加载",
                style = { width = 70 }
            };
            btnReload.clicked += Reload;
            toolbar.AddButtonToRight(btnReload);

            //保存
            ToolbarButton btnSave = new ToolbarButton();
            btnSave.text = "Save";
            btnSave.clicked += Save;
            toolbar.AddButtonToRight(btnSave);
        }

        //按键事件
        protected virtual void KeyDownCallback(KeyDownEvent evt)
        {
            if (evt.commandKey || evt.ctrlKey)
            {
                switch (evt.keyCode)
                {
                    case KeyCode.Z:
                        GraphView.CommandDispacter.Undo();
                        evt.StopPropagation();
                        break;
                    case KeyCode.Y:
                        GraphView.CommandDispacter.Redo();
                        evt.StopPropagation();
                        break;
                    case KeyCode.D:
                        GraphView.DuplicateSelection();
                        evt.StopPropagation();
                        break;
                    case KeyCode.S:
                        Save();
                        evt.StopPropagation();
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual void Clear()
        {
            if (GraphViewParent != null)
            {
                GraphViewParent.RemoveFromHierarchy();
            }

            Graph = null;
            GraphView = null;
            GraphViewParent = null;
            GraphAsset = null;
            CommandDispatcher = null;
        }

        // 重新加载Graph
        public virtual void Reload()
        {
            if (GraphAsset is IGraphAsset graphAsset)
            {
                Load(graphAsset);
            }
        }

        protected void InternalLoad(BaseGraphVM graph, CommandDispatcher commandDispatcher)
        {
            graph.Enable();
            
            GraphView = NewGraphView(graph);
            if (GraphView == null)
                return;
            OnGraphViewUndirty();
            GraphView.SetUp(graph, this, commandDispatcher);
            GraphView.onDirty += OnGraphViewDirty;
            GraphView.onUndirty += OnGraphViewUndirty;
            Graph = graph;
            GraphViewParent = new GraphViewParentElement();
            GraphViewParent.StretchToParentSize();
            rootVisualElement.Add(GraphViewParent);
            GraphView.RegisterCallback<KeyDownEvent>(KeyDownCallback);
            GraphViewParent.GraphViewElement.Add(GraphView);

            BuildToolbar(GraphViewParent.Toolbar);
        }

        // 从Graph资源加载
        public void Load(IGraphAsset graphAsset)
        {
            Clear();

            GraphAsset = graphAsset as UnityObject;
            CommandDispatcher = new CommandDispatcher();

            BaseGraph baseGraph = graphAsset.DeserializeGraph();
            BaseGraphVM graphVM = ViewModelFactory.CreateViewModel(baseGraph) as BaseGraphVM;
            InternalLoad(graphVM, CommandDispatcher);
        }

        // 保存
        void Save()
        {
            try
            {
                if (GraphAsset is IGraphAsset graphAsset)
                    graphAsset.SaveGraph(Graph.Model);
                GraphView.SetDirty();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                GraphView.SetUndirty();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Save>>>>{0}", e);
            }

        }

        public void OnGraphViewDirty()
        {
            if (!titleContent.text.EndsWith(" *"))
                titleContent.text += " *";
            if (GraphAsset != null)
                EditorUtility.SetDirty(GraphAsset);
        }

        public void OnGraphViewUndirty()
        {
            if (titleContent.text.EndsWith(" *"))
                titleContent.text = titleContent.text.Replace(" *", "");
        }
        #endregion

        #region Overrides
        protected virtual BaseGraphView NewGraphView(BaseGraphVM graph)
        {
            return new BaseGraphView();
        }
        #endregion

        #region Static
        /// <summary> 从Graph类型获取对应的GraphWindow </summary>
        public static BaseGraphWindow GetGraphWindow(Type graphType)
        {
            var windowType = GraphProcessorEditorUtility.GetGraphWindowType(graphType);
            UnityObject[] objs = Resources.FindObjectsOfTypeAll(windowType);
            BaseGraphWindow window = null;
            foreach (var obj in objs)
            {
                if (obj.GetType() == windowType)
                {
                    window = obj as BaseGraphWindow;
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
            if (graphAsset == null) return null;
            var window = GetGraphWindow(graphAsset.GraphType);
            window.Load(graphAsset);
            return window;
        }

        /// <summary>
        /// 跳转视图
        /// </summary>
        /// <param name="graphAsset"></param>
        /// <returns></returns>
        public static BaseGraphWindow JumpTo(IGraphAsset graphAsset)
        {
            var window = GetGraphWindow(graphAsset.GraphType);
            window.jumpAssets.Add(window.GraphAsset);
            window.Load(graphAsset);
            return window;
        }

        /// <summary> 双击资源 </summary>
        [OnOpenAsset(0)]
        public static bool OnOpen(int instanceID, int line)
        {
            UnityObject go = EditorUtility.InstanceIDToObject(instanceID);
            if (go == null) return false;
            IGraphAsset graphAsset = go as IGraphAsset;
            if (graphAsset == null)
                return false;
            Open(graphAsset);
            return true;
        }
        #endregion
    }
}