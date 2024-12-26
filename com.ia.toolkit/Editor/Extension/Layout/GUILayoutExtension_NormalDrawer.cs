using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using IAEngine;

namespace IAToolkit
{
    public static partial class GUILayoutExtension
    {
        /// <summary> 不是<see cref="private"/>、或者标记了<see cref="SerializeField"/>特性，并且没有标记<see cref="NonSerializedAttribute"/>特性，并且没有标记<see cref="HideInInspector"/>特性。 </summary>
        /// <returns> 满足以上条件返回<see cref="true"/> </returns>
        public static bool CanDraw(FieldInfo fieldInfo)
        {
            return ((!fieldInfo.IsPrivate && !fieldInfo.IsFamily) || AttributeHelper.TryGetFieldAttribute(fieldInfo, out SerializeField serAtt))
                    && !AttributeHelper.TryGetTypeAttribute(fieldInfo.DeclaringType, out NonSerializedAttribute nonAtt)
                    && !AttributeHelper.TryGetFieldAttribute(fieldInfo, out HideInInspector hideAtt);
        }

        /// <summary>
        /// 绘制折叠元素
        /// </summary>
        /// <param name="key">用于存取上下文数据</param>
        /// <param name="guiContent">元素显示配置</param>
        /// <returns></returns>
        public static bool DrawFoldout(int key, GUIContent guiContent)
        {
            string text = string.Concat(c_EditorPrefsFoldoutKey, key, ".", guiContent.text);
            var @bool = GUIHelper.GetContextData(text, false);
            @bool.value = EditorGUILayout.Foldout(@bool.value, guiContent, true);
            return @bool.value;
        }

        /// <summary>
        /// 绘制字段
        /// </summary>
        /// <param name="fieldInfo">字段</param>
        /// <param name="context">字段所属环境</param>
        public static void DrawField(FieldInfo fieldInfo, object context)
        {
            GUIContent label = null;
            if (AttributeHelper.TryGetFieldAttribute(fieldInfo, out TooltipAttribute tooltipAtt))
                label = GUIHelper.TextContent(ObjectNames.NicifyVariableName(fieldInfo.Name), tooltipAtt.tooltip);
            else
                label = GUIHelper.TextContent(ObjectNames.NicifyVariableName(fieldInfo.Name));
            DrawField(fieldInfo, context, label);
        }

        /// <summary>
        /// 绘制字段
        /// </summary>
        /// <param name="fieldInfo">字段</param>
        /// <param name="context">字段所属环境</param>
        public static void DrawField(FieldInfo fieldInfo, object context, string label)
        {
            DrawField(fieldInfo, context, GUIHelper.TextContent(label));
        }

        /// <summary>
        /// 绘制字段
        /// </summary>
        /// <param name="fieldInfo">字段</param>
        /// <param name="context">字段所属环境</param>
        /// <param name="label"></param>
        public static void DrawField(FieldInfo fieldInfo, object context, GUIContent label)
        {
            object value = fieldInfo.GetValue(context);
            object newValue = DrawField(fieldInfo.FieldType, value, label);
            if (value == null || newValue == null)
            {
                return;
            }
            if (!newValue.Equals(value))
            {
                fieldInfo.SetValue(context, newValue);
            }
        }

        public static object DrawField(Type type, object value, GUIContent label = null)
        {
            // 判断是否是数组
            if (typeof(IList).IsAssignableFrom(type))
                return DrawArrayField(type, value, label);
            else
                return DrawSingleField(type, value, label);
        }

        public static object DrawField(Type type, object value, string label,string tooltip = "")
        {
            GUIContent lable = GUIHelper.TextContent(label, tooltip);
            return DrawField(type, value, lable);
        }

        /// <summary>
        /// 绘制单个元素
        /// </summary>
        /// <param name="value">数据</param>
        /// <param name="label">名</param>
        /// <returns></returns>
        static object DrawSingleField(Type objType, object value, GUIContent label = null)
        {
            if (GUIExtension.IsBasicType(objType))
            {
                if (label == null)
                {
                    label = new GUIContent(objType.Name);
                }
                float tmpHeight = GUIExtension.GetHeight(objType, label);
                return GUIExtension.DrawField(EditorGUILayout.GetControlRect(true, tmpHeight), value, label);
            }

