using UnityEngine;
using Twelve.Common;


namespace Twelve.SelectMenu
{
    
    public class ModeButton : SelectMenuButton
    {
        [SerializeField] private ModeType modeType;

        public ModeType ModeType => modeType;
    }
}
