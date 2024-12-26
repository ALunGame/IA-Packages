using UnityEngine;

namespace IAEngine
{
    public static class PolygonCollider2DEx
    {
        /// <summary>
        /// 获得碰撞盒世界坐标点
        /// </summary>
        /// <param name="collider2D"></param>
        /// <returns></returns>
        public static Vector2[] GetPoints(this PolygonCollider2D collider2D)
        {
            if (collider2D == null)
                return null;
            Vector2[] vectors = new Vector2[collider2D.GetTotalPointCount()];
            for (int i = 0; i < collider2D.GetTotalPointCount(); i++)
            {
                //本地坐标变为世界坐标
                vectors[i] = new Vector2(collider2D.gameObject.transform.position.x, collider2D.gameObject.transform.position.y) + collider2D.points[i];
            }
            return vectors;
        }

        /// <summary>
        /// 检测点在碰撞区域
        /// </summary>
        /// <param name="collider2D"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool ContainPoint(this PolygonCollider2D collider2D, Vector2 pos)
        {
            if (collider2D == null)
                return false;
            Vector2[] vector = new Vector2[collider2D.GetTotalPointCount()];
            for (int i = 0; i < collider2D.GetTotalPointCount(); i++)
            {
                //本地坐标变为屏幕坐标
                vector[i] = new Vector2(collider2D.gameObject.transform.position.x, collider2D.gameObject.transform.position.y) + collider2D.points[i];
            }

            return PositionPnpoly(vector, pos);
        }

        /// <summary>
        /// 判断点是否在多边形内.
        /// </summary>
        /// <param name="Overlaps">不规则坐标集合</param>
        /// <param name="p">当前点击坐标</param>
        /// <returns></returns>
        public static bool PositionPnpoly(Vector2[] Overlaps, Vector2 p)
        {
            if (Overlaps == null || Overlaps.Length <= 0 || p == null)
            {
                return false;
            }

            int i, j, c = 0;
            for (i = 0, j = Overlaps.Length - 1; i < Overlaps.Length; j = i++)
            {
                if (((Overlaps[i].y > p.y) != (Overlaps[j].y > p.y)) && (p.x < (Overlaps[j].x - Overlaps[i].x) * (p.y - Overlaps[i].y) / (Overlaps[j].y - Overlaps[i].y) + Overlaps[i].x))
                {
                    c = 1 + c;
                }
            }
            if (c % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
