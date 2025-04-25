using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace IANodeGraph.Editors
{
    public static class GraphProcessorEditorStyles
    {
        public const string UXMLRootPath = "Packages/com.ia.nodegraph/UnityEX/Editor/UIAsset/UXML/";
        public const string USSRootPath = "Packages/com.ia.nodegraph/UnityEX/Editor/UIAsset/USS/";

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

        public static Styles DefaultStyles { get; private set; } = new Styles()
        {
            GraphWindowTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{UXMLRootPath}GraphWindow.uxml"),

            BasicStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{USSRootPath}BasicStyle.uss"),
            BaseGraphViewStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{USSRootPath}BaseGraphView.uss"),
            BaseNodeViewStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{USSRootPath}BaseNodeView.uss"),
            BaseSimpleNodeViewStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{USSRootPath}BaseSimpleNodeView.uss"),

            BasePortViewStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{USSRootPath}BasePortView.uss"),
            BasePortViewStyle_LeftInputContainer = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{USSRootPath}BasePortView_LeftInputContainer.uss"),
            BasePortViewStyle_RightInputContainer = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{USSRootPath}BasePortView_RightInputContainer.uss"),

            BaseConnectionViewStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{USSRootPath}BaseConnectionView.uss"),
            GroupViewStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{USSRootPath}GroupView.uss"),
            StickyNodeStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{USSRootPath}StickyNodeView.uss"),
            StickyNoteStyle = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{USSRootPath}StickyNoteView.uss"),
        };


        private static Dictionary<string, VisualTreeAsset> UXMLDict = null;
        public static VisualTreeAsset LoadUXML(string pUXMLName)
        {
            if (UXMLDict == null)
            {
                UXMLDict = new Dictionary<string, VisualTreeAsset>();
                string[] searchGuids = AssetDatabase.FindAssets("t:VisualTreeAsset", new string[] { $"{UXMLRootPath}" });
                for (int i = 0; i < searchGuids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(searchGuids[i]);
                    VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                    if (asset != null)
                    {
                        UXMLDict.Add(asset.name, asset);
                    }
                }
            }

            if (!UXMLDict.ContainsKey(pUXMLName))
            {
                return null;
            }
            return UXMLDict[pUXMLName];
        }

        private static Dictionary<string, StyleSheet> StyleDict = null;
        public static StyleSheet LoadStyle(string pStyleName)
        {
            if (StyleDict == null)
            {
                StyleDict = new Dictionary<string, StyleSheet>();
                string[] searchGuids = AssetDatabase.FindAssets("t:StyleSheet", new string[] { $"{USSRootPath}" });
                for (int i = 0; i < searchGuids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(searchGuids[i]);
                    StyleSheet asset = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                    if (asset != null)
                    {
                        StyleDict.Add(asset.name, asset);
                    }
                }
            }

            if (!StyleDict.ContainsKey(pStyleName))
            {
                return null;
            }
            return StyleDict[pStyleName];
        }
    }
}