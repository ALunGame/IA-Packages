using IANodeGraph.Model;
using IANodeGraph.Model.Internal;
using IANodeGraph.View;
using IAToolkit;
using UnityEditor;
using UnityEngine;

namespace IANodeGraph.Inspector
{
    [CustomEditor(typeof(InternalBaseGraphAsset), true)]
    public class BaseGraphAssetInspector : Editor
    {
        static GUIHelper.ContextDataCache ContextDataCache = new GUIHelper.ContextDataCache();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!ContextDataCache.TryGetContextData<GUIStyle>("BigLabel", out var bigLabel))
            {
                bigLabel.value = new GUIStyle(GUI.skin.label);
                bigLabel.value.fontSize = 18;
                bigLabel.value.fontStyle = FontStyle.Bold;
                bigLabel.value.alignment = TextAnchor.MiddleLeft;
                bigLabel.value.stretchWidth = true;
            }

            IGraphAsset graphAsset = target as IGraphAsset;

            if (GUILayout.Button("Open", GUILayout.Height(30)))
                BaseGraphWindow.Open(target as InternalBaseGraphAsset);

            if (GUILayout.Button("复制序列化数据到剪切板", GUILayout.Height(30)))
            {
                GUIUtility.systemCopyBuffer = graphAsset.GetSerializedStr();
            }
            
            if (GUILayout.Button("清空序列化数据", GUILayout.Height(30)))
            {
                graphAsset.ClearSerializedStr();
                EditorUtility.SetDirty(target as InternalBaseGraphAsset);
                AssetDatabase.SaveAssetIfDirty(target as InternalBaseGraphAsset);
            }
        }
    }
}
