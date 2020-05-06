namespace Twelve.Common
{
    /// <summary>
    /// puzzle内で使う情報を保持しておきzenjectでbind
    /// </summary>
    public class GameInfo
    {
        public ModeType ModeType { get; }

        public GameInfo(ModeType modeType)
        {
            ModeType = modeType;
        }
    }
}