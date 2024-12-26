using System;
using System.IO;
using IAEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace IAToolkit
{
    public abstract class GroupChildAsset : ScriptableObject
    {
        public virtual void Open(object pOwner = null, Action _clickBackFunc = null)
        {
            Selection.activeObject = this;
        }

        public GroupPath GetGroupPath()
        {
            foreach (GroupPath path in GroupAssetSetting.Setting.groupPaths)
            {
                if (path.typeChildFullName == this.GetType().FullName)
                {
                    return path;
                }
            }
            
            return null;
        }
        
        public virtual string FileName()
        {
            GroupPath groupPath = GetGroupPath();
            string fileName = string.Format(groupPath.exportFileName,name);
            return fileName;
        }

        /// <summary>
        /// 导出
        /// </summary>
        public virtual void Export(object pData, InternalGroupAsset pGroupAsset = null)
        {
            GroupPath groupPath = GetGroupPath();
            
            string fileName = FileName() + "." + groupPath.exportExName;
            string filePath = Path.Combine(groupPath.exportPath, fileName);
            string txt = ExportAsset(pData);
            
            IOHelper.WriteText(txt,filePath);
            Debug.Log($"导出成功：{groupPath.typeName}->{filePath}");
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获得导出路径
        /// </summary>
        /// <returns></returns>
        public virtual string GetExportFilePath()
        {
            GroupPath groupPath = GetGroupPath();
            
            string fileName = FileName() + "." + groupPath.exportExName;
            string filePath = Path.Combine(groupPath.exportPath, fileName);
            return filePath;
        }

        public virtual string ExportAsset(object pData)
        {
            return JsonMapper.ToJson(pData);
        }

        /// <summary> 双击资源 </summary>
        [OnOpenAsset(0)]
        public static bool OnOpen(int instanceID, int line)
        {
            UnityObject go = EditorUtility.InstanceIDToObject(instanceID);
            if (go == null) return false;
            GroupChildAsset asset = go as GroupChildAsset;
            if (asset == null)
                return false;
            asset.Open();
            return true;
        }
    }
}