using SoundFingerprinting.Audio;
using SoundFingerprinting.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KrokodylWinningSongTrigger
{
    internal class SongCheckPlayingTrigger
    {
        private readonly string _resampledSong;

        internal SongCheckPlayingTrigger(string resampledSong)
        {
            _resampledSong = resampledSong;
        }

        internal async Task PeriodicallyCheck(Action<bool> action)
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(Constants.CheckPeriodSeconds));

            while (true)
            {
                await File.WriteAllBytesAsync(Constants.KrokodylWavName,
                    new Mp3Resampler(Constants.KrokodylMp3SourceUri, true).GetWavBytes());

                var queryResult = await QueryCommandBuilder.Instance.BuildQueryCommand()
                    .From(Constants.KrokodylWavName)
                    .UsingServices(ServicesProvider.ModelService, ServicesProvider.AudioService)
                    .Query();

                action(queryResult.ContainsMatches);
                await timer.WaitForNextTickAsync();
            }
        }
    }
}
