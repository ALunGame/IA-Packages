using System;
using IANodeGraph.Model;
using UnityEngine;

namespace IANodeGraph
{
    /// <summary> 节点菜单，和自定义节点名 </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeMenuItemAttribute : Attribute
    {
        /// <summary> 节点路径 </summary>
        public string path;
        /// <summary> 是否要显示在节点菜单中 </summary>
        public bool hidden;

        public NodeMenuItemAttribute(string path)
        {
            this.path = path;
        }
    }

    /// <summary> 节点标题 </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeTitleAttribute : Attribute
    {
        /// <summary> 节点标题名称 </summary>
        public string title;

        public NodeTitleAttribute(string title)
        {
            this.title = title;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeTitleColorAttribute : Attribute
    {
        public readonly Color color;

        public NodeTitleColorAttribute(float r, float g, float b)
        {
            color = new Color(r, g, b, 1);
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NodeTooltipAttribute : Attribute
    {
        public readonly string Tooltip;

        public NodeTooltipAttribute(string tooltip)
        {
            Tooltip = tooltip;
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
        public PortDirection direction;
        public bool showDrawer;
        public Type portType;

        public NodeValueAttribute(string lable, bool showDrawer = false, PortDirection direction = PortDirection.Left, 
            string tooltip = "")
        {
            Lable = lable;
            if (string.IsNullOrEmpty(tooltip))
                Tooltip = lable;
            else
                Tooltip = tooltip;
            this.direction = direction;
            this.showDrawer = showDrawer;
        }
        
        public NodeValueAttribute(string lable, Type portType, PortDirection direction = PortDirection.Left, 
            string tooltip = "")
        {
            Lable = lable;
            if (string.IsNullOrEmpty(tooltip))
                Tooltip = lable;
            else
                Tooltip = tooltip;
            this.direction = direction;
            this.showDrawer = false;
            this.portType = portType;
        }
    }
}