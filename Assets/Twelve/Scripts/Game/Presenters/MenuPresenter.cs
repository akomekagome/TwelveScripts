using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Async;
using DG.Tweening;
using Twelve.Game.Mangers;
using Twelve.Utils;
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
            menuRectTransform = pauseMenu.GetComponent<RectTransform>();
            pauseMenu.SetActive(false);

            pauseButton
                .OnClickAsObservable()
                .Where(_ => !pauseMenuManager.IsPause.Value)
                .Do(_ => pauseMenuManager.SetIsPaused(true))
                .Subscribe(_ => ActiveMenuAnimation(true).ToObservable())
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
                .Subscribe(_ => ActiveMenuAnimation(false).ToObservable())
                .AddTo(this);
        }

        private async UniTask ActiveMenuAnimation(bool active)
        {
            if (active)
            {
                pauseMenu.SetActive(true);
                menuRectTransform.localScale = menuRectTransform.localScale.SetY(0f);
                await menuRectTransform.DOScaleY(1f, 0.2f);
            }
            else
            {
                menuRectTransform.localScale = menuRectTransform.localScale.SetY(1f);
                await menuRectTransform.DOScaleY(0f, 0.2f);
                pauseMenu.SetActive(false);
            }
        }
    }
}