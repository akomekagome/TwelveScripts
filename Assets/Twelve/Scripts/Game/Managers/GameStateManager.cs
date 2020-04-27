using System;
using UnityEngine;
using UniRx;
using UniRx.Async;
using Zenject;

namespace Twelve.Game.Mangers
{
    
    public class GameStateManager : MonoBehaviour
    {
        [Inject] private TileSpawner tileSpawner;
        [Inject] private TileManager tileManager;
        private Subject<Unit> onApplicationQuitSubject = new Subject<Unit>();
        public IObservable<Unit> OnApplicationQuitObservable => onApplicationQuitSubject.AsObservable();
        // 現在のゲームの状況
        private ReactiveProperty<GameState> currentGameState = new ReactiveProperty<GameState>(GameState.Initializing);
        public IReadOnlyReactiveProperty<GameState> CurrentGameState => currentGameState.ToReadOnlyReactiveProperty();

        private async UniTaskVoid Start()
        {
            await tileSpawner.FinishSpawnTileObservable.FirstOrDefault();

            currentGameState.Value = GameState.GameUpdate;

            await tileManager.FinishPuzzleObservable.FirstOrDefault();

            currentGameState.Value = GameState.Result;
        }

        private void OnApplicationQuit()
        {
            onApplicationQuitSubject.OnNext(Unit.Default);
        }
    }
}