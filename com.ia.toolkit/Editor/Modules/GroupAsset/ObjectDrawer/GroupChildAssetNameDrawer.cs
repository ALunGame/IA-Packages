using System.Collections.Generic;
using IAEngine;
using IAToolkit.Core;
using UnityEditor;
using UnityEngine;

namespace IAToolkit
{
    [CustomObjectDrawer(typeof(GroupChildAssetName))]
    internal class GroupChildAssetNameDrawer : ObjectDrawer
    {
        private object InspectorTargetObject;
        private object InspectorOwner;
        private GroupAssetTypeNameAttribute attr;
        private GroupPath groupPath;
        private List<InternalGroupAsset> groupAssets;
        
        public override void OnInit()
        {
            InspectorTargetObject = ObjectInspector.Instance.TargetObject;
            InspectorOwner = ObjectInspector.Instance.Owner;

            if (AttributeHelper.TryGetTypeAttribute<GroupAssetTypeNameAttribute>(Target.GetType(), out attr))
            {
                groupPath = GroupAssetSetting.Setting.GetSearchPath(attr.fullName);
                groupAssets = GroupAssetSetting.Setting.GetGroups(groupPath.searchPath);
            }
        }
        
        public override object OnGUI(Rect _position, GUIContent _label)
        {
            GroupChildAssetName geName = (GroupChildAssetName)Target;
            if (attr == null)
            {
                EditorGUILayout.LabelField("没有声明 GroupAssetTypeNameAttribute 属性！！！！！");
                return geName;
            }
            GUILayoutExtension.HorizontalGroup(() =>
            {
                EditorGUILayout.LabelField(_label);
                MiscHelper.Dropdown(geName.Name == "" ? "选择":geName.Name,GetAllNames(), (string selName) =>
                {
                    string[] tStr = selName.Split("/");
                    geName.Name = GroupChildAssetName.GetName(tStr[0],tStr[1]);
                },width:200);
                MiscHelper.Btn("打开", 50,GUIExtension.GetHeight(typeof(string),null), () =>
                {
                    foreach (InternalGroupAsset groupAsset in groupAssets)
                    {
                        foreach (GroupChildAsset childAsset in groupAsset.GetAllAssets())
                        {
                            if (GroupChildAssetName.GetName(groupAsset.name,childAsset.name) == geName.Name)
                            {
                                childAsset.Open(null, () =>
                                {
                                    InspectorExtension.DrawObjectInInspector(InspectorTargetObject,InspectorOwner);
                                });
                            }
                        }
                    }
                });
            });
            return geName;
        }

        public override float GetHeight()
        {
            return 0;
        }

        public List<string> GetAllNames()
        {
            List<string> geNames = new List<string>();

            foreach (InternalGroupAsset groupAsset in groupAssets)
            {
                foreach (GroupChildAsset childAsset in groupAsset.GetAllAssets())
                {
                    geNames.Add($"{groupAsset.name}/{childAsset.name}");
                }
            }

            return geNames;
        }
    }
}