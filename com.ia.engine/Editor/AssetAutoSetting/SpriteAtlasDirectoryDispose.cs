using System.Collections.Generic;
using System.IO;
using System.Reflection;
using IAToolkit;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace IAEngine
{
    public class SpriteAtlasDirectoryDispose : DirectoryDispose
    {
        public override void OnDirectoryChange(DirectoryChangeType pChangeType, FileSystemEventArgs pArgs)
        {
            if (pChangeType == DirectoryChangeType.Delete)
            {
                return;
            }

            if (pArgs.Name.Contains(".meta") || pArgs.Name.Contains("."))
            {
                return;
            }
            
            Debug.LogWarning($"OnDirectoryChange>>>{pArgs.Name}-->{pArgs.FullPath}");
            Debug.LogWarning($"OnDirectoryChange>>>{pArgs.Name}-->{IOHelper.GetUnityRelativePath(pArgs.FullPath)}");

            string[] fils = Directory.GetFiles(pArgs.FullPath);
            if (fils == null || fils.Length <= 0)
            {
                return;
            }
            
            CreateDirAtlas(pArgs.Name, pArgs.FullPath);
        }

        private void CreateDirAtlas(string pDirName, string pFullPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(pFullPath);
            string atlasName = $"a_{dirInfo.Parent.Name}_{pDirName}.spriteatlas".ToLower();
            atlasName.Replace(" ", "");
            
            string atlasFilePath = Path.Combine(pFullPath, atlasName);
            if (File.Exists(atlasFilePath))
            {
                return;
            }
            
            string atlasDirPath = IOHelper.GetUnityRelativePath(pFullPath);
            atlasFilePath = IOHelper.GetUnityRelativePath(atlasFilePath);
            
            SpriteAtlas atlas = new SpriteAtlas();
            SetPlatform(atlasDirPath, ref atlas);
            
            atlas.SetIsVariant(false);
            atlas.SetIncludeInBuild(false);
            
            // 设置参数 可根据项目具体情况进行设置
            SpriteAtlasPackingSettings packSetting = new SpriteAtlasPackingSettings()
            {
                blockOffset = 1,
                enableRotation = false,
                enableTightPacking = false,
                padding = 4,
            };
            atlas.SetPackingSettings(packSetting);
            
            SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
            {
                readable = false,
                generateMipMaps = false,
                sRGB = true,
                filterMode = FilterMode.Bilinear,
            };
            atlas.SetTextureSettings(textureSetting);
            
            AssetDatabase.CreateAsset(atlas, atlasFilePath);
            AssetDatabase.Refresh();
            
            Object obj = AssetDatabase.LoadAssetAtPath(atlasDirPath, typeof(Object));
            atlas.Add(new[] { obj });
            ClearAtlasCache(atlas);
            EditorUtility.SetDirty(atlas);
            SpriteAtlasUtility.PackAtlases(new SpriteAtlas[] { atlas }, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        private void SetPlatform(string dataPath, ref SpriteAtlas atlas)
        {
            Dictionary<string, TextureImporterFormat> PlatformDic = new Dictionary<string, TextureImporterFormat>() {
                { "Standalone", TextureImporterFormat.RGBA32 },
                { "Android", TextureImporterFormat.ETC2_RGBA8 },
                //{ "Standalone", TextureImporterFormat.PVRTC_RGBA4 },
            };

            foreach (var keyVal in PlatformDic)
            {
                TextureImporterPlatformSettings platform = atlas.GetPlatformSettings(keyVal.Key);
                if (!platform.overridden)
                {
                    platform.overridden = true;
                }
                if (dataPath.EndsWith("_1024"))
                {
                    platform.maxTextureSize = 1024;
                }
                else
                {
                    platform.maxTextureSize = 2048;
                }

                platform.format = keyVal.Value;
                platform.crunchedCompression = false;
                platform.textureCompression = TextureImporterCompression.Compressed;
                atlas.SetPlatformSettings(platform);
            }
        }
        
        private void ClearAtlasCache(SpriteAtlas atlas)
        {
            var cachDir = $"{Application.dataPath}/../Library/AtlasCache";
            if (!Directory.Exists(cachDir))
                return;

            Assembly unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            var spriteAtlasExtensionsType = unityEditorAssembly.GetType("UnityEditor.U2D.SpriteAtlasExtensions");
            var storeInfo = spriteAtlasExtensionsType.GetMethod("GetStoredHash",BindingFlags.Public|BindingFlags.Static|BindingFlags.NonPublic);
            var storeHash = storeInfo.Invoke(null, new object[] { atlas });

            DirectoryInfo dirInfo = new DirectoryInfo(cachDir);
            FileInfo[] fileInfosExtention = dirInfo.GetFiles("*.", SearchOption.AllDirectories);
            if (fileInfosExtention.Length > 0)
            {
                foreach (FileInfo item in fileInfosExtention)
                {
                    if(item.Name.ToString() == storeHash.ToString())
                    {
                        File.Delete(item.ToString());
                        Debug.Log($"删除图集缓存文件:{item.ToString()}");
                    }
                }
            }
        }
    }
}