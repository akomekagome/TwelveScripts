using UnityEngine;

namespace Twelve.Game.Tiles
{
    public class BackTile : MonoBehaviour, ITile
    {
        private Vector2Int coordinates;
        public Vector2Int Coordinates => coordinates;
        private RectTransform rectTransform;

        public Vector3 GetRectPosition() => rectTransform.position;
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        public void SetCoordinates(Vector2Int coordinates) => this.coordinates = coordinates;
    }
}