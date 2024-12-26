using System.Collections;
using UnityEngine;

namespace IAEngine
{
    public static class VectorEx
    {
        public static Vector2 ToVector2(this Vector3 pos)
        {
            return new Vector2(pos.x, pos.y);
        }

        public static Vector2Int ToVectorInt2(this Vector3Int pos)
        {
            return new Vector2Int(pos.x, pos.y);
        }
        
        public static Vector3 ToVector3(this Vector2 pos,float zValue = 0)
        {
            return new Vector3(pos.x, pos.y, zValue);
        }
        
        public static Vector3Int ToVectorInt3(this Vector2Int pos,int zValue = 0)
        {
            return new Vector3Int(pos.x, pos.y, zValue);
        }
    }
}