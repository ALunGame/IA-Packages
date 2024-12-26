using UnityEditor;

namespace IAToolkit
{
    [CustomEditor(typeof(GameplayTagCom), true)]
    public class GameplayTagComInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GameplayTagCom tagCom = target as GameplayTagCom;

            tagCom.tags = GUILayoutExtension.DrawField(typeof(GameplayTagContainer), tagCom.tags, GUIHelper.TextContent("标签")) as GameplayTagContainer;
        }
    }
}