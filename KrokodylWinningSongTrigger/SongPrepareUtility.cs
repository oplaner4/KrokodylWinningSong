using SoundFingerprinting.Builder;
using SoundFingerprinting.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrokodylWinningSongTrigger
{
    internal class SongPrepareUtility
    {
        private readonly string _sourceSong;

        internal string ResampledWav { get; private set; }


        internal SongPrepareUtility(string sourceSong)
        {
            _sourceSong = sourceSong;
        }

        internal async Task Prepare()
        {
            string fullName = Path.GetFullPath(_sourceSong);
            ResampledWav = $"~{string.Join("_", fullName.Split(Path.GetInvalidFileNameChars()))}.wav";

            if (!File.Exists(ResampledWav))
            {
                await File.WriteAllBytesAsync(ResampledWav, new Mp3Resampler(_sourceSong, false).GetWavBytes());
            }

            var songTrack = new TrackInfo("1", string.Empty, string.Empty);
            var avHashes = await FingerprintCommandBuilder.Instance
                                        .BuildFingerprintCommand()
                                        .From(ResampledWav)
                                        .UsingServices(ServicesProvider.AudioService)
                                        .Hash();

            ServicesProvider.ModelService.Insert(songTrack, avHashes);
        }
    }
}
