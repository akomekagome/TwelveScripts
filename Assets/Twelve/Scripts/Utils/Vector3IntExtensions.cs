using UnityEngine;

namespace Twelve.Utils
{
    public static class Vector3IntExtensions
	{
        
        public static Vector3Int SetX(this Vector3Int origin, int X)
        {
            return new Vector3Int(X, origin.y, origin.z);
        }

        public static Vector3Int SetY(this Vector3Int origin, int Y)
        {
            return new Vector3Int(origin.x, Y, origin.z);
        }

        public static Vector3Int SetZ(this Vector3Int origin, int Z)
        {
            return new Vector3Int(origin.x, origin.y, Z);
        }

        public static Vector3Int AddX(this Vector3Int origin, int X)
        {
            return new Vector3Int(origin.x + X, origin.y, origin.z);
        }

        public static Vector3Int AddY(this Vector3Int origin, int Y)
        {
            return new Vector3Int(origin.x, origin.y + Y, origin.z);
        }

        public static Vector3Int AddZ(this Vector3Int origin, int Z)
        {
            return new Vector3Int(origin.x, origin.y, origin.z + Z);
        }
    }
}
