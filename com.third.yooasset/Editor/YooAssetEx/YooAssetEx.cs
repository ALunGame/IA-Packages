using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine;

namespace YooAsset.Editor
{
    [DisplayName("忽略指定类型资源")]
    public class CollectAllIgnoreOthers : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            string extension = Path.GetExtension(data.AssetPath);
            string[] ignoreExNames = data.UserData.Split(",");

            if (ignoreExNames.Length <= 0)
            {
                return true;
            }

            for (int i = 0; i < ignoreExNames.Length; i++)
            {
                if (extension == ignoreExNames[i])
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// YooAsset编辑模式加载
    /// </summary>
    public static class YooAsset_EditorLoad
    {
        private static Dictionary<string, Dictionary<string, string>> packageDict = new Dictionary<string, Dictionary<string, string>>();

        public static T Load<T>(string pPath, string pPackageName = "DefaultPackage") where T : Object
        {
            if (!packageDict.ContainsKey(pPackageName))
            {
                string packageRootPath = $"{AssetBundleBuilderHelper.GetDefaultBuildOutputRoot()}/{EditorUserBuildSettings.activeBuildTarget}";
                string fileName = YooAssetSettingsData.GetManifestJsonFileName(pPackageName, "Simulate");
                string filePath = $"{packageRootPath}/{pPackageName}/Simulate/{fileName}";
                Debug.Log($"LoadPackage;{filePath}");

                string jsonStr = File.ReadAllText(filePath);
                PackageManifest packageManifest = ManifestTools.DeserializeFromJson(jsonStr);
                Dictionary<string, string> mappingDict = new Dictionary<string, string>();
                foreach (var item in packageManifest.AssetList)
                {
                    mappingDict.Add(item.Address, item.AssetPath);
                }
                packageDict.Add(pPackageName, mappingDict);
            }

            Dictionary<string, string> tMapping = packageDict[pPackageName];
            if (!tMapping.ContainsKey(pPath))
            {
                Debug.LogError($"加载失败，没有对应资源映射{pPackageName}-->{pPath}");
                return null;
            }

            return AssetDatabase.LoadAssetAtPath<T>(tMapping[pPath]);
        }
    }
}
