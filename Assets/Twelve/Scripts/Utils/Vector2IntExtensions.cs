using UnityEngine;

namespace Twelve.Utils
{
    public static class Vector2IntExtensions
	{
        
        public static Vector2Int SetX(this Vector2Int origin, int X)
        {
            return new Vector2Int(X, origin.y);
        }

        public static Vector2Int SetY(this Vector2Int origin, int Y)
        {
            return new Vector2Int(origin.x, Y);
        }

        public static Vector2Int AddX(this Vector2Int origin, int X)
        {
            return new Vector2Int(origin.x + X, origin.y);
        }

        public static Vector2Int AddY(this Vector2Int origin, int Y)
        {
            return new Vector2Int(origin.x, origin.y + Y);
        }
    }
}
