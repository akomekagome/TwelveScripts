using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Twelve.Game.Tiles
{
    public interface ITile
    {
        Vector2Int Coordinates { get; }
        void SetCoordinates(Vector2Int coordinates);
    }
}

