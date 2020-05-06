using TMPro;
using Twelve.Game.Mangers;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Game.Presenters
{
    public class ScorePresenter : MonoBehaviour
    {
        [Inject] private ScoreManager scoreManager;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private TextMeshProUGUI currentScoreText;

        private void Start()
        {
            scoreManager.BestScore
                .Subscribe(x => bestScoreText.text = "best : " + x)
                .AddTo(this);
            
            scoreManager.CurrentScore
                .Subscribe(x => currentScoreText.text = x.ToString())
                .AddTo(this);
        }
    }
}