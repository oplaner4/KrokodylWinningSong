using System.Collections.Generic;
using System;
using System.Linq;
using SongPlayingWatcher.Lib.Configuration;

namespace SongPlayingWatcher.Configuration
{
    internal class Settings
    {
        internal const int DefaultMinPeriodsPlaying = 1;
        internal string SongToBePlayed { get; set; }
        internal string ResambledSongName { get; set; }
        internal bool IsResampled { get; set; }
        internal bool ShowUsageMessage { get; set; }
        internal bool CheckForever { get; set; }
        internal string RealtimeStreamUri { get; set; }
        internal string StartExecName { get; set; }
        internal string StartExecArguments { get; set; }
        internal int MinPeriodsPlaying { get; set; }

        internal Settings()
        {
            SongToBePlayed = null;
            ResambledSongName = null;
            IsResampled = false;
            ShowUsageMessage = false;
            CheckForever = false;
            RealtimeStreamUri = LibConstants.KrokodylMp3SourceUri;
            StartExecName = null;
            StartExecArguments = null;
            MinPeriodsPlaying = DefaultMinPeriodsPlaying;
        }

        internal static Settings GetFromEnvArgs ()
        {
            IEnumerator<string> argsEnumerator = Environment.GetCommandLineArgs().AsEnumerable().GetEnumerator();
            argsEnumerator.MoveNext();

            var result = new Settings();

            while (argsEnumerator.MoveNext())
            {
                if ((argsEnumerator.Current == "-f") || (argsEnumerator.Current == "forever"))
                {
                    result.CheckForever = true;
                }
                else if ((argsEnumerator.Current == "-r") || (argsEnumerator.Current == "resampled"))
                {
                    result.IsResampled = true;
                }
                else if (argsEnumerator.Current == "-p")
                {
                    if (argsEnumerator.MoveNext() && int.TryParse(argsEnumerator.Current, out int minPeriodsPlaying))
                    {
                        result.MinPeriodsPlaying = minPeriodsPlaying;
                    }
                    else
                    {
                        result.SongToBePlayed = null;
                        break;
                    }
                }
                else if (argsEnumerator.Current == "-u")
                {
                    if (argsEnumerator.MoveNext())
                    {
                        result.RealtimeStreamUri = argsEnumerator.Current;
                    }
                    else
                    {
                        result.SongToBePlayed = null;
                        break;
                    }
                }
                else if (argsEnumerator.Current == "-s")
                {
                    if (argsEnumerator.MoveNext())
                    {
                        result.ResambledSongName = argsEnumerator.Current;
                    }
                    else
                    {
                        result.SongToBePlayed = null;
                        break;
                    }
                }

                else if (argsEnumerator.Current == "-e")
                {
                    if (argsEnumerator.MoveNext())
                    {
                        result.StartExecName = argsEnumerator.Current;
                    }
                    else
                    {
                        result.SongToBePlayed = null;
                        break;
                    }

                    if (argsEnumerator.MoveNext())
                    {
                        result.StartExecArguments = argsEnumerator.Current;
                    }
                    else
                    {
                        result.SongToBePlayed = null;
                        break;
                    }
                }

                else if ((argsEnumerator.Current == "-h") || (argsEnumerator.Current == "help"))
                {
                    result.ShowUsageMessage = true;
                    break;
                }
                else
                {
                    result.SongToBePlayed = argsEnumerator.Current;
                }
            }

            return result;
        }
    }
}
