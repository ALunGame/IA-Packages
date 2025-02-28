using UnityEngine;

namespace IAToolkit
{
    public static class Extension_Vector
    {
        public static Vector2 ToVector2(this Vector2Int self)
        {
            return new Vector2(self.x, self.y);
        }

        public static Vector2Int ToVector2Int(this Vector2 self)
        {
            return new Vector2Int((int)self.x, (int)self.y);
        }

        public static Vector3 ToVector3(this Vector3Int self)
        {
            return new Vector3(self.x, self.y, self.z);
        }

        public static Vector3Int ToVector3Int(this Vector3 self)
        {
            return new Vector3Int((int)self.x, (int)self.y, (int)self.z);
        }
    }
}
