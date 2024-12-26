using System.Collections.Generic;
using UnityEngine;

namespace IAEngine
{
    public static class RectEx
    {
        /// <summary>
        /// 区域边框类型
        /// </summary>
        public enum RectBorder : byte
        {
            None = 0,
            Up, 
            Down, 
            Left, 
            Right,
        }

        /// <summary>
        /// 添加区域点时更新区域范围
        /// </summary>
        /// <param name="pRect"></param>
        /// <param name="pPos"></param>
        public static void UpdateRectOnAddPoint(ref RectInt pRect, Vector2Int pPos)
        {
            //本身就在区域内，不处理
            if (pRect.Contains(pPos))
                return;
            if (pRect.Equals(new RectInt(0, 0, 0, 0)))
            {
                pRect.min = new Vector2Int(pPos.x, pPos.y);
                pRect.max = new Vector2Int(pPos.x, pPos.y);
            }
            else
            {
                if (pPos.x < pRect.min.x)
                {
                    pRect.min = new Vector2Int(pPos.x, pRect.min.y);
                }
                else if (pPos.x > pRect.max.x)
                {
                    pRect.max = new Vector2Int(pPos.x, pRect.max.y);
                }

                if (pPos.y < pRect.min.y)
                {
                    pRect.min = new Vector2Int(pRect.min.x, pPos.y);
                }
                else if (pPos.y > pRect.max.y)
                {
                    pRect.max = new Vector2Int(pRect.max.x, pPos.y);
                }
            }
        }

        /// <summary>
        /// 删除区域点时更新区域范围
        /// </summary>
        /// <param name="pRect"></param>
        /// <param name="pPos"></param>
        public static void UpdateRectOnRemovePoint(ref RectInt pRect, Vector2Int pPos)
        {
            if (RectEx.CheckInBorder(pRect, pPos, out RectBorder border))
            {
                RemoveRectBorder(ref pRect, border);
            }
        }

        /// <summary>
        /// 获取边框坐标集合
        /// </summary>
        /// <param name="pRect"></param>
        /// <param name="pBorder"></param>
        /// <returns></returns>
        public static List<Vector2Int> GetPointListByBorder(RectInt pRect, RectBorder pBorder)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            //左右
            if (pBorder == RectBorder.Left || pBorder == RectBorder.Right)
            {
                int tX = pBorder == RectBorder.Left ? pRect.xMin : pRect.xMax;
                for (int y = pRect.yMin; y <= pRect.yMax; y++)
                {
                    points.Add(new Vector2Int(tX, y));
                }
            }
            //上下
            else if (pBorder == RectBorder.Up || pBorder == RectBorder.Down)
            {
                int tY = pBorder == RectBorder.Up ? pRect.yMax : pRect.yMin;
                for (int x = pRect.xMin; x <= pRect.xMax; x++)
                {
                    points.Add(new Vector2Int(x, tY));
                }
            }
            return points;
        }

        /// <summary>
        /// 删除区域边框
        /// </summary>
        /// <param name="pRect"></param>
        /// <param name="pBorder"></param>
        public static void RemoveRectBorder(ref RectInt pRect, RectBorder pBorder)
        {
            Vector2Int minPos = pRect.position;
            Vector2Int rectSize = pRect.size;

            if (pBorder == RectBorder.Left)
            {
                minPos.x += 1;
                rectSize.x -= 1;
            }
            else if (pBorder == RectBorder.Right) 
            {
                rectSize.x -= 1;
            }
            else if (pBorder == RectBorder.Up)
            {
                rectSize.y -= 1;
            }
            else if (pBorder == RectBorder.Down)
            {
                minPos.y += 1;
                rectSize.y -= 1;
            }

            pRect = new RectInt(minPos, rectSize);
        }

        /// <summary>
        /// 检测坐标在边框
        /// </summary>
        /// <param name="pRect"></param>
        /// <param name="pPos"></param>
        /// <returns></returns>
        public static bool CheckInBorder(RectInt pRect, Vector2Int pPos, out RectBorder outBorder)
        {
            //左右
            for (int y = pRect.yMin; y <= pRect.yMax; y++)
            {
                if (pPos.y == y)
                {
                    //左边
                    if (pPos.x == pRect.xMin)
                    {
                        outBorder = RectBorder.Left;
                        return true;
                    }
                    //右边
                    else if (pPos.x == pRect.xMax)
                    {
                        outBorder = RectBorder.Right;
                        return true;
                    }
                }
            }

            //上下
            for (int x = pRect.xMin; x <= pRect.xMax; x++)
            {
                if (pPos.x == x)
                {
                    //下边
                    if (pPos.y == pRect.yMin)
                    {
                        outBorder = RectBorder.Down;
                        return true;
                    }
                    //上边
                    else if (pPos.y == pRect.yMax)
                    {
                        outBorder = RectBorder.Up;
                        return true;
                    }
                }
            }

            outBorder = RectBorder.None;
            return false;
        }

        /// <summary>
        /// 随机取得一个坐标点
        /// </summary>
        /// <param name="pRect"></param>
        /// <returns></returns>
        public static Vector2Int RandomGetPoint(this RectInt pRect)
        {
            int x = RandomHelper.Range(pRect.min.x, pRect.max.x);
            int y = RandomHelper.Range(pRect.min.y, pRect.max.y);
            return new Vector2Int(x, y);
        }
    }
}
