using DG.Tweening;
using TMPro;
using Twelve.Common;
using Twelve.Game.Mangers;
using Twelve.Utils;
using UniRx;
using UniRx.Async;
using UniRx.Async.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Game.Presenters
{
    public class ResultPresenter : MonoBehaviour
    {
        [Inject] private PauseMenuManager pauseMenuManager;
        [Inject] private ScoreManager scoreManager;
        [Inject] private GameStateManager gameStateManager;
        [Inject] private GameInfo gameInfo;
        [SerializeField] private GameObject resultMenu;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button restartButton;

        private RectTransform resultMenuRectTransform;

        private async UniTaskVoid Start()
        {
            resultMenu.SetActive(false);
            resultMenuRectTransform = resultMenu.GetComponent<RectTransform>();

            homeButton
                .OnClickAsObservable()
                .Subscribe(_ => pauseMenuManager.MoveToSelectMenuScene(false))
                .AddTo(this);
            
            restartButton
                .OnClickAsObservable()
                .Subscribe(_ => pauseMenuManager.MoveToPuzzleScene())
                .AddTo(this);

            await gameStateManager.CurrentGameState.Where(x => x == GameState.Result).FirstOrDefault();
            resultMenu.SetActive(true);
            resultMenuRectTransform.localScale = resultMenuRectTransform.localScale.SetY(0f);
            await resultMenuRectTransform.DOScaleY(1f, 0.2f).ToAwaiter(this.GetCancellationTokenOnDestroy());
            bestScoreText.text = "best: " + scoreManager.BestScore.Value;
            scoreText.text = "score: " + scoreManager.CurrentScore.Value;
            naichilab.RankingLoader.Instance.SendScoreAndShowRanking(scoreManager.CurrentScore.Value, (int)gameInfo.ModeType);
        }
    }
}