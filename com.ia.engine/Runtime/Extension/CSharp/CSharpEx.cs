using System.Collections.Generic;

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

    }
}