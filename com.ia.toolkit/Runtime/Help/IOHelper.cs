using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace IAToolkit
{
    /// <summary>
    /// 文件读写辅助
    /// </summary>
    public static class IOHelper
    {

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="str">内容</param>
        /// <param name="path">路径</param>
        public static void WriteText(string str, string path)
        {
            string fullPath = Path.GetFullPath(path);
            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }
            using (StreamWriter sw = new StreamWriter(fullPath, false, Encoding.UTF8))
            {
                sw.WriteLine(str);
            }
        }
        
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="bytes">内容</param>
        /// <param name="path">路径</param>
        public static void WriteBytes(Byte[] bytes, string path)
        {
            string fullPath = Path.GetFullPath(path);
            if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            }
            using (FileStream fs = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                fs.Write(bytes,0,bytes.Length);
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path">路径</param>
        public static string ReadText(string path)
        {
            string fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                return "";
            }
            string str = "";
            using (StreamReader sw = new StreamReader(fullPath, Encoding.UTF8))
            {
                string tmpStr = "";
                while ((tmpStr = sw.ReadLine()) != null)
                {
                    str += tmpStr + "\n";
                }
            }
            return str;
        }

        /// <summary>
        /// 移动文件夹
        /// </summary>
        public static void MoveDirectory(string pPath, string pTargetPath)
        {
            if (!Directory.Exists(pPath))
            {
                return;
            }

            if (Directory.Exists(pTargetPath))
                Directory.Delete(pTargetPath, true);

            Directory.Move(pPath, pTargetPath);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public static void DelFile(string path)
        {
            string fullPath = Path.GetFullPath(path);
            if (!File.Exists(fullPath))
            {
                return;
            }
            File.Delete(fullPath);
        }

        /// <summary>
        /// 删除目录下所有文件
        /// </summary>
        public static void DelDirectoryAllFile(string path, string exName = "", bool recursive = true)
        {
            List<string> allFile = GetAllFilePath(path, exName, recursive);
            for (int i = 0; i < allFile.Count; i++)
            {
                DelFile(allFile[i]);
            }
        }


        /// <summary>
        /// 获取目录下的所有对象路径 忽略meta文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="exName">扩展名</param>
        /// <param name="recursive">子目录搜索</param>
        /// <returns></returns>
        public static List<string> GetAllFilePath(string path, string exName = "", bool recursive = true)
        {
            var resultList = new List<string>();

            var dirArr = Directory.GetFiles(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            for (int i = 0; i < dirArr.Length; i++)
            {
                if (Path.GetExtension(dirArr[i]) != ".meta")
                {
                    if (exName != "")
                    {
                        if (Path.GetExtension(dirArr[i]) == exName)
                        {
                            resultList.Add(dirArr[i].Replace('\\', '/'));
                        }
                    }
                    else
                    {
                        resultList.Add(dirArr[i].Replace('\\', '/'));
                    }

                }
            }

            return resultList;
        }

        /// <summary>
        /// 获取目录下的所有对象路径 忽略meta文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="ignoreExNames">忽略的扩展名</param>
        /// <param name="recursive">子目录搜索</param>
        /// <returns></returns>
        public static List<string> GetAllFilePathIgnoreName(string path, List<string> ignoreExNames, bool recursive = true)
        {
            var resultList = new List<string>();

            var dirArr = Directory.GetFiles(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            for (int i = 0; i < dirArr.Length; i++)
            {
                if (Path.GetExtension(dirArr[i]) != ".meta")
                {
                    string exName = Path.GetExtension(dirArr[i]);
                    if (!ignoreExNames.Contains(exName))
                    {
                        resultList.Add(dirArr[i].Replace('\\', '/'));
                    }

                }
            }

            return resultList;
        }
        
        /// <summary>
        /// 绝对路径转相对路径
        /// </summary>
        /// <param name="strBasePath">基本路径</param>
        /// <param name="strFullPath">绝对路径</param>
        /// <returns>strFullPath相对于strBasePath的相对路径</returns>
        public static string GetRelativePath(string strBasePath, string strFullPath)
        {
            if (strBasePath == null)
                return "";
 
            if (strFullPath == null)
                return "";
            
            strBasePath = Path.GetFullPath(strBasePath);
            strFullPath = Path.GetFullPath(strFullPath);
            
            var DirectoryPos = new int[strBasePath.Length];
            int nPosCount = 0;
            
            DirectoryPos[nPosCount] = -1;
            ++nPosCount;
            
            int nDirectoryPos = 0;
            while (true)
            {
                nDirectoryPos = strBasePath.IndexOf('\\', nDirectoryPos);
                if (nDirectoryPos == -1)
                    break;
                
                DirectoryPos[nPosCount] = nDirectoryPos;
                ++nPosCount;
                ++nDirectoryPos;
            }
            
            if (!strBasePath.EndsWith("\\"))
            {
                DirectoryPos[nPosCount] = strBasePath.Length;
                ++nPosCount;
            }     
            
            int nCommon = -1;
            for (int i = 1; i < nPosCount; ++i)
            {
                int nStart = DirectoryPos[i - 1] + 1;
                int nLength = DirectoryPos[i] - nStart;
                
                if (string.Compare(strBasePath, nStart, strFullPath, nStart, nLength, true) != 0)
                    break;
                
                nCommon = i;
            }
            
            if (nCommon == -1)
                return strFullPath;
            
            var strBuilder = new StringBuilder();
            for (int i = nCommon + 1; i < nPosCount; ++i)
                strBuilder.Append("../");
            
            int nSubStartPos = DirectoryPos[nCommon] + 1;
            if (nSubStartPos < strFullPath.Length)
                strBuilder.Append(strFullPath.Substring(nSubStartPos));
            
            string strResult = strBuilder.ToString();
            return strResult == string.Empty ? "./" : strResult;
        }

        /// <summary>
        /// 绝对路径转Unity编辑器目录
        /// </summary>
        /// <returns></returns>
        public static string GetUnityRelativePath(string strFullPath)
        {
            if (!strFullPath.Contains("Assets"))
            {
                Debug.LogError("绝对路径转Unity编辑器目录出错，该目录不是编辑器下目录 "+strFullPath);
                return "";
            }

            string resPath = strFullPath.Replace("\\","/");
            int assetIndex = resPath.IndexOf("Assets");
            resPath = resPath.Substring(assetIndex, resPath.Length - assetIndex);
            return resPath;
        }
    }
}