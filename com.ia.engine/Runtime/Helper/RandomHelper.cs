using System;
using System.Collections.Generic;

namespace IAEngine
{
    public static class RandomHelper
    {
        private static readonly object lockObj = new object();

        private static Random GetRandom(int pAddSeed = 0)
        {
            lock (lockObj)
            {
                return new Random(DateTime.Now.Millisecond + pAddSeed);
            }
        }

        /// <summary>
        /// 随机一个布尔值
        /// </summary>
        /// <returns></returns>
        public static bool Range(int pAddSeed = 0)
        {
            return Range(0, 1) >= 1;
        }

        /// <summary>
        /// 随机整数
        /// </summary>
        /// <param name="pMin">最小数（包含）</param>
        /// <param name="pMax">最大数（包含）</param>
        /// <returns></returns>
        public static int Range(int pMin, int pMax, int pAddSeed = 0)
        {
            return GetRandom().Next(pMin, pMax + 1);
        }

        /// <summary>
        /// 随机浮点数
        /// </summary>
        /// <param name="pMin">最小数（包含）</param>
        /// <param name="pMax">最大数（包含）</param>
        /// <returns></returns>
        public static float Range(float pMin, float pMax, int pAddSeed = 0)
        {
            return (float)(pMin + (GetRandom().NextDouble() * (pMax - pMin)));
        }

        /// <summary>
        /// 获得一个打乱的字典
        /// </summary>
        /// <param name="pMin"></param>
        /// <param name="pMax"></param>
        /// <returns></returns>
        public static Dictionary<int,int> GetRandomNumList(int pMin, int pMax)
        {
            Dictionary<int, int> rsMap = new Dictionary<int, int>();
            for (int i = pMin; i <= pMax; i++)
            {
                rsMap.Add(i, i);
            }

            int num, tmp;
            for (int i = pMin; i <= pMax; i++)
            {
                num = Range(pMin, pMax);
                tmp = rsMap[i];
                rsMap[i] = rsMap[num];
                rsMap[num] = tmp;
            }

            return rsMap;
        }

        /// <summary>
        /// 打乱列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pList"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void ListRandom<T>(List<T> pList)
        {
            int num;
            T tmp;
            for (int i = 0; i < pList.Count; i++)
            {
                num = Range(0, pList.Count - 1);
                tmp = pList[i];
                pList[i] = pList[num];
                pList[num] = tmp;
            }
        }

        /// <summary>
        /// 权重随机取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pValueList">值列表</param>
        /// <param name="pWeightList">权重列表</param>
        /// <param name="outValue">返回值</param>
        /// <returns></returns>
        public static bool GetRandomValueByWeight<T>(List<T> pValueList, List<int> pWeightList, out int outIndex, out T outValue)
        {
            if (!pValueList.IsLegal() || !pWeightList.IsLegal())
            {
                outIndex = -1;
                outValue = default;
                return false;
            }

            if (pValueList.Count != pWeightList.Count)
            {
                outIndex = -1;
                outValue = default;
                return false;
            }

            int sum = 0;
            for (int i = 0; i < pWeightList.Count; i++)
            {
                sum += pWeightList[i];
            }

            int compareWeight = Range(0, sum + 1);
            int weightIndex = 0;
            while (sum > 0) 
            {
                sum -= pWeightList[weightIndex];
                if (sum < compareWeight)
                {
                    outIndex = weightIndex;
                    outValue = pValueList[weightIndex];
                    return false;
                }
                weightIndex++;
            }

            outIndex = -1;
            outValue = default;
            return false;
        }


    }
}
