using UnityEngine;
using Zenject;
using Twelve.Common;
using Twelve.Common.Defines;
using UniRx;

namespace Twelve.Game.Mangers
{
    /// <summary>
    /// scoreを管理している
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [Inject] private TileManager tileManager;
        [Inject] private GameInfo gameInfo;
        [Inject] private PauseMenuManager pauseMenuManager;
        [Inject] private GameStateManager gameStateManager;
        
        // ベストスコアの変更を通知
        private IntReactiveProperty bestScore = new IntReactiveProperty();
        public IReadOnlyReactiveProperty<int> BestScore => bestScore;
        // 現在のスコアの変更を通知
        private IntReactiveProperty currentScore = new IntReactiveProperty();
        public IReadOnlyReactiveProperty<int> CurrentScore => currentScore;

        private void Start()
        {
            bestScore.Value = LoadBestScore(gameInfo.ModeType);
            
            // 続きからだった場合途中までのスコアを読み込み
            if (gameInfo is ContinueGameInfo continueGameInfo)
                currentScore.Value = continueGameInfo.Score;
            
            tileManager.ChangeScoreObservable
                .Subscribe(ChangeScore)
                .AddTo(this);

            // SelectMenuに戻るかアプリケーションが終了した場合データを保存
            pauseMenuManager.MoveToSelectMenuSceneObservable
                .Merge(gameStateManager.OnApplicationQuitObservable)
                .Subscribe(_ =>
                {
                    SaveScore(gameInfo.ModeType);
                    SaveBestScore(gameInfo.ModeType);
                }).AddTo(this);

            gameStateManager.CurrentGameState
                .Where(x => x == GameState.Result)
                .Subscribe(_ => SaveBestScore(gameInfo.ModeType))
                .AddTo(this);
        }

        private void ChangeScore(int value)
        {
            currentScore.Value += value;
            
            if (CurrentScore.Value > bestScore.Value)
                bestScore.Value = CurrentScore.Value;
        }

        private void SaveScore(ModeType modeType)
        {
            ES3.Save<int>(SaveDataKeys.PreviousScore, CurrentScore.Value, SaveDataPaths.PuzzleDataPath);
        }
        
        // ゲームモードごとにbestscoreをセーブ
        private void SaveBestScore(ModeType modeType)
        {
            switch (modeType)
            {
                case ModeType.Normal:
                    ES3.Save<int>(SaveDataKeys.NormalBestScore, BestScore.Value, SaveDataPaths.BestScoreDataPath);
                    break;
                case ModeType.Hard:
                    ES3.Save<int>(SaveDataKeys.HardBestScore, BestScore.Value, SaveDataPaths.BestScoreDataPath);
                    break;
                case ModeType.Aggressive:
                    ES3.Save<int>(SaveDataKeys.AggressiveBestScore, BestScore.Value, SaveDataPaths.BestScoreDataPath);
                    break;
                case ModeType.Expert:
                    ES3.Save<int>(SaveDataKeys.ExpertBestScore, BestScore.Value, SaveDataPaths.BestScoreDataPath);
                    break;
            }
        }
        
        // ゲームモードごとにbestscoreを読み込み
        // ない場合は0を返す
        private int LoadBestScore(ModeType modeType)
        {
            switch (modeType)
            {
                case ModeType.Normal: 
                    return ES3.Load<int>(SaveDataKeys.NormalBestScore, SaveDataPaths.BestScoreDataPath, 0);
                    break;
                case ModeType.Hard:
                    return ES3.Load<int>(SaveDataKeys.HardBestScore, SaveDataPaths.BestScoreDataPath, 0);
                    break;
                case ModeType.Aggressive:
                    return ES3.Load<int>(SaveDataKeys.AggressiveBestScore, SaveDataPaths.BestScoreDataPath, 0);
                    break;
                case ModeType.Expert:
                    return ES3.Load<int>(SaveDataKeys.ExpertBestScore, SaveDataPaths.BestScoreDataPath, 0);
                    break;
            }
            return -1;
        }
    }
    
}