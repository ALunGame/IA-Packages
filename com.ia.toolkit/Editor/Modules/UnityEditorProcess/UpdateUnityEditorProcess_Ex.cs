// using System;
// using System.Diagnostics;
// using System.Runtime.InteropServices;
// using System.Text;
// using UnityEngine;
//
// namespace XPToolchains
// {
//     public partial class UpdateUnityEditorProcess
//     {
//         public IntPtr hwnd = IntPtr.Zero;
//         private bool haveMainWindow = false;
//         private IntPtr mainWindowHandle = IntPtr.Zero;
//         private int processId = 0;
//         private IntPtr hwCurr = IntPtr.Zero;
//         private static StringBuilder sbtitle = new StringBuilder(255);
//         private static string ProjectName = "";
//         private static string ProjectPath = "";
//         public static float lasttime = 0;
//
//         private static UpdateUnityEditorProcess _instance;
//         public static UpdateUnityEditorProcess Instance
//         {
//             get
//             {
//                 if (_instance == null)
//                 {
//                     _instance = new UpdateUnityEditorProcess();
//                     _instance.hwnd = _instance.GetMainWindowHandle(Process.GetCurrentProcess().Id);
//                     var strArr = Application.dataPath.Split('/');
//                     ProjectName = strArr[strArr.Length - 2];
//                     ProjectPath = Application.dataPath.Replace("/Assets", "");
//                 }
//                 return _instance;
//             }
//         }
//
//         public void SetTitle()
//         {
//
//             lasttime = 0;
//             if (Time.realtimeSinceStartup > lasttime)
//             {
//                 sbtitle.Length = 0;
//                 lasttime = Time.realtimeSinceStartup + 2f;
//                 int length = GetWindowTextLength(hwnd);
//                 hwnd = _instance.hwnd;
//                 GetWindowText(hwnd.ToInt32(), sbtitle, 255);
//                 string strTitle = sbtitle.ToString();
//                 string[] ss = strTitle.Split('-');
//                 if (ss.Length > 0 && !strTitle.Contains(ProjectPath))
//                 {
//                     SetWindowText(hwnd.ToInt32(), strTitle.Replace(ProjectName, ProjectPath));
//                 }
//             }
//         }
//
//         public IntPtr GetMainWindowHandle(int processId)
//         {
//             if (!this.haveMainWindow)
//             {
//                 this.mainWindowHandle = IntPtr.Zero;
//                 this.processId = processId;
//                 EnumThreadWindowsCallback callback = new EnumThreadWindowsCallback(this.EnumWindowsCallback);
//                 EnumWindows(callback, IntPtr.Zero);
//                 GC.KeepAlive(callback);
//                 this.haveMainWindow = true;
//             }
//             return this.mainWindowHandle;
//         }
//
//         private bool EnumWindowsCallback(IntPtr handle, IntPtr extraParameter)
//         {
//             int num;
//             GetWindowThreadProcessId(new HandleRef(this, handle), out num);
//             if ((num == this.processId) && this.IsMainWindow(handle))
//             {
//                 this.mainWindowHandle = handle;
//             }
//             return true;
//         }
//
//         private bool IsMainWindow(IntPtr handle)
//         {
//
//             return (GetParent(handle) == IntPtr.Zero && !(GetWindow(new HandleRef(this, handle), 4) != IntPtr.Zero) && IsWindowVisible(new HandleRef(this, handle)));
//         }
//     }
// }
