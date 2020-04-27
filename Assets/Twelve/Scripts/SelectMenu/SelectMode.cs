using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Twelve.Common;
using Twelve.Common.Defines;
using Twelve.Game.Tiles;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;
using Twelve.Utils;
using UnityEngine.WSA;

/// <summary>
/// modeを選択している
/// </summary>
namespace Twelve.SelectMenu
{
    public class SelectMode : MonoBehaviour
    {
        [Inject] private SelectMenuManager selectMenuManager;
        [SerializeField] private List<ModeButton> modeButtons;
        [SerializeField] private SelectMenuButton continueButton;
        [SerializeField] private TextMeshProUGUI titleText;

        private async UniTaskVoid Start()
        {
            SetActiveButtons(false);
            await MoveTitle();
            SetActiveButtons(true);
            await CallActiveAnimation(true);
            
            var pushedButtonObservables =
                modeButtons.Select(x =>
                    x.OnClickAsObservable
                        .Select(_ => x.ModeType)
                );
            
            // 押されたbuttonによってゲームモードを変える
            pushedButtonObservables.Merge()
                .Do(x => ES3.Save<ModeType>(SaveDataKeys.ModeType, x, SaveDataPaths.PuzzleDataPath))
                .Select(x => new GameInfo(x))
                .Subscribe(x => MoveScene(x).ToObservable())
                .AddTo(this);
            
            // 前回のデーターを読み込んで途中から開始
            if(continueButton.gameObject.activeSelf)
                continueButton.OnClickAsObservable
                    .Select(_ => new ContinueGameInfo(
                        ES3.Load<ModeType>(SaveDataKeys.ModeType, SaveDataPaths.PuzzleDataPath),
                        ES3.Load<int>(SaveDataKeys.PreviousScore, SaveDataPaths.PuzzleDataPath),
                        ES3.Load<List<TileInfo>>(SaveDataKeys.TileDataList, SaveDataPaths.PuzzleDataPath)))
                    .Subscribe(x => MoveScene(x).ToObservable())
                    .AddTo(this);
        }

        private void SetActiveButtons(bool value)
        {
            modeButtons.ForEach(x => x.gameObject.SetActive(value));
            continueButton.gameObject.SetActive(ES3.FileExists(SaveDataPaths.PuzzleDataPath) && value);
        }
        
        // シーンを移動する
        private async UniTask MoveScene(GameInfo gameInfo)
        {
            await CallActiveAnimation(false);
            SetActiveButtons(false);
            await UniTask.Delay(200);
            selectMenuManager.MoveToPuzzleScene(gameInfo);
        }
        
        // ボタンをactiveする際のアニメーションを呼び出し
        private async UniTask CallActiveAnimation(bool value)
        {
            var activeAnimationAsync =
                modeButtons.Select(x => x.ActiveButtonAnimation(value));
            await UniTask.WhenAll(continueButton.gameObject.activeSelf ?
                activeAnimationAsync.Append(continueButton.ActiveButtonAnimation(value)) :
                activeAnimationAsync);
        }

        // Tileを動かす
        private async UniTask MoveTitle()
        {
            var toPosition = titleText.rectTransform.position;
            var fromPosition = toPosition - titleText.rectTransform.right * 7f;
            titleText.rectTransform.position = fromPosition;
            await titleText.rectTransform.DOMove(toPosition, 0.5f);
        }
    }
}