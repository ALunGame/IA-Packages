using IANodeGraph.Model;
using IANodeGraph.View.Utils;
using IAToolkit;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using static IANodeGraph.Model.BasePort;

namespace IANodeGraph
{
    /// <summary> 
    /// 节点菜单，和自定义节点名 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeMenuItemAttribute : Attribute
    {
        /// <summary> 节点菜单路径 </summary>
        public string title;

        public NodeMenuItemAttribute(string title)
        {
            this.title = title;
        }
    }

    /// <summary>
    /// 节点字段，将会在节点中和Inspector显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class NodeValueAttribute : Attribute
    {
        public string Lable;
        public string Tooltip;

        public NodeValueAttribute(string lable, string tooltip = "")
        {
            Lable = lable;
            if (string.IsNullOrEmpty(""))
                Tooltip = lable;
            else
                Tooltip = tooltip;
        }
    }

    /// <summary>
    /// 输入端口
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InputPortAttribute : Attribute
    {
        public string name;
        public Orientation orientation;
        public Direction direction = Direction.Input;
        public Capacity capacity;
        public bool setIndex = false;

        /// <summary>
        /// 输入端口
        /// </summary>
        /// <param name="name">端口名</param>
        /// <param name="capacity">单个还是多个</param>
        /// <param name="orientation">水平还是竖直排列</param>
        /// <param name="setIndex">该端口设置索引</param>
        public InputPortAttribute(string name, Capacity capacity, Orientation orientation = Orientation.Horizontal)
        {
            this.name = name;
            this.capacity = capacity;
            this.orientation = orientation;
            if (capacity == Capacity.Multi)
            {
                this.setIndex = true;
            }
        }

        /// <summary>
        /// 输入端口
        /// </summary>
        /// <param name="name">端口名</param>
        /// <param name="capacity">单个还是多个</param>
        /// <param name="orientation">水平还是竖直排列</param>
        /// <param name="setIndex">该端口设置索引</param>
        public InputPortAttribute(string name, Capacity capacity, bool setIndex, Orientation orientation = Orientation.Horizontal)
        {
            this.name = name;
            this.capacity = capacity;
            this.orientation = orientation;
            this.setIndex = setIndex;
        }
    }

    /// <summary>
    /// 输出端口
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class OutputPortAttribute : Attribute
    {
        public string name;
        public Orientation orientation;
        public Direction direction = Direction.Output;
        public Capacity capacity;
        public bool setIndex = false;

        /// <summary>
        /// 输出端口
        /// </summary>
        /// <param name="name">端口名</param>
        /// <param name="capacity">单个还是多个</param>
        /// <param name="orientation">水平还是竖直排列</param>
        public OutputPortAttribute(string name, Capacity capacity, Orientation orientation = Orientation.Horizontal)
        {
            this.name = name;
            this.capacity = capacity;
            this.orientation = orientation;
            if (capacity == Capacity.Multi)
            {
                this.setIndex = true;
            }
        }

        /// <summary>
        /// 输出端口
        /// </summary>
        /// <param name="name">端口名</param>
        /// <param name="capacity">单个还是多个</param>
        /// <param name="orientation">水平还是竖直排列</param>
        /// <param name="setIndex">该端口设置索引</param>
        public OutputPortAttribute(string name, Capacity capacity, bool setIndex, Orientation orientation = Orientation.Horizontal)
        {
            this.name        = name;
            this.capacity    = capacity;
            this.orientation = orientation;
            this.setIndex    = setIndex;
        }
    }
}

namespace IANodeGraph.View
{
    public partial class BaseNodeView
    {
        protected virtual void OnInitialized() { }

        public virtual BasePortView NewPortView(BasePortVM port)
        {
            return new BasePortView(port, new EdgeConnectorListener(Owner));
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            foreach (var script in EditorExtension.FindAllScriptFromType(Model.GetType()))
            {
                evt.menu.AppendAction($"Open Script/" + script.name, _ => { AssetDatabase.OpenAsset(script); });
            }
            evt.menu.AppendSeparator();
        }

        public override void OnSelected()
        {
            base.OnSelected();
            BringToFront();
        }
    }

    public class BaseNodeView<M> : BaseNodeView where M : BaseNode
    {
        public M T_Model { get { return Model as M; } }
    }
}