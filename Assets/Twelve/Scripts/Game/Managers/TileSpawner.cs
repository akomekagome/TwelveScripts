using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Twelve.Common;
using UnityEngine;
using Twelve.Game.Tiles;
using UniRx;
using UniRx.Async;
using UnityEngine.UI;
using Zenject;

namespace Twelve.Game.Mangers
{
    /// <summary>
    /// Tileの生成をしている
    /// </summary>
    public class TileSpawner : MonoBehaviour
    {
        [Inject] private TileManager tileManager;
        [Inject] private GameInfo gameInfo;
        [SerializeField] private Canvas canvas;
        [SerializeField] private GridLayoutGroup backTileParent;
        [SerializeField] private FrontTile frontTilePrefab;
        [SerializeField] private BackTile backTilePrefab;
        
        private ReactiveCollection<BackTile> backTiles = new ReactiveCollection<BackTile>();
        private ReactiveCollection<FrontTile> frontTiles = new ReactiveCollection<FrontTile>();
        private Subject<Unit> finishSpawnTileSubject = new Subject<Unit>();
        public IReadOnlyReactiveCollection<BackTile> BackTiles => backTiles;
        public IReadOnlyReactiveCollection<FrontTile> FrontTiles => frontTiles;
        // 初期化の終了を通知
        public IObservable<Unit> FinishSpawnTileObservable => finishSpawnTileSubject.AsObservable();
        
        private Vector2 rectSize;

        public FrontTile GetTile(Vector2Int coordinates)
            => FrontTiles.FirstOrDefault(x => x.Coordinates == coordinates);
        
        public Vector3? GetBuckTilePosition(Vector2Int coordinates)
            => BackTiles.FirstOrDefault(x => x.Coordinates == coordinates)?.GetRectPosition();
        
        // 現在選択されていない座標からランダムにn個座標を取り出す
        private List<Vector2Int> GetRandomCoordinates(int n)
            => Enumerable.Range(0, 4)
                .SelectMany(_ => Enumerable.Range(0, 5), (x, y) => new Vector2Int(x, y))
                .Where(v => !FrontTiles.Any(x => x.Coordinates == v))
                .OrderBy(v => Guid.NewGuid())
                .Take(n)
                .ToList();

        private async UniTaskVoid Start()
        {
            rectSize = backTileParent.cellSize;
            
            // backTileを生成
            for(int y = 0; y < 5; y++)
                for(int x = 0; x < 4; x++)
                    CreateTile(backTilePrefab, new Vector2Int(x, y));
            
            await UniTask.DelayFrame(1);
            // FrontTilesに追加れた時tileが破壊されたことを購読して削除する
            FrontTiles
                .ObserveAdd()
                .Subscribe(x =>
                {
                    x.Value.DestroyObservable
                        .Subscribe(_ => frontTiles.Remove(x.Value)).AddTo(this);
                }).AddTo(this);

            // 続きだった場合前回のtileを生成
            if (gameInfo is ContinueGameInfo continueGameInfo)
                continueGameInfo.TileDataList
                    .ForEach(x => CreateTile(frontTilePrefab, x.coordinates, x.level));
            else
                CreateNTile(3);

            finishSpawnTileSubject.OnNext(Unit.Default);
            finishSpawnTileSubject.OnCompleted();
        }

        public void CreateNTile(int n)
        {
            GetRandomCoordinates(n)
                .ForEach(v => CreateTile(frontTilePrefab, v, UnityEngine.Random.Range(1, 4)));
        }

        private void CreateTile(BackTile backTile, Vector2Int coordinates)
        {
            var go = Instantiate(backTile, backTileParent.transform);
            go.SetCoordinates(coordinates);
            backTiles.Add(go);
        }

        private int count = 0;

        private void CreateTile(FrontTile frontTile, Vector2Int coordinates, int level)
        {
            var go = Instantiate(frontTile, canvas.transform);
            go.gameObject.name = go.gameObject.name + count++;
            go.Init(coordinates, level, GetBuckTilePosition(coordinates) ?? Vector3.zero, rectSize);
            frontTiles.Add(go);
        }
    }
}