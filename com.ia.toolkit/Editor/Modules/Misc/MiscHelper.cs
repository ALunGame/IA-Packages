using IAToolkit.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;
using Object = System.Object;
using UnityObject = UnityEngine.Object;

namespace IAToolkit
{
    /// <summary>
    /// 编辑器杂项辅助
    /// </summary>
    public static class MiscHelper
    {

        #region 按钮

        /// <summary>
        /// 绘制一个按钮
        /// </summary>
        /// <param name="btnName">按钮名</param>
        /// <param name="width">按钮宽</param>
        /// <param name="height">按钮高</param>
        /// <param name="callBack">按钮回调</param>
        public static void Btn(string btnName, float width, float height, Action callBack)
        {
            if (GUILayout.Button(new GUIContent(btnName), GUI.skin.button, GUILayout.Width(width), GUILayout.Height(height)))
            {
                callBack?.Invoke();
            }
        }

        #endregion

        #region 编辑器对话框

        /// <summary>
        /// 编辑器对话框
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="message">对话框内容</param>
        /// <param name="okFunc">确定回调</param>
        /// <param name="cancleFunc">取消回调</param>
        /// <param name="ok">确认按钮名</param>
        /// <param name="cancle">取消按钮名</param>
        public static void Dialog(string title, string message, Action okFunc = null, Action cancleFunc = null, string ok = "确定", string cancle = "取消")
        {
            bool isTrue = EditorUtility.DisplayDialog(title, message, ok, cancle);
            if (isTrue)
            {
                if (okFunc != null)
                {
                    okFunc();
                }
            }
            else
            {
                if (cancleFunc != null)
                {
                    cancleFunc();
                }
            }
        }

        #endregion

        #region 输入框

        /// <summary>
        /// 弹出输入框
        /// </summary>
        /// <param name="strContent">内容</param>
        /// <param name="callBack">输入回调</param>
        public static void Input(string strContent, Action<string> callBack)
        {
            InputWindow.PopWindow(strContent, callBack);
        }

        #endregion

        #region 点击下拉框

        /// <summary>
        /// 编辑器下拉框
        /// </summary>
        /// <param name="selItem">下拉框名</param>
        /// <param name="itemList">列表</param>
        /// <param name="selCallBack">选择回调</param>
        /// <param name="width">下拉框宽</param>
        public static void Dropdown(string selItem, List<string> itemList, Action<string> selCallBack, float width = 100)
        {
            if (EditorGUILayout.DropdownButton(new GUIContent(selItem), FocusType.Keyboard, GUILayout.Width(width)))
            {
                GenericMenu menu = new GenericMenu();
                foreach (string item in itemList)
                {
                    AddMenuItem(menu, item, selItem, selCallBack);
                }
                menu.ShowAsContext();
            }
        }

        private static void AddMenuItem(GenericMenu menu, string item, string selItem, Action<string> selCallBack)
        {
            menu.AddItem(new GUIContent(item), selItem.Equals(item), (x) =>
            {

                selItem = x.ToString();
                if (selCallBack != null)
                {
                    selCallBack(x.ToString());
                }

            }, item);
        }

        /// <summary>
        /// 编辑器下拉框
        /// </summary>
        /// <param name="selItem">下拉框名</param>
        /// <param name="itemList">列表</param>
        /// <param name="selCallBack">选择回调</param>
        /// <param name="width">下拉框宽</param>
        public static void Dropdown(string selItem, List<string> itemList, Action<int> selCallBack, float width = 100)
        {
            if (EditorGUILayout.DropdownButton(new GUIContent(selItem), FocusType.Keyboard, GUILayout.Width(width)))
            {
                GenericMenu menu = new GenericMenu();

                for (int i = 0; i < itemList.Count; i++)
                {
                    AddMenuItem(menu, itemList[i], selItem, i, selCallBack);
                }
                menu.ShowAsContext();
            }
        }

        private static void AddMenuItem(GenericMenu menu, string item, string selItem, int index, Action<int> selCallBack)
        {
            menu.AddItem(new GUIContent(item), selItem.Equals(item), (x) =>
            {

                selItem = x.ToString();
                if (selCallBack != null)
                {
                    selCallBack(index);
                }

            }, item);
        }

        #endregion

