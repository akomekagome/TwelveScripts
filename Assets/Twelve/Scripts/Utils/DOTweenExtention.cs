using DG.Tweening;

namespace Twelve.Utils
{
    
    public static class MyDOTweenExtention
    {
        // TweenのAwaiter
        public struct TweenAwaiter : System.Runtime.CompilerServices.ICriticalNotifyCompletion
        {
            Tween tween;

            public TweenAwaiter(Tween tween) => this.tween = tween;

            // 最初にすでに終わってるのか終わってないのかの判定のために呼び出されるメソッドらしい
            public bool IsCompleted => tween.IsComplete();

            // Tweenは値を返さないので特に処理がいらないと思う
            public void GetResult() { }

            // このAwaiterの処理が終わったらcontinuationを呼び出してほしいって感じのメソッドらしい
            public void OnCompleted(System.Action continuation) => tween.OnKill(() => continuation());

            // OnCompletedと同じでいいっぽい？
            public void UnsafeOnCompleted(System.Action continuation) => tween.OnKill(() => continuation());
        }

        // Tweenに対する拡張メソッド
        public static TweenAwaiter GetAwaiter(this Tween self)
        {
            return new TweenAwaiter(self);
        }
    }
}
