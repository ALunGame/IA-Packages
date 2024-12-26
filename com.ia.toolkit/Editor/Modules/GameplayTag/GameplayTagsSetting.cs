using System.IO;
using IAEngine;
using UnityEditor;
using UnityEngine;

namespace IAToolkit
{
    public class GameplayTagsSetting : ScriptableObject
    {
        public const string SetingPath = "Assets/Editor/GAS/TagsSetting";
        public const string TagsFileName = "GameplayTags.json";

        public string tagSaveRootPath;
        public bool gameplayTagContainerExpand;
        
        #region Static

        private static GameplayTagsSetting setting;
        public static GameplayTagsSetting Setting
        {
            get
            {
                if (setting == null)
                {
                    string[] tileAssetPath = new string[] { SetingPath };
                    string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", tileAssetPath);
                    foreach (var guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        GameplayTagsSetting asset = AssetDatabase.LoadAssetAtPath<GameplayTagsSetting>(path);
                        if (asset != null)
                        {
                            setting = asset;
                            break;
                        }
                    }
                }
                return setting;
            }
        }
        
        [MenuItem("Gameplay/GAS/GameplayTags")]
        public static void CreateSetting()
        {
            GameplayTagsSetting setting = Setting;
            if (setting == null)
            {
                if (!Directory.Exists(SetingPath))
                {
                    Directory.CreateDirectory(SetingPath);
                }
                setting = CreateInstance<GameplayTagsSetting>();
                setting.name = "标签设置";
                AssetDatabase.CreateAsset(setting, SetingPath + "/标签设置.asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Selection.activeObject = setting;
            }
            else
            {
                Selection.activeObject = setting;
            }
        }

        public static GameplayTags LoadTags()
        {
            string filePath = Setting.tagSaveRootPath + "/" + TagsFileName;
            filePath = Path.GetFullPath(filePath);
            string jsonStr = IOHelper.ReadText(filePath);
            if (string.IsNullOrEmpty(jsonStr))
            {
                return new GameplayTags();
            }
            return JsonMapper.ToObject<GameplayTags>(jsonStr);
        }

        public static void SaveTags(GameplayTags pTags)
        {
            string filePath = Setting.tagSaveRootPath + "/" + TagsFileName;
            filePath = Path.GetFullPath(filePath);
            string jsonStr = JsonMapper.ToJson(pTags);
            IOHelper.WriteText(jsonStr,filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #endregion
    }
}