using System.IO;
using IAToolkit;
using UnityEditor;

namespace IAEngine
{
    public enum DirectoryChangeType
    {
        /// <summary>
        /// 目录改变
        /// </summary>
        Change,
        /// <summary>
        /// 目录创建
        /// </summary>
        Create,
        /// <summary>
        /// 目录删除
        /// </summary>
        Delete,
    }
    
    /// <summary>
    /// 目录处理
    /// </summary>
    public class DirectoryDispose
    {
        public string WatchPath;

        public virtual void OnDirectoryChange(DirectoryChangeType pChangeType, FileSystemEventArgs pArgs)
        {
            
        }
    }
    
    /// <summary>
    /// 文件处理
    /// </summary>
    public class FileDispose
    {
        public string FileExName;
    }
    
    /// <summary>
    /// 资源改变处理
    /// </summary>
    [InitializeOnLoad]
    public class AssetChangeDisposeServer
    {
        private static SpriteAtlasDirectoryDispose _spriteAtlasDirectoryDispose;
        
        static AssetChangeDisposeServer()
        {
            _spriteAtlasDirectoryDispose = new SpriteAtlasDirectoryDispose();
            _spriteAtlasDirectoryDispose.WatchPath = Path.GetDirectoryName(Path.GetFullPath("./Assets/Demo/Asset/UI/Textures/Dynamic/"));
            DirectoryWatcher watcher = new DirectoryWatcher(_spriteAtlasDirectoryDispose.WatchPath, (o, args) =>
            {
                UnityEditorHelper.CallMainThread(() =>
                {
                    _spriteAtlasDirectoryDispose.OnDirectoryChange(DirectoryChangeType.Change,args);
                });
            }, (o, args) =>
            {
                UnityEditorHelper.CallMainThread(() =>
                {
                    _spriteAtlasDirectoryDispose.OnDirectoryChange(DirectoryChangeType.Create,args);
                });
            }, (o, args) =>
            {
                UnityEditorHelper.CallMainThread(() =>
                {
                    _spriteAtlasDirectoryDispose.OnDirectoryChange(DirectoryChangeType.Delete,args);
                });
            });
        }

        private static void OnDirectoryChange()
        {
            
        }
    }
}