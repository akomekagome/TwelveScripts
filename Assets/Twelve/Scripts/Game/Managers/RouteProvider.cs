using UnityEngine;
using System.Collections.Generic;

namespace Twelve.Game.Mangers
{
    /// <summary>
    /// astarの機能を提供する
    /// </summary>
    public class RouteProvider : MonoBehaviour
    {
        private AStar aStar;

        private void Awake()
        {
            Init();
        }

        void Init()
        {
            aStar = new AStar(new Vector2Int(4, 5));
        }
        
        public bool SearchRoute(Vector2Int startNodeId, Vector2Int goalNodeId, List<Vector2Int> result, List<Vector2Int> lockList)
        {
            return aStar.SearchRoute(startNodeId, goalNodeId, result, lockList);
        }
    }
}