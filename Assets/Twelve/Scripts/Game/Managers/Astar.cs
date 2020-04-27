using UnityEngine;
using System.Collections.Generic;
using Twelve.Utils;

/// <summary>
/// Astar
///
/// 以下から流用、終了条件の設定やアルゴリズムの変更などかなり変更
/// https://github.com/baobao/UnityAstarSample
/// 
/// Author:baobao
/// </summary>

namespace Twelve.Game.Mangers
{
    public class AStar
    {
        private Vector2Int fieldSize;
        private Node[,] nodes;
        private List<Vector2Int> openCoordinates = new List<Vector2Int>();
        private List<Vector2Int> closeCoordinates = new List<Vector2Int>();

        public AStar(Vector2Int size)
        {
            fieldSize = size;
            nodes = new Node[fieldSize.x, fieldSize.y];

            for (int x = 0; x < fieldSize.x; x++)
                for (int y = 0; y < size.y; y++)
                    nodes[x, y] = new Node(new Vector2Int(x, y));
        }

        /// <summary>
        /// ルート検索開始
        /// </summary>
        public bool SearchRoute(Vector2Int startCoordinates, Vector2Int goalCoordinates, List<Vector2Int> routeList, List<Vector2Int> lockList)
        {
            ResetNode();
            openCoordinates.Clear();
            closeCoordinates.Clear();
            if (startCoordinates == goalCoordinates)
            {
                Debug.Log($"{startCoordinates}/{goalCoordinates}/同じ場所なので終了");
                return false;
            }

            // 全ノード更新
            for (int x = 0; x < fieldSize.x; x++)
                for (int y = 0; y < fieldSize.y; y++)
                {
                    nodes[x, y].UpdateGoalCoordinates(goalCoordinates);
                }
            
            nodes[startCoordinates.x, startCoordinates.y].SetFromCoordinates(startCoordinates);
            openCoordinates.Add(startCoordinates);

            int cnt = 1000;
            while (true)
            {
                var bestScoreCoordinates = GetBestScoreCoordinates();
                OpenNode(bestScoreCoordinates, lockList);

                if (--cnt < 0)
                {
                    Debug.LogError("無限ループ");
                    break;
                }
                
                // ゴールに辿り着いたら終了
                if (bestScoreCoordinates == goalCoordinates)
                    break;

                if (openCoordinates.Count == 0)
                    return false;
            }

            ResolveRoute(startCoordinates, goalCoordinates, routeList);
            return true;
        }

        void ResetNode()
        {
            for (int x = 0; x < fieldSize.x; x++)
                for (int y = 0; y < fieldSize.y; y++)
                {
                    nodes[x, y].Clear();
                }
        }

        // ノードを展開する
        void OpenNode(Vector2Int bestCoordinates, List<Vector2Int> lockList)
        {
            Vector2Int[] crossCoordinates =
            {
                bestCoordinates.AddX(1),
                bestCoordinates.AddX(-1),
                bestCoordinates.AddY(1),
                bestCoordinates.AddY(-1)
            };
            
            foreach (var v in crossCoordinates)
            {
                if(!(v.x >= 0 && v.x < fieldSize.x & v.y >= 0 && v.y < fieldSize.y))
                    continue;
                if(lockList.Contains(v))
                    continue;

                var previousScore = nodes[v.x, v.y].GetScore();
                var previousMoveCost = nodes[v.x, v.y].MoveCost;
                nodes[v.x, v.y].SetMoveCost(nodes[bestCoordinates.x, bestCoordinates.y].MoveCost + 1);
                
                if (openCoordinates.Contains(v))
                {
                    if(nodes[v.x, v.y].GetScore() < previousScore)
                        nodes[v.x, v.y].SetFromCoordinates(bestCoordinates);
                    else
                        nodes[v.x, v.y].SetMoveCost(previousMoveCost);
                }
                else if (closeCoordinates.Contains(v))
                {
                    if (nodes[v.x, v.y].GetScore() < previousScore)
                    {
                        closeCoordinates.Remove(v);
                        openCoordinates.Add(v);
                        nodes[v.x, v.y].SetFromCoordinates(bestCoordinates);
                    }
                    else
                        nodes[v.x, v.y].SetMoveCost(previousMoveCost);
                }
                else
                {
                    nodes[v.x, v.y].SetFromCoordinates(bestCoordinates);
                    openCoordinates.Add(v);
                }
            }
            
            openCoordinates.Remove(bestCoordinates);
            closeCoordinates.Add(bestCoordinates);
        }

        void ResolveRoute(Vector2Int startCoordinates, Vector2Int goalCoordinates, List<Vector2Int> result)
        {
            var node = nodes[goalCoordinates.x, goalCoordinates.y];
            result.Add(goalCoordinates);

            bool isSuccess = false;
            while (true)
            {
                var beforeNode = result[0];
                if (beforeNode == node.FromCoordinates)
                {
                    // 同じポジションなので終了
                    Debug.LogError("同じポジションなので終了失敗" + beforeNode + " / " + node.FromCoordinates + " / " + goalCoordinates);
                    break;
                }

                if (node.FromCoordinates == startCoordinates)
                {
                    isSuccess = true;
                    break;
                }
                else
                {
                    // 開始座標は結果リストには追加しない
                    result.Insert(0, node.FromCoordinates);
                }

                node = nodes[node.FromCoordinates.x, node.FromCoordinates.y];
            }

            if (isSuccess == false)
            {
                Debug.LogError("失敗" + startCoordinates + " / " + node.FromCoordinates);
            }
        }

        /// <summary>
        /// 最良のノードIDを返却
        /// </summary>
        Vector2Int GetBestScoreCoordinates()
        {
            var result = new Vector2Int(0, 0);
            double min = double.MaxValue;
            foreach (var v in openCoordinates)
            {
                if (min > nodes[v.x, v.y].GetScore())
                {
                    // 優秀なコストの更新(値が低いほど優秀)
                    min =  nodes[v.x, v.y].GetScore();
                    result = v;
                }
            }
            return result;
        }
    }
}
