﻿using IANodeGraph.Model;

namespace IANodeGraph
{
    public static class ConstValues
    {
        public const string FLOW_IN_PORT_NAME = "FLOW_IN_PORT_NAME";
        public const string FLOW_OUT_PORT_NAME = "FLOW_OUT_PORT_NAME";
        
        public const string NODE_TITLE_NAME = nameof(BaseNodeProcessor.Title);
        public const string NODE_TITLE_COLOR_NAME = nameof(BaseNodeProcessor.TitleColor);
        public const string NODE_TOOLTIP_NAME = nameof(BaseNodeProcessor.Tooltip);
    }
}