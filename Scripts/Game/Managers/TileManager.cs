using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Twelve.Common;
using Twelve.Common.Defines;
using Twelve.Game.Tiles;
using Twelve.Game.Cameras;
using UnityEngine;
using UniRx;
using UniRx.Async;
using UniRx.Async.Triggers;
using UniRx.Triggers;
using UnityEngine.EventSystems;
using Zenject;

namespace Twelve.Game.Mangers
{
    /// <summary>
    /// Tileを管理をしている
    /// パズルの終了条件の判定やrayを飛ばす処理も行なっている
    /// </summary>
    public class TileManager : MonoBehaviour
    {
        [Inject] private RouteProvider routeProvider;
        [Inject] private TileSpawner tileSpawner;
        [Inject] private PauseMenuManager pauseMenuManager;
        [Inject] private GameStateManager gameStateManager;
        [Inject] private AudioManager audioManager;
        
        private ReactiveCollection<Vector2Int> selectedCoordinates = new ReactiveCollection<Vector2Int>();

        private Subject<int> changeScoreSubject = new Subject<int>();
        public IObservable<int> ChangeScoreObservable => changeScoreSubject.AsObservable();
        
        private Subject<Unit> finishPuzzleSubject = new Subject<Unit>();
        public IObservable<Unit> FinishPuzzleObservable => finishPuzzleSubject.AsObservable();

        private RaycastHit2D hit;
        private List<RaycastResult> rayResult = new List<RaycastResult>();
        // astar呼び出し用
        private List<Vector2Int> result = new List<Vector2Int>();

        private bool IsSelectable => selectedCoordinates.Count < 2;
        
        private void Start()
        {
            // マウスボタンが押されて条件を満たしている時rayを飛ばしてtileを選択
            this.UpdateAsObservable()
                .Where(_ => gameStateManager.CurrentGameState.Value == GameState.GameUpdate)
                .Where(_ => !pauseMenuManager.IsPause.Value)
                .Where(_ => Input.GetMouseButtonDown(0) && IsSelectable)
                .Subscribe(_ => SelectTile());

            // タイルを二個選択している時
            selectedCoordinates
                .ObserveCountChanged()
                .Where(x => x == 2)
                .Subscribe(_ => MoveTileAsync(this.GetCancellationTokenOnDestroy()).ToObservable())
                .AddTo(this);
            
            // selectmenuに移動した時現在の状況を保存
            pauseMenuManager.MoveToSelectMenuSceneObservable
                .Merge(gameStateManager.OnApplicationQuitObservable)
                .Subscribe(_ => SaveTileData())
                .AddTo(this);
        }

        // tileの座標とレベルを保存
        private void SaveTileData()
        {
            var tileDataList =
                tileSpawner.FrontTiles
                    .Select(x => new TileInfo(x.Coordinates, x.Level))
                    .ToList();
            ES3.Save<List<TileInfo>>(SaveDataKeys.TileDataList, tileDataList, SaveDataPaths.PuzzleDataPath);
        }

        // tileを選択
        private void SelectTile()
        {
            var currentPointData = new PointerEventData(EventSystem.current) { position = Input.mousePosition};
            EventSystem.current.RaycastAll(currentPointData, rayResult);
            
            // 飛ばしたrayの中にtileが含まれているか
            var tile = rayResult
                .Select(x => x.gameObject.GetComponent<ITile>())
                .FirstOrDefault(x => x != null);
            
            if(tile == null)
                return;
            
            audioManager.PlaySE(SfxType.TileClick);
            var coordinates = tile.Coordinates;
            var frontTile = tileSpawner.GetTile(coordinates);
            
            // 現在の選択するによって場合分け
            switch (selectedCoordinates.Count())
            {
                case 0:
                    if(frontTile == null)
                        return;
                    frontTile.SetIsSelected(true);
                    selectedCoordinates.Add(coordinates);
                    break;
                case 1:
                    if (selectedCoordinates.Contains(coordinates))
                    {
                        selectedCoordinates.Remove(coordinates);
                        if(frontTile != null)
                            frontTile.SetIsSelected(false);
                    }
                    else
                        selectedCoordinates.Add(coordinates);
                    break;
            }
        }
        private async UniTask MoveTileAsync(CancellationToken token)
        {
            var fromTile = tileSpawner.GetTile(selectedCoordinates[0]);
            if (fromTile == null)
                return;
            fromTile.SetIsSelected(false);
            
            var toTile = tileSpawner.GetTile(selectedCoordinates[1]);
            
            if (!(toTile != null && (toTile.Level != fromTile.Level || toTile.Level >= 12)))
            {
                // astarの呼び出し
                var isMovable = await SearchRouteAsync(selectedCoordinates.ToArray());

                if (isMovable)
                {
                    var paths = result
                        .Select(v => tileSpawner.GetBuckTilePosition(v) ?? Vector3.zero)
                        .ToArray();

                    await fromTile.OnMove(paths, token);
                    
                    // 場合分け
                    if (toTile != null)
                    {
                        audioManager.PlaySE(SfxType.TileMatch);
                        var direction = result.Count < 2
                            ? fromTile.Coordinates - result.Last()
                            : result[result.Count - 2] - result.Last();
                        
                        fromTile.DestroyTile();
                        
                        var score = toTile.Level + 1;
                        toTile.ChangeLevel(score, direction, token).Forget();
                        changeScoreSubject.OnNext(score);
                    }
                    else
                        fromTile.SetCoordinates(selectedCoordinates[1]);
                    // 動きが終了した後タイル生成を呼び出し
                    tileSpawner.CreateNTile(toTile == null ? 2 : 1);
                    // 終了判定を呼び出し
                    var isFinish = await CheckFinishPuzzle();
                    if (isFinish)
                    {
                        finishPuzzleSubject.OnNext(Unit.Default);
                        finishPuzzleSubject.OnCompleted();
                    }
                }
                else
                    audioManager.PlaySE(SfxType.NotMatch);
            }
            else
                audioManager.PlaySE(SfxType.NotMatch);
            selectedCoordinates.Clear();
        }

        // astarを呼び出してその判定を返す
        private async UniTask<bool> SearchRouteAsync(params Vector2Int[] vector2ints)
        {
            result.Clear();
            
            var lockList = 
                tileSpawner.FrontTiles
                    .Select(x => x.Coordinates)
                    .Where(v => !vector2ints.Contains(v))
                    .ToList();
            
            return await UniTask
                .Run(() => routeProvider.SearchRoute(vector2ints[0], vector2ints[1], result, lockList));
        }
        
        // 終了判定
        private async UniTask<bool> CheckFinishPuzzle()
        {
            var frontTiles = tileSpawner.FrontTiles;
            var length = frontTiles.Count;
            
            if (length < 20)
                return false;
            
            for (var i = 0; i < length; i++)
                for (var j = i + 1; j < length; j++)
                    if (frontTiles[i].Level == frontTiles[j].Level)
                        if(await SearchRouteAsync(frontTiles[i].Coordinates, frontTiles[j].Coordinates))
                            return false; 
            return true;
        }
    }
}