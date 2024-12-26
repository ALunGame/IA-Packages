using System;
using System.Collections.Generic;
using UnityEngine;

namespace IAToolkit
{
    public static partial class GUIExtension
    {
        static Stack<Font> fonts = new Stack<Font>();
        /// <summary>
        /// 设置环境字体
        /// </summary>
        /// <param name="_font"></param>
        public static void SetFont(Font _font,Action drawFunc)
        {
            fonts.Push(GUI.skin.font);
            GUI.skin.font = _font;
            drawFunc?.Invoke();
            GUI.skin.font = fonts.Pop();
        }

        static Stack<Color> colors = new Stack<Color>();
        /// <summary>
        /// 设置环境颜色
        /// </summary>
        /// <param name="_color"></param>
        public static void SetColor(Color _color, Action drawFunc)
        {
            colors.Push(GUI.color);
            GUI.color = _color;
            drawFunc?.Invoke();
            GUI.color = colors.Pop();
        }

        /// <summary>
        /// 设置环境透明
        /// </summary>
        /// <param name="alpha"></param>
        public static void SetAlpha(float alpha, Action drawFunc)
        {
            Color color = GUI.color;
            color.a *= alpha;
            SetColor(color, drawFunc);
        }

        static Stack<Matrix4x4> matrixs = new Stack<Matrix4x4>();
        /// <summary>
        /// 设置环境矩阵区域
        /// </summary>
        /// <param name="alpha"></param>
        public static void SetMatrix(Matrix4x4 matrix4X4, Action drawFunc)
        {
            matrixs.Push(GUI.matrix);
            GUI.matrix = matrix4X4;
            drawFunc?.Invoke();
            GUI.matrix = matrixs.Pop();
        }

        /// <summary>
        /// 设置缩放区域
        /// </summary>
        /// <param name="scale">尺寸</param>
        /// <param name="rect">区域</param>
        /// <param name="pivot">中点</param>
        public static void SetScale(Vector2 scale, Rect rect, Vector2 pivot, Action drawFunc)
        {
            Rect Scale(Rect _targetValue, Vector2 _scale, Vector2 _pivot)
            {
                Vector2 absPosition = _targetValue.position + _targetValue.size * _pivot;
                Vector2 size = _targetValue.size;
                size.x *= _scale.x;
                size.y *= _scale.y;
                _targetValue.size = size;
                _targetValue.position = absPosition - _targetValue.size * _pivot;
                return _targetValue;
            }

            Rect r = Scale(rect, scale, pivot);
            Vector2 offset = new Vector2(r.x - rect.x, r.y - rect.y);
            Matrix4x4 matrix = GUI.matrix;
            matrix.m03 += offset.x;
            matrix.m13 += offset.y;
            matrix.m00 *= scale.x;
            matrix.m11 *= scale.y;
            SetMatrix(matrix, drawFunc);
        }
    }
}
