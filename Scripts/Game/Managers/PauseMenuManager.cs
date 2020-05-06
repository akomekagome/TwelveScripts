using Twelve.Common;
using Twelve.Common.Defines;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using System;

/// <summary>
/// result画面やpause画面などを管理している
/// ResultPresenter, MenuPresenter
/// からの呼び出しを受ける
/// </summary>
namespace Twelve.Game.Mangers
{
    public class PauseMenuManager : MonoBehaviour
    {
        [Inject] private ZenjectSceneLoader SceneLoader;
        [Inject] private GameInfo gameInfo;

        private BoolReactiveProperty isPaused = new BoolReactiveProperty();
        // 一時停止中か
        public IReadOnlyReactiveProperty<bool> IsPause => isPaused;
        
        public void SetIsPaused(bool value) => isPaused.Value = value;
        
        // これを受け取るとセーブが行われる
        private Subject<Unit> moveToSelectMenuSceneSubject = new Subject<Unit>();
        public IObservable<Unit> MoveToSelectMenuSceneObservable => moveToSelectMenuSceneSubject.AsObservable();
        
        // 新規puzzleシーンの呼び出し
        public void MoveToPuzzleScene()
        {
            var nextGameInfo = new GameInfo(gameInfo.ModeType);
            
            SceneLoader.LoadScene(SceneNames.PuzzleScene, LoadSceneMode.Single, container =>
            {
                container.Bind<GameInfo>()
                    .FromInstance(nextGameInfo).AsCached();
            });
        }
        
        // selectシーンの呼び出し
        // セーブをするかしないかの場合わけ
        public void MoveToSelectMenuScene(bool isSave)
        {
            if(isSave)
                moveToSelectMenuSceneSubject.OnNext(Unit.Default);
            moveToSelectMenuSceneSubject.OnCompleted();
            SceneLoader.LoadScene(SceneNames.SelectMenuScene, LoadSceneMode.Single);
        }
    }
}