using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Async;
using DG.Tweening;
using Twelve.Game.Mangers;
using Twelve.Utils;
using UniRx.Async.Triggers;
using Zenject;

namespace Game.Scripts.Game.Presenters
{
    public class MenuPresenter : MonoBehaviour
    {
        [Inject] private PauseMenuManager pauseMenuManager;
        [SerializeField] private Button pauseButton;
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private Button homeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button playButton;

        private RectTransform menuRectTransform;
        private void Start()
        {
            var token = this.GetCancellationTokenOnDestroy();
            menuRectTransform = pauseMenu.GetComponent<RectTransform>();
            pauseMenu.SetActive(false);
            
            pauseButton
                .OnClickAsObservable()
                .Where(_ => !pauseMenuManager.IsPause.Value)
                .Do(_ => pauseMenuManager.SetIsPaused(true))
                .Subscribe(_ => ActiveMenuAnimation(true, token).ToObservable())
                .AddTo(this);
            
            homeButton
                .OnClickAsObservable()
                .Subscribe(_ => pauseMenuManager.MoveToSelectMenuScene(true))
                .AddTo(this);
            
            restartButton
                .OnClickAsObservable()
                .Subscribe(_ => pauseMenuManager.MoveToPuzzleScene())
                .AddTo(this);
            
            playButton
                .OnClickAsObservable()
                .Do(_ => pauseMenuManager.SetIsPaused(false))
                .Subscribe(_ => ActiveMenuAnimation(false, token).ToObservable())
                .AddTo(this);
        }

        private async UniTask ActiveMenuAnimation(bool active, CancellationToken token)
        {
            if (active)
            {
                pauseMenu.SetActive(true);
                menuRectTransform.localScale = menuRectTransform.localScale.SetY(0f);
                await menuRectTransform.DOScaleY(1f, 0.2f).ToAwaiter(token);
            }
            else
            {
                menuRectTransform.localScale = menuRectTransform.localScale.SetY(1f);
                await menuRectTransform.DOScaleY(0f, 0.2f).ToAwaiter(token);
                pauseMenu.SetActive(false);
            }
        }
    }
}