using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace IAEngine
{
    public static class CSharpEx
    {
        #region Object

        /// <summary>
        /// 空检查
        /// </summary>
        public static bool IsNull(this object input) => input is null;

        /// <summary>
        /// 非空检查
        /// </summary>
        public static bool NotNull(this object input) => !IsNull(input);

        #endregion
        
        #region Array

        /// <summary>
        /// 列表是否合法
        /// </summary>
        /// <param name="pList"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsLegal<T>(this List<T> pList)
        {
            if (pList == null || pList.Count <= 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 堆栈是否合法
        /// </summary>
        /// <param name="pStack"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsLegal<T>(this Stack<T> pStack)
        {
            if (pStack == null || pStack.Count <= 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 队列是否合法
        /// </summary>
        /// <param name="pStack"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsLegal<T>(this Queue<T> pQueue)
        {
            if (pQueue == null || pQueue.Count <= 0)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Enum

        private static Dictionary<Enum, string> enumDesDict = new Dictionary<Enum, string>();

        /// <summary>
        /// 获取枚举描述
        /// </summary>
        public static string GetDescription(this Enum pEnum)
        {
            if (enumDesDict.ContainsKey(pEnum))
            {
                return enumDesDict[pEnum];
            }
            
            FieldInfo field = pEnum.GetType().GetField(pEnum.ToString());
            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();
            if (attribute == null)
            {
                enumDesDict.Add(pEnum, pEnum.ToString());
            }
            else
            {
                enumDesDict.Add(pEnum, attribute.Description);
            }
            
            return enumDesDict[pEnum];
        }
        
        #endregion
    }
}