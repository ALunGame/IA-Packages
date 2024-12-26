// using System.Collections.Generic;
// using System.IO;
//
// namespace LCLoad
// {
//     /// <summary>
//     /// 监视目录变化
//     /// </summary>
//     public class DirectoryWatcher
//     {
//         static readonly Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();
//
//         /// <summary>
//         /// 监视一个目录，如果有修改则触发事件函数, 包含其子目录！
//         /// <para>使用更大的buffer size确保及时触发事件</para>
//         /// <para>不用includesubdirect参数，使用自己的子目录扫描，更稳健</para>
//         /// </summary>
//         /// <param name="dirPath"></param>
//         /// <param name="handler"></param>
//         /// <returns></returns>
//         public DirectoryWatcher(string dirPath, FileSystemEventHandler changedHandler,
//             FileSystemEventHandler createdHandler, FileSystemEventHandler deleteHandler,string filter = "*")
//         {
//             CreateWatch(dirPath, changedHandler, createdHandler, deleteHandler,filter);
//         }
//
//         void CreateWatch(string dirPath, FileSystemEventHandler changedHandler,
//             FileSystemEventHandler createdHandler, FileSystemEventHandler deleteHandler,string filter = "*")
//         {
//             if (_watchers.ContainsKey(dirPath))
//             {
//                 _watchers[dirPath].Dispose();
//                 _watchers[dirPath] = null;
//             }
//
//             if (!Directory.Exists(dirPath)) return;
//             
//             var watcher = new FileSystemWatcher();
//             watcher.NotifyFilter = NotifyFilters.Size;
//
//             watcher.IncludeSubdirectories = false;//includeSubdirectories;
//             watcher.Path = dirPath;
//             watcher.Filter = filter;
//             watcher.Changed += changedHandler;
//             watcher.Created += createdHandler;
//             watcher.Deleted += deleteHandler;
//             watcher.EnableRaisingEvents = true;
//             watcher.InternalBufferSize = 10240;
//             //return watcher;
//             _watchers[dirPath] = watcher;
//
//
//             foreach (var childDirPath in Directory.GetDirectories(dirPath))
//             {
//                 CreateWatch(childDirPath, changedHandler, createdHandler, deleteHandler);
//             }
//         }
//     }
// }