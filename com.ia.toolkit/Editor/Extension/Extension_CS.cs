using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IAToolkit
{
    public static partial class Extension_CS
    {
        public static bool TryGet<T>(this T[] array, int index, out T element)
        {
            element = default;
            if (array.Length > index)
            {
                element = array[index];
                return true;
            }

            return false;
        }

        /// <summary> 快速排序(第二个参数是中间值) </summary>
        public static void QuickSort<T>(this IList<T> original, Func<T, T, int> comparer)
        {
            QuickSort(0, original.Count - 1);

            void QuickSort(int left, int right)
            {
                if (left < right)
                {
                    int middleIndex = (left + right) / 2;
                    T middle = original[middleIndex];
                    int i = left;
                    int j = right;
                    while (true)
                    {
                        // 双指针收缩
                        // 找到一个大于中数的下标和一个小于中数的下标，交换位置
                        while (i < j && comparer(original[i], middle) < 0)
                        {
                            i++;
                        }

                        ;
                        while (j > i && comparer(original[j], middle) > 0)
                        {
                            j--;
                        }

                        ;
                        if (i == j) break;

                        T temp = original[i];
                        original[i] = original[j];
                        original[j] = temp;

                        if (comparer(original[i], original[j]) == 0) j--;
                    }

                    QuickSort(left, i);
                    QuickSort(i + 1, right);
                }
            }
        }

        /// <summary>
        /// 是否可以强转
        /// </summary>
        /// <param name="type"></param>
        /// <param name="otherType"></param>
        /// <returns></returns>
        public static bool IsReallyAssignableFrom(this Type type, Type otherType)
        {
            if (type.IsAssignableFrom(otherType))
                return true;
            if (otherType.IsAssignableFrom(type))
                return true;

            try
            {
                var v = Expression.Variable(otherType);
                var expr = Expression.Convert(v, type);
                return expr.Method != null && expr.Method.Name != "op_Implicit";
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
