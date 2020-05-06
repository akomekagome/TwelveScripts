using UnityEngine;

namespace Twelve.Game.Tiles
{
    [System.Serializable]
    public struct TileInfo
    {
        public Vector2Int coordinates;
        public int level;

        public TileInfo(Vector2Int coordinates, int level)
        {
            this.coordinates = coordinates;
            this.level = level;
        }
    }
}