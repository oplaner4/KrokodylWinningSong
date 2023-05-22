using SongPlayingWatcher.Lib.Utils;
using SongPlayingWatcher.Lib.Configuration;
using SoundFingerprinting.Audio;
using SoundFingerprinting.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SongPlayingWatcher.Lib
{
    public class CheckPlaying
    {
        private readonly string _resampledSong;
        private readonly string _realtimeStreamUri;

        public readonly CheckPlayingEvents Events;

        public CheckPlaying(string resampledSong, string realtimeStreamUri)
        {
            _resampledSong = resampledSong;
            _realtimeStreamUri = realtimeStreamUri;
            Events = new();
        }

        public async Task PeriodicallyCheck(bool forever)
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(LibConstants.CheckPeriodSeconds));
            var tomorrow = DateTime.Now.AddDays(1).Date;

            int playingPeriods = 1;

            while (forever || DateTime.Now < tomorrow)
            {
                Events.OnListening?.Invoke();
                new WavExporter(_realtimeStreamUri, true).Export(LibConstants.RealtimeWavName);
                var queryResult = await QueryCommandBuilder.Instance.BuildQueryCommand()
                    .From(LibConstants.RealtimeWavName)
                    .UsingServices(ServicesProvider.ModelService, ServicesProvider.AudioService)
                    .Query();

                if (Events.OnChecked != null && !Events.OnChecked(queryResult.ContainsMatches)) {
                    return;
                }

                if (queryResult.ContainsMatches)
                {
                    Events.OnPlaying?.Invoke(playingPeriods++);
                }
                else
                {
                    playingPeriods = 0;
                    Events.OnNotPlaying?.Invoke();
                }

                await timer.WaitForNextTickAsync();
            }
        }
    }
}
