using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAEngine
{
    /// <summary>
    /// 形状类型
    /// </summary>
    public enum ShapeType
    {
        /// <summary>
        /// 矩形
        /// </summary>
        AABB,

        /// <summary>
        /// 圆形
        /// </summary>
        Circle,

        /// <summary>
        /// 多边形
        /// </summary>
        Polygon,
    }

    /// <summary>
    /// 形状
    /// </summary>
    public struct Shape
    {
        public ShapeType ShapeType;

        public Vector2 AABBMin;
        public Vector2 AABBMax;

        public Vector2 Center;
        public float CircleRadius;

        public List<Vector2> PolygonVertices;

        /// <summary>
        /// 获得中心点
        /// </summary>
        /// <returns></returns>
        public Vector2 GetCenter()
        {
            switch (ShapeType)
            {
                case ShapeType.AABB:
                    return (AABBMin + AABBMax) / 2;
                case ShapeType.Circle:
                case ShapeType.Polygon:
                    return Center;
                default:
                    break;
            }
            return Center;
        }

        /// <summary>(
        /// 移动delta位置
        /// </summary>
        /// <param name="delta"></param>
        public void Translate(Vector2 delta)
        {
            switch (ShapeType)
            {
                case ShapeType.AABB:
                    AABBMin += delta;
                    AABBMax += delta;
                    break;
                case ShapeType.Circle:
                    Center += delta;
                    break;
                case ShapeType.Polygon:
                    Center += delta;
                    for (var i = 0; i < PolygonVertices.Count; i++)
                        PolygonVertices[i] += delta;
                    break;
            }
        }

        /// <summary>
        /// X轴翻折
        /// </summary>
        public void FlipX()
        {
            switch (ShapeType)
            {
                case ShapeType.AABB:
                    Vector2 min;
                    Vector2 max;
                    ShapeMath2D.AABBFlipX(AABBMin, AABBMax, out min, out max);
                    AABBMin = min;
                    AABBMax = max;
                    break;
                case ShapeType.Circle:
                    Center = new Vector2(Center.x * -1, Center.y);
                    break;
                case ShapeType.Polygon:
                    Debug.LogError("多边形需要旋转>>>>>>");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 旋转角度（多边形才可以旋转角度）
        /// </summary>
        /// <param name="degrees">角度</param>
        public void Rotate(float degrees)
        {
            if (ShapeType != ShapeType.Polygon)
                return;

            var vertexSum = Vector2.zero;
            for (var i = 0; i < PolygonVertices.Count; i++)
            {
                PolygonVertices[i] = PolygonVertices[i].RotatedByDegrees(degrees, Center);
                vertexSum += PolygonVertices[i];
            }

            Center = vertexSum / PolygonVertices.Count;
        }

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="size"></param>
        public void Scale(float size)
        {
            switch (ShapeType)
            {
                case ShapeType.AABB:
                    AABBMin *= size;
                    AABBMax *= size;
                    break;
                case ShapeType.Circle:
                    CircleRadius *= size;
                    break;
                case ShapeType.Polygon:
                    for (var i = 0; i < PolygonVertices.Count; i++)
                        PolygonVertices[i] *= size;
                    break;
            }
        }

        /// <summary>
        /// 检测包含点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool ContainPoint(Vector2 point)
        {
            switch (ShapeType)
            {
                case ShapeType.AABB:
                    return ShapeMath2D.AABBContainsPoint(AABBMin, AABBMax, point);
                case ShapeType.Circle:
                    return ShapeMath2D.CircleContainsPoint(Center, CircleRadius, point);
                case ShapeType.Polygon:
                    return ShapeMath2D.PolygonContainsPoint(PolygonVertices.ToArray(), point);
                default:
                    break;
            }
            return false;
        }

        /// <summary>
        /// 检测相交
        /// </summary>
        /// <param name="otherShape"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool Intersects(Shape otherShape)
        {
            switch (ShapeType)
            {
                case ShapeType.AABB:
                    switch (otherShape.ShapeType)
                    {
                        case ShapeType.AABB:
                            return ShapeMath2D.AABBIntersectsAABB(AABBMin, AABBMax, otherShape.AABBMin, otherShape.AABBMax);
                        case ShapeType.Circle:
                            return ShapeMath2D.CircleIntersectsAABB(otherShape.Center, otherShape.CircleRadius,
                                AABBMin, AABBMax);
                        case ShapeType.Polygon:
                            return ShapeMath2D.PolygonIntersectsAABB(otherShape.PolygonVertices.ToArray(), AABBMin, AABBMax);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ShapeType.Circle:
                    switch (otherShape.ShapeType)
                    {
                        case ShapeType.AABB:
                            return ShapeMath2D.CircleIntersectsAABB(Center, CircleRadius, otherShape.AABBMin, otherShape.AABBMax);
                        case ShapeType.Circle:
                            return ShapeMath2D.CircleIntersectsCircle(Center, CircleRadius, otherShape.Center, otherShape.CircleRadius);
                        case ShapeType.Polygon:
                            return ShapeMath2D.PolygonIntersectsCircle(otherShape.PolygonVertices.ToArray(), Center, CircleRadius);
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                case ShapeType.Polygon:
                    switch (otherShape.ShapeType)
                    {
                        case ShapeType.AABB:
                            return ShapeMath2D.PolygonIntersectsAABB(PolygonVertices.ToArray(), otherShape.AABBMin, otherShape.AABBMax);
                        case ShapeType.Circle:
                            return ShapeMath2D.PolygonIntersectsCircle(PolygonVertices.ToArray(), otherShape.Center, otherShape.CircleRadius);
                        case ShapeType.Polygon:
                            return ShapeMath2D.PolygonIntersectsPolygon(PolygonVertices.ToArray(), otherShape.PolygonVertices.ToArray());
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 获得顶点数
        /// </summary>
        /// <returns></returns>
        public int VertexCnt()
        {
            switch (ShapeType)
            {
                case ShapeType.AABB:
                    return 4;
                case ShapeType.Circle:
                    return 0;
                case ShapeType.Polygon:
                    return PolygonVertices.Count;
                default:
                    break;
            }
            return 0;
        }

        /// <summary>
        /// 绘制形状
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="_cachedVertices"></param>
        public static void RenderShape(Shape shape, Vector2[] _cachedVertices)
        {
            var numVertices = 0;
            switch (shape.ShapeType)
            {
                case ShapeType.AABB:
                    numVertices = 4;
                    ShapeMath2D.GetVerticesAABB(shape.AABBMin, shape.AABBMax, _cachedVertices);
                    break;
                case ShapeType.Circle:
                    Gizmos.DrawWireSphere(shape.Center, shape.CircleRadius);
                    return;
                case ShapeType.Polygon:
                    numVertices = shape.PolygonVertices.Count;
                    Array.Copy(shape.PolygonVertices.ToArray(), _cachedVertices, shape.PolygonVertices.Count);
                    break;
            }

            for (var i = 0; i < numVertices; i++)
            {
                var nextIndex = (i + 1) % numVertices;
                Gizmos.DrawLine(_cachedVertices[i], _cachedVertices[nextIndex]);
            }
        }
    }
}
