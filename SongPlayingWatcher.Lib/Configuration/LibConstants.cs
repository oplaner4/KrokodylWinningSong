using System.IO;

namespace SongPlayingWatcher.Lib.Configuration
{
    public class LibConstants
    {
        public const string KrokodylMp3SourceUri = @"https://icecast4.play.cz/krokodyl128.mp3";
        internal static readonly string RealtimeWavName = Path.GetTempFileName();
        internal const int ResampleRate = 96000;

        internal const int TakeSeconds = 10;
        internal const int TrimSeconds = 5;
        internal const int CheckPeriodSeconds = 10;
    }
}
