using Twelve.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Twelve.Common.Defines;

namespace Twelve.SelectMenu
{
    public class SelectMenuManager : MonoBehaviour
    {
        [Inject] private ZenjectSceneLoader sceneLoader;
        [Inject] private AudioManager audioManager;

        private void Start()
        {
            audioManager.PlayBGM(BfxType.MainBgm);
        }

        public void MoveToPuzzleScene(GameInfo gameInfo)
        {
            sceneLoader.LoadScene(SceneNames.PuzzleScene, LoadSceneMode.Single, container =>
            {
                container.Bind<GameInfo>()
                    .FromInstance(gameInfo).AsCached();
            });
        }
    }
}