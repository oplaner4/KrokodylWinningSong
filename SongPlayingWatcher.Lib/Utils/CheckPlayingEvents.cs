using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SongPlayingWatcher.Lib.Utils
{
    public class CheckPlayingEvents
    {
        internal CheckPlayingEvents() { }

        public delegate bool OnCheckedHandler(bool isPlaying);
        public delegate void OnPlayingHandler(int periods);
        public delegate void OnNotPlayingHandler();
        public delegate void OnRepeatedlyPlayingHandler();
        public delegate void OnListeningHandler();

        public OnCheckedHandler OnChecked = null;
        public OnPlayingHandler OnPlaying = null;
        public OnNotPlayingHandler OnNotPlaying = null;
        public OnRepeatedlyPlayingHandler OnRepeatedlyPlaying = null;
        public OnListeningHandler OnListening = null;
    }
}
