using SongPlayingWatcher.Lib;
using Xunit;

namespace SongPlayingWatcher.Test
{
    public class GeneralTest
    {
        [Fact]
        public async void StreamEqualToSong_ShouldSucceed()
        {
            var sourceSong = "../../../Songs/Lost For Words.mp3";
            var songPreprocessor = new SongPreprocessor(sourceSong);
            await songPreprocessor.Prepare();
            var checkPlaying = new CheckPlaying(songPreprocessor.ResampledSongName, sourceSong);
            checkPlaying.Events.OnChecked = (playing) =>
            {
                Assert.True(playing);
                return false;
            };

            await checkPlaying.PeriodicallyCheck(false);
        }
    }
}