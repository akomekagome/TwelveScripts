using System.Collections.Generic;
using Twelve.Game.Tiles;


namespace Twelve.Common
{
    public class ContinueGameInfo : GameInfo
    {
        public int Score { get; }
        public IReadOnlyCollection<TileInfo> TileDataList { get; }
        
        public ContinueGameInfo(ModeType modeType, int sore, List<TileInfo> tileDataList) : base(modeType)
        {
            Score = sore;
            TileDataList = tileDataList;
        }
    }
}