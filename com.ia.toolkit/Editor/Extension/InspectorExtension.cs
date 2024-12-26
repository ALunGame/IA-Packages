using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using IAToolkit.Core;
using UnityObject = UnityEngine.Object;

namespace IAToolkit
{
    /// <summary>
    /// Inspector面板扩展
    /// </summary>
    public static partial class InspectorExtension
    {
        /// <summary>
        /// 在Inspector面板上绘制一个对象
        /// </summary>
        public static void DrawObjectInInspector(object _targetObject, object _owner = null, Action _clickBackFunc = null)
        {
            if (_targetObject is UnityObject)
            {
                Selection.activeObject = _targetObject as UnityObject;
            }
            else
            {
                Selection.activeObject = ObjectInspector.Instance;
                ObjectInspector.Instance.onClickBackFunc = _clickBackFunc;
                ObjectInspector.Instance.Init(_targetObject, _owner);
            }
        }

        /// <summary>
        /// 在Inspector面板上绘制一个对象
        /// </summary>
        public static void DrawObjectInInspector(string _title, object _targetObject, object _owner = null, Action _clickBackFunc = null)
        {
            DrawObjectInInspector(_targetObject, _owner, _clickBackFunc);
            ObjectInspector.Instance.name = _title;
        }
    }
}