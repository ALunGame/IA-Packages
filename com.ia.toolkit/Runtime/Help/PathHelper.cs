using System.IO;
using UnityEngine;

namespace IAToolkit
{
    /// <summary>
    /// 文件路径辅助
    /// </summary>
    public static class PathHelper
    {
        static string mSandboxDir = string.Empty;         //沙盒路径
        public static string SandboxDir
        {
            get
            {
                if (string.IsNullOrEmpty(mSandboxDir))
                {
                    mSandboxDir = Application.persistentDataPath;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                    mSandboxDir = Path.Combine(Path.GetDirectoryName(Application.dataPath), "Sandbox").Replace("\\", "/");
                    if (!Directory.Exists(mSandboxDir))
                        Directory.CreateDirectory(mSandboxDir);
#endif
                }
                return mSandboxDir;
            }
        }
    }
}
