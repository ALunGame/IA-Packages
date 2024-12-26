using UnityEditor;
using UnityEngine.UIElements;

namespace IANodeGraph.View.Utils
{
    /// <summary>
    /// 视图样式
    /// </summary>
    public static class GraphProcessorStyles
    {
        private const string StyleFileRootPath = "Assets/com.lc.node/Editor/Resources/Style/";

        public const string BassicStyleFile = StyleFileRootPath + "BasicStyle.uss";
        public const string GraphViewStyleFile = StyleFileRootPath + "BaseGraphView.uss";
        
        public const string BaseNodeViewStyleFile = StyleFileRootPath + "BaseNodeView.uss";
        public const string SimpleNodeViewStyleFile = StyleFileRootPath + "BaseSimpleNodeView.uss";

        public const string PortViewStyleFile = StyleFileRootPath + "BasePortView.uss";
        public const string PortViewTypesStyleFile = StyleFileRootPath + "PortViewTypes.uss";
        
        public const string EdgeStyleFile = StyleFileRootPath + "BaseEdgeView.uss";
        
        public const string GroupStyleFile = StyleFileRootPath + "BaseGroupView.uss";
        
        public const string NodeSettingViewStyleFile = StyleFileRootPath + "NodeSettings.uss";

        public const string SettingNodeViewStyleFile = StyleFileRootPath + "SettingsNodeView.uss";
        public const string RelayNodeViewStyleFile = StyleFileRootPath + "RelayNode.uss";

        public const string NodeSettingsTreeFile = "Assets/NodeGraph/Editor/Resources/UXML/NodeSettings.uxml";

        
        

        public static StyleSheet BasicStyle { get; } = AssetDatabase.LoadAssetAtPath<StyleSheet>(BassicStyleFile);
        public static StyleSheet GraphViewStyle { get; } = AssetDatabase.LoadAssetAtPath<StyleSheet>(GraphViewStyleFile);
        public static StyleSheet BaseNodeViewStyle { get; } = AssetDatabase.LoadAssetAtPath<StyleSheet>(BaseNodeViewStyleFile);
        public static StyleSheet BaseGroupViewStyle { get; } = AssetDatabase.LoadAssetAtPath<StyleSheet>(GroupStyleFile);
        public static StyleSheet SimpleNodeViewStyle { get; } = AssetDatabase.LoadAssetAtPath<StyleSheet>(SimpleNodeViewStyleFile);
        public static StyleSheet SettingsNodeViewStyle { get; } = AssetDatabase.LoadAssetAtPath<StyleSheet>(SettingNodeViewStyleFile);
        public static StyleSheet PortViewStyle { get; } = AssetDatabase.LoadAssetAtPath<StyleSheet>(PortViewStyleFile);
        public static StyleSheet PortViewTypesStyle { get; } = AssetDatabase.LoadAssetAtPath<StyleSheet>(PortViewTypesStyleFile);
        public static StyleSheet EdgeViewStyle { get; } = AssetDatabase.LoadAssetAtPath<StyleSheet>(EdgeStyleFile);
        public static StyleSheet NodeSettingsViewStyle { get; } = AssetDatabase.LoadAssetAtPath<StyleSheet>(NodeSettingViewStyleFile);
        public static VisualTreeAsset NodeSettingsViewTree { get; } = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(NodeSettingsTreeFile);
        public static StyleSheet RelayNodeViewStyle { get; } = AssetDatabase.LoadAssetAtPath<StyleSheet>(RelayNodeViewStyleFile);
    }
}
