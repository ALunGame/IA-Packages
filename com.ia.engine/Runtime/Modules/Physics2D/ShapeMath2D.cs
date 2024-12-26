
using System;
using System.Collections;
using UnityEngine;

namespace IAEngine
{
    /// <summary>
    /// 2D数学库（主要求解多边形相交问题）
    /// 1，现在只要凸边形
    /// 2，不规则多边形必须按照顺时针排列
    /// </summary>
    public static class ShapeMath2D
    {
        public const float DegToRad = Mathf.PI / 180f;
        public const float PI2 = Mathf.PI * 2f;

        #region Line

        public static Vector2 ClosestPointOnLine(Vector2 a, Vector2 b, Vector2 point)
        {
            var ap = point - a;
            var ab = b - a;
            var dist = Vector2.Dot(ap, ab) / ab.sqrMagnitude;
            return a + ab * dist;
        }

        public static Vector2 ClosestPointOnLineSegment(Vector2 a, Vector2 b, Vector2 point)
        {
            var ap = point - a;
            var ab = b - a;
            var dist = Vector2.Dot(ap, ab) / ab.sqrMagnitude;
            return dist switch
            {
                < 0 => a,
                > 1 => b,
                _ => a + ab * dist
            };
        }

        public static bool PointIsOnLeftSideOfLine(Vector2 a, Vector2 b, Vector2 point) =>
            (b.x - a.x) * (point.y - a.y) - (b.y - a.y) * (point.x - a.x) > 0;

        public static bool LineIntersectsLine(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2,
            out Vector2 intersection, float tolerance = 0.0001f)
        {
            intersection = default;

            var x1 = a1.x;
            var x2 = b1.x;
            var x3 = a2.x;
            var x4 = b2.x;

            var y1 = a1.y;
            var y2 = b1.y;
            var y3 = a2.y;
            var y4 = b2.y;

            // equations of the form x = c (two vertical lines)
            if (Mathf.Abs(x1 - x2) < tolerance && Mathf.Abs(x3 - x4) < tolerance && Mathf.Abs(x1 - x3) < tolerance)
                return false;

            //equations of the form y=c (two horizontal lines)
            if (Mathf.Abs(y1 - y2) < tolerance && Mathf.Abs(y3 - y4) < tolerance && Mathf.Abs(y1 - y3) < tolerance)
                return false;

            //equations of the form x=c (two vertical parallel lines)
            if (Mathf.Abs(x1 - x2) < tolerance && Mathf.Abs(x3 - x4) < tolerance)
                return false;

            //equations of the form y=c (two horizontal parallel lines)
            if (Mathf.Abs(y1 - y2) < tolerance && Mathf.Abs(y3 - y4) < tolerance)
                return false;

            //general equation of line is y = mx + c where m is the slope
            //assume equation of line 1 as y1 = m1x1 + c1 
            //=> -m1x1 + y1 = c1 ----(1)
            //assume equation of line 2 as y2 = m2x2 + c2
            //=> -m2x2 + y2 = c2 -----(2)
            //if line 1 and 2 intersect then x1=x2=x & y1=y2=y where (x,y) is the intersection p
            //so we will get below two equations 
            //-m1x + y = c1 --------(3)
            //-m2x + y = c2 --------(4)

            float x, y;

            //lineA is vertical x1 = x2
            //slope will be infinity
            //so lets derive another solution
            if (Mathf.Abs(x1 - x2) < tolerance)
            {
                //compute slope of line 2 (m2) and c2
                var m2 = (y4 - y3) / (x4 - x3);
                var c2 = -m2 * x3 + y3;

                //equation of vertical line is x = c
                //if line 1 and 2 intersect then x1=c1=x
                //subsitute x=x1 in (4) => -m2x1 + y = c2
                // => y = c2 + m2x1 
                x = x1;
                y = c2 + m2 * x1;
            }
            //other is vertical x3 = x4
            //slope will be infinity
            //so lets derive another solution
            else if (Mathf.Abs(x3 - x4) < tolerance)
            {
                //compute slope of line 1 (m1) and c2
                var m1 = (y2 - y1) / (x2 - x1);
                var c1 = -m1 * x1 + y1;

                //equation of vertical line is x = c
                //if line 1 and 2 intersect then x3=c3=x
                //subsitute x=x3 in (3) => -m1x3 + y = c1
                // => y = c1 + m1x3 
                x = x3;
                y = c1 + m1 * x3;
            }
            //lineA & other are not vertical 
            //(could be horizontal we can handle it with slope = 0)
            else
            {
                //compute slope of line 1 (m1) and c2
                var m1 = (y2 - y1) / (x2 - x1);
                var c1 = -m1 * x1 + y1;

                //compute slope of line 2 (m2) and c2
                var m2 = (y4 - y3) / (x4 - x3);
                var c2 = -m2 * x3 + y3;

                //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
                //plugging x value in equation (4) => y = c2 + m2 * x
                x = (c1 - c2) / (m2 - m1);
                y = c2 + m2 * x;

                //verify by plugging intersection p (x, y)
                //in orginal equations (1) & (2) to see if they intersect
                //otherwise x,y values will not be finite and will fail this check
                if (!(Mathf.Abs(-m1 * x + y - c1) < tolerance
                      && Mathf.Abs(-m2 * x + y - c2) < tolerance))
                {
                    //return default (no intersection)
                    return false;
                }
            }

            //x,y can intersect outside the line segment since line is infinitely long
            //so finally check if x, y is within both the line segments
            intersection = new Vector2(x, y);
            return true;
        }

