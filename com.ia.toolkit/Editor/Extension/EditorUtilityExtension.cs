using System;
using System.Collections.Generic;
using UnityEditor;

namespace IAToolkit
{
    /// <summary>
    /// 编辑器通用扩展
    /// </summary>
    public class EditorExtension
    {
        /// <summary>
        /// 查找Mono脚本
        /// </summary>
        /// <returns></returns>
        public static MonoScript FindScriptFromType(Type _type, Func<MonoScript, bool> _pattern = null, bool _compareTypeName = true)
        {
            string findStr = "t:script " + (_compareTypeName ? _type.Name : "");
            var scriptGUIDs = AssetDatabase.FindAssets(findStr);
            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null)
                {
                    if (_pattern == null || _pattern(script))
                        return script;
                }
            }
            return null;
        }

        /// <summary>
        /// 查找所有指定类型的Mono脚本
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MonoScript> FindAllScriptFromType(Type _type, Func<MonoScript, bool> _pattern = null, bool _compareTypeName = true)
        {
            string findStr = "t:script " + (_compareTypeName ? _type.Name : "");
            var scriptGUIDs = AssetDatabase.FindAssets(findStr);
            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                if (script != null)
                {
                    if (_pattern == null || _pattern(script))
                        yield return script;
                }
            }
        }

        /// <summary> 
        /// 添加宏定义 
        /// </summary>
        /// <param name="_define"></param>
        /// <param name="targetGroup"></param>
        public static void AddDefine(string _define, BuildTargetGroup targetGroup = BuildTargetGroup.Standalone)
        {
            string s = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            List<string> defines = new List<string>(s.Split(';'));
            if (!defines.Contains(_define))
            {
                defines.Add(_define);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", defines.ToArray()));
            }
        }

        /// <summary> 
        /// 移除宏定义 
        /// </summary>
        /// <param name="_define"></param>
        /// <param name="targetGroup"></param>
        public static void RemoveDefine(string _define, BuildTargetGroup targetGroup = BuildTargetGroup.Standalone)
        {
            string s = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            List<string> defines = new List<string>(s.Split(';'));
            if (defines.Contains(_define))
            {
                defines.Remove(_define);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", defines.ToArray()));
            }
        }
    }
}
