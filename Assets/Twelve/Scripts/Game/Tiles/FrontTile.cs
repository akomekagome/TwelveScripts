using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Async;
using UnityEngine;
using DG.Tweening;
using TMPro;
using Twelve.Common;
using Twelve.Utils;
using UnityEngine.UI;
using Zenject;

namespace Twelve.Game.Tiles
{
    public class FrontTile : MonoBehaviour, ITile
    {
        [SerializeField] private TextMeshProUGUI tmpText;
        [SerializeField] private List<Color> levelColors;
        private Vector2Int coordinates;
        public Vector2Int Coordinates => coordinates;
        public int Level { get; private set; }
        
        private Subject<Unit> destroySubject = new Subject<Unit>();
        public IObservable<Unit> DestroyObservable => destroySubject.AsObservable();

        private BoolReactiveProperty isSelected = new BoolReactiveProperty();
        public IReadOnlyReactiveProperty<bool> IsSelected => isSelected.ToReadOnlyReactiveProperty();    

        public void SetIsSelected(bool value) => isSelected.Value = value;
        private RectTransform rectTransform;
        private GlowImage glowImage;
        private Image image;
        public void SetCoordinates(Vector2Int coordinates) => this.coordinates = coordinates;

        // 初期化
        public void Init(Vector2Int coordinates, int level, Vector3 position, Vector2 size)
        {
            SetCoordinates(coordinates);
            SetLevel(level);
            rectTransform.position = position;
            rectTransform.sizeDelta = size;
            rectTransform.localScale = Vector3.zero;
            rectTransform.DOScale(Vector3.one, 0.15f);
        }
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            glowImage = GetComponent<GlowImage>();
            image = GetComponent<Image>();

            // 選択されたら光らせる
            IsSelected
                .Subscribe(x => glowImage.glowSize = x ? 1f : 10f)
                .AddTo(this);
        }
        // レベルを変更し、色も変更
        private void SetLevel(int level)
        {
            this.Level = level;
            tmpText.text = level.ToString();
            image.color = levelColors[level - 1];
        }

        public async UniTask OnMove(Vector3[] paths)
        {
            await rectTransform.DOPath(paths, paths.Length / 8f, PathType.CatmullRom);
        }
        
        // tileを破壊して通知
        public void DestroyTile()
        {
            destroySubject.OnNext(Unit.Default);
            destroySubject.OnCompleted();
            Destroy(gameObject);
        }
        
        // 渡された軸によって回る軸を変える
        public async UniTaskVoid ChangeLevel(int level, Vector2Int direction)
        {
            var rotate = direction.x != 0 ? new Vector3(0, 90f, 0) : new  Vector3(90f, 0, 0);
            await rectTransform.DOLocalRotate(rotate, 0.1f, RotateMode.FastBeyond360);
            SetLevel(level);
            await rectTransform.DOLocalRotate(Vector3.zero, 0.1f, RotateMode.FastBeyond360);
        }
    }
}

