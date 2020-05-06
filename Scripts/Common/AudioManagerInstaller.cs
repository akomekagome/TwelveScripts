using System.Collections;
using System.Collections.Generic;
using Twelve.Common;
using UnityEngine;
using Zenject;

namespace Twelve.Common
{
    public class AudioManagerInstaller : MonoInstaller
    {
        [SerializeField]
        private AudioManager audioManagerPrefab;

        public override void InstallBindings()
        {
            Container.Bind<AudioManager>().FromComponentInNewPrefab(audioManagerPrefab).AsSingle();
        }
    }
}