            Type type = objType;
            //如果类型是类 或者 是值类型并且不是基元类型
            if (type.IsClass || (type.IsValueType && !type.IsPrimitive))
            {
                if (label == null)
                {
                    label = new GUIContent(objType.Name);
                }

                //委托类型
                if (typeof(Delegate).IsAssignableFrom(type))
                    return null;
                //基础类型并且为空
                if (typeof(object).IsAssignableFrom(type) && value == null)
                    return null;
                int hashCode = value.GetHashCode();

                //值为空
                if (value == null)
                {
                    //泛型
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        type = Nullable.GetUnderlyingType(type);
                    value = Activator.CreateInstance(type, true);
                }

                //遍历类型字段
                EditorGUILayout.BeginVertical();
                if (DrawFoldout(hashCode, label))
                {
                    EditorGUI.indentLevel++;
                    foreach (var field in ReflectionHelper.GetFieldInfos(type))
                    {
                        if (!CanDraw(field)) continue;

                        GUIContent fileLable  = new GUIContent(field.Name);
                        if (AttributeHelper.TryGetFieldAttribute(field,out HeaderAttribute attr))
                        {
                            fileLable = new GUIContent(attr.header);
                        }
                        if (GUIExtension.IsBasicType(type))
                        {
                            float tmpHeight = GUIExtension.GetHeight(type, fileLable);
                            return GUIExtension.DrawField(EditorGUILayout.GetControlRect(true, tmpHeight), field.GetValue(value), fileLable);
                        }
                        else
                        {
                            DrawField(field, value, fileLable);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
                return value;
            }

            float height = GUIExtension.GetHeight(type, label);
            return GUIExtension.DrawField(EditorGUILayout.GetControlRect(true, height), value, label);
        }

        static object DrawArrayField(Type objType, object value, GUIContent label)
        {
            Type fieldType = objType;

            Type elementType;
            if (fieldType.IsArray)
                elementType = fieldType.GetElementType();
            else
            {
                Type type2 = fieldType;
                while (!type2.IsGenericType)
                {
                    type2 = type2.BaseType;
                }
                elementType = type2.GetGenericArguments()[0];
            }

            IList list;
            if (value == null)
            {
                if (fieldType.IsGenericType || fieldType.IsArray)
                {
                    value = list = (Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[]
                    {
                        elementType
                    }), true) as IList);
                }
                else
                {
                    value = list = (Activator.CreateInstance(fieldType, true) as IList);
                }
                if (fieldType.IsArray)
                {
                    Array array = Array.CreateInstance(elementType, list.Count);
                    list.CopyTo(array, 0);
                    value = list = array;
                }
                GUI.changed = true;
            }
            else
            {
                list = (IList)value;
            }

            bool drawItems = DrawArrayFoldout(value.GetHashCode(), label,list.Count, () =>
            {
                if (fieldType.IsArray)
                {
                    Array array2 = Array.CreateInstance(elementType, 1);
                }
                else
                {
                    Type type = list.Count > 0 ? list[list.Count - 1].GetType() : elementType;
                    if (!typeof(UnityObject).IsAssignableFrom(type))
                    {
                        list.Add(ReflectionHelper.CreateInstance(type));
                    }
                    else
                    {
                        list.Add(null);
                    }
                }
            });
            
            if (drawItems)
            {
                EditorGUI.indentLevel++;
                
                for (int k = 0; k < list.Count; k++)
                {
                    object obj =  DrawArrayItem(list[k], k);
                    if (obj == null)
                    {
                        list.RemoveAt(k);
                    }
                    else
                    {
                        list[k] = obj;
                    }
                }
                
                EditorGUI.indentLevel--;
            }
            return list;
        }

        static bool DrawArrayFoldout(object value, GUIContent label, int cnt, Action addFunc)
        {
            string text = string.Concat(c_EditorPrefsFoldoutKey, value.GetHashCode(), ".", label.text);
            var @bool = GUIHelper.GetContextData(text, false);

            Rect rect = GUILayoutUtility.GetRect(50, 25);
            rect = EditorGUI.IndentedRect(rect);
            
            Rect addRect = new Rect(rect.position+new Vector2(rect.width-20,0),new Vector2(20,rect.height));
            
            Rect cntRect = new Rect(rect.position+new Vector2(rect.width-120,0),new Vector2(120,rect.height));
            
            Event current = Event.current;
            if (current.type == EventType.MouseDown && current.button == 0)
            {
                if (addRect.Contains(current.mousePosition))
                {
                    addFunc?.Invoke();
                }
                else
                {
                    if (rect.Contains(current.mousePosition))
                    {
                        @bool.value = !@bool.value;
                    }
                }
            }
                
            GUI.Box(rect, string.Empty, GUI.skin.button);
            
            GUI.Box(addRect, "+", GUI.skin.button);
            
            GUI.Box(cntRect, $"{cnt}items", GUI.skin.label);
                        
            Rect t = rect;
            t.xMin += 5;
            t.xMax -= 5;
            EditorGUI.Foldout(t, @bool.value, label);
            
            return @bool.value;
        }

        static object DrawArrayItem(object value,int index)
        {
            string text = string.Concat(c_EditorPrefsFoldoutKey, value.GetHashCode(), ".", value.GetType().Name);
            var @bool = GUIHelper.GetContextData(text, false);
            
            Rect rect = GUILayoutUtility.GetRect(50, 25);
            rect = EditorGUI.IndentedRect(rect);

            Rect removeRect = new Rect(rect.position+new Vector2(rect.width-20,0),new Vector2(20,rect.height));
            
            Event current = Event.current;
            if (current.type == EventType.MouseDown && current.button == 0)
            {
                if (removeRect.Contains(current.mousePosition))
                {
                    return null;
                }
                else
                {
                    if (rect.Contains(current.mousePosition))
                    {
                        @bool.value = !@bool.value;
                    }
                }
            }
                
            GUI.Box(rect, string.Empty, GUI.skin.button);
            
            GUI.Box(removeRect, "-", GUI.skin.button);

            Rect t = rect;
            t.xMin += 5;
            t.xMax -= 5;
            EditorGUI.Foldout(t, @bool.value, $"{index}-{value.GetType().Name}");

            if (@bool.value)
            {
                return DrawField(value.GetType(), value);
            }
            return value;
        }

        const string c_EditorPrefsFoldoutKey = "CZToolKit.Core.Editors.Foldout.";
    }
}