using UnityEngine;

namespace Twelve.Game.Mangers
{
    /// <summary>
    /// Astar
    ///
    /// 以下から流用、一部無駄な関数を削除
    /// https://github.com/baobao/UnityAstarSample
    /// 
    /// Author:baobao
    /// </summary>
    public struct Node
    {
        /// <summary>
        /// ノードのポジション
        /// </summary>
        internal Vector2Int Coordinates { get; }

        /// <summary>
        /// このノードにたどり着く前のノードポジション
        /// </summary>
        internal Vector2Int FromCoordinates { get; private set; }

        /// <summary>
        /// 必要コスト
        /// </summary>
        internal double MoveCost { get; private set; }

        /// <summary>
        /// ヒューリスティックなコスト
        /// </summary>
        private double _heuristicCost;
        
        internal Node(Vector2Int Coordinates) : this()
        {
            this.Coordinates = Coordinates;
            MoveCost = 0;
        }

        /// <summary>
        /// ゴール更新 ヒューリスティックコストの更新
        /// </summary>
        internal void UpdateGoalCoordinates(Vector2Int goal)
        {
            _heuristicCost = Mathf.Abs(goal.x - Coordinates.x) + Mathf.Abs(goal.y - Coordinates.y);
        }

        internal double GetScore()
        {
            return MoveCost + _heuristicCost;
        }

        internal void SetFromCoordinates(Vector2Int value)
        {
            FromCoordinates = value;
        }

        internal void SetMoveCost(double cost)
        {
            MoveCost = cost;
        }

        internal void Clear()
        {
            MoveCost = 0;
        }
    }
}
