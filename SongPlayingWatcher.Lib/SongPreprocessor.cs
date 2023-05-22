using SongPlayingWatcher.Lib.Utils;
using SoundFingerprinting.Builder;
using SoundFingerprinting.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SongPlayingWatcher.Lib
{
    public class SongPreprocessor
    {
        private readonly string _sourceSong;
        private readonly bool _isResampled;

        public string ResampledSongName { get; private set; }

        public SongPreprocessor(string sourceSong, bool isResampled)
        {
            _sourceSong = sourceSong;
            _isResampled = isResampled;
            ResampledSongName = _isResampled ? sourceSong : Path.GetTempFileName();
        }

        public SongPreprocessor(string sourceSong, string resampledSongName = null)
        {
            _sourceSong = sourceSong;
            _isResampled = false;
            ResampledSongName = resampledSongName ?? Path.GetTempFileName();
        }

        public async Task Prepare()
        {
            if (!_isResampled)
            {
                new WavExporter(_sourceSong, true).Export(ResampledSongName);
            }

            var songTrack = new TrackInfo("1", string.Empty, string.Empty);

            var avHashes = await FingerprintCommandBuilder.Instance
                                        .BuildFingerprintCommand()
                                        .From(ResampledSongName)
                                        .UsingServices(ServicesProvider.AudioService)
                                        .Hash();

            ServicesProvider.ModelService.Insert(songTrack, avHashes);
        }
    }
}
