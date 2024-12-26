using UnityEngine;

namespace IAEngine
{
    /// <summary>
    /// Unity杂项扩展
    /// </summary>
    public static class UnityMiscEx
    {
        public static Rect ToRect(this Bounds pBounds)
        {
            // Vector2 leftPos = new Vector2(pBounds.center.x-pBounds.extents.x,pBounds.center.y-pBounds.extents.y);
            // return new Rect(leftPos, pBounds.size);
            return new Rect(pBounds.min, pBounds.max - pBounds.min);
        }
    }
}