        #region 菜单

        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="itemList">菜单选项</param>
        /// <param name="selCallBack">选择回调</param>
        public static void Menu(List<string> itemList, Action<string> selCallBack)
        {
            GenericMenu menu = new GenericMenu();
            foreach (string item in itemList)
            {
                AddMenuItem(menu, item, selCallBack);
            }
            menu.ShowAsContext();
        }

        private static void AddMenuItem(GenericMenu menu, string item, Action<string> selCallBack)
        {
            menu.AddItem(new GUIContent(item), false, (x) =>
            {
                selCallBack?.Invoke(x.ToString());
            }, item);
        }

        private static void AddMenuItem(GenericMenu menu, string item, int index, Action<int> selCallBack)
        {
            menu.AddItem(new GUIContent(item), false, (x) =>
            {
                if (selCallBack != null)
                {
                    selCallBack(index);
                }

            }, item);
        }

        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="itemList">菜单选项</param>
        /// <param name="selCallBack">选择回调</param>
        public static void Menu<T>(List<T> itemList, Action<T> selCallBack)
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < itemList.Count; i++)
            {
                AddMenuItem(menu, itemList[i], itemList[i].ToString(),selCallBack);
            }
            menu.ShowAsContext();
        }

        private static void AddMenuItem<T>(GenericMenu menu, T obj, string item,Action<T> selCallBack)
        {
            menu.AddItem(new GUIContent(item), false, (x) =>
            {
                if (selCallBack != null)
                {
                    selCallBack((T)x);
                }
            }, obj);
        }

        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="itemList">菜单选项</param>
        /// <param name="selCallBack">选择回调</param>
        public static void Menu<T>(List<T> itemList, Action<T> selCallBack,Func<T,string> menuName)
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < itemList.Count; i++)
            {
                AddMenuItem(menu, itemList[i], menuName(itemList[i]), selCallBack);
            }
            menu.ShowAsContext();
        }

        /// <summary>
        /// 菜单
        /// </summary>
        /// <param name="itemList">菜单选项</param>
        /// <param name="selCallBack">选择回调</param>
        public static void Menu(List<string> itemList, Action<int> selCallBack)
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < itemList.Count; i++)
            {
                AddMenuItem(menu, itemList[i], i, selCallBack);
            }
            menu.ShowAsContext();
        }

        #endregion

        #region 搜索框

        public static string CreateSearch(string value, string defaultStr, float width)
        {
            return CreateSearch(value, defaultStr, GUILayout.Width(width));
        }

        private static string CreateSearch(string value, string defaultStr, params GUILayoutOption[] options)
        {
            value = value == "" ? defaultStr : value;
            MethodInfo info = typeof(EditorGUILayout).GetMethod("ToolbarSearchField", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string), typeof(GUILayoutOption[]) }, null);
            if (info != null)
            {
                value = (string)info.Invoke(null, new object[] { value, options });
            }
            return value;
        }

        #endregion

        #region 画线

        /// <summary>
        /// 画贝赛尔曲线
        /// </summary>
        /// <param name="startPosition">起始位置</param>
        /// <param name="endPosition">结束位置</param>
        /// <param name="width">线宽</param>
        /// <param name="color">颜色</param>
        /// <param name="texture">贴图</param>
        public static void DrawBezierLine(Vector3 startPosition, Vector3 endPosition, float width, Color color = default(Color), Texture2D texture = null)
        {
            Handles.DrawBezier(startPosition, endPosition, startPosition - Vector3.left * 50f, endPosition + Vector3.left * 50f, color, texture, width);
            
        }

        private static Type EditorGUI;
        private static object EditorGUIObj;
        /// <summary>
        /// 绘制边框线
        /// </summary>
        /// <param name="rect">区域</param>
        /// <param name="size">大小</param>
        /// <param name="color">颜色</param>
        public static void DrawOutline(Rect rect, float size, Color color)
        {
            if (EditorGUI == null)
            {
                Assembly asm = Assembly.Load("UnityEditor");
                EditorGUI = asm.GetType("UnityEditor.EditorGUI");
                EditorGUIObj = Activator.CreateInstance(EditorGUI);
            }
            MethodInfo oMethod = EditorGUI.GetMethod("DrawOutline", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            oMethod.Invoke(EditorGUIObj, new Object[] { rect, size, color });
        }

        #endregion

        #region 文件和目录

        /// <summary>
        /// 绘制一个文件选择窗口
        /// </summary>
        /// <param name="label">提示名</param>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string FilePath(string label, string path)
        {
            EditorGUILayout.BeginHorizontal();
            path = EditorGUILayout.TextField(label, path);
            Rect rect = GUILayoutUtility.GetLastRect();
            UnityObject uObj = GUIExtension.DragDropAreaSingle(rect, DragAndDropVisualMode.Copy);
            if (uObj != null && AssetDatabase.IsMainAsset(uObj))
            {
                string p = AssetDatabase.GetAssetPath(uObj);
                if (!AssetDatabase.IsValidFolder(p))
                    path = p;
            }
            if (GUILayout.Button(EditorGUIUtility.FindTexture("FolderEmpty Icon"), EditorStylesExtension.OnlyIconButtonStyle, GUILayout.Width(18), GUILayout.Height(18)))
            {
                path = EditorUtility.OpenFilePanel($"{label}文件选择", Application.dataPath, "*");
                if (!string.IsNullOrEmpty(path))
                    path = path.Substring(path.IndexOf("Assets"));
            }
            EditorGUILayout.EndHorizontal();
            return path;
        }

        /// <summary>
        /// 绘制一个目录选择窗口
        /// </summary>
        /// <param name="label">提示名</param>
        /// <param name="folderPath">目录路径</param>
        /// <returns></returns>
        public static string FolderPath(string label, string folderPath)
        {
            EditorGUILayout.BeginHorizontal();
            folderPath = EditorGUILayout.TextField(label, folderPath);
            Rect rect = GUILayoutUtility.GetLastRect();
            UnityObject uObj = GUIExtension.DragDropAreaSingle(rect, DragAndDropVisualMode.Copy);
            if (uObj != null && AssetDatabase.IsMainAsset(uObj))
            {
                string p = AssetDatabase.GetAssetPath(uObj);
                if (AssetDatabase.IsValidFolder(p))
                    folderPath = p;
            }
            if (GUILayout.Button(EditorGUIUtility.FindTexture("FolderEmpty Icon"), EditorStylesExtension.OnlyIconButtonStyle, GUILayout.Width(18), GUILayout.Height(18)))
            {
                folderPath = EditorUtility.OpenFolderPanel($"{label}目录选择", Application.dataPath, string.Empty);
                if (!string.IsNullOrEmpty(folderPath))
                {
                    if (folderPath.IndexOf("Assets") != -1)
                    {
                        folderPath = folderPath.Substring(folderPath.IndexOf("Assets"));
                    }
                    else
                    {
                        folderPath = IOHelper.GetRelativePath(Environment.CurrentDirectory, folderPath);
                    }
                }
                //GUIUtility.ExitGUI();
            }
            EditorGUILayout.EndHorizontal();
            return folderPath;
        }

        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="pDirPath"></param>
        public static void OpenDirectory(string pDirPath)
        {
            string fullPath = Path.GetFullPath(pDirPath);
            Process.Start(fullPath);
        }

        #endregion

        #region 执行批处理

        private static Process CreateShellExProcess(string cmd, string args, string workingDir = "")
        {
            var en = System.Text.UTF8Encoding.UTF8;
            if (Application.platform == RuntimePlatform.WindowsEditor)
                en = System.Text.Encoding.GetEncoding("gb2312");

            var pStartInfo = new ProcessStartInfo(cmd);
            pStartInfo.Arguments = args;
            pStartInfo.CreateNoWindow = true;
            pStartInfo.UseShellExecute = false;
            pStartInfo.RedirectStandardError = true;
            pStartInfo.RedirectStandardInput = true;
            pStartInfo.RedirectStandardOutput = true;
            pStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pStartInfo.StandardErrorEncoding = en;
            pStartInfo.StandardOutputEncoding = en;

            pStartInfo.WorkingDirectory = workingDir;

            return Process.Start(pStartInfo);
        }

        /// <summary>
        /// 执行批处理
        /// </summary>
        /// <param name="batfile">批处理文件</param>
        /// <param name="args">参数</param>
        /// <param name="workingDir">工作目录</param>
        public static void ExecuteBat(string batfile, string args, Action action, string workingDir = "")
        {
            var p = CreateShellExProcess(batfile, args, workingDir);

            string output = p.StandardOutput.ReadToEnd();
            UnityEngine.Debug.Log(output); // 输出标准输出的内容

            var err = p.StandardError.ReadToEnd();
            UnityEngine.Debug.Log(err); // 输出标准错误的内容

            p.WaitForExit();
            p.Close();
            if (action != null)
            {
                action();
            }
        }

        #endregion
    }
}
