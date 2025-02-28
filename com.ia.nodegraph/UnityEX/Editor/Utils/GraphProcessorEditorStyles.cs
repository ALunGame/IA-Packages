using UnityEngine;
using UnityEngine.UIElements;

namespace IANodeGraph.Editors
{
    public static class GraphProcessorEditorStyles
    {
        public static Styles DefaultStyles { get; private set; } = new Styles()
        {
            GraphWindowTree = Resources.Load<VisualTreeAsset>("UXML/GraphWindow"),

            BasicStyle = Resources.Load<StyleSheet>("USS/BasicStyle"),
            BaseGraphViewStyle = Resources.Load<StyleSheet>("USS/BaseGraphView"),
            BaseNodeViewStyle = Resources.Load<StyleSheet>("USS/BaseNodeView"),
            BaseSimpleNodeViewStyle = Resources.Load<StyleSheet>("USS/BaseSimpleNodeView"),

            BasePortViewStyle = Resources.Load<StyleSheet>("USS/BasePortView"),
            BasePortViewStyle_LeftInputContainer = Resources.Load<StyleSheet>("USS/BasePortView_LeftInputContainer"),
            BasePortViewStyle_RightInputContainer = Resources.Load<StyleSheet>("USS/BasePortView_RightInputContainer"),

            BaseConnectionViewStyle = Resources.Load<StyleSheet>("USS/BaseConnectionView"),
            GroupViewStyle = Resources.Load<StyleSheet>("USS/GroupView"),
            StickyNodeStyle = Resources.Load<StyleSheet>("USS/StickyNodeView"),
            StickyNoteStyle = Resources.Load<StyleSheet>("USS/StickyNoteView"),
        };

        public class Styles
        {
            public VisualTreeAsset GraphWindowTree;
            
            public StyleSheet BasicStyle;
            public StyleSheet BaseGraphViewStyle;
            public StyleSheet BaseNodeViewStyle;
            public StyleSheet BaseSimpleNodeViewStyle;

            public StyleSheet BasePortViewStyle;
            public StyleSheet BasePortViewStyle_LeftInputContainer;
            public StyleSheet BasePortViewStyle_RightInputContainer;

            public StyleSheet BaseConnectionViewStyle;
            public StyleSheet GroupViewStyle;
            public StyleSheet StickyNodeStyle;
            public StyleSheet StickyNoteStyle;
        }
    }
}