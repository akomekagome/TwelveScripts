using UnityEngine;
using System;
using System.Threading;
using UniRx;
using UnityEngine.UI;
using Twelve.Utils;
using UniRx.Async;
using DG.Tweening;

namespace Twelve.SelectMenu
{
    public class SelectMenuButton : MonoBehaviour
    {
        
        private Button button;
        public IObservable<Unit> OnClickAsObservable => button.OnClickAsObservable();
        private RectTransform rectTransform;
        
        
        public async UniTask ActiveButtonAnimation(bool active, CancellationToken token)
        {
            if (active)
            {
                rectTransform.localScale = Vector2.zero;
                await rectTransform.DOScale(Vector3.one, 0.3f).ToAwaiter(token);
            }
            else
            {
                rectTransform.localScale = Vector2.one;
                await rectTransform.DOScale(Vector3.zero, 0.3f).ToAwaiter(token);
            }
        }
        private void Awake()
        {
            button = GetComponent<Button>();
            rectTransform = GetComponent<RectTransform>();
        }
    }
}