        public static bool LineIntersectsLineSegment(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2,
            out Vector2 intersection) => LineIntersectsLine(a1, b1, a2, b2, out intersection) &&
                                         IsInsideLineSegment(a2, b2, intersection);

        public static bool LineSegmentIntersectsLineSegment(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2,
            out Vector2 intersection) => LineIntersectsLineSegment(a1, b1, a2, b2, out intersection) &&
                   IsInsideLineSegment(a1, b1, intersection);

        private static bool IsInsideLineSegment(Vector2 a, Vector2 b, Vector2 point) =>
            (point.x >= a.x && point.x <= b.x || point.x >= b.x && point.x <= a.x) &&
            (point.y >= a.y && point.y <= b.y || point.y >= b.y && point.y <= a.y);

        #endregion

        #region AABB

        /// <summary>
        /// 检测点在矩形范围内
        /// </summary>
        /// <param name="min">矩形最小点</param>
        /// <param name="max">矩形最大点</param>
        /// <param name="point">检测点</param>
        /// <returns></returns>
        public static bool AABBContainsPoint(Vector2 min, Vector2 max, Vector2 point) =>
           point.x >= min.x && point.x <= max.x && point.y >= min.y && point.y <= max.y;

        /// <summary>
        /// 检测矩形与矩形相交
        /// </summary>
        /// <returns></returns>
        public static bool AABBIntersectsAABB(Vector2 minA, Vector2 maxA, Vector2 minB, Vector2 maxB) =>
            maxA.x - minA.x + maxB.x - minB.x > Mathf.Max(maxA.x, maxB.x) - Mathf.Min(minA.x, minB.x)
            && maxA.y - minA.y + maxB.y - minB.y > Mathf.Max(maxA.y, maxB.y) - Mathf.Min(minA.y, minB.y);

        /// <summary>
        /// 获得矩形四个顶点
        /// </summary>
        public static unsafe void GetVerticesAABB(Vector2 min, Vector2 max, Vector2[] vertices)
        {
            fixed (Vector2* ptr = vertices)
                GetVerticesAABBUnsafe(min, max, ptr);
        }

        /// <summary>
        /// X轴翻折
        /// </summary>
        public static void AABBFlipX(Vector2 min, Vector2 max, out Vector2 aabbMin, out Vector2 aabbMax)
        {
            Vector2[] vertices = new Vector2[4];
            GetVerticesAABB(min, max, vertices);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector2(vertices[i].x * -1, vertices[i].y);
            }
            GetBoundingAABB(vertices, out aabbMin, out aabbMax);
        }

        /// <summary>
        /// 获得矩形四个顶点
        /// </summary>
        public static unsafe void GetVerticesAABBUnsafe(Vector2 min, Vector2 max, Vector2* vertices)
        {
            vertices[0] = max;
            vertices[1] = new Vector2(max.x, min.y);
            vertices[2] = min;
            vertices[3] = new Vector2(min.x, max.y);
        }

        /// <summary>
        /// 获得矩形包围盒
        /// </summary>
        public static unsafe void GetBoundingAABB(Vector2[] vertices, out Vector2 aabbMin, out Vector2 aabbMax)
        {
            fixed (Vector2* ptr = vertices)
                GetBoundingAABBUnsafe(ptr, vertices.Length, out aabbMin, out aabbMax);
        }

        /// <summary>
        /// 获得矩形包围盒
        /// </summary>
        public static unsafe void GetBoundingAABBUnsafe(Vector2* vertices, int length, out Vector2 aabbMin, out Vector2 aabbMax)
        {
            aabbMin = default;
            aabbMax = default;
            for (var i = 0; i < length; i++)
            {
                aabbMin = new Vector2(Mathf.Min(aabbMin.x, vertices[i].x), Mathf.Min(aabbMin.y, vertices[i].y));
                aabbMax = new Vector2(Mathf.Max(aabbMax.x, vertices[i].x), Mathf.Max(aabbMax.y, vertices[i].y));
            }
        }

        #endregion

        #region Circle

        /// <summary>
        /// 检测点在圆内
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool CircleContainsPoint(Vector2 center, float radius, Vector2 point) =>
            (point - center).sqrMagnitude <= radius * radius;

        /// <summary>
        /// 圆与矩形相交
        /// </summary>
        /// <param name="center">圆心</param>
        /// <param name="radius">圆半径</param>
        /// <param name="aabbMin">矩形最小</param>
        /// <param name="aabbMax">矩形最大</param>
        /// <returns></returns>
        public static unsafe bool CircleIntersectsAABB(Vector2 center, float radius, Vector2 aabbMin, Vector2 aabbMax)
        {
            if (AABBContainsPoint(aabbMin, aabbMax, center))
                return true;

            var aabbVertices = stackalloc Vector2[4];
            GetVerticesAABBUnsafe(aabbMin, aabbMax, aabbVertices);
            for (var i = 0; i < 4; i++)
            {
                var nextIndex = (i + 1) % 4;
                var closestPointOnEdge = ClosestPointOnLineSegment(aabbVertices[i], aabbVertices[nextIndex], center);
                if (CircleContainsPoint(center, radius, closestPointOnEdge))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 圆与圆相交
        /// </summary>
        /// <param name="centerA"></param>
        /// <param name="radiusA"></param>
        /// <param name="centerB"></param>
        /// <param name="radiusB"></param>
        /// <returns></returns>
        public static bool CircleIntersectsCircle(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
        {
            var radiusSum = radiusA + radiusB;
            return (centerB - centerA).sqrMagnitude <= radiusSum * radiusSum;
        }

        /// <summary>
        /// 获得圆的边界点
        /// </summary>
        /// <param name="points"></param>
        /// <param name="circleCenter"></param>
        /// <param name="circleRadius"></param>
        public static unsafe void GetBoundingCircle(Vector2[] points, out Vector2 circleCenter, out float circleRadius)
        {
            fixed (Vector2* ptr = points)
                GetBoundingCircleUnsafe(ptr, points.Length, out circleCenter, out circleRadius);
        }

        /// <summary>
        /// Implements Welzl's algorithm for finding the smallest bounding circle containing a set of points in O(n) time.
        /// This website was very helpful: http://www.sunshine2k.de/coding/java/Welzl/Welzl.html
        /// </summary>
        public static unsafe void GetBoundingCircleUnsafe(Vector2* points, int length, out Vector2 circleCenter,
            out float circleRadius)
        {
            var pointsOnCircle = stackalloc Vector2[3];

            welzl(length, 0, out circleCenter, out circleRadius);

            void welzl(int numUnchecked, int numPointsOnCircle, out Vector2 c, out float r)
            {
                if (numUnchecked <= 0 || numPointsOnCircle == 3)
                {
                    calculateCircle(numPointsOnCircle, out c, out r);
                    return;
                }

                var p = points[numUnchecked - 1];
                welzl(numUnchecked - 1, numPointsOnCircle, out c, out r);
                if (CircleContainsPoint(c, r, p))
                    return;

                pointsOnCircle[numPointsOnCircle] = p;
                welzl(numUnchecked - 1, numPointsOnCircle + 1, out c, out r);
            }

            void calculateCircle(int numPointsOnCircle, out Vector2 c, out float r)
            {
                c = default;
                r = default;
                switch (numPointsOnCircle)
                {
                    case 1:
                        c = pointsOnCircle[0];
                        r = 0f;
                        break;
                    case 2:
                        c = (pointsOnCircle[1] + pointsOnCircle[0]) / 2f;
                        r = (pointsOnCircle[1] - pointsOnCircle[0]).magnitude / 2f;
                        break;
                    case 3:
                        GetCircleFromTriangleUnsafe(pointsOnCircle, out c, out r);
                        break;
                }
            }
        }

        public static unsafe void GetCircleFromTriangle(Vector2[] points, out Vector2 circleCenter,
            out float circleRadius)
        {
            fixed (Vector2* ptr = points)
                GetCircleFromTriangleUnsafe(ptr, out circleCenter, out circleRadius);
        }

        public static unsafe void GetCircleFromTriangleUnsafe(Vector2* points, out Vector2 circleCenter,
            out float circleRadius)
        {
            GetLongestEdgeOfPolygonUnsafe(points, 3, out var longestEdge);

            var a = (longestEdge + 1) % 3;
            var b = (longestEdge + 2) % 3;
            var aToBMiddle = (points[a] + points[b]) / 2f;
            var bToCMiddle = (points[b] + points[longestEdge]) / 2f;
            var perpA = (aToBMiddle - points[a]).RotatedByRadians(MathF.PI / 2f) + aToBMiddle;
            var perpB = (bToCMiddle - points[b]).RotatedByRadians(MathF.PI / 2f) + bToCMiddle;

            LineIntersectsLine(aToBMiddle, perpA, bToCMiddle, perpB, out circleCenter);

            circleRadius = (points[0] - circleCenter).magnitude;
        }

        #endregion

        #region Polygon

        /// <summary>
        /// 检测点包含多边形内
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static unsafe bool PolygonContainsPoint(Vector2[] vertices, Vector2 point)
        {
            fixed (Vector2* ptr = vertices)
                return PolygonContainsPointUnsafe(ptr, vertices.Length, point);
        }

        public static unsafe bool PolygonContainsPointUnsafe(Vector2* vertices, int length, Vector2 point)
        {
            var firstSide = PointIsOnLeftSideOfLine(vertices[0], vertices[1], point);
            for (var i = 1; i < length; i++)
            {
                var nextIndex = (i + 1) % length;
                if (firstSide != PointIsOnLeftSideOfLine(vertices[i], vertices[nextIndex], point))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 多边形和矩形相交
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="aabbMin"></param>
        /// <param name="aabbMax"></param>
        /// <returns></returns>
        public static unsafe bool PolygonIntersectsAABB(Vector2[] vertices, Vector2 aabbMin, Vector2 aabbMax)
        {
            fixed (Vector2* ptr = vertices)
                return PolygonIntersectsAABBUnsafe(ptr, vertices.Length, aabbMin, aabbMax);
        }

        public static unsafe bool PolygonIntersectsAABBUnsafe(Vector2* vertices, int length, Vector2 aabbMin, Vector2 aabbMax)
        {
            for (var i = 0; i < length; i++)
            {
                if (AABBContainsPoint(aabbMin, aabbMax, vertices[i]))
                    return true;
            }

            var aabbVertices = stackalloc Vector2[4];
            GetVerticesAABBUnsafe(aabbMin, aabbMax, aabbVertices);

            for (var i = 0; i < 4; i++)
            {
                if (PolygonContainsPointUnsafe(vertices, length, aabbVertices[i]))
                    return true;
            }

            for (var i = 0; i < length; i++)
            {
                var a = vertices[i];
                var b = vertices[(i + 1) % length];
                for (var j = 0; j < 4; j++)
                {
                    var otherA = aabbVertices[j];
                    var otherB = aabbVertices[(j + 1) % 4];

                    if (LineSegmentIntersectsLineSegment(a, b, otherA, otherB, out _))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 矩形与圆相交
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="circleCenter"></param>
        /// <param name="circleRadius"></param>
        /// <returns></returns>
        public static unsafe bool PolygonIntersectsCircle(Vector2[] vertices, Vector2 circleCenter, float circleRadius)
        {
            fixed (Vector2* ptr = vertices)
                return PolygonIntersectsCircleUnsafe(ptr, vertices.Length, circleCenter, circleRadius);
        }

        public static unsafe bool PolygonIntersectsCircleUnsafe(Vector2* vertices, int length, Vector2 circleCenter,
            float circleRadius)
        {
            if (PolygonContainsPointUnsafe(vertices, length, circleCenter))
                return true;

            for (var i = 0; i < length; i++)
            {
                var nextIndex = (i + 1) % length;
                var closestPoint = ClosestPointOnLineSegment(vertices[i], vertices[nextIndex], circleCenter);
                if (CircleContainsPoint(circleCenter, circleRadius, closestPoint))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 多边形与多边形相交
        /// </summary>
        /// <param name="verticesA"></param>
        /// <param name="verticesB"></param>
        /// <returns></returns>
        public static unsafe bool PolygonIntersectsPolygon(Vector2[] verticesA, Vector2[] verticesB)
        {
            fixed (Vector2* ptrA = verticesA)
            {
                fixed (Vector2* ptrB = verticesB)
                    return PolygonIntersectsPolygonUnsafe(ptrA, verticesA.Length, ptrB, verticesB.Length);
            }
        }

        public static unsafe bool PolygonIntersectsPolygonUnsafe(Vector2* verticesA, int lengthA, Vector2* verticesB,
            int lengthB)
        {
            for (var i = 0; i < lengthA; i++)
            {
                if (PolygonContainsPointUnsafe(verticesB, lengthB, verticesA[i]))
                    return true;
            }

            for (var i = 0; i < lengthB; i++)
            {
                if (PolygonContainsPointUnsafe(verticesA, lengthA, verticesB[i]))
                    return true;
            }

            for (var i = 0; i < lengthA; i++)
            {
                var nextIndexA = (i + 1) % lengthA;
                for (var j = 0; j < lengthB; j++)
                {
                    var nextIndexB = (j + 1) % lengthB;
                    if (LineSegmentIntersectsLineSegment(verticesA[i], verticesA[nextIndexA], verticesB[j],
                            verticesB[nextIndexB], out _))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获得多边形中心
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="center"></param>
        public static unsafe void GetCenterOfPolygon(Vector2[] vertices, out Vector2 center)
        {
            fixed (Vector2* ptr = vertices)
                GetCenterOfPolygonUnsafe(ptr, vertices.Length, out center);
        }

        public static unsafe void GetCenterOfPolygonUnsafe(Vector2* vertices, int length, out Vector2 center)
        {
            center = Vector2.zero;
            for (var i = 0; i < length; i++)
                center += vertices[i];

            center /= length;
        }

        /// <summary>
        /// 获得多边形最长一边
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="longestEdge"></param>
        public static unsafe void GetLongestEdgeOfPolygon(Vector2[] vertices, out int longestEdge)
        {
            fixed (Vector2* ptr = vertices)
                GetLongestEdgeOfPolygonUnsafe(ptr, vertices.Length, out longestEdge);
        }

        public static unsafe void GetLongestEdgeOfPolygonUnsafe(Vector2* vertices, int length, out int longestEdge)
        {
            var longestLengthSquared = (vertices[1] - vertices[0]).sqrMagnitude;
            longestEdge = 0;
            for (var i = 1; i < length; i++)
            {
                var nextVertex = (i + 1) % length;
                var lengthSquared = (vertices[nextVertex] - vertices[i]).sqrMagnitude;
                if (!(lengthSquared > longestLengthSquared))
                    continue;

                longestEdge = i;
                longestLengthSquared = lengthSquared;
            }
        }

        /// <summary>
        /// 圆转多边形
        /// </summary>
        /// <param name="circleCenter"></param>
        /// <param name="circleRadius"></param>
        /// <param name="vertices"></param>
        public static unsafe void GetPolygonFromCircle(Vector2 circleCenter, float circleRadius, Vector2[] vertices)
        {
            fixed (Vector2* ptr = vertices)
                GetPolygonFromCircleUnsafe(circleCenter, circleRadius, ptr, vertices.Length);
        }

        public static unsafe void GetPolygonFromCircleUnsafe(Vector2 circleCenter, float circleRadius,
            Vector2* vertices, int length)
        {
            var angleStep = Mathf.PI * 2f / length;
            for (var i = 0; i < length; i++)
                vertices[i] = circleCenter + new Vector2(circleRadius, 0f).RotatedByRadians(angleStep * i);
        }

        /// <summary>
        /// 获得多边形包围盒
        /// </summary>
        /// <param name="points"></param>
        /// <param name="boundingPolygonVertices"></param>
        /// <param name="numBoundingPolygonVertices"></param>
        public static unsafe void GetBoundingPolygon(Vector2[] points, Vector2[] boundingPolygonVertices, out int numBoundingPolygonVertices)
        {
            fixed (Vector2* ptrA = points)
            {
                fixed (Vector2* ptrB = boundingPolygonVertices)
                    GetBoundingPolygonUnsafe(ptrA, points.Length, ptrB, boundingPolygonVertices.Length, out numBoundingPolygonVertices);
            }
        }

        public static unsafe void GetBoundingPolygonUnsafe(Vector2* points, int pointsLength,
            Vector2* boundingPolygonVertices, int boundingPolygonVerticesLength, out int numBoundingPolygonVertices)
        {
            numBoundingPolygonVertices = 0;
            var leftMostIndex = 0;
            for (var i = 1; i < pointsLength; i++)
            {
                if (points[i].x < points[leftMostIndex].x)
                    leftMostIndex = i;
            }

            var p = leftMostIndex;
            do
            {
                boundingPolygonVertices[numBoundingPolygonVertices++] = points[p];
                var q = (p + 1) % pointsLength;
                for (var i = 0; i < pointsLength; i++)
                {
                    if (orientation(points[p], points[i], points[q]) == 2)
                        q = i;
                }
                p = q;
            } while (p != leftMostIndex);

            int orientation(Vector2 p, Vector2 q, Vector2 r)
            {
                var val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
                if (val == 0f) return 0;
                return val > 0f ? 1 : 2;
            }
        }

        #endregion

        #region Vector2

        /// <summary>
        /// 坐标旋转角度
        /// </summary>
        /// <param name="v"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Vector2 RotatedByDegrees(this Vector2 v, float degrees) => v.RotatedByDegrees(degrees, Vector2.zero);

        public static Vector2 RotatedByDegrees(this Vector2 v, float degrees, Vector2 center) =>
            v.RotatedByRadians(degrees * DegToRad, center);

        /// <summary>
        /// 坐标旋转弧度
        /// </summary>
        /// <param name="v"></param>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Vector2 RotatedByRadians(this Vector2 v, float radians) => v.RotatedByRadians(radians, Vector2.zero);

        public static Vector2 RotatedByRadians(this Vector2 v, float radians, Vector2 center)
        {
            var cosTheta = Mathf.Cos(radians);
            var sinTheta = Mathf.Sin(radians);
            return new Vector2
            {
                x = cosTheta * (v.x - center.x) - sinTheta * (v.y - center.y) + center.x,
                y = sinTheta * (v.x - center.x) + cosTheta * (v.y - center.y) + center.y,
            };
        }

        /// <summary>
        /// 夹角
        /// </summary>
        /// <param name="v"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static float AngleBetween(this Vector2 v, Vector2 p) => Mathf.Acos(Vector2.Dot(v, p) / (v.magnitude * p.magnitude));

        public static float AngleBetweenSigned(this Vector2 v, Vector2 p) =>
            v.AngleBetween(p) * (PointIsOnLeftSideOfLine(Vector2.zero, v, p) ? 1f : -1f);

        #endregion

        #region Miscellaneous

        private static unsafe void SortArrayUnsafe<T>(T* arr, int left, int right, Comparison<T> comp) where T : unmanaged
        {
            if (left >= right)
                return;

            var p = partition(left, right);
            SortArrayUnsafe(arr, left, p - 1, comp);
            SortArrayUnsafe(arr, p + 1, right, comp);

            void swap(int a, int b) => (arr[a], arr[b]) = (arr[b], arr[a]);

            int partition(int l, int r)
            {
                var pValue = arr[r];
                var i = l - 1;
                for (var j = l; j <= r - 1; j++)
                {
                    if (comp(arr[j], pValue) >= 0)
                        continue;

                    i++;
                    swap(i, j);
                }

                swap(i + 1, r);
                return i + 1;
            }
        }

        public static float SmallestAngleDifferenceDegrees(float fromDeg, float toDeg) =>
            SmallestAngleDifferenceRadians(fromDeg * DegToRad, toDeg * DegToRad);

        public static float SmallestAngleDifferenceRadians(float fromRad, float toRad)
        {
            var diff = (toRad - fromRad + Mathf.PI) % Mathf.PI;
            return diff < -Mathf.PI ? diff + Mathf.PI * 2f : diff;
        }

        #endregion
    }
}